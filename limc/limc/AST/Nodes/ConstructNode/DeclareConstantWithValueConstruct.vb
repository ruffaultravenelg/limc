Imports limc.Context

Namespace AST
    Public Class DeclareConstantWithValueConstruct
        Inherits ConstructNode

        Private ConstantName As String
        Private ConstantValue As AST.ExpressionNode
        Public Data As ConstantData

        Public Sub New(ConstantName As String, ConstantValue As AST.ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.ConstantName = ConstantName
            Me.ConstantValue = ConstantValue

            If TypeOf ConstantValue IsNot IConstantExpression Then
                Throw New SyntaxError("The value of a file constant must be of a primary type (""int"", ""float"", ""bool"", ""str"").", ConstantValue.Location)
            End If

        End Sub

        Public Sub Compile(ConstantStore As Dictionary(Of String, DeclareConstantWithValueConstruct))

            ' Check if constant already exist
            If ConstantStore.ContainsKey(ConstantName) Then
                Throw New ConstantAlreadyExistError(ConstantName, Location)
            End If

            ' Create constant element
            Dim TmpScope As New Scope(Nothing, Location)
            Dim TmpWriter As New CWriter()
            Dim Type As TypeSystem.Type = ConstantValue.GetExpressionReturnType(TmpScope)
            Data = New ConstantData(CodeGen.Namer.Constant(ConstantName).ToUpper(), Type)
            ConstantStore(ConstantName) = Me

            ' Register const to c file
            CodeGen.RegisterConst($"#define {Data.CompiledName} {ConstantValue.CompileExpression(TmpWriter, TmpScope)}", $"{Location.File.RelativePath} -> {ConstantName}")

            ' Check that nothing was written on scope
            If TmpWriter.GetLines().Count > 0 Then
                Throw New InternalError()
            End If

        End Sub

    End Class

End Namespace