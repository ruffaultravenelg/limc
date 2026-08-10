Namespace Lazy
    Public Class Problem

        Public ReadOnly Property Name As String
            Get
                Return Node.Name
            End Get
        End Property

        Public ReadOnly Property Exported As Boolean
            Get
                Return Node.Exported
            End Get
        End Property

        Public ReadOnly Property MessageConstCompiledName As String

        Private Node As AST.ProblemConstruct

        Public Sub New(Node As AST.ProblemConstruct)
            Me.Node = Node
            Me.MessageConstCompiledName = CodeGen.Namer.Problem(Name)
            CodeGen.RegisterConst($"const char* {MessageConstCompiledName} = ""{CodeGen.Sanitize(Node.Message)}"";", $"problem {Name}")
        End Sub

    End Class
End Namespace