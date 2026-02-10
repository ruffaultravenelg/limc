Imports limc.AST

Namespace Lazy
    Public MustInherit Class Constructor

        ' Main properties
        Public MustOverride ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Public ReadOnly Property AssociatedType As TypeSystem.Type

        ' Constructor
        Protected Sub New(AssociatedType As TypeSystem.Type)
            Me.AssociatedType = AssociatedType
        End Sub

        ' Compile to C
        Protected MustOverride Function GenerateCompiledMethod() As CodeGen.ContextedFunction
        Protected MustOverride Sub CompileBody()
        Private _GeneratedMethod As CodeGen.ContextedFunction
        Protected ReadOnly Property GeneratedMethod As CodeGen.ContextedFunction
            Get
                If _GeneratedMethod Is Nothing Then
                    _GeneratedMethod = GenerateCompiledMethod()
                    CodeGen.RegisterFunction(_GeneratedMethod)
                    CompileBody()
                End If
                Return _GeneratedMethod
            End Get
        End Property

        ' Compile a call
        Public Function CompileCall(Arguments As IEnumerable(Of ExpressionNode), Writer As CWriter, Context As Context.Context, Location As Location) As String

            ' Check argument count
            If Not Arguments.Count = ArgumentTypes.Count Then
                If Arguments.Count > 0 Then
                    Throw New SyntaxError($"{ArgumentTypes.Count} arguments requiered, instead of {Arguments.Count}", Arguments.Last.Location)
                Else
                    Throw New SyntaxError($"{ArgumentTypes.Count} arguments requiered, instead of {Arguments.Count}", Location)
                End If
            End If

            ' Check & compile arguments
            Dim CompiledArguments As New List(Of String)
            For i As Integer = 0 To Arguments.Count - 1
                If Not Arguments(i).GetExpressionReturnType(Context) = ArgumentTypes(i) Then
                    Throw New TypeMismatchError(ArgumentTypes(i), Arguments(i).GetExpressionReturnType(Context), Arguments(i).Location)
                End If
                CompiledArguments.Add(Arguments(i).CompileExpression(Writer, Context))
            Next

            'Return call
            Return GeneratedMethod.WriteCall(CompiledArguments)

        End Function

    End Class
End Namespace