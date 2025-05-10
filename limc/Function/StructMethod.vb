Namespace Lim
    Public Class StructMethod
        Inherits Lim.Function

        Private ParentStruct As Lim.StructType

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

        Public Overrides Function ToString() As String
            Return ParentStruct.ToString & "." & MyBase.ToString()
        End Function

    End Class

End Namespace