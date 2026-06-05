Namespace AST
    Public Class FunctionCallExpression
        Inherits ExpressionNode

        Private Target As ExpressionNode
        Private PassedArguments As IEnumerable(Of ExpressionNode)

        Public Sub New(Target As ExpressionNode, PassedArguments As IEnumerable(Of ExpressionNode), Location As Location)
            MyBase.New(Location)
            Me.Target = Target
            Me.PassedArguments = PassedArguments
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type

            ' Enum option
            If TypeOf Target Is IMissingValueEnumReference AndAlso PassedArguments.Count = 1 Then
                Dim EnumType = DirectCast(Target, IMissingValueEnumReference).TryGetEnumType(Context)
                If EnumType IsNot Nothing Then
                    Return EnumType
                End If
            End If

            ' Direct reference
            If TypeOf Target Is IFunctionReference Then
                Dim Func As Lazy.Function = DirectCast(Target, IFunctionReference).TryGetReferencedFunction(Context)
                If Func IsNot Nothing Then
                    If Func.ReturnType Is Nothing Then
                        Throw New SyntaxError($"The designated function is of type ""{Func.AssociatedFunctionType.ToString()}"" and does not return a value. However, this expression must return a value.", Location)
                    End If
                    Return Func.ReturnType
                End If
            End If

            If TypeOf Target Is IMethodReference Then
                Dim Meth As Lazy.Method = DirectCast(Target, IMethodReference).TryGetReferencedMethod(Context)
                If Meth IsNot Nothing Then
                    If Meth.ReturnType Is Nothing Then
                        Throw New SyntaxError($"The designated function is of type ""{Meth.AssociatedFunctionType.ToString()}"" and does not return a value. However, this expression must return a value.", Location)
                    End If
                    Return Meth.ReturnType
                End If
            End If

            ' Expression as a callable
            Dim FunctionType As TypeSystem.Type = Target.GetExpressionReturnType(Context)
            If TypeOf FunctionType IsNot TypeSystem.FunType Then
                Throw New TypeMismatchError("fun", FunctionType.ToString(), Location)
            End If

            Dim FunctionReturnType As TypeSystem.Type = DirectCast(FunctionType, TypeSystem.FunType).ReturnType
            If FunctionReturnType Is Nothing Then
                Throw New SyntaxError($"The designated function is of type ""{FunctionType.ToString()}"" and does not return a value. However, this expression must return a value.", Target.Location)
            End If

            Return FunctionReturnType

        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

            ' Enum option
            If TypeOf Target Is IMissingValueEnumReference AndAlso PassedArguments.Count = 1 Then
                Dim Result = DirectCast(Target, IMissingValueEnumReference).CompileEnumValue(Scope, Writer, PassedArguments(0))
                If Not String.IsNullOrEmpty(Result) Then
                    Return Result
                End If
            End If

            ' Direct reference
            If TypeOf Target Is IFunctionReference Then
                Dim Func As Lazy.Function = DirectCast(Target, IFunctionReference).TryGetReferencedFunction(Scope)
                If Func IsNot Nothing Then
                    Return Func.CompileCall(PassedArguments, Writer, Scope, Location)
                End If
            End If
            If TypeOf Target Is IMethodReference Then
                Dim Meth As Lazy.Method = DirectCast(Target, IMethodReference).TryGetReferencedMethod(Scope)
                If Meth IsNot Nothing Then
                    Return Meth.CompileCall(DirectCast(Target, IMethodReference).GetCompiledInstance(Writer, Scope), PassedArguments, Writer, Scope, Location)
                End If

            End If

            ' Expression as a callable
            Dim FunctionType As TypeSystem.Type = Target.GetExpressionReturnType(Scope)
            If TypeOf FunctionType IsNot TypeSystem.FunType Then
                Throw New TypeMismatchError("fun", FunctionType.ToString(), Location)
            End If

            Return DirectCast(FunctionType, TypeSystem.FunType).ExecuteProcedure(Writer, Scope, Target.CompileExpression(Writer, Scope), PassedArguments)

        End Function

    End Class
End Namespace