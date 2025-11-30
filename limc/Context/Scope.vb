Namespace Context
    Public Class Scope
        Inherits Context

        Public ReadOnly Property Location As Location
        Public Sub New(Parent As Context, Location As Location)
            MyBase.New(Parent)
            Me.Location = Location
        End Sub


        ' Lines
        Private Lines As New List(Of String)

        Public Sub WriteLine(Line As String)
            Lines.Add(Line)
        End Sub
        Public Sub WriteLines(Lines As IEnumerable(Of String))
            Me.Lines.AddRange(Lines)
        End Sub
        Public Sub AppendLastLine(Chars As String)
            If Lines.Count > 0 Then
                Lines(Lines.Count - 1) &= Chars
            Else
                Throw New InternalError()
            End If
        End Sub

        Public Sub WriteScope(Scope As Scope)
            For Each Line In Scope.Lines
                Lines.Add(vbTab & Line)
            Next
        End Sub

        Public Function GetLines() As IEnumerable(Of String)
            Return Lines
        End Function

        ' Variables
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

        ' Constants
        Private ConstantStore As New Dictionary(Of String, ConstantData)

        Public Function CreateConstant(Name As String, Type As TypeSystem.Type) As ConstantData
            Dim ConstData As New ConstantData(CodeGen.Namer.Constant(Name), Type)
            ConstantStore(Name) = ConstData
            Return ConstData
        End Function
        Public Function CreateConstant(Name As String, Type As TypeSystem.Type, Location As Location) As ConstantData
            If ConstantStore.ContainsKey(Name) Then
                Throw New ConstantAlreadyExistError(Name, Location)
            End If
            Return CreateConstant(Name, Type)
        End Function

        ' Search
        Protected Overrides Function GetLocalMatchingElement(Name As String) As IEnumerable(Of SearchMatch)
            Dim Results As New List(Of SearchMatch)
            If VariableStore.ContainsKey(Name) Then
                Results.Add(New SearchMatch(VariableStore(Name)))
            End If
            If ConstantStore.ContainsKey(Name) Then
                Results.Add(New SearchMatch(ConstantStore(Name)))
            End If
            Return Results
        End Function

    End Class

End Namespace