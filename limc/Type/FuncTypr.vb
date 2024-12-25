Public Class FuncType
    Inherits Lim.Type

    'Name
    Public Overrides ReadOnly Property Name As String = "fun"

    'Passed generic types
    Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of Lim.Type) = {}

    'The return type of the fun type (fun<str, int><[HERE]>)
    Public ReadOnly Property ReturnType As Lim.Type

    'Constructor
    Private Sub New(GenericTypes As IEnumerable(Of Lim.Type), ReturnType As Lim.Type)
        MyBase.New(Nothing) 'Se if this thing make crash lol

        'Set elements
        Me.ArgumentTypes = GenericTypes
        Me.ReturnType = ReturnType

        'Generate structure
        C.Generator.AddStructure(New C.Structure(CompiledName, {
            If(ReturnType Is Nothing, "void", ReturnType.CompiledName) & " (*fn)(" & String.Join(", ", GenericTypes.Select(Function(T As Lim.Type) T.CompiledName)) & ")"
        }))

        'Generate default function


    End Sub

    'Compile
    Public Overrides Sub Compile()
    End Sub

    'Default value
    Public Overrides Function DefaultValue() As String
        Return "NULL" 'TODO: default variable
    End Function

    'Assignation
    Public Overrides Function Assignation(Variable As String, Value As String) As String
        Return $"{Variable} = {Value};"
    End Function

    'To string
    Public Overrides Function ToString() As String
        If ReturnType Is Nothing Then
            Return MyBase.ToString()
        Else
            Return MyBase.ToString() & "<" & ReturnType.ToString() & ">"
        End If
    End Function

    'All existing func types
    Private Shared ExistingTypes As New HashSet(Of FuncType)

    'From
    Public Shared Function From(Arguments As IEnumerable(Of Lim.Type), ReturnType As Lim.Type) As FuncType

        'If already exist
        For Each i As FuncType In ExistingTypes


        Next

        'Do not exist -> new one
        Return New FuncType(Arguments, ReturnType)

    End Function

    'Compare functype
    Private Shared Function SameType(Target As FuncType, Arguments As IEnumerable(Of Lim.Type), ReturnType As Lim.Type) As Boolean

        'Compare argument count
        If Not Target.ArgumentTypes.Count = Arguments.Count Then
            Return False
        End If

        'Compare arguments
        For i As Integer = 0 To Arguments.Count - 1
            If Not Target.ArgumentTypes(i) = Arguments(i) Then
                Return False
            End If
        Next

        'Compare return type
        If Target.ReturnType = ReturnType Then
            Return False
        End If

        'Every test pass
        Return True

    End Function

End Class
