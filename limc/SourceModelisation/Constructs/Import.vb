Namespace Source

    Public Class Import
        Inherits ConstructNode

        'Properties
        Public ReadOnly Property Filename As String
        Public ReadOnly Property Library As Boolean
        Public ReadOnly Property Naming As String

        'Constructor
        Public Sub New(Location As Location, Filename As String, Library As Boolean, Naming As String)
            MyBase.New(Location)
            Me.Filename = Filename
            Me.Library = Library
            Me.Naming = Naming
        End Sub

        'To string
        Public Overrides Function ToString() As String

            Dim AsNaming As String = If(Naming = "", "", " as " & Naming)
            Dim Name As String = If(Library, Filename, """" & Filename & """")

            Return "import " & Name & AsNaming

        End Function

    End Class

End Namespace