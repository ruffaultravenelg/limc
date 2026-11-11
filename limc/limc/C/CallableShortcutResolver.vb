Imports limc.HIR

Namespace C
    Friend Class CallableShortcutResolver

        '
        ' [node](arg1, arg2)
        '
        ' [node] could be :
        '   function name
        '   variable (of type func<><>)
        '   method name (from type body)
        '   object.method
        '   object.variable (of type func<><>)
        '
        ' This function helps simplify a lot of calls
        '   For exemple for a "function name" it just returns the C function name instead of compiling a FunctionReferenceNode to unwrap ip just after
        '
        ' Arguments is used to insert "self" if needed
        '
        Public Shared Function ResolveCallable(Procedure As ExpressionNode, Arguments As List(Of String), fs As WriterWrapper, namer As Naming.ISymbolNamer, exprCom As ExpressionCompiler)
            Select Case True

                Case TypeOf Procedure Is FunctionReferenceExpression
                    Dim Callable As FunctionReferenceExpression = Procedure
                    Return namer.GetCompiledName(Callable.FuncSymbol)

                Case TypeOf Procedure Is ObjMethodReferenceExpression
                    Dim Callable As ObjMethodReferenceExpression = Procedure

                    Arguments.Insert(0, exprCom.CompileAsPointer(Callable.Element))

                    Return namer.GetCompiledName(Callable.Method)

                Case Else
                    Throw New NotImplementedException() 'TODO: handle unwrapping "function pointer" to handle stored object

            End Select
        End Function

    End Class
End Namespace