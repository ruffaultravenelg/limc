Namespace Context
    Public Class FunctionScope
        Inherits Scope

        Private ReadOnly ArgumentTypes As New List(Of TypeSystem.Type)
        Private ReadOnly BodyScope As Scope
        Public ReadOnly Property ReturnType As TypeSystem.Type
            Get
                If TypeOf BodyScope Is ReturnableScope Then
                    Return DirectCast(BodyScope, ReturnableScope).ReturnType
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly GeneratedFunction As CodeGen.Function
        Private Node As AST.FunctionConstruct

        Public Sub New(Parent As Context, Node As AST.FunctionConstruct)
            MyBase.New(Parent, Node.Location)
            Me.Node = Node

            ' Arguments
            Dim Arguments As New List(Of Tuple(Of String, String))
            For Each Arg As AST.ArgumentNode In Node.Arguments
                Dim Var As VariableData = CreateVariable(Arg.ArgumentName, Arg.ArgumentType.GetAssociatedType(Parent), Arg.Location)
                Arguments.Add(New Tuple(Of String, String)(Var.CompiledName, Var.Type.cRepresentation))
                ArgumentTypes.Add(Var.Type)
            Next

            ' Register this function to final file
            GeneratedFunction = New CodeGen.Function(
                Node.Name, ' "myFunction"
                CodeGen.Namer.Function(Node.Name), ' "ad_f"
                Arguments ' { ("arg", "type"), ("arg2", "type2"), ... }
            )
            CodeGen.RegisterFunction(GeneratedFunction)

            ' Create body scope
            Dim ContainsReturn As Boolean = Node.DoContainsStatement(Of AST.ReturnStatement)
            If ContainsReturn Then
                BodyScope = New ReturnableScope(Me, Location)
            Else
                BodyScope = New Scope(Me, Location)
            End If

        End Sub

        ' Compile body (for Lazy.Function)
        Public Sub CompileBody()

            'Compile to the body scope
            For Each Statement In Node.Body
                Statement.Compile(BodyScope)
            Next

            'Update the generated function
            GeneratedFunction.SetReturnType(If(ReturnType Is Nothing, "void", DirectCast(BodyScope, ReturnableScope).ReturnType.cRepresentation))
            GeneratedFunction.AppendBody(BodyScope.GetLines())

        End Sub

        'Function type
        Public ReadOnly Property AssociatedFunctionType As TypeSystem.FunType
            Get
                Return TypeSystem.FunType.From(ArgumentTypes, ReturnType)
            End Get
        End Property

    End Class

End Namespace