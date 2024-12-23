Public Class ArgumentHandler

    'Singleton
    Private Shared _Instance As ArgumentHandler
    Public Shared ReadOnly Property Instance As ArgumentHandler
        Get
            Return _Instance
        End Get
    End Property

    'Input
    Public ReadOnly Property Input As String = Nothing

    'Output
    Public ReadOnly Property Output As String = Nothing

    'Help
    Public ReadOnly Property Help As Boolean = False

    'Version
    Public ReadOnly Property Version As Boolean = False

    'Libs directory
    Public ReadOnly Property LibsDirectory As String = AppContext.BaseDirectory & "/libs"


    'Load arguments
    Private Sub New(Args As IEnumerable(Of String))

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

    'Shared load
    Public Shared Sub LoadArgs(Args As IEnumerable(Of String))

        'Create instance
        _Instance = New ArgumentHandler(Args)

    End Sub

End Class
