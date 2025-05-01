Namespace Source

    Public Class PropertieDelcaration
        Inherits Source.KeyNameType

        'Accessors
        Public ReadOnly [GET] As Boolean
        Public ReadOnly [SET] As Boolean

        'Constructor
        Public Sub New(Location As Location, Name As String, Type As Source.Type, [SET] As Boolean, [GET] As Boolean)
            MyBase.New(Location, Name, Type)
            Me.GET = [GET]
            Me.SET = [SET]
        End Sub

        'To string
        Public Overrides Function ToString() As String
            Dim Accessors = " "
            If [GET] And [SET] Then
                Accessors = "(get, set)"
            ElseIf [GET] Then
                Accessors = "(get)"
            ElseIf [SET] Then
                Accessors = "(set)"
            End If
            Return Name & Accessors & ":" & Type.ToString()
        End Function

    End Class

End Namespace