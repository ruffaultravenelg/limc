Imports limc.CodeGen
Imports limc.Context

Namespace Lazy
    Public Class UserFunction
        Inherits Lazy.Function

        ' Function name
        Public Overrides ReadOnly Property Name As String
            Get
                Return Node.Name
            End Get
        End Property

        ' Passed generic types
        Public Overrides ReadOnly Property PassedGenericTypes As IEnumerable(Of TypeSystem.Type)

        ' Function arguments types
        Private _ArgumentTypes As IEnumerable(Of TypeSystem.Type) = Nothing
        Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
            Get
                If _ArgumentTypes Is Nothing Then
                    _ArgumentTypes = Node.Arguments.Select(Function(a) a.ArgumentType.GetAssociatedType(FunctionScope))
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
                        _ReturnType = Node.ReturnType.GetAssociatedType(FunctionScope)
                    End If
                    Return _ReturnType
                End If
            End Get
        End Property

        ' Function is exported
        Public Overrides ReadOnly Property Exported As Boolean
            Get
                Return Node.Exported
            End Get
        End Property

        ' Not compiled
        Private Node As AST.FunctionConstruct
        Private FunctionScope As Context.Scope

        ' Constructor
        Public Sub New(Node As AST.FunctionConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type), Context As Context.Context)
            Me.Node = Node
            Me.PassedGenericTypes = PassedGenericTypes
            If Not PassedGenericTypes.Count = Node.GenericArguments.Count Then
                Throw New InternalError()
            End If

            If PassedGenericTypes.Count > 0 Then
                Dim GenericContext As New Context.GenericContext(Context)
                For i As Integer = 0 To PassedGenericTypes.Count - 1
                    GenericContext.RegisterGenericType(Node.GenericArguments(i), PassedGenericTypes(i))
                Next
                Me.FunctionScope = New Scope(GenericContext, Node.Location)
            Else
                Me.FunctionScope = New Scope(Context, Node.Location)
            End If

        End Sub

        ' Create compiled function (to provide compiledName)
        Protected Overrides Function GenerateCompiledFunction() As ContextedFunction

            Dim ArgumentSignature As New List(Of String)
            For Each Arg In Node.Arguments
                Dim Variable As New VariableData(Namer.Variable(Arg.ArgumentName), Arg.ArgumentType.GetAssociatedType(FunctionScope))
                FunctionScope.RegisterVariable(Arg.ArgumentName, Variable, Arg.Location)
                ArgumentSignature.Add($"{Variable.Type.cRepresentation} {Variable.CompiledName}")
            Next

            Dim ReturnTypeSignature As String = If(Node.ReturnType Is Nothing, "void", ReturnType.cRepresentation)

            Return New OwnContextFunction(Name, ArgumentSignature, ReturnTypeSignature)

        End Function

        ' Compile body (after compiledName is known)
        Protected Overrides Sub CompileBody()
            If ReturnType IsNot Nothing Then
                FunctionScope = New Context.MustReturnScope(FunctionScope, Node.Location, ReturnType) 'TODO: this create another scope, update this later
            End If

            Dim Writer As New CWriter()
            For Each Statement In Node.Body
                Statement.Compile(Writer, FunctionScope)
            Next
            DirectCast(GeneratedFunction, OwnContextFunction).AppendBody(Writer.GetLines())
        End Sub

    End Class
End Namespace