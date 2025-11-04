Public Class NotTheRightAmountOfGenericTypesException
    Inherits MissingLocationError

    Public Sub New(GivenTypes As Integer, WantedTypes As Integer)
        MyBase.New($"{GivenTypes} generic types were given where {WantedTypes} {If(WantedTypes < 2, "was", "were")} expected.")
    End Sub

    Public Overrides Function CreateError(Location As Location) As RenderableException
        Return New SyntaxError(Message, Location)
    End Function

End Class
