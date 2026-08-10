Namespace AST
    Public Class ProblemConstruct
        Inherits ConstructNode

        Public ReadOnly Property Name As String
        Public ReadOnly Property Message As String

        Public Sub New(Name As String, Message As String, Location As Location)
            MyBase.New(Location)
            Me.Name = Name
            Me.Message = Message
        End Sub

    End Class
End Namespace