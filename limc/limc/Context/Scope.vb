Imports limc.TypeSystem

Namespace Context
    Public Class Scope
        Inherits Context

        Private VariableStore As New Dictionary(Of String, VariableData)
        Private ConstantStore As New Dictionary(Of String, ConstantData)
        Public ReadOnly Property Location As Location

        Public Sub New(Parent As Context, Location As Location)
            MyBase.New(Parent)
            Me.Location = Location
        End Sub

        ' Variables
        Public Sub RegisterVariable(Name As String, Data As VariableData, Location As Location)
            If VariableStore.ContainsKey(Name) Then
                Throw New VariableAlreadyExistError(Name, Location)
            Else
                VariableStore(Name) = Data
            End If
        End Sub

        Public Function CreateVariable(Name As String, Type As Type, Location As Location) As VariableData
            Dim VariableInfo As New VariableData(CodeGen.Namer.Variable(Name), Type)
            Me.RegisterVariable(Name, VariableInfo, Location)
            Return VariableInfo
        End Function

        ' Constants
        Public Sub RegisterConstant(Name As String, Data As ConstantData, Location As Location)
            If ConstantStore.ContainsKey(Name) Then
                Throw New ConstantAlreadyExistError(Name, Location)
            Else
                ConstantStore(Name) = Data
            End If
        End Sub

        Public Function CreateConstant(Name As String, Type As Type, Location As Location) As ConstantData
            Dim ConstantData As New ConstantData(CodeGen.Namer.Variable(Name), Type)
            Me.RegisterConstant(Name, ConstantData, Location)
            Return ConstantData
        End Function

        Protected Overrides Function GetLocalMatchingElements(Name As String, GenericTypes As IEnumerable(Of Type)) As IEnumerable(Of SearchMatch)
            Dim Matches As New List(Of SearchMatch)

            If GenericTypes.Count > 0 Then
                Return Matches
            End If

            If VariableStore.ContainsKey(Name) Then
                Matches.Add(New SearchMatch(VariableStore(Name)))
            End If

            If ConstantStore.ContainsKey(Name) Then
                Matches.Add(New SearchMatch(ConstantStore(Name)))
            End If

            Return Matches
        End Function

    End Class
End Namespace