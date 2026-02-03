Imports limc.TypeSystem

Namespace Repository
    Public Class ConstructorRepository

        ' Contains all functions with their C variants
        Private ReadOnly Constructors As IEnumerable(Of Lazy.Constructor)

        ' Constructor (get uncompiled constructors)
        Public Sub New(AssociatedType As Type, Constructors As IEnumerable(Of AST.ConstructorConstruct))
            Me.Constructors = Constructors.Select(Function(c) New Lazy.UserConstructor(AssociatedType, c))
        End Sub

        ' General repository endpoint
        Public Function RetrieveConstructor(ArgumentTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Constructor
            For Each C In Constructors

                If Not C.ArgumentTypes.Count = ArgumentTypes.Count Then
                    Continue For
                End If

                If Not C.ArgumentTypes.SequenceEqual(ArgumentTypes) Then
                    Continue For
                End If

                Return C

            Next
            Return Nothing
        End Function

    End Class
End Namespace