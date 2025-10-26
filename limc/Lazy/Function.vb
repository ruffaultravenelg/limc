Namespace Lazy
    Public Class [Function]

        ' Main properties
        Public ReadOnly Property Name As String
        Public ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Public ReadOnly Property ReturnType As TypeSystem.Type
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        ' Not compiled
        Private Node As AST.FunctionConstruct
        Private Context As Context.Context

        ' Constructor
        Public Sub New(Node As AST.FunctionConstruct, Context As Context.Context)
            Me.Node = Node
            Name = Node.Name
            Dim Arguments = New List(Of TypeSystem.Type)
            For Each Arg In Node.Arguments
                Arguments.Add(Arg.ArgumentType.GetAssociatedType(Context))
            Next
            ArgumentTypes = Arguments
            Me.Context = Context
        End Sub

        ' Compiled
        Private _FuncScope As Context.FunctionScope = Nothing
        Public ReadOnly Property FuncScope As Context.FunctionScope
            Get
                If _FuncScope Is Nothing Then
                    _FuncScope = New Context.FunctionScope(Context, Node)
                End If
                Return _FuncScope
            End Get
        End Property

    End Class
End Namespace