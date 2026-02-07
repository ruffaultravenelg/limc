Namespace Lazy
    Public Class DirectAccessGetter
        Inherits Getter

        Public Overrides ReadOnly Property Name As String
        Public Overrides ReadOnly Property Type As TypeSystem.Type

        Private Callback As Func(Of String, String)
        Private GetPointerValueCallBack As Func(Of String, String)

        Public Sub New(Name As String, Type As TypeSystem.Type, Callback As Func(Of String, String), Optional GetPointerValueCallBack As Func(Of String, String) = Nothing)
            Me.Name = Name
            Me.Type = Type
            Me.Callback = Callback
            Me.GetPointerValueCallBack = GetPointerValueCallBack
        End Sub

        Public Overrides Function CallGetter(CompiledObject As String) As String
            Return Callback(CompiledObject)
        End Function

        Public Overrides Function GetReference(CompiledObjectPointer As String, Location As Location) As String
            If GetPointerValueCallBack IsNot Nothing Then
                Return GetPointerValueCallBack(CompiledObjectPointer)
            Else
                Throw New ExpressionDoesNotReferToAVariableError(Location)
            End If
        End Function

    End Class
End Namespace