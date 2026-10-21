Namespace HIR
    Friend Class ProgramNode
        Inherits ConstructNode

        Public Sub New(Location As Location)
            MyBase.New(Location)
        End Sub

        Public ReadOnly Property Functions As IReadOnlyList(Of FunctionConstruct) = New List(Of FunctionConstruct)
        Public ReadOnly Property EntryFunction As Symbol.FunctionSymbol

        Public Overrides Sub Accept(Visitor As IHIRVisitor)
            Visitor.VisitProgramNode(Me)
        End Sub

        ' Sub class to help adding the content of the HIR.ProgramNode in the lazy compilation loop
        ' this avoid poluting the main node with util functions & restricting Symbol.ICompilableSymbol to only useful functions
        Public Class Builder

            Private Node As ProgramNode

            Public Sub New(MainFileLocation As Location)
                Node = New ProgramNode(MainFileLocation)
            End Sub

            Public Sub AppendFunction(Func As FunctionConstruct)
                DirectCast(Node.Functions, List(Of FunctionConstruct)).Add(Func)
            End Sub

            Public Sub SetEntryPoint(Func As Symbol.FunctionSymbol)
                Node._EntryFunction = Func
            End Sub

            Public Function Build() As Node
                Return Node
            End Function

        End Class

    End Class
End Namespace