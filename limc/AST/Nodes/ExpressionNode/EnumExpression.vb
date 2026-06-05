Imports limc.TypeSystem

Namespace AST
    Public Class EnumExpression
        Inherits ExpressionNode

        Private Typenode As TypeNode
        Private OptionName As String
        Private OptionValue As ExpressionNode 'may be null

        Public Sub New(Typenode As TypeNode, OptionName As String, OptionValue As ExpressionNode, Location As Location)
            MyBase.New(Location)
            Me.Typenode = Typenode
            Me.OptionName = OptionName
            Me.OptionValue = OptionValue
        End Sub

        ' Get enum type & validate TypeSystem.EnumType
        Private Function GetEnumType(Context As Context.Context) As TypeSystem.EnumType
            Dim T = Typenode.GetAssociatedType(Context)
            If TypeOf T IsNot EnumType Then
                Throw New SyntaxError("Expected an enum type", Location)
            End If
            Return T
        End Function

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return GetEnumType(Context)
        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String

            ' Retrieve the enum
            Dim E = GetEnumType(Scope)

            ' Get option
            Dim Opt = E.GetOptionByName(OptionName)
            If Opt Is Nothing Then
                Throw New SyntaxError($"Enum '{E.Name}' does not contain an option named '{OptionName}'", Location)
            End If

            ' Check if user provided a value
            If OptionValue IsNot Nothing Then

                ' user provided a value
                If Opt.HasValue Then
                    Return Opt.CompileValue(Writer, Scope, OptionValue)
                Else
                    Throw New SyntaxError($"Option '{OptionName}' of enum '{E.Name}' does not take a value", Location)
                End If

            Else

                ' user provided no value

                If Opt.HasValue Then
                    Throw New SyntaxError($"Option '{OptionName}' of enum '{E.Name}' requires a value", Location)
                Else
                    Return Opt.CompileValue()
                End If

            End If

        End Function

    End Class
End Namespace