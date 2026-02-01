Namespace Lazy
    Public Class UserMethod
        Inherits Lazy.Method

        ' Not compiled
        Private Node As AST.FunctionConstruct
        Private BoneContext As Context.Context

        Private MethodScope As Context.Scope
        Private BodyScope As Context.Scope

        Private ShouldHaveAReturnValue As Boolean

        ' Constructor
        Public Sub New(ParentType As TypeSystem.Type, Node As AST.FunctionConstruct)
            MyBase.New(ParentType)
            Me.Node = Node

            ' Get bone context
            If TypeOf ParentType IsNot TypeSystem.ITypeWithBoneContext Then
                Throw New InternalError()
            End If
            Me.BoneContext = DirectCast(ParentType, TypeSystem.ITypeWithBoneContext).BoneContext

            Me.MethodScope = New Context.Scope(BoneContext, Node.Location)

            ShouldHaveAReturnValue = Node.ReturnType IsNot Nothing OrElse Node.DoContainsStatement(Of AST.ReturnStatement)()
            If ShouldHaveAReturnValue Then
                Dim ReturnableScope As New Context.ReturnableScope(MethodScope, Node.Location)
                If Node.ReturnType IsNot Nothing Then
                    ReturnableScope.DefineReturnType(Node.ReturnType.GetAssociatedType(BoneContext), Node.Location)
                End If
                Me.BodyScope = ReturnableScope
            Else
                Me.BodyScope = New Context.Scope(MethodScope, Node.Location)
            End If

        End Sub

        Public Overrides ReadOnly Property Name As String
            Get
                Return Node.Name
            End Get
        End Property

        Private _ArgumentTypes As IEnumerable(Of TypeSystem.Type) = Nothing
        Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
            Get
                If _ArgumentTypes Is Nothing Then
                    _ArgumentTypes = Node.Arguments.Select(Function(a) a.ArgumentType.GetAssociatedType(BoneContext))
                End If
                Return _ArgumentTypes
            End Get
        End Property

        Public Overrides ReadOnly Property ReturnType As TypeSystem.Type
            Get
                If ShouldHaveAReturnValue Then
                    Dim a = MyBase.GeneratedFunction 'Compile body: mega sus
                    Return DirectCast(BodyScope, Context.ReturnableScope).ReturnType
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Protected Overrides Function CompileGeneratedFunction() As CodeGen.PassingContextFunction

            ' Arguments
            Dim CompiledArguments As New List(Of String) From {$"{ParentType.cRepresentation} {Constants.INSTANCE_ARGUMENT_NAME}"}
            For Each Arg As AST.ArgumentNode In Node.Arguments
                Dim Var As VariableData = MethodScope.CreateVariable(Arg.ArgumentName, Arg.ArgumentType.GetAssociatedType(MethodScope), Arg.Location)
                CompiledArguments.Add($"{Var.Type.cRepresentation} {Var.CompiledName}")
            Next

            ' Create c function
            Dim CFun As New CodeGen.ContextedFunction(Name, CodeGen.Namer.Method(Name), CompiledArguments)

            ' Compile body
            For Each Statement In Node.Body
                Statement.Compile(BodyScope)
            Next

            ' Set return type
            If ShouldHaveAReturnValue Then
                CFun.SetReturnType(DirectCast(BodyScope, Context.ReturnableScope).ReturnType.cRepresentation)
            Else
                CFun.SetReturnType("void")
            End If
            CFun.AppendBody(BodyScope.GetLines())

            ' Return c function
            Return CFun

        End Function

    End Class
End Namespace