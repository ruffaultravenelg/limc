Imports limc.CodeGen
Imports limc.Context
Imports limc.TypeSystem

Namespace Lazy
    Public Class UserRelation
        Inherits Lazy.Relation

        ' Function name
        Public Overrides ReadOnly Property Type As RelationType
            Get
                Return Node.Type
            End Get
        End Property

        ' Function arguments types
        Private _ArgumentTypes As IEnumerable(Of TypeSystem.Type) = Nothing
        Public Overrides ReadOnly Property ArgumentsTypes As IEnumerable(Of TypeSystem.Type)
            Get
                If _ArgumentTypes Is Nothing Then
                    _ArgumentTypes = Node.Arguments.Select(Function(a) a.ArgumentType.GetAssociatedType(RelationScope))
                End If
                Return _ArgumentTypes
            End Get
        End Property

        ' Function return type
        Private _ReturnType As TypeSystem.Type = Nothing
        Public Overrides ReadOnly Property ReturnType As TypeSystem.Type
            Get
                If Node.ReturnType Is Nothing Then
                    Return Nothing
                Else
                    If _ReturnType Is Nothing Then
                        _ReturnType = Node.ReturnType.GetAssociatedType(RelationScope)
                    End If
                    Return _ReturnType
                End If
            End Get
        End Property

        ' Not compiled
        Private Node As AST.RelationConstruct
        Private RelationScope As Context.Scope

        ' Constructor
        Public Sub New(ParentType As TypeSystem.Type, Node As AST.RelationConstruct)
            MyBase.New(ParentType)
            Me.Node = Node

            If Not TypeOf ParentType Is ITypeWithBoneContext Then
                Throw New InternalError()
            End If

            ' Register type inner context, if do not use self (add, sub, div, ect) add a instance variable
            If RelationUseSelf(Type) Then
                RelationScope = New Scope(DirectCast(ParentType, ITypeWithBoneContext).InnerContext, Node.Location)
            Else
                RelationScope = New Scope(DirectCast(ParentType, ITypeWithBoneContext).BoneContext, Node.Location)
                RelationScope.RegisterVariable(Node.InstanceArgument.ArgumentName, New VariableData(Constants.INSTANCE_ARGUMENT_NAME, ParentType), Node.InstanceArgument.Location)
            End If

            ' Verify
            Verify()
        End Sub

        ' Create compiled relation (to provide compiledName)
        Protected Overrides Function GenerateCompiledRelation() As ContextedFunction

            Dim ArgumentSignature As New List(Of String)
            If Type = TypeSystem.RelationType.RELATION_BRACKETS_PTR Then
                ArgumentSignature.Add($"{ParentType.pointerCRepresentation} {Constants.INSTANCE_ARGUMENT_NAME}")
            Else
                ArgumentSignature.Add($"{ParentType.cRepresentation} {Constants.INSTANCE_ARGUMENT_NAME}")
            End If
            For Each Arg In Node.Arguments
                Dim Variable As New VariableData(Namer.Variable(Arg.ArgumentName), Arg.ArgumentType.GetAssociatedType(RelationScope))
                RelationScope.RegisterVariable(Arg.ArgumentName, Variable, Arg.Location)
                ArgumentSignature.Add($"{Variable.Type.cRepresentation} {Variable.CompiledName}")
            Next

            Dim ReturnTypeSignature As String = If(Node.ReturnType Is Nothing, "void", ReturnType.cRepresentation)
            If Type = RelationType.RELATION_BRACKETS_PTR Then
                ReturnTypeSignature = ReturnType.pointerCRepresentation
            End If

            Return New OwnContextFunction($"{ParentType.ToString()} -> {Type.ToString()}", Node.Location, ArgumentSignature, ReturnTypeSignature)

        End Function

        ' Compile body (after compiledName is known)
        Protected Overrides Sub CompileBody()
            If ReturnType IsNot Nothing Then
                RelationScope = New Context.MustReturnScope(RelationScope, Node.Location, ReturnType) 'TODO: this create another scope, update this later
            End If

            Dim Writer As New CWriter()
            For Each Statement In Node.Body
                Statement.Compile(Writer, RelationScope)
            Next
            DirectCast(GeneratedRelation, OwnContextFunction).AppendBody(Writer.GetLines())
        End Sub

        Private Sub Verify()

            ' Verify first argument is itself
            If (Not RelationUseSelf(Type)) AndAlso Node.InstanceArgument.ArgumentType.GetAssociatedType(RelationScope) <> ParentType Then
                Throw New TypeMismatchError(ParentType, Node.InstanceArgument.ArgumentType.GetAssociatedType(RelationScope), Node.InstanceArgument.Location)
            End If

            ' Check with type
            Select Case Type
                Case RelationType.RELATION_ADD, RelationType.RELATION_SUB, RelationType.RELATION_MULT, RelationType.RELATION_DIV, RelationType.RELATION_MODULO,
                     RelationType.RELATION_LESSTHAN, RelationType.RELATION_LESSTHANEQUAL, RelationType.RELATION_GREATERTHAN, RelationType.RELATION_GREATERTHANEQUAL,
                     RelationType.RELATION_EQUAL
                    If Node.Arguments.Count <> 1 Then
                        Throw New SyntaxError($"The ""{Type.ToString()}"" relation must have exactly two argument.", Node.Location) ' "two" argument because the first is in .InstanceArgument
                    End If

                    If ReturnType Is Nothing Then
                        Throw New SyntaxError("This relation type must return a value.", Node.Location)
                    End If

                Case RelationType.RELATION_UNARY_MINUS
                    If Node.Arguments.Count <> 0 Then
                        Throw New SyntaxError("This relation must have only one argument.", Node.Location) ' "one" argument because the first is in .InstanceArgument")
                    End If

                Case RelationType.RELATION_BRACKETS
                    If ReturnType Is Nothing Then
                        Throw New SyntaxError("This relation type must return a value.", Node.Location)
                    End If

                Case RelationType.RELATION_SET_BRACKETS
                    If Node.Arguments.Count < 2 Then
                        Throw New SyntaxError($"The ""{Type.ToString()}"" relation must have at least two argument.", Node.Location) ' "two" argument because the first is in .InstanceArgument
                    End If
                    If ReturnType IsNot Nothing Then
                        Throw New SyntaxError("This relation may not return a value.", Node.Location)
                    End If

            End Select

        End Sub

    End Class
End Namespace