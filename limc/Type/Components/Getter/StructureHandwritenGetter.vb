Imports limc.Lim
Imports limc.Source

Public Class StructureHandwritenGetter
    Implements Lim.IGetter

    Public ReadOnly Property Name As String Implements IGetter.Name
        Get
            Return Getter.Name
        End Get
    End Property
    Public ReadOnly Property Type As Lim.Type Implements IGetter.Type
        Get
            Compile()
            Return CompiledFunctionReturnType
        End Get
    End Property

    Private Getter As Source.Getter
    Private CompiledFunctionName As String = Nothing
    Private CompiledFunctionReturnType As Lim.Type = Nothing
    Private ParentStructCompiledName As String
    Private ParentStructContext As Context

    Public Sub New(Getter As Source.Getter, ParentStructCompiledName As String, ParentStructContext As Context)

        'Skill issue test
        If Not StatementNode.ListContainsReturnStatement(Getter.Body) Then
            Throw New SyntaxException("A getter must return a value. However, this getter does not contain a ""return"" statement.", Getter.Location)
        End If

        'Values
        Me.Getter = Getter
        Me.ParentStructCompiledName = ParentStructCompiledName
        Me.ParentStructContext = ParentStructContext

    End Sub

    Private Sub Compile()

        'Already compiled
        If CompiledFunctionName IsNot Nothing Then
            Exit Sub
        End If

        'Compile the return type
        Dim ReturnableContext As ReturnableContext
        If Getter.DefinedType IsNot Nothing Then
            ReturnableContext = New ReturnableContext(ParentStructContext, Getter.DefinedType.GetTargetedType(ParentStructContext))
        Else
            ReturnableContext = New ReturnableContext(ParentStructContext) 'say that there is a type but we don't now it for now
        End If

        'Compile body
        Dim Scope As New Scope(ReturnableContext)
        For Each Statement As StatementNode In Getter.Body
            Scope.WriteLine()
            Statement.Compile(Scope)
        Next

        'Get return type
        Try
            CompiledFunctionReturnType = ReturnableContext.ConstructReturnType
        Catch ex As ReturnTypeNotKnownYet
            Throw New SyntaxException("The return type cannot be inferred from the getter, therefore it must be explicitly stated in the getter definition.", Getter.Location)
        End Try

        'Create C function for final comppilation
        CompiledFunctionName = C.Generator.Namer.GenerateGetterName()
        C.Generator.AddFunction(
            New C.Function(
                CompiledFunctionName,
                {$"{ParentStructCompiledName}* self"},
                CompiledFunctionReturnType.CompiledName,
                Scope.Build(),
                $"GET {ToString()}.{Getter.Name}"
            )
        )

    End Sub

    Public Function CompileCall(Scope As Scope, ParentObject As ExpressionNode) As String Implements IGetter.CompileCall
        Compile()
        Return C.Function.WriteCall(CompiledFunctionName, C.Utils.ResolvePointerOfStaticObject(Scope, ParentObject.Compile(Scope), ParentStructCompiledName))
    End Function

End Class
