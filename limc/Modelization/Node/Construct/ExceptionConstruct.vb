Public Class ExceptionConstructNode
    Inherits ConstructNode

    '================================
    '======== EXCEPTION NAME ========
    '================================
    Public ReadOnly Name As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Name As String)
        MyBase.New(Location)

        'Set name
        Me.Name = Name

    End Sub

End Class
