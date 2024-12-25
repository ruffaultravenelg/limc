Namespace Lim
    Public Class [Function]

        'Function compiled name
        Public ReadOnly Property CompiledName As String

        'Base code
        Public ReadOnly Property Base As Source.Function

        'Passed Generic types
        Public ReadOnly Property GenericTypes As IEnumerable(Of Lim.Type)
            Get
                Return Scope.GenericTypes.Values
            End Get
        End Property

        'Arguments types
        Public ReadOnly Iterator Property Arguments As IEnumerable(Of Lim.Type)
            Get
                For Each Variable As Lim.Variable In Scope.LocalVariables.Values
                    Yield Variable.Type
                Next
            End Get
        End Property

        'Fonction inner context
        Private ReadOnly Scope As New Scope

        'Function return type
        Private Property ReturnType As Lim.Type

        'Constructor -> Mustn't start compiling because it's instance is not yet added to the FunctionContainer
        Public Sub New(Base As Source.Function, GenericTypes As IEnumerable(Of Lim.Type))

            'Set base source
            Me.Base = Base

            'Create compiled name
            Me.CompiledName = C.Generator.Namer.GenerateFunctionName()

            'Add generic types
            For i As Integer = 0 To GenericTypes.Count - 1
                Scope.GenericTypes.Add(Base.GenericTypes(i).Name, GenericTypes(i))
            Next

            'Create arguments
            For Each Argument As Source.Argument In Me.Base.Arguments
                Scope.WriteVariableDeclaration(Argument.Name, Argument.Type.GetTargetedType(Scope))
            Next

        End Sub

        'Compile
        Public Sub Compile()

            'Compile function return type
            If Base.ReturnType Is Nothing Then
                ReturnType = Nothing
            Else
                ReturnType = Base.ReturnType.GetTargetedType(Scope)
            End If

            'TODO: incroement 1 stack variable count on all heap arguments

            'Compile body
            Dim InnerScope As New Scope(Scope)
            For Each Statement As StatementNode In Base.Body
                InnerScope.WriteLine()
                Statement.Compile(InnerScope)
            Next
            Scope.WriteScope(InnerScope)

            'Compile signature
            Dim Signature As String = CompileSignature()

            'Add this function to the final file
            C.Generator.AddFunction(New C.Function(Signature, Scope.Build()))

        End Sub

        'Generate signature
        Private Function CompileSignature() As String

            'Create result
            Dim Signature As String = ""

            'Add return type
            If ReturnType Is Nothing Then
                Signature &= "void"
            Else
                Signature &= ReturnType.CompiledName
            End If

            'Add name
            Signature &= " " & CompiledName

            'Add arguments
            Signature &= "("
            For Each Variable As Lim.Variable In Scope.LocalVariables.Values
                Signature &= Variable.Type.CompiledName & " " & Variable.CompiledName & ", "
            Next
            Signature &= ")"

            'Return
            Return Signature

        End Function

    End Class

End Namespace