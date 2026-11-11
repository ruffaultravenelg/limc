Imports limc.Symbol

Namespace Environment
    Friend Class ReturnableEnv
        Inherits BlockEnv

        Private ExpectedType As TypeSymbol

        Public Sub New(Parent As Env, ExpectedType As TypeSymbol)
            MyBase.New(Parent)
            Me.ExpectedType = ExpectedType
        End Sub

        ' Throw an exception if no return statement is accepted / the type is wrong
        Public Shared Sub ValidateReturn(Ctx As Env, ReturnValue As HIR.ExpressionNode, ReturnLocation As Location)
            For Each e In Ctx.AllParents()
                If TypeOf e Is ReturnableEnv Then
                    If DirectCast(e, ReturnableEnv).ExpectedType = ReturnValue.ExpressionReturnType Then
                        Exit Sub
                    Else
                        Throw New TypeMismatchException("R02", ReturnValue.ExpressionReturnType, DirectCast(e, ReturnableEnv).ExpectedType, ReturnValue.Location)
                    End If
                End If
            Next
            Throw New SyntaxException("R01", "This context doesn't accept a return statement", ReturnLocation)
        End Sub

    End Class
End Namespace