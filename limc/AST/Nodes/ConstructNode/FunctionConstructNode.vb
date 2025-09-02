Imports limc.TypeSystem

Namespace AST
    Public Class FunctionConstructNode
        Inherits ConstructNode
        Implements UncompiledProcedure

        Public ReadOnly Property Name As String Implements UncompiledProcedure.Name
        Public ReadOnly Property Arguments As IEnumerable(Of ArgumentNode)
        Public ReadOnly Property ReturnType As TypeNode
        Public ReadOnly Property Body As IEnumerable(Of StatementNode)

        Public Sub New(Name As String, Arguments As IEnumerable(Of ArgumentNode), ReturnType As TypeNode, Body As IEnumerable(Of StatementNode), Location As Location)
            MyBase.New(Location)
            Me.Name = Name
            Me.Arguments = Arguments
            Me.ReturnType = ReturnType
            Me.Body = Body
        End Sub

        Public Function GetArgumentTypes(CompilingContext As Context) As IEnumerable(Of Type) Implements UncompiledProcedure.GetArgumentTypes
            Return Arguments.Select(Function(arg) arg.ArgumentType.GetAssociatedType(CompilingContext))
        End Function

        Public Function CompileProcedure(CompilingContext As Context) As CompiledProcedure Implements UncompiledProcedure.CompileProcedure
            Return New Lazy.Function(Me, CompilingContext)
        End Function
    End Class

End Namespace