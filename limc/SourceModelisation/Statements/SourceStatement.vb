Imports System.Text.RegularExpressions

Namespace Source

    Public Class SourceStatement
        Inherits StatementNode

        'Properties
        Private Source As String

        'Constructor
        Public Sub New(Location As Location, Source As String)
            MyBase.New(Location)
            Me.Source = Source
            ContainsReturnStatement = Source.Contains("return ")
        End Sub

        'Compile
        Public Overrides Sub Compile(Scope As Scope)

            'No variable -> just add line
            If Not Source.Contains("$") Then
                Scope.WriteLine(Source)
            End If

            'Get variable
            Dim Variables As IEnumerable(Of String) = ExtractVariableNames(Source)

            'Replaces
            Dim Result As String = Source
            For Each VariableName As String In Variables

                'Search variable
                Dim Var As Lim.Variable = Scope.Variable(VariableName)
                If Var Is Nothing Then
                    Throw New SyntaxException("The """ & VariableName & """ variable doesn't exist in this context.", Location)
                End If

                'Replace
                Result = Result.Replace("$" & VariableName, Var.CompiledName)

            Next

            'Add line to scope
            Scope.WriteLine(Result)

        End Sub

        'Extract variables from source
        Private Function ExtractVariableNames(Input As String) As IEnumerable(Of String)
            Dim pattern As String = "\$(\w+)" ' Search variables like $nom
            Dim matches = Regex.Matches(Input, pattern)
            Dim variables As New List(Of String)

            For Each match As Match In matches
                variables.Add(match.Groups(1).Value) ' Add match without the $
            Next

            Return variables.Distinct()
        End Function

        'Contains return statement
        Public Overrides ReadOnly Property ContainsReturnStatement As Boolean

    End Class

End Namespace