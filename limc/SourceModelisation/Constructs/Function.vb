Namespace Source

    Public Class [Function]
        Inherits ConstructNode

        'Properties
        Public ReadOnly Property Name As String
        Public ReadOnly Property GenericTypes As IEnumerable(Of Source.GenericType)
        Public ReadOnly Property Arguments As IEnumerable(Of Source.Argument)
        Public ReadOnly Property ReturnType As Source.Type

        'Constructor
        Public Sub New(Location As Location, Name As String, GenericTypes As IEnumerable(Of Source.GenericType), Arguments As IEnumerable(Of Source.Argument), ReturnType As Source.Type)
            MyBase.New(Location)
            Me.Name = Name
            Me.GenericTypes = GenericTypes
            Me.Arguments = Arguments
            Me.ReturnType = ReturnType
        End Sub

        'To string
        Public Overrides Function ToString() As String
            If ReturnType Is Nothing Then
                Return "func " & Name & Source.GenericType.ListToString(GenericTypes) & Source.Argument.ListToString(Arguments)
            Else
                Return "func " & Name & Source.GenericType.ListToString(GenericTypes) & Source.Argument.ListToString(Arguments) & ":" & ReturnType.ToString()
            End If
        End Function

    End Class

End Namespace