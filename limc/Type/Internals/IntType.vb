' Represent a int32 type (just int in C)
Public Class IntType
    Inherits Lim.Type
    Implements Lim.InternalType

    'Constructor
    Public Sub New(Base As TypeConstruct, GenericTypes As IEnumerable(Of Lim.Type))
        MyBase.New(Base)
        SetCompiledName("int")
    End Sub

    'Name
    Public Overrides ReadOnly Property Name As String = "int"

    'Passed generic types
    Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of Lim.Type) = {}

    'Compile
    Public Overrides Sub Compile()
    End Sub

    'Default value
    Public Overrides Function DefaultValue() As String
        Return "0"
    End Function

    'Assignation
    Public Overrides Function Assignation(Variable As String, Value As String) As String
        Return $"{Variable} = {Value};"
    End Function

End Class
