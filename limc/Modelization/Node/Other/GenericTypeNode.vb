'
' Represent a genric type name
'    T
'    T:contract
'
Public Class GenericTypeNode
    Inherits Node

    '======================
    '======== NAME ========
    '======================
    Public ReadOnly Property Name As String

    '======================================
    '======== CONTRACT (optionnal) ========
    '======================================
    Public ReadOnly Property Contract As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Name As String, Optional Contract As TypeNode = Nothing)
        MyBase.New(Location)

        'Set name
        Me.Name = Name

        'Set wanted contract
        Me.Contract = Contract

    End Sub

    '===============================
    '======== IS COMPATIBLE ========
    '===============================
    Public Function IsComptatible(Type As Type) As Boolean

        'No contract
        If Contract Is Nothing Then
            Return True
        End If

        'Only none primitive class can sign contracts
        If TypeOf Type IsNot HeapClassType Then
            Return False
        End If

        'Get the contract type
        Dim ContractRealType As Type = Contract.AssociatedType(Me.Location.File.Scope)

        'Check if the type is a contract
        If TypeOf ContractRealType IsNot ContractType Then
            Throw New TypeErrorException("A contract was expected here.", Contract.Location)
        End If

        'Check if the class signs the contract
        If Not DirectCast(Type, HeapClassType).Signs(ContractRealType) Then
            Return False
        End If

        'Everything is good
        Return True

    End Function

End Class
