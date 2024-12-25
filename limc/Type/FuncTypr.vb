' Represent a function type (doesn't contains a object, not for methods)
Public Class FuncType
    Inherits Lim.Type

    'Name
    Public Overrides ReadOnly Property Name As String = "fun"

    'Passed generic types
    Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of Lim.Type) = {}

    'The return type of the fun type (fun<str, int><[HERE]>)
    Public ReadOnly Property ReturnType As Lim.Type

    'Constructor
    Public Sub New(GenericTypes As IEnumerable(Of Lim.Type), ReturnType As Lim.Type)
        MyBase.New(Nothing) 'Se if this thing make crash lol
        SetCompiledName("int")
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

End Class
