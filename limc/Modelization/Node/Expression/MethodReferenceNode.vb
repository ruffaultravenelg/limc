Public Class MethodReferenceNode
    Inherits ExpressionNode
    Implements ProcedureSelectorNode

    '========================
    '======== PARENT ========
    '========================
    Private Parent As ExpressionNode

    '=====================================
    '======== METHOD INFORMATIONS ========
    '=====================================
    Private MethodName As String
    Private PassedGenericTypes As IEnumerable(Of TypeNode)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Parent As ExpressionNode, MethodName As String, PassedGenericTypes As IEnumerable(Of TypeNode))
        MyBase.New(Location)

        'Set values
        Me.Parent = Parent
        Me.MethodName = MethodName
        Me.PassedGenericTypes = PassedGenericTypes

    End Sub

    '=============================================
    '======== COMPILE PASSED GENERIC TYPE ========
    '=============================================
    Private Function CompilePassedGenericTypes(Scope As Scope) As List(Of Type)

        'Create the result
        Dim Result As New List(Of Type)

        'Compile each argument
        For Each Argument As TypeNode In PassedGenericTypes
            Result.Add(Argument.AssociatedType(Scope))
        Next

        'Return result
        Return Result

    End Function

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'Compile types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search method
        Try
            Return ParentType.Method(MethodName, GenericTypes).SignatureType
        Catch ex As UnableToChooseProcedure
            Throw New LocalizedException("There are several """ & MethodName & Type.StringifyListOfType(GenericTypes) & """ methods.", "The name is ambiguous because it can refer to multiple methods.", Me.Location)
        Catch ex As UnableToFindProcedure
            Throw New LocalizedException("The """ & MethodName & Type.StringifyListOfType(GenericTypes) & """ method cannot be found.", "No method is named """ & MethodName & """ with " & GenericTypes.Count.ToString() & " generic types exist within the """ & ParentType.ToString() & """ type.", Me.Location)
        End Try

    End Function

    Protected Overrides Function CanReturnType(Request As Type, Scope As Scope) As Boolean

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'Compile types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search method
        If TypeOf Request Is FunctionSignatureType AndAlso ParentType.HasMethod(MethodName, GenericTypes, DirectCast(Request, FunctionSignatureType)) Then
            Return True
        End If

        'Cannot
        Return False

    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'Compile types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Method
        Try

            'Get targeted function
            Dim TargetedMethod As CMethod = ParentType.Method(MethodName, GenericTypes)

            'Return variable
            Return TargetedMethod.SignatureType.NewMethodCompiledName & "(" & Parent.Compile(Scope) & ", " & TargetedMethod.CompiledName & ")"

        Catch ex As UnableToChooseProcedure
            Throw New LocalizedException("There are several """ & MethodName & Type.StringifyListOfType(GenericTypes) & """ methods.", "The name is ambiguous because it can refer to multiple methods.", Me.Location)
        Catch ex As UnableToFindProcedure
        End Try

        'Not found
        Throw New LocalizedException("The """ & MethodName & Type.StringifyListOfType(GenericTypes) & """ method cannot be found.", "No method is named """ & MethodName & """ with " & GenericTypes.Count.ToString() & " generic types exist within the """ & ParentType.ToString() & """ type.", Me.Location)

    End Function

    Protected Overrides Function Assemble(Scope As Scope, RequestedType As Type) As String

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'Compile types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Method
        If TypeOf RequestedType Is FunctionSignatureType Then
            Try

                'Get targeted function
                Dim TargetedMethod As CMethod = ParentType.Method(MethodName, GenericTypes, RequestedType)

                'Return variable
                Return TargetedMethod.SignatureType.NewMethodCompiledName & "(" & Parent.Compile(Scope) & ", " & TargetedMethod.CompiledName & ")"

            Catch ex As UnableToChooseProcedure
                Throw New LocalizedException("There are several """ & MethodName & Type.StringifyListOfType(GenericTypes) & """ methods.", "The name is ambiguous because it can refer to multiple methods.", Me.Location)
            Catch ex As UnableToFindProcedure
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

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'Compile types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function
        Try
            Return ParentType.Method(MethodName, GenericTypes)
        Catch ex As SearchProcedureException
        End Try

        'Nothing found
        Return Nothing

    End Function

    ' Searches for a procedure using the given arguments, returns Nothing if no function is found.
    Private Function GetProcedureWithHelpOfArgs(Scope As Scope, ProvidedArguments As IEnumerable(Of ExpressionNode)) As ICompiledProcedure Implements ProcedureSelectorNode.GetProcedureWithHelpOfArgs

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'Compile types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function
        Try
            Return ParentType.Method(Scope, MethodName, GenericTypes, ProvidedArguments)
        Catch ex As SearchProcedureException
        End Try

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
        Dim Arguments As String = Scope.GetAdressOf(Parent.Compile(Scope), Parent.ReturnType(Scope))
        For i As Integer = 0 To Procedure.Arguments.Count - 1

            Dim WantedType As Type = Procedure.Arguments(i)
            Dim PassedArgument As ExpressionNode = ProvidedArguments(i)

            If Not PassedArgument.CanReturn(WantedType, Scope) Then
                Throw New LocalizedException("The expected type was """ & WantedType.ToString() & """", "This argument cannot return the """ & WantedType.ToString() & """ type desired by the method.", ProvidedArguments(i).Location)
            End If

            Arguments &= ", " & PassedArgument.Compile(Scope, WantedType)

        Next

        'Compile call
        Return Procedure.CompiledName & "(" & Arguments & ")"

    End Function

End Class
