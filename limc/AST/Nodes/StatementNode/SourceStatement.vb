Imports System.Text.RegularExpressions

Namespace AST
    Public Class SourceStatement
        Inherits StatementNode

        Private Source As String

        Public Sub New(Source As String, Location As Location)
            MyBase.New(Location)
            Me.Source = Source
        End Sub

        Public Overrides Sub Compile(Scope As Context.Scope)

            ' Regex to remplaces $word by values
            Dim result As String = Regex.Replace(Source, "\$(\w+)", Function(m As Match) ReplaceVariable(m.Groups(1).Value, Scope))

            'Write string directly to source file
            Scope.WriteLine(result)

        End Sub

        Private Function ReplaceVariable(VariableName As String, Context As Context.Context)
            Dim Results As IEnumerable(Of SearchMatch) = Context.RetrieveMatchingElements(VariableName)
            For Each Result In Results
                If Result.Type = SearchMatch.MatchType.MATCH_VARIABLE Then
                    Return Result.MatchingVariable.CompiledName
                End If
            Next
            Throw New SyntaxError($"The variable ""{VariableName}"" could not be identified in this context. Check its name and visibility.", Location)
        End Function

    End Class
End Namespace