Public Class Scope

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

    '========================
    '======== RESULT ========
    '========================
    Private ReadOnly Lines As New List(Of String)
    Public ReadOnly Property Result As IEnumerable(Of String)
        Get
            Return Lines
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

        'Compile it
        If DefaultValue = Nothing Then
            Lines.Add($"{Type.CompiledName} {Variable.CompiledName} = {Type.DefaultValue};")
        Else
            Lines.Add($"{Type.CompiledName} {Variable.CompiledName} = {DefaultValue};")
        End If

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

        If TypeOf Variable.Type Is HeapType Then

            'Heap type -> garbage collector assignation
            'TODO
            Throw New NotImplementedException()

        Else

            'Primitive type -> normal assignation
            Lines.Add(Variable.CompiledName & " = " & Value & ";")

        End If

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

        'TODO: decrease all heap variable reference by one

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


    '=============================================
    '======== WRITE PROPERTIE ASSIGNATION ========
    '=============================================
    '
    ' WARNING: This function does not check if the type of the new value is that of the propertie.
    '
    Public Sub WritePropertieAssignation(Propertie As Propertie, Value As String)

        Lines.Add(Propertie.Setter(Value) & ";")

    End Sub

End Class
