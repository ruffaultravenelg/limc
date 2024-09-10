Public Class ChildNode
    Inherits ExpressionNode
    Implements ProcedureSelectorNode

    '========================
    '======== PARENT ========
    '========================
    Private Parent As ExpressionNode

    '============================
    '======== CHILD NAME ========
    '============================
    Private ChildName As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Parent As ExpressionNode, ChildName As String)
        MyBase.New(Location)

        'Set values
        Me.Parent = Parent
        Me.ChildName = ChildName

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'TODO: search getter

        'Search method
        Try
            Return ParentType.Method(ChildName, {}).SignatureType
        Catch ex As UnableToChooseProcedure
            Throw New LocalizedException("There are several """ & ChildName & """ methods.", "The name is ambiguous because it can refer to multiple methods.", Me.Location)
        Catch ex As UnableToFindProcedure
            Throw New LocalizedException("The """ & ChildName & """ element cannot be found.", "No variable or function is named """ & ChildName & """ in the """ & ParentType.ToString() & """ type. Check that the element is accessible.", Me.Location)
        End Try

    End Function

    Protected Overrides Function CanReturnType(Request As Type, Scope As Scope) As Boolean

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'TODO: search getter

        'Search method
        If TypeOf Request Is FunctionSignatureType AndAlso ParentType.HasMethod(ChildName, {}, DirectCast(Request, FunctionSignatureType)) Then
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

        'TODO: search getter

        'Method
        Try

            'Get targeted function
            Dim TargetedMethod As CMethod = ParentType.Method(ChildName, {})

            'Return variable
            Return TargetedMethod.SignatureType.NewMethodCompiledName & "(" & Parent.Compile(Scope) & ", " & TargetedMethod.CompiledName & ")"

        Catch ex As UnableToChooseProcedure
            Throw New LocalizedException("There are several """ & ChildName & """ methods.", "The name is ambiguous because it can refer to multiple methods.", Me.Location)
        Catch ex As UnableToFindProcedure
        End Try

        'Not found
        Throw New LocalizedException("The """ & ChildName & """ element cannot be found.", "No variable or function is named """ & ChildName & """ in the """ & ParentType.ToString() & """ type. Check that the element is accessible.", Me.Location)

    End Function

    Protected Overrides Function Assemble(Scope As Scope, RequestedType As Type) As String

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'TODO: search getter

        'Method
        If TypeOf RequestedType Is FunctionSignatureType Then
            Try

                'Get targeted function
                Dim TargetedMethod As CMethod = ParentType.Method(ChildName, {}, RequestedType)

                'Return variable
                Return TargetedMethod.SignatureType.NewMethodCompiledName & "(" & Parent.Compile(Scope) & ", " & TargetedMethod.CompiledName & ")"

            Catch ex As UnableToChooseProcedure
                Throw New LocalizedException("There are several """ & ChildName & """ methods.", "The name is ambiguous because it can refer to multiple methods.", Me.Location)
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

        'Search function
        Try
            Return ParentType.Method(ChildName, {})
        Catch ex As SearchProcedureException
        End Try

        'Nothing found
        Return Nothing

    End Function

    ' Searches for a procedure using the given arguments, returns Nothing if no function is found.
    Private Function GetProcedureWithHelpOfArgs(Scope As Scope, ProvidedArguments As IEnumerable(Of ExpressionNode)) As ICompiledProcedure Implements ProcedureSelectorNode.GetProcedureWithHelpOfArgs

        'Get parent type
        Dim ParentType As Type = Parent.ReturnType(Scope)

        'Search function
        Try
            Return ParentType.Method(Scope, ChildName, {}, ProvidedArguments)
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

        'Compile parent
        Dim CompiledParent As String
        Dim ParentType As Type = Parent.ReturnType(Scope)
        CompiledParent = Parent.Compile(Scope)

        If TypeOf ParentType IsNot HeapClassType Then
            CompiledParent = Scope.GetAdressOf(Parent.Compile(Scope), ParentType)
        End If

        'Compile args
        Dim Arguments As String = CompiledParent
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
