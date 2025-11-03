Imports limc.AST

Namespace Lazy
    Public MustInherit Class Method

        Public Const METHOD_INSTANCE_ARGUMENT_NAME As String = "instance"

        ' Main properties
        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Public MustOverride ReadOnly Property ReturnType As TypeSystem.Type
        Public ReadOnly Property ParentType As TypeSystem.Type

        ' Constructor
        Protected Sub New(ParentType As TypeSystem.Type)
            Me.ParentType = ParentType
        End Sub

        ' Generated function -> compilation
        Protected MustOverride Function CompileGeneratedFunction() As CodeGen.UtilFunction
        Private _GeneratedFunction As CodeGen.UtilFunction = Nothing
        Public ReadOnly Property GeneratedFunction As CodeGen.UtilFunction
            Get
                If _GeneratedFunction Is Nothing Then
                    _GeneratedFunction = CompileGeneratedFunction()
                    CodeGen.RegisterFunction(_GeneratedFunction)
                End If
                Return _GeneratedFunction
            End Get
        End Property

        ' Associated function type
        Public ReadOnly Property AssociatedFunctionType As TypeSystem.FunType
            Get
                Return TypeSystem.FunType.From(ArgumentTypes, ReturnType)
            End Get
        End Property

        ' Compile a call
        Public Function CompileCall(Instance As ExpressionNode, Arguments As IEnumerable(Of ExpressionNode), Scope As Context.Scope) As String

            ' Check instance type
            If Not Instance.GetExpressionReturnType(Scope) = ParentType Then
                Throw New TypeMismatchError(ParentType, Instance.GetExpressionReturnType(Scope), Instance.Location)
            End If

            ' Check argument count
            If Not Arguments.Count = ArgumentTypes.Count Then
                If Arguments.Count > 0 Then
                    Throw New SyntaxError($"{ArgumentTypes.Count} arguments requiered, instance of {Arguments.Count}", Arguments.Last.Location)
                Else
                    Throw New SyntaxError($"{ArgumentTypes.Count} arguments requiered, instance of {Arguments.Count}", Instance.Location)
                End If
            End If

            ' Check & compile arguments
            Dim CompiledArguments As New List(Of String) From {Instance.CompileExpression(Scope)}
            For i As Integer = 0 To Arguments.Count - 1
                If Not Arguments(i).GetExpressionReturnType(Scope) = ArgumentTypes(i) Then
                    Throw New TypeMismatchError(ArgumentTypes(i), Arguments(i).GetExpressionReturnType(Scope), Arguments(i).Location)
                End If
                CompiledArguments.Add(Arguments(i).CompileExpression(Scope))
            Next

            'Return call
            Return GeneratedFunction.WriteCall(CompiledArguments)

        End Function

    End Class
End Namespace