'Represent a context of a construct that have some methods (bassicaly structs & classes)
Public Class MethodContext
    Inherits Context

    ' Function container
    Public ReadOnly Property Functions As FunctionContainer

    ' Constructor
    Public Sub New(Parent As Context, UncompiledFunctions As IEnumerable(Of Source.Function))
        MyBase.New(Parent)
        Functions = New FunctionContainer(UncompiledFunctions, Me)
    End Sub

End Class
