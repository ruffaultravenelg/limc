Imports limc.CodeGen

Namespace Lazy
    Public Class UserConstructor
        Inherits Lazy.Constructor

        ' Method arguments
        Protected _ArgumentTypes As IEnumerable(Of TypeSystem.Type) = Nothing
        Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
            Get
                If _ArgumentTypes Is Nothing Then
                    _ArgumentTypes = Node.Arguments.Select(Function(a) a.ArgumentType.GetAssociatedType(ConstructorScope))
                End If
                Return _ArgumentTypes
            End Get
        End Property

        ' Not compiled
        Protected Node As AST.ConstructorConstruct
        Protected AssociatedTypeContext As Context.Context
        Protected ConstructorScope As Context.Scope

        ' Constructor
        Public Sub New(ParentType As TypeSystem.Type, Node As AST.ConstructorConstruct)
            MyBase.New(ParentType)
            Me.Node = Node

            ' Get bone context
            If TypeOf ParentType IsNot TypeSystem.ITypeWithBoneContext Then
                Throw New InternalError()
            End If
            Me.AssociatedTypeContext = DirectCast(ParentType, TypeSystem.ITypeWithBoneContext).InnerContext

            Me.ConstructorScope = New Context.Scope(AssociatedTypeContext, Node.Location)
        End Sub


        ' Create compiled function (to provide compiledName)
        Protected Overrides Function GenerateCompiledMethod() As ContextedFunction
            Dim ArgumentSignature As New List(Of String)
            For Each Arg In Node.Arguments
                Dim Variable As New VariableData(Namer.Variable(Arg.ArgumentName), Arg.ArgumentType.GetAssociatedType(ConstructorScope))
                ConstructorScope.RegisterVariable(Arg.ArgumentName, Variable, Arg.Location)
                ArgumentSignature.Add($"{Variable.Type.cRepresentation} {Variable.CompiledName}")
            Next

            Return New OwnContextFunction($"new_{AssociatedType.ToString()}", ArgumentSignature, AssociatedType.cRepresentation)

        End Function

        ' Compile body (after compiledName is known)
        Protected Overrides Sub CompileBody()
            Dim Writer As New CWriter()

            ' Create self
            If TypeOf AssociatedType Is TypeSystem.ClassType Then
                Writer.WriteLine($"{AssociatedType.cRepresentation} {INSTANCE_ARGUMENT_NAME} = {Constants.LIM_ALLOC}(sizeof({AssociatedType.cRepresentation}));")
                Writer.WriteLine($"if ({INSTANCE_ARGUMENT_NAME} == NULL) {CodeGen.WritePanicCall("""Not enough memory""")};")

            ElseIf TypeOf AssociatedType Is TypeSystem.StructType Then
                Writer.WriteLine($"{AssociatedType.cRepresentation} {INSTANCE_ARGUMENT_NAME} = {AssociatedType.DefaultValue(ConstructorScope)};")

            Else
                Throw New InternalError()
            End If

            ' Compile content
            For Each Statement In Node.Body
                Statement.Compile(Writer, ConstructorScope)
            Next

            ' Add return self
            Writer.WriteLine($"return {INSTANCE_ARGUMENT_NAME};")

            DirectCast(GeneratedMethod, OwnContextFunction).AppendBody(Writer.GetLines())
        End Sub

    End Class
End Namespace