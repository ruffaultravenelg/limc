Public Class HeapClassType
    Inherits HeapType
    'Implements ClassType

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

    '=======================
    '======== SIGNS ========
    '=======================
    Public ReadOnly Property Signs(Contract As ContractType) As Boolean
        Get
            Throw New NotImplementedException
        End Get
    End Property



End Class
