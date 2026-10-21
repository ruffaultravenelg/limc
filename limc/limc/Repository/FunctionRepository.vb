Imports limc.Symbol

Namespace Repository
    Friend Class FunctionRepository

        Private ParentEnv As Environment.Env ' Used to pass FileEnv to Symbol
        Private FunctionNodes As IEnumerable(Of AST.FunctionConstruct)
        Private SymbolTable As Dictionary(Of (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol)), IReadOnlyList(Of FunctionSymbol))
        ' SymbolTable:
        '   ("func", [int]) => [ func<int>(a:bool), func<int>(a:int), func<int>(a:float) ]

        Public Sub New(FunctionNodes As IReadOnlyList(Of AST.FunctionConstruct), ParentEnv As Environment.Env)
            Me.FunctionNodes = FunctionNodes
            Me.ParentEnv = ParentEnv
            SymbolTable = New Dictionary(Of (Name As String, PassedGenerics As IEnumerable(Of TypeSymbol)), IReadOnlyList(Of FunctionSymbol))(New Comparer.NameWithGenericComparer())
        End Sub

        '
        ' Search for matching functions
        ' if symbol exists -> return thems
        ' if construct exists -> create symbols -> return thems
        ' else return {}
        '
        ' returns a list cause myfunc<int> can have multiple functions signatures
        '
        Public Function RetrieveFunctionSymbols(Name As String, PassedGenerics As IEnumerable(Of TypeSymbol)) As IReadOnlyList(Of FunctionSymbol)
            Dim Key = (Name, PassedGenerics)

            ' If exists
            If SymbolTable.ContainsKey(Key) Then
                Return SymbolTable(Key)
            End If

            ' Get matching node
            Dim MatchingNodes As IReadOnlyList(Of AST.FunctionConstruct) = FindMatchingNodes(Name, PassedGenerics)
            Dim NewSymbols As IReadOnlyList(Of FunctionSymbol) = MatchingNodes.Select(Function(node) New FunctionSymbol(node, PassedGenerics, ParentEnv)).ToList()

            ' Add it to the hashmap
            SymbolTable(Key) = NewSymbols

            ' Return the created symbols
            Return NewSymbols

        End Function

        Private Function FindMatchingNodes(Name As String, PassedGenerics As IEnumerable(Of TypeSymbol)) As IReadOnlyList(Of AST.FunctionConstruct)
            Dim Result As New List(Of AST.FunctionConstruct)
            For Each Node In FunctionNodes
                If Node.Name = Name AndAlso PassedGenerics.Count = Node.GenericTypes.Count Then
                    Result.Add(Node)
                End If
            Next
            Return Result
        End Function

    End Class
End Namespace