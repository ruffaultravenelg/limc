Imports System.Text

Namespace CodeGen
    Public Class UtilFunction
        Inherits BaseFunction

        Public ReadOnly Property CompiledName As String
        Protected Args As String
        Protected ReturnType As String

        Protected Overrides ReadOnly Property Signature As String
            Get
                Return $"{ReturnType} {CompiledName}({Args})"
            End Get
        End Property

        Public Sub New(Arguments As IEnumerable(Of String), ReturnType As String, Body As IEnumerable(Of String), Comment As String)
            MyBase.New("", Body, Comment)

            'Create compiledname
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
            Me.Args = Args.ToString()

            ' Create signature
            Me.ReturnType = ReturnType

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