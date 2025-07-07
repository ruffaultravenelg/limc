Imports limc.Lim
Imports limc.Source

Public Class StructureHandwritenSetter
    Implements Lim.ISetter

    Public ReadOnly Property Name As String Implements ISetter.Name
        Get
            Return Setter.Name
        End Get
    End Property
    Public ReadOnly Property Type As Lim.Type Implements ISetter.Type
        Get
            Compile()
            Return SetterType
        End Get
    End Property

    Private Setter As Source.Setter
    Private CompiledFunctionName As String = Nothing
    Private SetterType As Lim.Type = Nothing
    Private ParentStructCompiledName As String
    Private ParentStructContext As Context
    Private ParentStructToString As String

    Public Sub New(Setter As Source.Setter, ParentStructCompiledName As String, ParentStructContext As Context, ParentStructToString As String)
        Me.Setter = Setter
        Me.ParentStructCompiledName = ParentStructCompiledName
        Me.ParentStructContext = ParentStructContext
        Me.ParentStructToString = ParentStructToString
    End Sub

    Private Sub Compile()

        'Already compiled
        If CompiledFunctionName IsNot Nothing Then
            Exit Sub
        End If

        'Compile argument
        SetterType = Setter.ValueType.GetTargetedType(ParentStructContext)
        Dim Scope As New Scope(ParentStructContext)
        Dim NewValueVariable As Lim.Variable = Scope.RegisterVariable(Setter.ValueVariableName, SetterType)

        'Compile body
        For Each Statement As StatementNode In Setter.Body
            Scope.WriteLine()
            Statement.Compile(Scope)
        Next

        'Create C function for final comppilation
        CompiledFunctionName = C.Generator.Namer.GenerateSetterName()
        C.Generator.AddFunction(
            New C.Function(
                CompiledFunctionName,
                {$"{ParentStructCompiledName}* self, {NewValueVariable.Type.CompiledName} {NewValueVariable.CompiledName}"},
                "void",
                Scope.Build(),
                $"SET {ParentStructToString}.{Setter.Name}"
            )
        )

    End Sub

    Public Sub CompileCall(Scope As Scope, ParentObject As ExpressionNode, NewValue As ExpressionNode) Implements ISetter.CompileCall
        Compile()
        Scope.WriteLine(C.Function.WriteCall(CompiledFunctionName, C.Utils.ResolvePointerOfStaticObject(Scope, ParentObject.Compile(Scope), ParentStructCompiledName), NewValue.Compile(Scope)) & ";")
    End Sub

End Class
