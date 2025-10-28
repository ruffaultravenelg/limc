Namespace Lazy
    Public Class [Function]

        ' Main properties
        Public ReadOnly Property Name As String
        Public ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Public ReadOnly Property ReturnType As TypeSystem.Type
            Get
                If Node.ReturnType IsNot Nothing Then
                    Return Node.ReturnType.GetAssociatedType(Context)
                ElseIf Node.DoContainsStatement(Of AST.ReturnStatement) Then
                    Return FuncScope.ReturnType
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Public ReadOnly Property Exported As Boolean
            Get
                Return Node.Exported
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
                    _FuncScope.CompileBody()
                End If
                Return _FuncScope
            End Get
        End Property

    End Class
End Namespace