Namespace Context
    Public Class Context

        Public ReadOnly Property Parent As Context

        Public Sub New(Parent As Context)
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
        Public Iterator Function GetParents(Of T As Context)() As IEnumerable(Of T)
            Dim Current As Context = Me
            While Current IsNot Nothing
                If TypeOf Current Is T Then
                    Yield Current
                End If
                Current = Current.Parent
            End While
        End Function

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

        Public Function TryGetVariable(Name As String) As VariableData
            Dim Results As IEnumerable(Of SearchMatch) =
                RetrieveMatchingElements(Name) _
                .Where(Function(e) e.Type = SearchMatch.MatchType.MATCH_VARIABLE)

            If Results.Count > 0 Then
                Return Results(0).MatchingVariable
            Else
                Return Nothing
            End If
        End Function
        Public Function GetVariable(Name As String, Location As Location) As VariableData
            Dim VarData As VariableData = TryGetVariable(Name)
            If VarData Is Nothing Then
                Throw New SyntaxError($"Variable not found: {Name}", Location)
            End If
            Return VarData
        End Function

        'Get all name matching element from lower to upper context
        Public Function RetrieveMatchingElements(Name As String) As IEnumerable(Of SearchMatch)
            Dim Result As New List(Of SearchMatch)
            For Each Ctx As Context In AllParents
                Result.AddRange(Ctx.GetLocalMatchingElement(Name))
            Next
            Return Result
        End Function
        Public Function RetrieveMatchingElement(Name As String, Location As Location) As SearchMatch
            Dim Results As IEnumerable(Of SearchMatch)
            Try
                Results = RetrieveMatchingElements(Name)
            Catch ex As MissingLocationError
                Throw ex.CreateError(Location)
            End Try
            If Results.Count > 0 Then
                Return Results(0)
            Else
                Throw New UnknownOrUnreachableElementError(Name, Location)
            End If
        End Function
        Protected Overridable Function GetLocalMatchingElement(Name As String) As IEnumerable(Of SearchMatch)
            Return {}
        End Function

        ' Get matching but with generic
        Public Function RetrieveMatchingElement(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type), Location As Location)
            For Each Ctx In AllParents
                Dim Match As SearchMatch
                Try
                    Match = Ctx.GetLocalMatchingElement(Name, GenericTypes)
                Catch ex As MissingLocationError
                    Throw ex.CreateError(Location)
                End Try
                If Match IsNot Nothing Then
                    Return Match
                End If
            Next
            Throw New UnknownOrUnreachableElementError(Name & "<" & String.Join(", ", GenericTypes.Select(Function(g) g.ToString())) & ">", Location)
        End Function
        Protected Overridable Function GetLocalMatchingElement(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As SearchMatch
            Return Nothing
        End Function

    End Class

End Namespace