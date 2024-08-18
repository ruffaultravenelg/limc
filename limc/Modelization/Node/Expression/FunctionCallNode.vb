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

    '
    ' WAY TO DETERMINE WICH PROCEDURE TO CALL :
    ' 
    ' If the Node is a ProcedureExplicitName
    '     If there is only one procedure that match this name
    '         -> call it will trying to convert provided arguments
    '     Elseif there is multiple matchs
    '         -> Choose the procedure that have the sames arguments types
    '
    ' If the return type of the node is a FunctionSignatureType
    '     -> Return the return type of the FunctionSignatureType
    ' Else
    '     -> Error
    '

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        'If procedure
        If TypeOf TargetedFunction Is ProcedureSelectorNode Then

            'cast ExplicitName
            Dim ExplicitName As ProcedureSelectorNode = TargetedFunction

            'Get procedure return type
            Dim Procedure As CompiledProcedure = ExplicitName.GetProcedureByYourself(Scope)
            If Procedure IsNot Nothing Then
                Return Procedure.ReturnType
            End If

            'Choose by argument
            Procedure = ExplicitName.GetProcedureWithHelpOfArgs(Scope, PassedArguments)
            If Procedure IsNot Nothing Then
                Return Procedure.ReturnType
            End If

        End If

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

        'If procedure
        If TypeOf TargetedFunction Is ProcedureSelectorNode Then

            'cast ExplicitName
            Dim ExplicitName As ProcedureSelectorNode = TargetedFunction

            'Get procedure return type
            Dim Procedure As CompiledProcedure = ExplicitName.GetProcedureByYourself(Scope)
            If Procedure IsNot Nothing Then
                Return ExplicitName.CompileCallTo(Procedure, Scope, PassedArguments)
            End If

            'Choose by argument
            Procedure = ExplicitName.GetProcedureWithHelpOfArgs(Scope, PassedArguments)
            If Procedure IsNot Nothing Then
                Return ExplicitName.CompileCallTo(Procedure, Scope, PassedArguments)
            End If

        End If

        'Get function signature
        If TypeOf TargetedFunction.ReturnType(Scope) IsNot FunctionSignatureType Then
            Throw New LocalizedException("This expression does not return a function.", $"The expression returns an ""{TargetedFunction.ReturnType(Scope).ToString()}"" value, but only ""fun"" values can be called.", TargetedFunction.Location)
        End If
        Dim FunctionSignature As FunctionSignatureType = TargetedFunction.ReturnType(Scope)

        'Argument count
        If FunctionSignature.ArgumentsTypes.Count > PassedArguments.Count Then
            Throw New LocalizedException("Not enough arguments", "Not enough arguments for a function of type """ & FunctionSignature.ToString() & """.", Me.Location)
        ElseIf FunctionSignature.ArgumentsTypes.Count < PassedArguments.Count Then
            Throw New LocalizedException("Too many arguments", "Too many arguments for a function of type """ & FunctionSignature.ToString() & """.", Me.Location)
        End If

        'Compile arguments
        Dim CompiledPassedArguments As String = ""
        For i As Integer = 0 To PassedArguments.Count - 1

            'Check type error
            If Not PassedArguments(i).CanReturn(FunctionSignature.ArgumentsTypes(i), Scope) Then
                Throw New LocalizedException("The expected type was """ & FunctionSignature.ArgumentsTypes(i).ToString() & """", "This argument cannot return the """ & FunctionSignature.ArgumentsTypes(i).ToString() & """ type desired by the procedure.", PassedArguments(i).Location)
            End If

            'Compile call
            CompiledPassedArguments &= ", " & PassedArguments(i).Compile(Scope, FunctionSignature.ArgumentsTypes(i))

        Next
        If PassedArguments.Count > 0 Then
            CompiledPassedArguments = CompiledPassedArguments.Substring(2)
        End If

        'Return function signature's return type
        Return FunctionSignature.CallCompiledName & "(" & TargetedFunction.Compile(Scope) & If(CompiledPassedArguments = "", "", ", ") & CompiledPassedArguments & ")"

    End Function

End Class
