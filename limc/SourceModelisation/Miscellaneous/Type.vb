Imports System.Threading.Tasks.Dataflow

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
        Public Overridable Function GetTargetedType(Context As Context) As Lim.Type

            'Search for generic type
            If PassedGenericTypes.Count = 0 Then

                'Search for generic type named {TypeName}
                Dim Result As Lim.Type = Context.GenericType(TypeName)

                'If found, return it
                If Result IsNot Nothing Then
                    Return Result
                End If

            End If

            'Compile passed generic types
            Dim CompiledPassedGenericTypes As IEnumerable(Of Lim.Type) = PassedGenericTypes.Select(Function(GenericType) GenericType.GetTargetedType(Context))

            'Search for type from the current file
            Dim TargetedType As Lim.Type = Location.File.GetAType(TypeName, CompiledPassedGenericTypes)
            If TargetedType IsNot Nothing Then
                Return TargetedType
            End If

            'Not found
            Throw New SyntaxException($"The type ""{ToString()}"" is unknown or unreachable", Location)

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

            'Compile passed generic types
            Dim CompiledPassedGenericTypes As IEnumerable(Of Lim.Type) = PassedGenericTypes.Select(Function(GenericType) GenericType.GetTargetedType(Context))

            'Search for type into the selected file
            Dim TargetedType As Lim.Type = Location.File.GetAType(Filename, TypeName, CompiledPassedGenericTypes)
            If TargetedType IsNot Nothing Then
                Return TargetedType
            End If

            'Not found
            Throw New SyntaxException($"The type ""{ToString()}"" is unknown or unreachable", Location)

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

    'TypeNode for a function -> "fn<int, str><bool>"
    Public Class FunType
        Inherits Type

        'Propertie
        Protected ReadOnly ReturnType As Source.Type

        'Constructor
        Public Sub New(Location As Location, PassedGenericTypes As IEnumerable(Of Source.Type), ReturnType As Source.Type)
            MyBase.New(Location, "fun", PassedGenericTypes)
            Me.ReturnType = ReturnType
        End Sub

        'Get targeted type
        Public Overrides Function GetTargetedType(Context As Context) As Lim.Type



        End Function

        'To string
        Public Overrides Function ToString() As String
            Dim Result As String = "fun"
            Result &= "<" & String.Join(", ", PassedGenericTypes.Select(Function(GenericType) GenericType.ToString())) & ">"
            Result &= If(ReturnType Is Nothing, "<>", "<" & ReturnType.ToString() & ">")
            Return Result
        End Function

    End Class

End Namespace