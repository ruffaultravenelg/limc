Namespace Lazy
    Public MustInherit Class Relation

        Public ReadOnly Property ParentType As TypeSystem.Type
        Public MustOverride ReadOnly Property Type As TypeSystem.RelationType
        Public MustOverride ReadOnly Property ArgumentsTypes As IEnumerable(Of TypeSystem.Type)
        Public MustOverride ReadOnly Property ReturnType As TypeSystem.Type

        ' Compiled function
        Public MustOverride ReadOnly Property GeneratedFunction As CodeGen.ContextedFunction 'IMPORTANT: first argument must be the instance

        ' Constructor
        Public Sub New(ParentType As TypeSystem.Type)
            Me.ParentType = ParentType
        End Sub

        ' Compile call
        Public Function CompileCall(Instance As AST.ExpressionNode, Arguments As IEnumerable(Of AST.ExpressionNode), Writer As CWriter, Scope As Context.Scope, Location As Location)

            ' Check instance type
            If Not Instance.GetExpressionReturnType(Scope) = ParentType Then
                Throw New TypeMismatchError(ParentType, Instance.GetExpressionReturnType(Scope), Instance.Location)
            End If

            ' Check argument count
            If Not Arguments.Count = ArgumentsTypes.Count Then
                If Arguments.Count > 0 Then
                    Throw New SyntaxError($"{ArgumentsTypes.Count} arguments requiered, instead of {Arguments.Count}", Arguments.Last.Location)
                Else
                    Throw New SyntaxError($"{ArgumentsTypes.Count} arguments requiered, instead of {Arguments.Count}", Instance.Location)
                End If
            End If

            ' Compile arguments
            Dim CompiledArguments As New List(Of String) From {Instance.CompileExpression(Writer, Scope)}
            For i As Integer = 0 To Arguments.Count - 1

                ' Check if types are the same type
                If Not Arguments(i).GetExpressionReturnType(Scope) = ArgumentsTypes(i) Then
                    Throw New TypeMismatchError(ArgumentsTypes(i), Arguments(i).GetExpressionReturnType(Scope), Arguments(i).Location)
                End If

                ' Compile
                CompiledArguments.Add(Arguments(i).CompileExpression(Writer, Scope))

            Next

            ' Return call
            Return GeneratedFunction.WriteCall(CompiledArguments)

        End Function

    End Class
End Namespace