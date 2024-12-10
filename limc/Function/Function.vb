Namespace Lim
    Public Class [Function]

        'Informations
        Public ReadOnly Property Base As Source.Function
        Public ReadOnly Property GenericTypes As IEnumerable(Of Type)

        'Constructor -> Mustn't start compiling because it's instance is not yet added to the FunctionContainer
        Public Sub New(Base As Source.Function, GenericTypes As IEnumerable(Of Type))
            Me.Base = Base
            Me.GenericTypes = GenericTypes
        End Sub

        'Compile
        Public Sub Compile()

        End Sub

    End Class

End Namespace