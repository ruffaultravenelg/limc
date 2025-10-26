Imports System.Text

Namespace Context
    Public Class FunctionScope
        Inherits Scope

        Public ReadOnly Property CompiledName As String

        Public Sub New(Parent As Context, Node As AST.FunctionConstruct)
            MyBase.New(Parent, Node.Location)
            CompiledName = CodeGen.Namer.Function(Node.Name)

            'Arguments
            Dim ArgumentString As New StringBuilder
            For Each Arg As AST.ArgumentNode In Node.Arguments
                Dim Var As VariableData = CreateVariable(Arg.ArgumentName, Arg.ArgumentType.GetAssociatedType(Parent), Arg.Location)
                If ArgumentString.Length > 0 Then
                    ArgumentString.Append(", ")
                End If
                ArgumentString.Append(Var.Type.cRepresentation)
                ArgumentString.Append(" "c)
                ArgumentString.Append(Var.CompiledName)
            Next

            'Compile body
            Dim BodyScope As Scope
            Dim ContainsReturn As Boolean = Node.DoContainsStatement(Of AST.ReturnStatement)
            If ContainsReturn Then
                BodyScope = New ReturnableScope(Me, Location)
            Else
                BodyScope = New Scope(Me, Location)
            End If
            For Each Statement In Node.Body
                Statement.Compile(BodyScope)
            Next

            'Create function signature : type name(type arg, ...)
            Dim Signature As New StringBuilder
            If ContainsReturn Then
                Signature.Append(DirectCast(BodyScope, ReturnableScope).ReturnType.cRepresentation)
            Else
                Signature.Append("void")
            End If
            Signature.Append(" "c)
            Signature.Append(CompiledName)
            Signature.Append("("c)
            Signature.Append(ArgumentString)
            Signature.Append(")"c)

            ' Register this function to final file
            CodeGen.RegisterFunction(New CodeGen.Function(Signature.ToString(), BodyScope.GetLines(), Node.Name))

        End Sub

    End Class

End Namespace