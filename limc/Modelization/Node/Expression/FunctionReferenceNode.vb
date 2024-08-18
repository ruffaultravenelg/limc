Public Class FunctionReferenceNode
    Inherits ExpressionNode
    Implements ProcedureSelectorNode

    '===============================
    '======== FUNCTION NAME ========
    '===============================
    Private FunctionName As String

    '==========================================
    '======== PASSED GENERIC ARGUMENTS ========
    '==========================================
    Private PassedGenericArguments As List(Of TypeNode)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, FunctionName As String, PassedGenericArguments As List(Of TypeNode))
        MyBase.New(Location)

        'Set values
        Me.FunctionName = FunctionName
        Me.PassedGenericArguments = PassedGenericArguments

    End Sub

    '=============================================
    '======== COMPILE PASSED GENERIC TYPE ========
    '=============================================
    Private Function CompilePassedGenericTypes(Scope As Scope) As List(Of Type)

        'Create the result
        Dim Result As New List(Of Type)

        'Compile each argument
        For Each Argument As TypeNode In PassedGenericArguments
            Result.Add(Argument.AssociatedType(Scope))
        Next

        'Return result
        Return Result

    End Function

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        'Get generic types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function
        Try
            Return Me.Location.File.Function(FunctionName, GenericTypes).SignatureType
        Catch ex As SearchProcedureException
            Throw New LocalizedException($"The ""{FunctionName}{Type.StringifyListOfType(GenericTypes)}"" function/method cannot be found in this scope.", $"No function or method named ""{FunctionName}"" exist or have {GenericTypes.Count} generic types.", Me.Location)
        End Try

    End Function

    Protected Overrides Function CanReturnType(Request As Type, Scope As Scope) As Boolean

        'Get generic types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function
        If TypeOf Request Is FunctionSignatureType AndAlso Me.Location.File.HasFunction(FunctionName, GenericTypes, DirectCast(Request, FunctionSignatureType)) Then
            Return True
        End If

        'Cannot
        Return False

    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        'Get generic types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function just by name
        Try

            'Get targeted function
            Dim TargetedFunction As CFunction = Me.Location.File.Function(FunctionName, GenericTypes)

            'Return variable
            Return TargetedFunction.SignatureType.NewFuncCompiledName & "(" & TargetedFunction.CompiledName & ")"

        Catch ex As SearchProcedureException
            Throw New LocalizedException($"The ""{FunctionName}{Type.StringifyListOfType(GenericTypes)}"" function/method cannot be found in this scope.", $"No function or method named ""{FunctionName}"" exist or have {GenericTypes.Count} generic types.", Me.Location)
        End Try

    End Function

    Protected Overrides Function Assemble(Scope As Scope, RequestedType As Type) As String

        'Get generic types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function
        If TypeOf RequestedType Is FunctionSignatureType Then
            Try
                Dim Result As CFunction = Me.Location.File.Function(FunctionName, GenericTypes, DirectCast(RequestedType, FunctionSignatureType))
                Return DirectCast(RequestedType, FunctionSignatureType).NewFuncCompiledName & "(" & Result.CompiledName & ")"
            Catch ex As SearchProcedureException
            End Try
        End If

        'Cannot
        Return Nothing

    End Function

    '=============================
    '======== DIRECT CALL ========
    '=============================

    ' Allows you to search for a procedure without using the given arguments, returns Nothing if no unique function is found.
    Private Function GetProcedureByYourself(Scope As Scope) As CompiledProcedure Implements ProcedureSelectorNode.GetProcedureByYourself

        'Compile types
        Dim PassedGenericTypes As New List(Of Type)
        For Each PassedGenericType As TypeNode In PassedGenericArguments
            PassedGenericTypes.Add(PassedGenericType.AssociatedType(Scope))
        Next

        'Search function
        Try
            Return Me.Location.File.Function(FunctionName, PassedGenericTypes)
        Catch ex As SearchProcedureException
            Return Nothing
        End Try

    End Function

    ' Searches for a procedure using the given arguments, returns Nothing if no function is found.
    Private Function GetProcedureWithHelpOfArgs(Scope As Scope, ProvidedArguments As IEnumerable(Of ExpressionNode)) As CompiledProcedure Implements ProcedureSelectorNode.GetProcedureWithHelpOfArgs

        'Compile types
        Dim PassedGenericTypes As New List(Of Type)
        For Each PassedGenericType As TypeNode In PassedGenericArguments
            PassedGenericTypes.Add(PassedGenericType.AssociatedType(Scope))
        Next

        'Search function
        Try
            Return Me.Location.File.Function(Scope, FunctionName, PassedGenericTypes, ProvidedArguments)
        Catch ex As SearchProcedureException
            Return Nothing
        End Try

    End Function

    ' Compiles a call to the targeted procedure
    Function CompileCallTo(Procedure As CompiledProcedure, Scope As Scope, ProvidedArguments As IEnumerable(Of ExpressionNode)) As String Implements ProcedureSelectorNode.CompileCallTo

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
