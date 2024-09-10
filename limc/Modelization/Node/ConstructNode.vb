Public MustInherit Class ConstructNode
    Inherits Node

    '==========================
    '======== EXPORTED ========
    '==========================
    Public Property Exported As Boolean

    '=============================
    '======== ANNOTATIONS ========
    '=============================
    Public Property Annotations As Annotations

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Protected Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

End Class
