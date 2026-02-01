Namespace Context
    Public Class TypeContext
        Inherits Context

        Private AssociatedType As TypeSystem.Type

        Public Sub New(Parent As Context, AssociatedType As TypeSystem.Type)
            MyBase.New(Parent)
            Me.AssociatedType = AssociatedType
        End Sub

        Protected Overrides Function GetLocalMatchingElement(Name As String) As IEnumerable(Of SearchMatch)
            Return AssociatedType.RetrieveElements(Name)
        End Function

    End Class
End Namespace