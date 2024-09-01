'
' Represents a procedure from the C point of view, in other words a function, a method, a getter, a setter, a helper function, etc...
'
Public Interface IBuildableFunction

    Function BuildPrototypeWithoutSemiColon() As String
    Function BuildLogic() As IEnumerable(Of String)

End Interface
