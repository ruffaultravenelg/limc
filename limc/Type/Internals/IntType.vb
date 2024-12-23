' Represent a int32 type (just int in C)
Public Class IntType
    Inherits Lim.Type
    Implements Lim.InternalType

    'Constructor
    Public Sub New(GenericTypes As IEnumerable(Of Lim.Type))
        MyBase.New()
    End Sub

    'Name
    Public Overrides ReadOnly Property Name As String = "int"

    'Passed generic types
    Public Overrides ReadOnly Property GenericTypes As IEnumerable(Of Lim.Type) = {}

    'Compile
    Public Overrides Sub Compile()
    End Sub

End Class
