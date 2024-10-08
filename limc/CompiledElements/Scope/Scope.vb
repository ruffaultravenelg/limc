﻿Public Class Scope

    '===================================
    '======== CONTEXT VARIABLES ========
    '===================================
    Public ReadOnly Property Variables As New List(Of Variable)

    '=======================================
    '======== CONTEXT GENERIC TYPES ========
    '=======================================
    Public ReadOnly Property GenericTypes As New List(Of GenericType)

    '================================
    '======== PARENT CONTEXT ========
    '================================
    Private ReadOnly Parent As Scope
    Private ReadOnly ReturnTypeSetter As Action(Of Type, Location) = Nothing

    '=======================================
    '======== CLEAR LOCAL VARIABLES ========
    '=======================================
    Private Function GenerateVariableDeferencing() As String

        'Create result
        Dim Result As String = ""

        'Add reference uncount
        For Each Variable As Variable In Variables
            If TypeOf Variable.Type Is IGarbageCollectedType Then
                Result &= " " & Variable.CompiledName & "->stackReferences--;"
            End If
        Next

        'Add comment
        If Not Result = "" Then
            Result = "/* Deletes local references */" & Result
        End If

        'Return result
        Return Result

    End Function

    '========================
    '======== RESULT ========
    '========================
    Private ReadOnly Lines As New List(Of String)
    Public ReadOnly Property Result As IEnumerable(Of String)
        Get

            'Create result
            Dim Final As New List(Of String)
            Final.AddRange(Lines)

            'Add reference uncount
            Final.Add(GenerateVariableDeferencing())

            'Return final
            Return Final

        End Get
    End Property
    Public ReadOnly Property IndentedResult(Indent As Integer) As IEnumerable(Of String)
        Get
            Dim IdentedLines As New List(Of String)
            For Each Line As String In Lines
                IdentedLines.Add(StrDup(Indent, vbTab) & Line)
            Next
            Return IdentedLines
        End Get
    End Property

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Optional Parent As Scope = Nothing, Optional ReturnTypeSetter As Action(Of Type, Location) = Nothing)
        Me.Parent = Parent
        Me.ReturnTypeSetter = ReturnTypeSetter
    End Sub

    '==========================================
    '======== SET FUNCTION RETURN TYPE ========
    '==========================================
    Public Sub SetFunctionReturnType(Type As Type, Location As Location)

        'Search ReturnTypeSetter & execute it
        For Each Scope As Scope In Uppers
            If Scope.ReturnTypeSetter IsNot Nothing Then
                Scope.ReturnTypeSetter(Type, Location)
                Exit Sub
            End If
        Next

        'Hummmmm
        Throw New SyntaxErrorException("A ""return"" statement has been used outside a function.", Location)

    End Sub

    '=======================
    '======== UPPER ========
    '=======================
    Public ReadOnly Iterator Property Uppers As IEnumerable(Of Scope)
        Get
            Dim Upper As Scope = Me
            While Upper IsNot Nothing
                Yield Upper
                Upper = Upper.Parent
            End While
        End Get
    End Property

    '=================================
    '======== CREATE VARIABLE ========
    '=================================
    Public Function CreateVariable(Name As String, Type As Type, Optional DefaultValue As String = Nothing) As Variable

        'Create variable
        Dim Variable As New Variable(Name, Type)

        'Add it to scope
        Variables.Add(Variable)

        'Get default value
        If DefaultValue = Nothing Then
            DefaultValue = Type.DefaultValue
        End If

        'Create variable
        If Type.CompiledName.Contains("*"c) Then 'If variable is a pointeur -> default value MUST be NULL.
            WriteLine($"{Type.CompiledName} {Variable.CompiledName} = NULL;")
        Else
            WriteLine($"{Type.CompiledName} {Variable.CompiledName};")
        End If
        WriteVariableAssignation(Variable, DefaultValue)

        'Return variable
        Return Variable

    End Function

    '=======================================
    '======== NOTIFY VARIABLE EXIST ========
    '=======================================
    '
    ' Basically just for function arguments & type variables
    '
    Public Sub NotifyVariableExist(Variable As Variable)
        Variables.Add(Variable)
    End Sub

    '======================================
    '======== CREATE TEMP VARIABLE ========
    '======================================
    Public Function CreateTemporaryVariable(Type As Type) As Variable

        'Create variable
        Dim Variable As New Variable("", Type) 'No name needed

        'Return variable
        Return Variable

    End Function

    '============================
    '======== WRITE LINE ========
    '============================
    Public Sub WriteLine(Line As String)
        Lines.Add(Line)
    End Sub

    '============================================
    '======== WRITE VARIABLE ASSIGNATION ========
    '============================================
    '   
    ' WARNING: This function does not check if the type of the new value is that of the variable.
    '
    Public Sub WriteVariableAssignation(Variable As Variable, Value As String)
        WriteLine(Variable.Type.SetVariable(Variable.CompiledName, Value))
    End Sub

    '=============================================
    '======== WRITE PROPERTIE ASSIGNATION ========
    '=============================================
    '
    ' WARNING: This function does not check if the type of the new value is that of the propertie, neighter if we are in a class.
    '
    Public Sub WritePropertieAssignation(Propertie As Propertie, Value As String)
        WriteLine(Propertie.AcessName & " = " & Value & ";")
    End Sub

    '========================
    '======== INSERT ========
    '========================
    Public Sub Insert(Lines As IEnumerable(Of String))
        Me.Lines.AddRange(Lines)
    End Sub

    '==============================
    '======== WRITE RETURN ========
    '==============================
    Public Sub WriteReturn(Value As String)

        'Dereference local variables
        WriteLine(GenerateVariableDeferencing())

        'Compile return
        Me.Lines.Add("return " & Value & ";")

    End Sub

    '================================
    '======== ATTACHED CLASS ========
    '================================
    Private _AttachedClass As ClassType
    Public ReadOnly Property AttachedClass As ClassType
        Get
            For Each Upper As Scope In Uppers
                If Upper._AttachedClass IsNot Nothing Then
                    Return Upper._AttachedClass
                End If
            Next
            Return Nothing
        End Get
    End Property
    Public Sub AttachClass(Type As ClassType)
        _AttachedClass = Type
    End Sub


    '====================================
    '======== HAS ATTACHED CLASS ========
    '====================================
    Public ReadOnly Property HasAttachedClass As Boolean
        Get
            Return AttachedClass IsNot Nothing
        End Get
    End Property


    '================================
    '======== GET ADDRESS OF ========
    '================================
    Private Shared TempPrimitiveValueKeeper As Integer = 0
    Public Function GetAdressOf(Value As String, Type As PrimitiveClassType) As String

        'Pattern to check if a string is a direct variable access in C
        Dim pattern As String = "^[a-zA-Z_][a-zA-Z0-9_]*(->)?[a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z_][a-zA-Z0-9_]*)*$"

        'Direct access (just add '&')
        If System.Text.RegularExpressions.Regex.IsMatch(Value, pattern) Then
            Return "&" & Value
        End If

        'Create a temp variable
        TempPrimitiveValueKeeper += 1
        Dim VarName As String = "keeper" & TempPrimitiveValueKeeper.ToString()
        WriteLine(Type.CompiledName & " " & VarName & " = " & Value & ";")
        Return "&" & VarName

    End Function

End Class
