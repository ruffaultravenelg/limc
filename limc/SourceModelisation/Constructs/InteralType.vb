Imports System.Reflection

Namespace Source

    'Kind of a factory
    Public Class InteralType
        Inherits TypeConstruct

        'Name
        Public Overrides ReadOnly Property Name As String

        'Generic types needed
        Public Overrides ReadOnly Property GenericTypes As IEnumerable(Of GenericType)

        'Constructor
        Public Sub New(Location As Location, Name As String, GenericTypes As IEnumerable(Of GenericType))
            MyBase.New(Location)
            Me.Name = Name
            Me.GenericTypes = GenericTypes
        End Sub

        'Get internal class that represent {name}{generic types}
        Public Overrides Function Compile(PassedGenericTypes As IEnumerable(Of Lim.Type)) As Lim.Type

            'Get class
            Dim assembly As Assembly = Assembly.GetExecutingAssembly()
            Dim targetType As System.Type = assembly.GetTypes().FirstOrDefault(Function(t) t.IsClass AndAlso Not t.IsAbstract AndAlso t.Name.Equals(Name & "Type", StringComparison.OrdinalIgnoreCase) AndAlso GetType(Lim.InternalType).IsAssignableFrom(t))

            'If type do not exist
            If targetType Is Nothing Then
                Throw New SyntaxException($"The type '{Name}' does not exist in the current assembly of limc.", Location)
            End If

            'Instanciate the type
            Return CType(Activator.CreateInstance(targetType, PassedGenericTypes), Lim.Type)

        End Function

        'To string
        Public Overrides Function ToString() As String
            Return "internal type " & Name & "<" & String.Join(", ", GenericTypes) & ">"
        End Function

    End Class

End Namespace