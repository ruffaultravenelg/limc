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
        Private Context As Context
        Private ReturnScope As ReturnableContext = Nothing

        'Function return type
        Public ReadOnly Property ReturnType As Lim.Type
            Get

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
        Public Sub New(Base As Source.Function, GenericTypes As IEnumerable(Of Lim.Type), Optional ParentContext As Context = Nothing)

            'Set base source
            Me.Base = Base

            'Create compiled name
            Me.CompiledName = C.Generator.Namer.GenerateFunctionName()

            'Create context
            Me.Context = New Context(ParentContext)

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
            Dim ReturnableContext As Context = Me.Context
            If Base.ContainsReturnStatement OrElse Base.ReturnType IsNot Nothing Then
                If Base.ReturnType IsNot Nothing Then
                    If (Not Base.ContainsReturnStatement) AndAlso Base.Body.Count > 0 Then
                        Throw New SyntaxException($"No ""return"" instruction is present, although the function is defined to return a ""{Base.ReturnType.ToString()}"" value.", Base.ReturnType.Location)
                    End If
                    ReturnScope = New ReturnableContext(Context, Base.ReturnType.GetTargetedType(Context)) 'set the type explicitly writen
                Else
                    ReturnScope = New ReturnableContext(Context) 'say that there is a type but we don't now it for now
                End If
                ReturnableContext = ReturnScope
            End If

            'Compile body
            Dim Scope As New Scope(ReturnableContext)
            For Each Statement As StatementNode In Base.Body
                Scope.WriteLine()
                Statement.Compile(Scope)
            Next

            'Add this function to the final file
            Dim Arguments As New List(Of String)
            If Base.IsMethod Then
                Arguments.Add($"{Base.ParentType.CompiledName} self")
            End If
            Arguments.AddRange(Context.LocalVariables.Values.Select(Function(Var As Lim.Variable) Var.Type.CompiledName & " " & Var.CompiledName))
            C.Generator.AddFunction(New C.Function(CompiledName, Arguments, If(ReturnType Is Nothing, "void", ReturnType.CompiledName), Scope.Build()))

        End Sub

        Public Overrides Function ToString() As String

            Dim GenericTypes_STR As String = ""
            For Each T As Lim.Type In GenericTypes
                GenericTypes_STR &= ", " & T.ToString()
            Next
            If GenericTypes_STR.StartsWith(", ") Then
                GenericTypes_STR = "<" & GenericTypes_STR.Substring(2) & ">"
            End If
            Return Base.Name & GenericTypes_STR '& Source.KeyNameType.ListToString(Arguments)
        End Function

    End Class

End Namespace