Imports limc.TypeSystem

Namespace Context
    Public Class TypeContext
        Inherits Context

        Private AssociatedType As TypeSystem.Type

        Public Sub New(Parent As Context, AssociatedType As TypeSystem.Type)
            MyBase.New(Parent)
            Me.AssociatedType = AssociatedType
        End Sub

        Protected Overrides Function GetLocalMatchingElements(Name As String, GenericTypes As IEnumerable(Of Type)) As IEnumerable(Of SearchMatch)
            Return AssociatedType.RetrieveElementsFromInside(Name, GenericTypes)
        End Function

    End Class
End Namespace