Public Interface ClassType

    'Constructors
    ReadOnly Property Constructor(ArgumentTypes As IEnumerable(Of Type)) As CConstructor
    Sub NotifyNewCompiledConstructor(Constructor As CConstructor)

    'Properties
    ReadOnly Property Properties As  List(Of Propertie)

    'When a getter is not found
    Class ConstructorNotFoundException
        Inherits CompilerException
        Public Sub New(Type As String)
            MyBase.New("Constructor not found", $"The ""{Type}"" type does not contain an adapter constructor.")
        End Sub
    End Class

End Interface
