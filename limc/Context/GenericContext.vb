Namespace Context
    Public Class GenericContext
        Inherits Context

        Private GenericTypes As New Dictionary(Of String, TypeSystem.Type)

        Public Sub New(Parent As Context)
            MyBase.New(Parent)
        End Sub

        Public Sub RegisterGenericType(Name As String, Type As TypeSystem.Type)
            GenericTypes(Name) = Type
        End Sub

        Public Shared Function SearchForGenericType(Context As Context, Name As String) As TypeSystem.Type
            For Each Ctx In Context.GetParents(Of GenericContext)()
                If Ctx.GenericTypes.ContainsKey(Name) Then
                    Return Ctx.GenericTypes(Name)
                End If
            Next
            Return Nothing
        End Function

    End Class
End Namespace