Imports limc.AST
Imports limc.CodeGen

Namespace Lazy
    Public MustInherit Class [Function]

        ' Main properties
        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property PassedGenericTypes As IEnumerable(Of TypeSystem.Type)
        Public MustOverride ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Public MustOverride ReadOnly Property ReturnType As TypeSystem.Type
        Public MustOverride ReadOnly Property Exported As Boolean

        ' AssociatedFunctionType
        Public ReadOnly Property AssociatedFunctionType As TypeSystem.FunType
            Get
                Return TypeSystem.FunType.From(ArgumentTypes, ReturnType)
            End Get
        End Property

        ' Compile to C
        Protected MustOverride Function GenerateCompiledFunction() As CodeGen.ContextedFunction
        Protected MustOverride Sub CompileBody()
        Private _GeneratedFunction As CodeGen.ContextedFunction
        Protected ReadOnly Property GeneratedFunction As CodeGen.ContextedFunction
            Get
                If _GeneratedFunction Is Nothing Then
                    _GeneratedFunction = GenerateCompiledFunction()
                    CodeGen.RegisterFunction(_GeneratedFunction)
                    CompileBody()
                End If
                Return _GeneratedFunction
            End Get
        End Property


        ' Compile call
        Public Function CompileCall(Arguments As IEnumerable(Of ExpressionNode), Writer As CWriter, Scope As Context.Scope, Location As Location)

            If Not Arguments.Count = ArgumentTypes.Count Then
                Throw New NotTheRightAmountOfArgumentsException(Arguments.Count, ArgumentTypes.Count, Location)
            End If

            Dim CompiledArguments As New List(Of String)
            For i As Integer = 0 To Arguments.Count - 1

                ' Check if types are the same type
                If Not Arguments(i).GetExpressionReturnType(Scope) = ArgumentTypes(i) Then
                    Throw New TypeMismatchError(ArgumentTypes(i), Arguments(i).GetExpressionReturnType(Scope), Arguments(i).Location)
                End If

                ' Compile
                CompiledArguments.Add(Arguments(i).CompileExpression(Writer, Scope))

            Next

            Return GeneratedFunction.WriteCall(CompiledArguments)

        End Function

        ' Compile function pointer
        Public Function CompileFunctionPointer() As String
            Return GeneratedFunction.CompiledName & "/* todo */"
        End Function

        ' Get compiled name, only for entry point
        Public Function GetCompiledFunction() As ContextedFunction
            Return GeneratedFunction
        End Function

    End Class
End Namespace