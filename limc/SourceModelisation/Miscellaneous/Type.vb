Namespace Source

    'Basic TypeNode -> "list<str>"
    Public Class Type
        Inherits Node

        'Properties
        Protected ReadOnly TypeName As String
        Protected ReadOnly PassedGenericTypes As IEnumerable(Of Source.Type)

        'Constructor
        Public Sub New(Location As Location, TypeName As String, PassedGenericTypes As IEnumerable(Of Source.Type))
            MyBase.New(Location)
            Me.TypeName = TypeName
            Me.PassedGenericTypes = PassedGenericTypes
        End Sub

        'Get targeted type
        Public Overridable Function GetTargetedType(Context As context) As Lim.Type

            'Search for generic type
            If PassedGenericTypes.Count = 0 Then

                'Search for generic type named {TypeName}
                Dim Result As Lim.Type = Context.GenericType(TypeName)

                'If found, return it
                If Result IsNot Nothing Then
                    Return Result
                End If

            End If

            'Search for type in current file
            Dim TargetedType As Lim.Type = Location.File.Types.GetCorrespondance(TypeName, PassedGenericTypes)
            If TargetedType IsNot Nothing Then
                Return TargetedType
            End If

            'Search in imported files
            'TODO

            'Not found
            Throw New SyntaxException($"Type ""{ToString()}"" is unknown or unreachable" & Me.ToString(), Location)

        End Function

        'To string
        Public Overrides Function ToString() As String
            Dim Result As String = TypeName
            If PassedGenericTypes.Count > 0 Then
                Result &= "<" & String.Join(", ", PassedGenericTypes.Select(Function(GenericType) GenericType.ToString())) & ">"
            End If
            Return Result
        End Function

    End Class

    'TypeNode from another file -> "utils::linkedList<str>"
    Public Class FiledType
        Inherits Type

        'Propertie
        Protected ReadOnly Filename As String

        'Constructor
        Public Sub New(Location As Location, Filename As String, TypeName As String, PassedGenericTypes As IEnumerable(Of Source.Type))
            MyBase.New(Location, TypeName, PassedGenericTypes)
            Me.Filename = Filename
        End Sub

        'Get targeted type
        Public Overrides Function GetTargetedType(Context As Context) As Lim.Type
            Throw New NotImplementedException 'Search in a specific file
        End Function

        'To string
        Public Overrides Function ToString() As String
            Dim Result As String = Filename & "::" & TypeName
            If PassedGenericTypes.Count > 0 Then
                Result &= "<" & String.Join(", ", PassedGenericTypes.Select(Function(GenericType) GenericType.ToString())) & ">"
            End If
            Return Result
        End Function

    End Class

End Namespace