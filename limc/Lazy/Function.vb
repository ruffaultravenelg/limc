Imports limc.AST

Namespace Lazy
    Public MustInherit Class [Function]

        ' Main properties
        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Public MustOverride ReadOnly Property ReturnType As TypeSystem.Type
        Public MustOverride ReadOnly Property Exported As Boolean
        Public MustOverride ReadOnly Property PassedGenericTypes As IEnumerable(Of TypeSystem.Type)

        ' Compiled function
        Public MustOverride ReadOnly Property GeneratedFunction As CodeGen.UtilFunction

        ' AssociatedFunctionType
        Public ReadOnly Property AssociatedFunctionType As TypeSystem.FunType
            Get
                Return TypeSystem.FunType.From(ArgumentTypes, ReturnType)
            End Get
        End Property

        ' Compile call
        Public Function CompileCall(Arguments As IEnumerable(Of ExpressionNode), Scope As Context.Scope)

            If Not Arguments.Count = ArgumentTypes.Count Then
                Throw New InternalError()
            End If

            Dim CompiledArguments As New List(Of String)
            For i As Integer = 0 To Arguments.Count - 1

                ' Check if types are the same type
                If Not Arguments(i).GetExpressionReturnType(Scope) = ArgumentTypes(i) Then
                    Throw New TypeMismatchError(ArgumentTypes(i), Arguments(i).GetExpressionReturnType(Scope), Arguments(i).Location)
                End If

                ' Compile
                CompiledArguments.Add(Arguments(i).CompileExpression(Scope))

            Next

            Return GeneratedFunction.WriteCall(CompiledArguments)

        End Function

    End Class
End Namespace