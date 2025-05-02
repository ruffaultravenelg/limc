Namespace Source

    'Kind of a factory
    Public Class PropertieDeclaration
        Inherits ConstructNode
        Implements IPropertieDefinition

        'Properties
        Public ReadOnly Property Name As String Implements IPropertieDefinition.Name
        Public ReadOnly Property Type As Source.Type Implements IPropertieDefinition.Type

        'let (set, get)
        Private _GET As Boolean
        Private _SET As Boolean

        'Real visibility
        Public ReadOnly Property [GET] As Boolean Implements IPropertieDefinition.GET
            Get
                Return _GET OrElse Exported
            End Get
        End Property
        Public ReadOnly Property [SET] As Boolean Implements IPropertieDefinition.SET
            Get
                Return _SET Or Exported
            End Get
        End Property

        'Constructor
        Public Sub New(Location As Location, Name As String, Type As Source.Type, Optional _GET As Boolean = False, Optional _SET As Boolean = False)
            MyBase.New(Location)
            Me.Name = Name
            Me.Type = Type
            Me._GET = _GET
            Me._SET = _SET
        End Sub

        'To string
        Public Overrides Function ToString() As String

            Dim Exported_STR As String = If(Exported, "export ", "")

            Dim Accessors_STR As String = ""
            If (_GET AndAlso _SET) Then
                Accessors_STR = "(get, set)"
            ElseIf (_GET) Then
                Accessors_STR = "(get)"
            ElseIf (_SET) Then
                Accessors_STR = "(set)"
            End If

            Return $"{Exported_STR}let{Accessors_STR} {Name}:{Type.ToString()}"

        End Function

    End Class

End Namespace