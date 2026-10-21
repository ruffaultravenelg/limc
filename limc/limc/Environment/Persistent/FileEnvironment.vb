Imports limc.AST
Imports limc.Repository
Imports limc.Symbol

Namespace Environment
    Friend Class FileEnvironment
        Inherits Env

        Private ReadOnly Node As FileNode
        Private ReadOnly FunctionRepository As FunctionRepository

        Public Sub New(Parent As GlobalEnvironment, Node As FileNode)
            MyBase.New(Parent)
            Me.Node = Node
            Me.FunctionRepository = New FunctionRepository(Node.Functions, Me)
        End Sub

        Protected Overrides Iterator Function ResolveSymbol(ElementName As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As IEnumerable(Of SymbolMatch)

            ' Function repository
            For Each Res In FunctionRepository.RetrieveFunctionSymbols(ElementName, PassedGenericTypes)
                Yield New SymbolMatch(Res)
            Next

            ' Class repository


            ' Enum repository


        End Function

        Protected Overrides Function ResolveType(Typename As String, PassedGenericTypes As IReadOnlyList(Of TypeSymbol)) As TypeSymbol
            Return Nothing
        End Function


    End Class
End Namespace