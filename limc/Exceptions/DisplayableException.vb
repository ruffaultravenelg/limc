Public MustInherit Class DisplayableException
    Inherits Exception

    Public Sub Display()
        Console.ResetColor()
        Print()
        Console.ResetColor()
    End Sub

    Protected MustOverride Sub Print()

End Class
