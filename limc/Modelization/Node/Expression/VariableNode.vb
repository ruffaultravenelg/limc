Imports System.Runtime.InteropServices

Public Class VariableNode
    Inherits ExpressionNode
    Implements ProcedureSelectorNode

    '===============================
    '======== VARIABLE NAME ========
    '===============================
    Private VariableName As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Value As Token)
        MyBase.New(Value.Location)

        'Set values
        Me.VariableName = Value.Value

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        'Search variable
        For Each UpperScope As Scope In Scope.Uppers
            For Each Var As Variable In UpperScope.Variables
                If Var.Name = VariableName Then
                    Return Var.Type
                End If
            Next
        Next

        'Search for properties/method
        If Scope.HasAttachedClass Then

            'Get type
            Dim Type As ClassType = Scope.AttachedClass

            ' Propertie
            For Each Propertie As Propertie In Type.Properties
                If Propertie.Name = VariableName Then
                    Return Propertie.Type
                End If
            Next

            'Method
            Try
                Return DirectCast(Type, Type).Method(VariableName, {}).SignatureType
            Catch ex As UnableToChooseProcedure
                Throw New LocalizedException("There are several """ & VariableName & """ methods.", "The name is ambiguous because it can refer to multiple methods. Specify with an explicit type.", Me.Location)
            Catch ex As UnableToFindProcedure
            End Try

        End If

        'Search function just by name
        Try
            Return Me.Location.File.Function(VariableName, {}).SignatureType
        Catch ex As UnableToChooseProcedure
            Throw New LocalizedException("There are several """ & VariableName & """ functions.", "The name is ambiguous because it can refer to multiple functions. Specify with an explicit type.", Me.Location)
        Catch ex As UnableToFindProcedure
            Throw New LocalizedException("The """ & VariableName & """ element cannot be found in this scope.", "No variable or function is named """ & VariableName & """. Check that the element is accessible.", Me.Location)
        End Try

    End Function

    Protected Overrides Function CanReturnType(Request As Type, Scope As Scope) As Boolean

        'Search variable
        For Each UpperScope As Scope In Scope.Uppers
            For Each Var As Variable In UpperScope.Variables
                If Var.Name = VariableName AndAlso Var.Type = Request Then
                    Return True
                End If
            Next
        Next


        'Search for properties/method
        If Scope.HasAttachedClass Then

            'Get type
            Dim Type As ClassType = Scope.AttachedClass

            ' Propertie
            For Each Propertie As Propertie In Type.Properties
                If Propertie.Name = VariableName AndAlso Propertie.Type = Request Then
                    Return True
                End If
            Next

            'Method
            If TypeOf Request Is FunctionSignatureType AndAlso DirectCast(Type, Type).HasMethod(VariableName, {}, DirectCast(Request, FunctionSignatureType)) Then
                Return True
            End If

        End If

        'Search function
        If TypeOf Request Is FunctionSignatureType AndAlso Me.Location.File.HasFunction(VariableName, {}, DirectCast(Request, FunctionSignatureType)) Then
            Return True
        End If

        'Cannot
        Return False

    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        'Search variable
        For Each UpperScope As Scope In Scope.Uppers
            For Each Var As Variable In UpperScope.Variables
                If Var.Name = VariableName Then
                    Return Var.CompiledName
                End If
            Next
        Next

        'Search for properties/method
        If Scope.HasAttachedClass Then

            'Get type
            Dim Type As ClassType = Scope.AttachedClass

            ' Propertie
            For Each Propertie As Propertie In Type.Properties
                If Propertie.Name = VariableName Then
                    Return Propertie.AcessName
                End If
            Next

            'Method
            Try

                'Get targeted function
                Dim TargetedMethod As CMethod = DirectCast(Type, Type).Method(VariableName, {})

                'Return variable
                Return TargetedMethod.SignatureType.NewMethodCompiledName & "(self, " & TargetedMethod.CompiledName & ")"

            Catch ex As UnableToChooseProcedure
                Throw New LocalizedException("There are several """ & VariableName & """ methods.", "The name is ambiguous because it can refer to multiple methods. Specify with an explicit type.", Me.Location)
            Catch ex As UnableToFindProcedure
            End Try

        End If

        'Search function just by name
        Try

            'Get targeted function
            Dim TargetedFunction As CFunction = Me.Location.File.Function(VariableName, {})

            'Return variable
            Return TargetedFunction.SignatureType.NewFuncCompiledName & "(" & TargetedFunction.CompiledName & ")"

        Catch ex As UnableToChooseProcedure
            Throw New LocalizedException("There are several """ & VariableName & """ functions.", "The name is ambiguous because it can refer to multiple functions. Specify with an explicit type.", Me.Location)
        Catch ex As UnableToFindProcedure
            Throw New LocalizedException("The """ & VariableName & """ element cannot be found in this scope.", "No variable or function is named """ & VariableName & """. Check that the element is accessible.", Me.Location)
        End Try

    End Function

    Protected Overrides Function Assemble(Scope As Scope, RequestedType As Type) As String

        'Search variable
        For Each UpperScope As Scope In Scope.Uppers
            For Each Var As Variable In UpperScope.Variables
                If Var.Name = VariableName AndAlso Var.Type = RequestedType Then
                    Return Var.CompiledName
                End If
            Next
        Next

        'Search for properties/method
        If Scope.HasAttachedClass Then

            'Get type
            Dim Type As ClassType = Scope.AttachedClass

            ' Propertie
            For Each Propertie As Propertie In Type.Properties
                If Propertie.Name = VariableName AndAlso Propertie.Type = RequestedType Then
                    Return Propertie.AcessName
                End If
            Next

            'Method
            If TypeOf RequestedType Is FunctionSignatureType Then
                Try

                    'Get targeted function
                    Dim TargetedMethod As CMethod = DirectCast(Type, Type).Method(VariableName, {})

                    'Return variable
                    Return TargetedMethod.SignatureType.NewMethodCompiledName & "(self, " & TargetedMethod.CompiledName & ")"

                Catch ex As UnableToChooseProcedure
                    Throw New LocalizedException("There are several """ & VariableName & """ methods.", "The name is ambiguous because it can refer to multiple methods. Specify with an explicit type.", Me.Location)
                Catch ex As UnableToFindProcedure
                End Try

            End If

        End If


            'Search function just by name
            If TypeOf RequestedType Is FunctionSignatureType Then
            Try

                'Get targeted function
                Dim TargetedFunction As CFunction = Me.Location.File.Function(VariableName, {}, DirectCast(RequestedType, FunctionSignatureType))

                'Return variable
                Return TargetedFunction.SignatureType.NewFuncCompiledName & "(" & TargetedFunction.CompiledName & ")"

            Catch ex As UnableToChooseProcedure
                Throw New LocalizedException("There are several """ & VariableName & """ functions.", "The name is ambiguous because it can refer to multiple functions. Specify with an explicit type.", Me.Location)
            Catch ex As UnableToFindProcedure
                Throw New LocalizedException("The """ & VariableName & """ element cannot be found in this scope.", "No variable or function is named """ & VariableName & """. Check that the element is accessible.", Me.Location)
            End Try
        End If

        'Return nothing
        Return Nothing

    End Function

    '=============================
    '======== DIRECT CALL ========
    '=============================

    ' Allows you to search for a procedure without using the given arguments, returns Nothing if no unique function is found.
    Private Function GetProcedureByYourself(Scope As Scope) As ICompiledProcedure Implements ProcedureSelectorNode.GetProcedureByYourself

        'Search function
        Try
            Return Me.Location.File.Function(VariableName, {})
        Catch ex As SearchProcedureException
        End Try

        'Search method
        If Scope.HasAttachedClass Then
            Try
                Return DirectCast(Scope.AttachedClass, Type).Method(VariableName, {})
            Catch ex As SearchProcedureException
            End Try
        End If

        'Nothing found
        Return Nothing

    End Function

    ' Searches for a procedure using the given arguments, returns Nothing if no function is found.
    Private Function GetProcedureWithHelpOfArgs(Scope As Scope, ProvidedArguments As IEnumerable(Of ExpressionNode)) As ICompiledProcedure Implements ProcedureSelectorNode.GetProcedureWithHelpOfArgs

        'Search function
        Try
            Return Me.Location.File.Function(Scope, VariableName, {}, ProvidedArguments)
        Catch ex As SearchProcedureException
        End Try

        'Search method
        If Scope.HasAttachedClass Then
            Try
                Return DirectCast(Scope.AttachedClass, Type).Method(Scope, VariableName, {}, ProvidedArguments)
            Catch ex As SearchProcedureException
            End Try
        End If

        'Nothing found
        Return Nothing

    End Function

    ' Compiles a call to the targeted procedure
    Function CompileCallTo(Procedure As ICompiledProcedure, Scope As Scope, ProvidedArguments As IEnumerable(Of ExpressionNode)) As String Implements ProcedureSelectorNode.CompileCallTo

        'Argument count
        If Procedure.Arguments.Count > ProvidedArguments.Count Then
            Throw New LocalizedException("Not enough arguments", Procedure.Arguments.Count & " arguments were expected instead of " & ProvidedArguments.Count & ".", Me.Location)
        ElseIf Procedure.Arguments.Count < ProvidedArguments.Count Then
            Throw New LocalizedException("Too many arguments", Procedure.Arguments.Count & " arguments were expected instead of " & ProvidedArguments.Count & ".", Me.Location)
        End If

        'Compile args
        Dim Arguments As String = ""
        For i As Integer = 0 To Procedure.Arguments.Count - 1

            Dim WantedType As Type = Procedure.Arguments(i)
            Dim PassedArgument As ExpressionNode = ProvidedArguments(i)

            If Not PassedArgument.CanReturn(WantedType, Scope) Then
                Throw New LocalizedException("The expected type was """ & WantedType.ToString() & """", "This argument cannot return the """ & WantedType.ToString() & """ type desired by the procedure.", ProvidedArguments(i).Location)
            End If

            Arguments &= ", " & PassedArgument.Compile(Scope, WantedType)

        Next
        If Arguments.StartsWith(", ") Then
            Arguments = Arguments.Substring(2)
        End If

        'If Fn is a Method then it's a method call in a class
        If TypeOf Procedure Is CMethod Then
            If Arguments = "" Then
                Arguments = "self"
            Else
                Arguments = "self, "
            End If
        End If

        'Compile call
        Return Procedure.CompiledName & "(" & Arguments & ")"

    End Function

End Class
