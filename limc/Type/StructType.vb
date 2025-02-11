Namespace Lim

    Public Class StructType
        Inherits Lim.Type

        'Name of the struct
        Public Overrides ReadOnly Property Name As String
            Get
                Return Base.Name
            End Get
        End Property

        'Context
        Private Context As New Context()

        Public Overrides ReadOnly Property ArgumentTypes As IEnumerable(Of Lim.Type)

        'Constructor
        Public Sub New(Base As Source.Struct, ArgumentTypes As IEnumerable(Of Lim.Type))
            MyBase.New(Base)
            Me.ArgumentTypes = ArgumentTypes

            'Add context types
            For i As Integer = 0 To ArgumentTypes.Count - 1
                Context.GenericTypes.TryAdd(Base.GenericTypes(i).Name, ArgumentTypes(i))
            Next

        End Sub

        Public Overrides Sub Compile()

            'Compile struct


        End Sub

        Public Overrides Function DefaultValue() As String
            Return "((" & CompiledName & "){" & String.Join(", ", ) & "})"
        End Function

        Public Overrides Function Assignation(Variable As String, Value As String) As String
            Return $"{Variable} = {Value}"
        End Function

    End Class

End Namespace