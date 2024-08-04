Public Class CSourceFunction
    Implements BuildableFunction

    '=========================
    '======== CONTENT ========
    '=========================
    Private Prototype As String
    Private Code As IEnumerable(Of String)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Private Sub New(Prototype As String, Code As IEnumerable(Of String))

        'Notify ourself
        FileBuilder.NotifyNewFunction(Me)

        'Set values
        Me.Prototype = Prototype
        Me.Code = Code

    End Sub

    '=================================
    '======== BUILD PROTOTYPE ========
    '=================================
    Private Function BuildPrototype() As String Implements BuildableFunction.BuildPrototypeWithoutSemiColon
        Return Prototype
    End Function

    '=============================
    '======== BUILD LOGIC ========
    '=============================
    Private Function BuildLogic() As IEnumerable(Of String) Implements BuildableFunction.BuildLogic
        Return Code
    End Function

    '==========================================
    '======== GENERATE SOURCE FUNCTION ========
    '==========================================
    Public Shared Sub GenerateSourceFunction(Prototype As String, Code As IEnumerable(Of String))
        Dim Temp As New CSourceFunction(Prototype, Code)
    End Sub

End Class
