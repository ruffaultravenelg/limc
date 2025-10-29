Imports System.Text

Namespace CodeGen
    Public Class [Function]
        Inherits UtilFunction

        Private Shared FunctionId As Integer = -1
        Public Shared ReadOnly FunctionNames As New List(Of String)

        Public Sub New(Name As String, CompiledName As String, Arguments As IEnumerable(Of String))
            MyBase.New(Arguments, "void", New List(Of String), Name)
            FunctionId += 1
            FunctionNames.Add("""" & Name & """")

            ' Compile arguments
            Dim Args As New StringBuilder
            Args.Append(RuntimeContextStructName)
            Args.Append(" _")
            Args.Append(RuntimeContextVariableName)
            For Each Arg In Arguments
                Args.Append(", ")
                Args.Append(Arg) 'type arg
            Next
            Me.Args = Args.ToString()

            ' Add context creation to body
            If INTEGRATE_DEBUG Then
                DirectCast(MyBase.Body, List(Of String)).Add($"{RuntimeContextStructName} {RuntimeContextVariableName} = {{&_{RuntimeContextVariableName}, {FunctionId}, _{RuntimeContextVariableName}.gc}};")
            Else
                DirectCast(MyBase.Body, List(Of String)).Add($"{RuntimeContextStructName} {RuntimeContextVariableName} = {{&_{RuntimeContextVariableName}, _{RuntimeContextVariableName}.gc}};")
            End If

        End Sub

        Public Sub SetReturnType(ReturnType As String)
            Me.ReturnType = ReturnType
        End Sub

        Public Sub AppendBody(Body As IEnumerable(Of String))
            DirectCast(MyBase.Body, List(Of String)).AddRange(Body)
        End Sub

    End Class
End Namespace