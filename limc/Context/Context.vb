Public Class Context

    Public ReadOnly Property Parent As Context

    Public Sub New(Optional Parent As Context = Nothing)
        Me.Parent = Parent
    End Sub

    Public ReadOnly Iterator Property AllParents As IEnumerable(Of Context)
        Get
            Dim Current As Context = Me
            While Current IsNot Nothing
                Yield Current
                Current = Current.Parent
            End While
        End Get
    End Property

    Public Function HasParent(Of T As Context)() As Boolean
        For Each Ctx As Context In AllParents
            If TypeOf Ctx Is T Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Function GetParent(Of T As Context)() As T
        For Each Ctx As Context In AllParents
            If TypeOf Ctx Is T Then
                Return Ctx
            End If
        Next
        Return Nothing
    End Function

    Private VariableStore As New Dictionary(Of String, VariableData)

    Public Function TryGetVariable(Name As String) As VariableData
        For Each Ctx As Context In AllParents
            If Ctx.VariableStore.ContainsKey(Name) Then
                Return Ctx.VariableStore(Name)
            End If
        Next
        Return Nothing
    End Function
    Public Function GetVariable(Name As String, Location As Location) As VariableData
        Dim VarData As VariableData = TryGetVariable(Name)
        If VarData Is Nothing Then
            Throw New SyntaxError($"Variable not found: {Name}", Location)
        End If
        Return VarData
    End Function

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

    Public Class VariableData
        Public ReadOnly Property CompiledName As String
        Public ReadOnly Property Type As TypeSystem.Type

        Public Sub New(CompiledName As String, Type As TypeSystem.Type)
            Me.CompiledName = CompiledName
            Me.Type = Type
        End Sub

    End Class

    'Get all name matching element from lower to upper context
    Public Function RetrieveMatchingElementByName(Name As String) As IEnumerable(Of SearchMatch)
        Dim Result As New List(Of SearchMatch)
        For Each Ctx As Context In AllParents



        Next
        Return Result
    End Function

End Class
