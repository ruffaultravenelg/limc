Imports limc.CodeGen

Namespace Lazy
    Public Class HardRelation
        Inherits Relation

        Public Overrides ReadOnly Property Type As TypeSystem.RelationType
        Public Overrides ReadOnly Property ArgumentsTypes As IEnumerable(Of TypeSystem.Type)
        Public Overrides ReadOnly Property ReturnType As TypeSystem.Type

        Private Body As IEnumerable(Of String)
        Private ArgumentNames As IEnumerable(Of String)

        Public Sub New(ParentType As TypeSystem.Type, Type As TypeSystem.RelationType, ArgumentTypes As IEnumerable(Of TypeSystem.Type), ArgumentNames As IEnumerable(Of String), ReturnType As TypeSystem.Type, Body As IEnumerable(Of String))
            MyBase.New(ParentType)
            Me.Type = Type
            Me.ArgumentsTypes = ArgumentTypes
            Me.ArgumentNames = ArgumentNames
            Me.ReturnType = ReturnType
            Me.Body = Body
        End Sub

        Protected Overrides Function GenerateCompiledRelation() As ContextedFunction
            If ArgumentsTypes.Count <> ArgumentNames.Count Then
                Throw New InternalError()
            End If

            Dim Args As New List(Of String)
            If Type = TypeSystem.RelationType.RELATION_BRACKETS_PTR Then
                Args.Add($"{ParentType.pointerCRepresentation} {Constants.INSTANCE_ARGUMENT_NAME}")
            Else
                Args.Add($"{ParentType.cRepresentation} {Constants.INSTANCE_ARGUMENT_NAME}")
            End If
            For i As Integer = 0 To ArgumentsTypes.Count - 1
                Args.Add($"{ArgumentsTypes(i).cRepresentation} {ArgumentNames(i)}")
            Next

            Dim returnType_STR As String = If(Type = TypeSystem.RelationType.RELATION_BRACKETS_PTR, ReturnType.pointerCRepresentation, ReturnType.cRepresentation)
            Return New CodeGen.ContextedFunction(Args, returnType_STR, Body, $"{ParentType.ToString()} -> {Type.ToString()}({String.Join(", ", ArgumentsTypes.Select(Function(arg) arg.ToString()))}):{ReturnType.ToString()}")
        End Function

        Protected Overrides Sub CompileBody()

        End Sub

    End Class
End Namespace