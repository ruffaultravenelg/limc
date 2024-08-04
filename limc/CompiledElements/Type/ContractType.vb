Public Class ContractType
    Inherits HeapType

    '=============================
    '======== SOURCE NAME ========
    '=============================
    Protected Overrides ReadOnly Property SourceName As String
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    '=============================
    '======== CONSTRUCTOR ========
    '=============================

    Public Sub New(Owner As LimSource)
        MyBase.New(Owner)
    End Sub

End Class
