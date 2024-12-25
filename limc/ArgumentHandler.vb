Public Module ArgumentHandler

    'Input
    Public Property Input As String = Nothing

    'Output
    Public Property Output As String = Nothing

    'Help
    Public Property Help As Boolean = False

    'Version
    Public Property Version As Boolean = False

    'Libs directory
    Public Property LibsDirectory As String = IO.Path.Combine(AppContext.BaseDirectory, "libs")

    'Shared load
    Public Sub LoadArgs(Args As IEnumerable(Of String))

        'Parse arguments
        Dim i As Integer = -1
        While i + 1 < Args.Count

            'Get arg
            i += 1
            Dim Arg As String = Args(i)

            'Flag
            If Arg.StartsWith("-") Then

                'Help
                If Arg = "-h" OrElse Arg = "--help" Then
                    Help = True
                    Return 'If need to show help, don't need to parse all args
                End If

                'Version
                If Arg = "-v" OrElse Arg = "--version" Then
                    Version = True
                    Return 'If need to show version, don't need to parse all args
                End If

                'libs
                If Arg = "-l" OrElse Arg = "--libs" Then
                    i += 1
                    If i >= Args.Count Then
                        Throw New SimpleException("Missing argument", "Flag """ & Arg & """ need an argument.")
                    End If
                    LibsDirectory = Args(i)
                    Continue While
                End If

                'Unknown flag
                Throw New SimpleException("Unknown flag", "Flag """ & Arg & """ is unknown.")

            End If

            'Input
            If Input = Nothing Then
                Input = Arg
                Continue While
            End If

            'Output
            If Output = Nothing Then
                Output = Arg
                Continue While
            End If

            'No more args
            Throw New SimpleException("Too many arguments", "No arguments """ & Arg & """ needed.")

        End While

    End Sub

End Module
