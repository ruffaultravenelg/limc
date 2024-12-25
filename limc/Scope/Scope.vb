'Represent the context for statements and expressions, but with lines (a scope)

Public Class Scope
    Inherits Context

    'Lines
    Private Lines As New List(Of String)

    'New
    Public Sub New(Optional Parent As Context = Nothing)
        MyBase.New(Parent)
    End Sub

    'Build
    Public Function Build() As IEnumerable(Of String)
        Return Lines
    End Function

    'Write lines
    Public Sub WriteScope(Scope As Scope)
        For Each Line As String In Scope.Build()
            WriteLine(Line)
        Next
    End Sub

    'Write lines with tab
    Public Sub WriteScopeWithIndentation(Scope As Scope)
        For Each Line As String In Scope.Build()
            WriteLine(vbTab & Line)
        Next
    End Sub

    'Declare variable
    Public Function WriteVariableDeclaration(Name As String, Type As Lim.Type) As Lim.Variable

        'Create variable
        Dim Variable As Lim.Variable = RegisterVariable(Name, Type)

        'If the variable is null -> variable name already exist -> return nothing
        If Variable Is Nothing Then
            Return Nothing
        End If

        'Declare it
        WriteLine($"{Variable.Type.CompiledName} {Variable.CompiledName};")
        WriteVariableAssignation(Variable, Variable.Type.DefaultValue())

        'Return variable
        Return Variable

    End Function
    Public Function WriteVariableDeclaration(Name As String, Type As Lim.Type, Value As String) As Lim.Variable

        'Create variable
        Dim Variable As Lim.Variable = RegisterVariable(Name, Type)

        'If the variable is null -> variable name already exist -> return nothing
        If Variable Is Nothing Then
            Return Nothing
        End If

        'Declare it
        WriteLine($"{Variable.Type.CompiledName} {Variable.CompiledName};")
        WriteVariableAssignation(Variable, Value)

        'Return variable
        Return Variable

    End Function

    'Write return
    Public Sub WriteReturn(Value As String)
        'TODO: clear references of variables
        WriteLine($"return {Value};")
    End Sub

    'Variable assignation
    Public Sub WriteVariableAssignation(Variable As Lim.Variable, Value As String)
        WriteLine(Variable.Type.Assignation(Variable.CompiledName, Value))
    End Sub

    'Writeline
    Public Sub WriteLine(Optional Line As String = "")
        Lines.Add(Line)
    End Sub

End Class
