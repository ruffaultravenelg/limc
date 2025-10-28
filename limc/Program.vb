Module Program

    Public Property VERBOSE As Boolean = True
    Public Property INTEGRATE_DEBUG As Boolean = True

    Sub Main(args As String())

        'Check arguments requirements
        If Not args.Length = 2 Then
            Console.WriteLine("Usage: [input] [output]")
            Exit Sub
        End If

        'Get arguments
        Dim SourcePath As String = args(0)
        Dim DestinationPath As String = args(1)

        'Start compiling
        Try
            Compile(SourcePath, DestinationPath)
        Catch ex As RenderableException
            ex.Render()
#If Not DEBUG Then
        Catch ex As Exception
            Dim InternalExcepetion As New BasicException("Internal error", "A unexpected error was thrown by the compiler.")
            InternalExcepetion.Render()
#End If
        End Try

    End Sub

End Module
