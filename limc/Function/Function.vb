Namespace Lim
    Public Class [Function]

        'Function compiled name
        Public ReadOnly Property CompiledName As String

        'Base code
        Public ReadOnly Property Base As Source.Function

        'Passed Generic types
        Public ReadOnly Property GenericTypes As IEnumerable(Of Lim.Type)
            Get
                Return Context.GenericTypes.Values
            End Get
        End Property

        'Arguments types
        Public ReadOnly Iterator Property Arguments As IEnumerable(Of Lim.Type)
            Get
                For Each Variable As Lim.Variable In Context.LocalVariables.Values
                    Yield Variable.Type
                Next
            End Get
        End Property

        'Function pointer type
        Public ReadOnly Property PointerType As FuncType
            Get
                Return FuncType.From(Arguments, ReturnType)
            End Get
        End Property

        'Fonction inner context
        Private Context As New Context

        'Function return type
        Public ReadOnly Property ReturnType As Lim.Type
            Get

                'Get returnable scope
                Dim ReturnScope As ReturnableScope = Context.ReturnableScope

                'No returnable scope -> doesn't return a value
                If ReturnScope Is Nothing Then
                    Return Nothing
                End If

                'Get return type
                Try
                    Return ReturnScope.ConstructReturnType
                Catch ex As ReturnTypeNotKnownYet
                    Throw New SyntaxException("The return type cannot be inferred from the function, therefore it must be explicitly stated in the function definition.", Base.Location)
                End Try

            End Get
        End Property

        'Constructor -> Mustn't start compiling because it's instance is not yet added to the FunctionContainer
        Public Sub New(Base As Source.Function, GenericTypes As IEnumerable(Of Lim.Type))

            'Set base source
            Me.Base = Base

            'Create compiled name
            Me.CompiledName = C.Generator.Namer.GenerateFunctionName()

            'Add generic types
            For i As Integer = 0 To GenericTypes.Count - 1
                Context.GenericTypes.Add(Base.GenericTypes(i).Name, GenericTypes(i))
            Next

            'Create arguments
            For Each Argument As Source.KeyNameType In Me.Base.Arguments
                Context.RegisterVariable(Argument.Name, Argument.Type.GetTargetedType(Context))
            Next

        End Sub

        'Compile
        Public Sub Compile()

            'Compile the function return something
            If Base.ContainsReturnStatement Then

                If Base.ReturnType IsNot Nothing Then
                    Context = New ReturnableScope(Context, Base.ReturnType.GetTargetedType(Context)) 'set the type explicitly writen
                Else
                    Context = New ReturnableScope(Context) 'say that there is a type but we don't now it for now
                End If

            End If

            'Compile body
            Dim Scope As New Scope(Context)
            For Each Statement As StatementNode In Base.Body
                Scope.WriteLine()
                Statement.Compile(Scope)
            Next

            'Add this function to the final file
            C.Generator.AddFunction(New C.Function(CompiledName, Context.LocalVariables.Values.Select(Function(Var As Lim.Variable) Var.Type.CompiledName & " " & Var.CompiledName), If(ReturnType Is Nothing, "void", ReturnType.CompiledName), Scope.Build()))

        End Sub

    End Class

End Namespace