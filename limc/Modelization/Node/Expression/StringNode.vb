Public Class StringNode
    Inherits ExpressionNode

    '=======================
    '======== VALUE ========
    '=======================
    Private Value As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Value As String)
        MyBase.New(Location)

        'Set values
        Me.Value = Value

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type
        Return Type.str
    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        'Get string
        Dim Str As String = Sanitaze(Value)

    End Function

    '==========================
    '======== SANITIZE ========
    '==========================
    Function Sanitaze(input As String) As String
        Return input.Replace("\", "\\") _
                    .Replace("""", "\""")
    End Function


End Class
