Namespace Source
    Public Class Struct
        Inherits TypeConstruct

        'Properties
        Public Overrides ReadOnly Property Name As String
        Public Overrides ReadOnly Property GenericTypes As IEnumerable(Of GenericType)
        Public ReadOnly Property InlineProperties As IEnumerable(Of Source.KeyNameType)

        Public Property Properties As IEnumerable(Of Source.PropertieDeclaration)
        Public Property Methods As IEnumerable(Of Source.Function)
        Public Property Constructors As IEnumerable(Of Source.Function)
        Public Property Getters As IEnumerable(Of Source.Getter)


        'Inline properties definitions -> struct point(x:int, y:int)
        Public Sub New(Location As Location, Name As String, GenericTypes As IEnumerable(Of GenericType), InlineProperties As IEnumerable(Of Source.KeyNameType), Constructs As IEnumerable(Of ConstructNode))
            MyBase.New(Location)
            Me.Name = Name
            Me.GenericTypes = GenericTypes
            Me.InlineProperties = InlineProperties

            ConstructNode.HandleExports(Constructs)
            ExplodeConstructs(Constructs)
        End Sub

        ' Divide
        Private Sub ExplodeConstructs(Constructs As IEnumerable(Of ConstructNode))

            Dim Methods As New List(Of Source.Function)
            Dim Constructors As New List(Of Source.Function)
            For Each Method As Source.Function In ConstructNode.GetConstructsOfType(Of Source.Function)(Constructs).ToList()
                If Method.Name = "new" Then
                    Constructors.Add(Method)
                Else
                    Methods.Add(Method)
                End If
            Next
            Me.Methods = Methods
            Me.Constructors = Constructors

            Properties = ConstructNode.GetConstructsOfType(Of Source.PropertieDeclaration)(Constructs)
            If InlineProperties.Count > 0 AndAlso Properties.Count > 0 Then
                Throw New SyntaxException("It's impossible to define properties both inline and in the structure body.", Properties(0).Location)
            End If

            Getters = ConstructNode.GetConstructsOfType(Of Source.Getter)(Constructs)

        End Sub

        'To string
        Public Overrides Function ToString() As String
            Return "struct " & Name & GenericType.ListToString(GenericTypes)
        End Function

        Public Overrides Function Compile(PassedGenericTypes As IEnumerable(Of Lim.Type)) As Lim.Type
            Return New Lim.StructType(Me, PassedGenericTypes)
        End Function

    End Class
End Namespace