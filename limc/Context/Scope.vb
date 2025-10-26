Namespace Context
    Public Class Scope
        Inherits Context

        Private Lines As New List(Of String)
        Public ReadOnly Property Location As Location

        Public Sub New(Parent As Context, Location As Location)
            MyBase.New(Parent)
            Me.Location = Location
        End Sub

        Public Sub WriteLine(Line As String)
            Lines.Add(Line)
        End Sub

        Public Sub WriteScope(Scope As Scope)
            For Each Line In Scope.Lines
                Lines.Add(vbTab & Line)
            Next
        End Sub

        Public Function GetLines() As IEnumerable(Of String)
            Return Lines
        End Function

        Private VariableStore As New Dictionary(Of String, VariableData)

        Public Function CreateVariable(Name As String, Type As TypeSystem.Type) As VariableData
            Dim VarData As New VariableData(CodeGen.Namer.Variable(Name), Type)
            VariableStore(Name) = VarData
            Return VarData
        End Function
        Public Function CreateVariable(Name As String, Type As TypeSystem.Type, Location As Location) As VariableData
            If VariableStore.ContainsKey(Name) Then
                Throw New VariableAlreadyExistError(Name, Location)
            End If
            Return CreateVariable(Name, Type)
        End Function
        Protected Overrides Function GetLocalMatchingElement(Name As String) As IEnumerable(Of SearchMatch)
            Return If(VariableStore.ContainsKey(Name), {New SearchMatch(VariableStore(Name))}, {})
        End Function

    End Class

End Namespace