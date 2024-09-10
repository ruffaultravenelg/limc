Public Class Annotations

    Private Annotations As List(Of AnnotationNode)

    Public Sub New(Annotations As List(Of AnnotationNode))

        Me.Annotations = Annotations

    End Sub

    Public Function HasAnnotation(Name As String) As Boolean
        For Each Annotation As AnnotationNode In Annotations
            If Annotation.Name = Name Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function GetAnnotationParameters(Name As String) As IEnumerable(Of String)
        For Each Annotation As AnnotationNode In Annotations
            If Annotation.Name = Name Then
                Return Annotation.Args
            End If
        Next
        Throw New AnnotationDoesntExist(Name)
    End Function

    Public Function GetAnnotationLocation(Name As String) As Location
        For Each Annotation As AnnotationNode In Annotations
            If Annotation.Name = Name Then
                Return Annotation.Location
            End If
        Next
        Throw New AnnotationDoesntExist(Name)
    End Function

    Public Class AnnotationDoesntExist
        Inherits Exception

        Public Sub New(Name As String)
            MyBase.New("Annotation """ & Name & """ doesn't exist, please use 'HasAnnotation(Name As String)'")
        End Sub
    End Class

End Class
