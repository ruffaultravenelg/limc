Imports System.Text

Namespace CodeGen
    Public Class [Function]
        Inherits BaseFunction

        Private Shared FunctionId As Integer = -1
        Public Shared ReadOnly FunctionNames As New List(Of String)

        Public ReadOnly Property CompiledName As String
        Private Args As String

        Public Sub New(Name As String, CompiledName As String, Arguments As IEnumerable(Of Tuple(Of String, String)))
            MyBase.New("", New List(Of String), Name)
            FunctionId += 1
            FunctionNames.Add("""" & Name & """")
            Me.CompiledName = CompiledName

            ' Compile arguments
            Dim Args As New StringBuilder
            Args.Append(RuntimeContextStructName)
            Args.Append(" _")
            Args.Append(RuntimeContextVariableName)
            For Each Arg In Arguments
                Args.Append(", ")
                Args.Append(Arg.Item2) 'Item2 = type
                Args.Append(" "c)
                Args.Append(Arg.Item1) 'Item1 = name
            Next
            Me.Args = Args.ToString()

            ' Create signature
            MyBase.Signature = $"void {CompiledName}({Me.Args})" 'temp void return type in case SetReturnType is never called

            ' Add context creation to body
            If INTEGRATE_DEBUG Then
                DirectCast(MyBase.Body, List(Of String)).Add($"{RuntimeContextStructName} {RuntimeContextVariableName} = {{&_{RuntimeContextVariableName}, {FunctionId}, _{RuntimeContextVariableName}.gc}};")
            Else
                DirectCast(MyBase.Body, List(Of String)).Add($"{RuntimeContextStructName} {RuntimeContextVariableName} = {{&_{RuntimeContextVariableName}, _{RuntimeContextVariableName}.gc}};")
            End If

        End Sub

        Public Sub AppendBody(Body As IEnumerable(Of String))
            DirectCast(MyBase.Body, List(Of String)).AddRange(Body)
        End Sub
        Public Sub SetReturnType(ReturnType As String)
            MyBase.Signature = $"{ReturnType} {CompiledName}({Args})"
        End Sub

        Public Function WriteCall(Args As IEnumerable(Of String)) As String
            If Args.Count = 0 Then
                Return $"{CompiledName}({RuntimeContextVariableName})"
            Else
                Return $"{CompiledName}({RuntimeContextVariableName}, {String.Join(", ", Args)})"
            End If
        End Function

    End Class
End Namespace