Imports System.Text.RegularExpressions

Public Class SourceStatement
    Inherits StatementNode

    '=======================
    '======== VALUE ========
    '=======================
    Private ReadOnly Source As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Source As String)
        MyBase.New(Location)

        'Set value
        Me.Source = Source

    End Sub

    '=========================
    '======== COMPILE ========
    '=========================
    Public Overrides Sub Compile(Scope As Scope)

        'Replace variables names by there values
        Dim regex As New Regex("\$(\w+)")
        Dim evaluator As MatchEvaluator = New MatchEvaluator(Function(match)
                                                                 Dim variableName As String = match.Groups(1).Value
                                                                 Return getVariableValue(variableName, Scope)
                                                             End Function)
        Dim Result As String = regex.Replace(Source, evaluator)

        'Write result
        Scope.WriteLine(Result)

    End Sub

    Private Function getVariableValue(VariableName As String, Scope As Scope) As String

        'Search variable
        For Each Upper As Scope In Scope.Uppers
            For Each Variable As Variable In Upper.Variables
                If Variable.Name = VariableName Then
                    Return Variable.CompiledName
                End If
            Next
        Next

        'Throw error when no variable is found
        Throw New SyntaxErrorException("No variable named """ & VariableName & """ is accessible in this context.", Me.Location)

    End Function

End Class
