Namespace Lim

    Public Class Variable

        'Properties
        Public ReadOnly CompiledName As String 'Compiled name
        Public ReadOnly Type As Lim.Type 'Lim type

        'Constructor
        Public Sub New(Name As String, Type As Lim.Type)
            Me.CompiledName = Name
            Me.Type = Type
        End Sub

    End Class

End Namespace