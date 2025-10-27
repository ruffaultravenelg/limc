Imports System.Text

Namespace CodeGen
    Public Class UtilFunction
        Inherits BaseFunction

        Public ReadOnly Property CompiledName As String

        Public Sub New(Arguments As IEnumerable(Of String), ReturnType As String, Body As IEnumerable(Of String), Comment As String)
            MyBase.New("", Body, Comment)
            CompiledName = Namer.Function(Comment)

            ' Compile arguments
            Dim Args As New StringBuilder
            Args.Append(RuntimeContextStructName)
            Args.Append(" "c)
            Args.Append(RuntimeContextVariableName)
            For Each Arg In Arguments
                Args.Append(", ")
                Args.Append(Arg)
            Next

            ' Create signature
            MyBase.Signature = $"{ReturnType} {CompiledName}({Args.ToString()})"

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