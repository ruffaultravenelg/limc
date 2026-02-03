Imports System.Globalization
Imports System.Text

Namespace AST
    Public Class StringExpression
        Inherits ExpressionNode
        Implements IConstantExpression

        Private Value As String
        Public Sub New(Value As String, Location As Location)
            MyBase.New(Location)
            Me.Value = Value
        End Sub

        Public Overrides Function GetExpressionReturnType(Context As Context.Context) As TypeSystem.Type
            Return TypeSystem.Type.Str
        End Function

        Public Overrides Function CompileExpression(Writer As CWriter, Scope As Context.Scope) As String
            Return """" & Sanitaze(Value) & """"
        End Function

        Private Shared Function Sanitaze(Value As String) As String

            Dim normalized As String = Value.Normalize(NormalizationForm.FormD)
            Dim sb As New StringBuilder()

            For Each c As Char In normalized
                Dim uc As UnicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
                If uc <> UnicodeCategory.NonSpacingMark Then
                    sb.Append(c)
                End If
            Next

            Dim noAccents As String = sb.ToString().Normalize(NormalizationForm.FormC)

            Dim escaped As String = noAccents.Replace("""", "\""")

            Return escaped
        End Function


    End Class
End Namespace