Namespace Source
    Public Class IntNode
        Inherits ExpressionNode

        'Value
        Private Value As Integer

        'Constructor
        Public Sub New(Location As Location, Value As Integer)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type
            Return Lim.Type.Int
        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String
            Return Value.ToString()
        End Function
    End Class

End Namespace