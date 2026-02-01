Namespace Lazy
    Public Class UserFunction
        Inherits Lazy.Function

        ' Main properties
        Public Overrides ReadOnly Property Name As String
            Get
                Return Node.Name
            End Get
        End Property
        Private _ArgumentTypes As List(Of TypeSystem.Type) = Nothing
        Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
            Get
                If _ArgumentTypes Is Nothing Then
                    _ArgumentTypes = New List(Of TypeSystem.Type)
                    For Each Arg In Node.Arguments
                        _ArgumentTypes.Add(Arg.ArgumentType.GetAssociatedType(Context))
                    Next
                End If
                Return _ArgumentTypes
            End Get
        End Property
        Public Overrides ReadOnly Property ReturnType As TypeSystem.Type
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
        Public Overrides ReadOnly Property Exported As Boolean
            Get
                Return Node.Exported
            End Get
        End Property
        Public Overrides ReadOnly Property PassedGenericTypes As IEnumerable(Of TypeSystem.Type)

        ' Not compiled
        Private Node As AST.FunctionConstruct
        Private Context As Context.Context

        ' Constructor
        Public Sub New(Node As AST.FunctionConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type), Context As Context.Context)
            Me.Node = Node
            Me.PassedGenericTypes = PassedGenericTypes
            If Not PassedGenericTypes.Count = Node.GenericArguments.Count Then
                Throw New InternalError()
            End If

            If PassedGenericTypes.Count > 0 Then
                Me.Context = New Context.GenericContext(Context)
                For i As Integer = 0 To PassedGenericTypes.Count - 1
                    DirectCast(Me.Context, Context.GenericContext).RegisterGenericType(Node.GenericArguments(i), PassedGenericTypes(i))
                Next
            Else
                Me.Context = Context
            End If

        End Sub

        ' Functino scope -> compilation
        Private _FuncScope As Context.FunctionScope = Nothing
        Private ReadOnly Property FuncScope As Context.FunctionScope
            Get
                If _FuncScope Is Nothing Then
                    _FuncScope = New Context.FunctionScope(Context, Node)
                    _FuncScope.CompileBody()
                End If
                Return _FuncScope
            End Get
        End Property

        ' Get generated function
        Public Overrides ReadOnly Property GeneratedFunction As CodeGen.PassingContextFunction
            Get
                Return FuncScope.GeneratedFunction
            End Get
        End Property

    End Class
End Namespace