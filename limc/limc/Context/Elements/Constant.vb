Public Class ConstantData
    Public ReadOnly Property CompiledName As String
    Public ReadOnly Property Type As TypeSystem.Type

    Public Sub New(CompiledName As String, Type As TypeSystem.Type)
        Me.CompiledName = CompiledName
        Me.Type = Type
    End Sub

End Class
