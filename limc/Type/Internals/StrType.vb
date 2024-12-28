' Represent a string (char*)
Imports limc.Lim

Public Class StrType
    Inherits Lim.Type
    Implements Lim.InternalType

    'Constructor
    Public Sub New(Base As TypeConstruct, GenericTypes As IEnumerable(Of Lim.Type))
        MyBase.New(Base)
        SetCompiledName("char*")
    End Sub

    'Name
    Public Overrides ReadOnly Property Name As String = "str"

    'Passed generic types
    Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of Lim.Type) = {}

    'Compile
    Public Overrides Sub Compile()

        'Add "len" getter
        RegisterGetter("len", New HardCodedGetterComponent(Me, Lim.Type.Int, {"if (self == NULL) return 0;", "return strlen(self);"}))

    End Sub

    'Default value
    Public Overrides Function DefaultValue() As String
        Return "NULL"
    End Function

    'Assignation
    Public Overrides Function Assignation(Variable As String, Value As String) As String
        Return $"{Variable} = {Value};"
    End Function

End Class
