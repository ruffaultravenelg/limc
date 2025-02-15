'Represent the context for statements and expressions
Public Class Context

    'Generic types
    Public ReadOnly Property GenericTypes As New Dictionary(Of String, Lim.Type)

    'Scope variables
    Public ReadOnly LocalVariables As New Dictionary(Of String, Lim.Variable)

    'Search a variable
    Public ReadOnly Property Variable(Name As String) As Lim.Variable
        Get
            For Each Upper As Context In Parents
                If Upper.LocalVariables.ContainsKey(Name) Then
                    Return Upper.LocalVariables(Name)
                End If
            Next
            Return Nothing
        End Get
    End Property

    'Parent context
    Private Parent As Context

    'Constructor
    Public Sub New(Optional Parent As Context = Nothing)
        Me.Parent = Parent
    End Sub

    'Parents
    Protected ReadOnly Iterator Property Parents As IEnumerable(Of Context)
        Get
            Dim Parent As Context = Me
            While Parent IsNot Nothing
                Yield Parent
                Parent = Parent.Parent
            End While
        End Get
    End Property

    'Search generic Type
    Public ReadOnly Property GenericType(Name As String) As Lim.Type
        Get

            'Search for generic type
            For Each Parent As Context In Parents
                If Parent.GenericTypes.ContainsKey(Name) Then
                    Return Parent.GenericTypes(Name)
                End If
            Next

            'Not found
            Return Nothing

        End Get
    End Property

    'Register the existance of a variable
    Public Function RegisterVariable(Name As String, Type As Lim.Type) As Lim.Variable

        'Create variable
        Dim Variable As New Lim.Variable(C.Namer.GenerateVariableName(), Type)

        'Add it to context
        If Not LocalVariables.TryAdd(Name, Variable) Then
            Return Nothing 'If the variable is not added -> return nothing
        End If

        'Return varaible
        Return Variable

    End Function

    'Register the existance of a variable with a custom compiled name
    Public Function RegisterVariable(Name As String, CompiledName As String, Type As Lim.Type) As Lim.Variable

        'Create variable
        Dim Variable As New Lim.Variable(CompiledName, Type)

        'Add it to context
        If Not LocalVariables.TryAdd(Name, Variable) Then
            Return Nothing 'If the variable is not added -> return nothing
        End If

        'Return varaible
        Return Variable

    End Function

    'Get specific scope
    Public Function GetScope(Of T As Context)() As T

        'Search for returnable scope
        For Each Parent As Context In Parents
            If TypeOf Parent Is T Then
                Return Parent
            End If
        Next

        'Not found
        Return Nothing

    End Function

End Class
