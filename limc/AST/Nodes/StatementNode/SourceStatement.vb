Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace AST
    Public Class SourceStatement
        Inherits StatementNode

        Private Source As String

        Public Sub New(Source As String, Location As Location)
            MyBase.New(Location)
            Me.Source = Source
        End Sub

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            ' Regex to remplaces $word by values
            Dim result As String = Regex.Replace(Source, "\$(\w+)", Function(m As Match) ReplaceVariable(m.Groups(1).Value, Scope))
            result = ExpandConstants(result)

            'Write string directly to source file
            Writer.WriteLine(result)

        End Sub

        Private Function ReplaceVariable(VariableName As String, Context As Context.Context)
            Dim Results As IEnumerable(Of SearchMatch) = Context.RetrieveMatchingElements(VariableName, {})
            For Each Result In Results
                If Result.Type = SearchMatch.MatchType.MATCH_VARIABLE Then
                    Return Result.MatchingVariable.CompiledName
                End If
            Next
            Throw New SyntaxError($"The variable ""{VariableName}"" could not be identified in this context. Check its name and visibility.", Location)
        End Function

        Public Function ExpandConstants(input As String) As String
            Dim constantsType As Type = GetType(limc.Constants)
            Dim fields = constantsType.GetFields(BindingFlags.Public Or BindingFlags.Static)
            Dim pattern As String = "@(\w+)"  ' Match @CONSTANT_NAME

            Dim result As String = Regex.Replace(input, pattern, Function(m)
                                                                     Dim constName = m.Groups(1).Value
                                                                     Dim field = fields.FirstOrDefault(Function(f) f.Name = constName)
                                                                     If field IsNot Nothing Then
                                                                         Return field.GetValue(Nothing).ToString()
                                                                     Else
                                                                         Return m.Value 'Do nothing if no field found
                                                                     End If
                                                                 End Function)

            Return result
        End Function

    End Class
End Namespace