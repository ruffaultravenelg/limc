Public Module Compiler

    '===========================
    '======== MAIN FILE ========
    '===========================
    Private _MainFile As LimSource
    Public ReadOnly Property MainFile As LimSource
        Get
            Return _MainFile
        End Get
    End Property

    '====================================
    '======== COMPILE INPUT FILE ========
    '====================================
    Public Sub Compile()

        'Parse main file
        _MainFile = New LimSource(Program.InputFile)

    End Sub

    '==============================================
    '======== EXCEPTION DURING COMPILATION ========
    '==============================================
    Public Class CompilerException
        Inherits Exception

        Private Name As String

        Public Sub New(Name As String, Message As String)
            MyBase.New(Message)
            Me.Name = Name
        End Sub

        Public Sub Print()

            'Print content
            PrintContent()

            'Reset console color
            Console.ResetColor()

        End Sub
        Protected Overridable Sub PrintContent()

            'Print title
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.WriteLine("ERROR: " & Name)

            'Print message
            Console.ResetColor()
            Console.WriteLine(Message)

        End Sub


    End Class

End Module
