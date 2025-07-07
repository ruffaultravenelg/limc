Namespace Lim
    Public Class StructMethod
        Inherits Lim.Method

        Public ReadOnly Property ParentStruct As Lim.StructType

        Public Sub New(Base As Source.Function, GenericTypes As IEnumerable(Of Type), StructContext As Context, ParentStruct As StructType)
            MyBase.New(Base, {}, StructContext)
            Me.ParentStruct = ParentStruct
        End Sub

        'Compile argument but add a "self" argument at first
        Protected Overrides Function GenerateArguments() As IEnumerable(Of String)
            Dim Arguments As New List(Of String)
            Arguments.Add($"{ParentStruct.CompiledName}* self") 'Add self
            Arguments.AddRange(Context.LocalVariables.Values.Select(Function(Var As Lim.Variable) Var.Type.CompiledName & " " & Var.CompiledName))
            Return Arguments
        End Function

        'Compile a call
        Protected Overrides Function CompileObjectForCall(Scope As Scope, Obj As ExpressionNode) As String
            Dim ObjectReference As String = Obj.Compile(Scope)
            Return C.Utils.ResolvePointerOfStaticObject(Scope, ObjectReference, ParentStruct.CompiledName)
        End Function

        Public Overrides Function ToString() As String
            Return ParentStruct.ToString & "." & MyBase.ToString()
        End Function

    End Class

End Namespace