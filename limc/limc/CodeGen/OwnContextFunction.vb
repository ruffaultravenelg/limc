Imports System.Text

Namespace CodeGen
    Public Class OwnContextFunction
        Inherits ContextedFunction

        Private Shared FunctionId As Integer = -1
        Public Shared ReadOnly FunctionInfos As New List(Of String)

        Public Sub New(Name As String, Location As Location, Arguments As IEnumerable(Of String), ReturnType As String)
            MyBase.New(Arguments, ReturnType, New List(Of String), Name)
            FunctionId += 1
            Dim DebugLocation As String = $"{Location.File.RelativePath} (l.{Location.FromLineNumber + 1})"
            FunctionInfos.Add("{""" & Sanitize(Name) & """, """ & Sanitize(DebugLocation) & """}")

            ' Compile arguments
            Dim Args As New StringBuilder
            Args.Append(RUNTIME_CONTEXT_STRUCT_NAME)
            Args.Append("* ")
            Args.Append(RUNTIME_PARENT_CONTEXT_VARIABLE_NAME)
            For Each Arg In Arguments
                Args.Append(", ")
                Args.Append(Arg) 'type arg
            Next
            Me.Args = Args.ToString()

            ' Add context creation to body
            Dim StackContextVariableName As String = $"local_{RUNTIME_CONTEXT_VARIABLE_NAME}"
            If INTEGRATE_DEBUG Then
                DirectCast(MyBase.Body, List(Of String)).Add($"{RUNTIME_CONTEXT_STRUCT_NAME} {StackContextVariableName} = {{{RUNTIME_PARENT_CONTEXT_VARIABLE_NAME}, {FunctionId}, {RUNTIME_PARENT_CONTEXT_VARIABLE_NAME}->gc, false, NULL}};")
            Else
                DirectCast(MyBase.Body, List(Of String)).Add($"{RUNTIME_CONTEXT_STRUCT_NAME} {StackContextVariableName} = {{{RUNTIME_PARENT_CONTEXT_VARIABLE_NAME}, {RUNTIME_PARENT_CONTEXT_VARIABLE_NAME}->gc, false, NULL}};")
            End If
            DirectCast(MyBase.Body, List(Of String)).Add($"{RUNTIME_CONTEXT_STRUCT_NAME}* {RUNTIME_CONTEXT_VARIABLE_NAME} = &local_{RUNTIME_CONTEXT_VARIABLE_NAME};")

        End Sub

        Public Sub AppendBody(Body As IEnumerable(Of String))
            DirectCast(MyBase.Body, List(Of String)).AddRange(Body)
        End Sub

    End Class
End Namespace