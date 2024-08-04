Public Class FunctionCallNode
    Inherits ExpressionNode

    '========================
    '======== VALUES ========
    '========================
    Private ReadOnly TargetedFunction As ExpressionNode
    Private ReadOnly PassedArguments As IEnumerable(Of ExpressionNode)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, TargetedFunction As ExpressionNode, PassedArguments As IEnumerable(Of ExpressionNode))
        MyBase.New(Location)

        'Set values
        Me.TargetedFunction = TargetedFunction
        Me.PassedArguments = PassedArguments

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        'Get function signature
        If TypeOf TargetedFunction.ReturnType(Scope) IsNot FunctionSignatureType Then
            Throw New LocalizedException("This expression does not return a function.", $"The expression returns an ""{TargetedFunction.ReturnType(Scope).ToString()}"" value, but only ""fun"" values can be called.", TargetedFunction.Location)
        End If
        Dim FunctionSignature As FunctionSignatureType = TargetedFunction.ReturnType(Scope)

        'Return function signature's return type
        Return FunctionSignature.ReturnType

    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        'Get function signature
        If TypeOf TargetedFunction.ReturnType(Scope) IsNot FunctionSignatureType Then
            Throw New LocalizedException("This expression does not return a function.", $"The expression returns an ""{TargetedFunction.ReturnType(Scope).ToString()}"" value, but only ""fun"" values can be called.", TargetedFunction.Location)
        End If
        Dim FunctionSignature As FunctionSignatureType = TargetedFunction.ReturnType(Scope)

        'Compile arguments
        Dim CompiledPassedArguments As String = ""
        For Each PassedArgument As ExpressionNode In PassedArguments
            CompiledPassedArguments &= ", " & PassedArgument.Compile(Scope)
        Next
        If PassedArguments.Count > 0 Then
            CompiledPassedArguments = CompiledPassedArguments.Substring(2)
        End If

        'Return function signature's return type
        Return FunctionSignature.CallCompiledName & "(" & TargetedFunction.Compile(Scope) & If(CompiledPassedArguments = "", "", ", ") & CompiledPassedArguments & ")"

    End Function

End Class
