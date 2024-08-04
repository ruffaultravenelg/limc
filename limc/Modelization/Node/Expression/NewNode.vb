Public Class NewNode
    Inherits ExpressionNode

    '===============================
    '======== TARGETED TYPE ========
    '===============================
    Private TargetedType As TypeNode

    '==================================
    '======== PASSED ARGUMENTS ========
    '==================================
    Private PassedArguments As IEnumerable(Of ExpressionNode)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, TargetedType As TypeNode, PassedArguments As IEnumerable(Of ExpressionNode))
        MyBase.New(Location)

        'Set values
        Me.TargetedType = TargetedType
        Me.PassedArguments = PassedArguments

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type
        Return TargetedType.AssociatedType(Scope)
    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        'Get type
        Dim Type As Type = TargetedType.AssociatedType(Scope)
        If TypeOf Type IsNot ClassType Then
            Throw New LocalizedException($"""{Type.ToString()}"" is not a class", $"The type ""{Type.ToString()}"" does not come from a class and therefore cannot be instantiated with the ""new"" keyword.", Me.Location)
        End If

        'Compile arguments
        Dim CompiledPassedArguments As New List(Of Type)
        Dim CompiledArguments As String = ""
        For Each PassedArgument As ExpressionNode In PassedArguments
            CompiledPassedArguments.Add(PassedArgument.ReturnType(Scope))
            CompiledArguments &= ", " & PassedArgument.Compile(Scope)
        Next
        If CompiledArguments.StartsWith(", ") Then
            CompiledArguments = CompiledArguments.Substring(2)
        End If

        'Get constructor
        Dim Constructor As CConstructor = DirectCast(Type, ClassType).Constructor(CompiledPassedArguments)

        'Compile constructor call
        Return Constructor.CompiledName & "(" & CompiledArguments & ")"

    End Function

End Class
