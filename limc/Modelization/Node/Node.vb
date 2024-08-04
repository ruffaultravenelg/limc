Public MustInherit Class Node

    '==========================
    '======== LOCATION ========
    '==========================
    Public ReadOnly Location As Location

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location)
        Me.Location = Location
    End Sub

End Class
