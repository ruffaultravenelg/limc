Public Class UnknownOrUnreachableConstructorError
    Inherits LocatedError
    Public Sub New(Classe As TypeSystem.Type, ArgumentTypes As IEnumerable(Of TypeSystem.Type), Location As Location)
        MyBase.New($"No constructor match thoses arguments", $"The ""{Classe.ToString()}"" type does not have an accessible constructor that use types ({String.Join(", ", ArgumentTypes.Select(Function(t) t.ToString()))}).", Location)
    End Sub
End Class
