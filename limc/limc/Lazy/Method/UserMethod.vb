Imports limc.CodeGen
Imports limc.TypeSystem

Namespace Lazy
    Public Class UserMethod
        Inherits Lazy.Method

        ' Name
        Public Overrides ReadOnly Property Name As String
            Get
                Return Node.Name
            End Get
        End Property

        ' Passed generic types
        Public Overrides ReadOnly Property PassedGenericTypes As IEnumerable(Of Type)

        ' Method arguments
        Private _ArgumentTypes As IEnumerable(Of TypeSystem.Type) = Nothing
        Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
            Get
                If _ArgumentTypes Is Nothing Then
                    _ArgumentTypes = Node.Arguments.Select(Function(a) a.ArgumentType.GetAssociatedType(MethodScope))
                End If
                Return _ArgumentTypes
            End Get
        End Property

        ' Method return type
        Private _ReturnType As TypeSystem.Type = Nothing
        Public Overrides ReadOnly Property ReturnType As TypeSystem.Type
            Get
                If Node.ReturnType Is Nothing Then
                    Return Nothing
                Else
                    If _ReturnType Is Nothing Then
                        _ReturnType = Node.ReturnType.GetAssociatedType(MethodScope)
                    End If
                    Return _ReturnType
                End If
            End Get
        End Property

        ' Not compiled
        Private Node As AST.FunctionConstruct
        Private ParentTypeContext As Context.Context
        Private MethodScope As Context.Scope

        ' Constructor
        Public Sub New(ParentType As TypeSystem.Type, Node As AST.FunctionConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type))
            MyBase.New(ParentType)
            Me.Node = Node

            ' Get bone context
            If TypeOf ParentType IsNot TypeSystem.ITypeWithBoneContext Then
                Throw New InternalError()
            End If
            Me.ParentTypeContext = DirectCast(ParentType, TypeSystem.ITypeWithBoneContext).InnerContext

            Me.PassedGenericTypes = PassedGenericTypes
            If Not PassedGenericTypes.Count = Node.GenericArguments.Count Then
                Throw New InternalError()
            End If

            If PassedGenericTypes.Count > 0 Then
                Dim GenericContext As New Context.GenericContext(ParentTypeContext)
                For i As Integer = 0 To PassedGenericTypes.Count - 1
                    GenericContext.RegisterGenericType(Node.GenericArguments(i), PassedGenericTypes(i))
                Next
                Me.MethodScope = New Context.Scope(GenericContext, Node.Location)
            Else
                Me.MethodScope = New Context.Scope(ParentTypeContext, Node.Location)
            End If

        End Sub


        ' Create compiled function (to provide compiledName)
        Protected Overrides Function GenerateCompiledMethod() As ContextedFunction
            Dim ArgumentSignature As New List(Of String) From {$"{ParentType.cRepresentation} {Constants.INSTANCE_ARGUMENT_NAME}"}
            For Each Arg In Node.Arguments
                Dim Variable As New VariableData(Namer.Variable(Arg.ArgumentName), Arg.ArgumentType.GetAssociatedType(MethodScope))
                MethodScope.RegisterVariable(Arg.ArgumentName, Variable, Arg.Location)
                ArgumentSignature.Add($"{Variable.Type.cRepresentation} {Variable.CompiledName}")
            Next

            Dim ReturnTypeSignature As String = If(Node.ReturnType Is Nothing, "void", ReturnType.cRepresentation)

            Return New OwnContextFunction($"{ParentType.ToString()}.{Name}", Node.Location, ArgumentSignature, ReturnTypeSignature)

        End Function

        ' Compile body (after compiledName is known)
        Protected Overrides Sub CompileBody()
            If ReturnType IsNot Nothing Then
                MethodScope = New Context.MustReturnScope(MethodScope, Node.Location, ReturnType) 'TODO: this create another scope, update this later
            End If

            Dim Writer As New CWriter()
            For Each Statement In Node.Body
                Statement.Compile(Writer, MethodScope)
            Next
            DirectCast(GeneratedMethod, OwnContextFunction).AppendBody(Writer.GetLines())
        End Sub

    End Class
End Namespace