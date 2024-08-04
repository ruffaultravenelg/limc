Public MustInherit Class StatementNode
    Inherits Node

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

    '==========================
    '======== CONTAINS ========
    '==========================
    Public Shared Function ListContains(Of Statement As StatementNode)(List As List(Of StatementNode)) As Boolean

        ' Return true if list contains a [Statement] statement.
        For Each Line As StatementNode In List
            If TypeOf Line Is LogicContainerStatement Then
                If DirectCast(Line, LogicContainerStatement).HasStatement(Of Statement)() Then
                    Return True
                End If
            End If
        Next

        ' No statement found
        Return False

    End Function

    '=========================
    '======== COMPILE ========
    '=========================
    Public MustOverride Sub Compile(Scope As Scope)

End Class
