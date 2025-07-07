
Namespace Lim
    Public MustInherit Class Method

        Inherits Lim.Function

        Public Sub New(Base As Source.Function, GenericTypes As IEnumerable(Of Lim.Type), Optional ParentContext As Context = Nothing)
            MyBase.New(Base, GenericTypes, ParentContext)
        End Sub

        Public Function CompileMethodCall(Scope As Scope, Obj As ExpressionNode, PassedArguments As IEnumerable(Of ExpressionNode)) As String

            ' Get arguments types
            Dim ArgumentTypes As IEnumerable(Of Lim.Type) = ExpressionNode.GetTypesOfExpressions(PassedArguments, Scope)

            ' Compile
            Dim ArgsLocation As Location = Obj.Location
            If PassedArguments.Count > 0 Then
                ArgsLocation += PassedArguments.Last.Location
            End If
            Dim Args As String = Source.CallNode.CompileArguments(Arguments, PassedArguments, Scope, ArgsLocation)

            'Compile target
            Dim Target As String = CompileObjectForCall(Scope, Obj)

            'Return compiled call
            Return C.Function.WriteCall(CompiledName, Target, Args)

        End Function

        Protected Overridable Function CompileObjectForCall(scope As Scope, Obj As ExpressionNode) As String
            Return Obj.Compile(scope)
        End Function

    End Class
End Namespace