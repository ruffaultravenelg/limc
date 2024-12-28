Namespace Source
    Public Class StrNode
        Inherits ExpressionNode

        'Value
        Private Value As String

        'Constructor
        Public Sub New(Location As Location, Value As String)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type
            Return Lim.Type.Str
        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String
            Return """" & ConvertToCString(Value) & """"
        End Function

        'Clean string
        Private Function ConvertToCString(Input As String) As String
            Return Input.Replace("\", "\\")
        End Function

    End Class

End Namespace