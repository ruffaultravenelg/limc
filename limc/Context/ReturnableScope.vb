Namespace Context
    Public Class ReturnableScope
        Inherits Scope

        Private _ReturnType As TypeSystem.Type = Nothing
        Public ReadOnly Property ReturnType As TypeSystem.Type
            Get
                If _ReturnType Is Nothing Then
                    Throw New TypeError("The return type of this block is used before it is defined. Remember to explicitly define the return type when defining the block.", Location)
                End If
                Return _ReturnType
            End Get
        End Property

        Public Sub New(Parent As Context, Location As Location)
            MyBase.New(Parent, Location)
        End Sub

        Public Sub DefineReturnType(Type As TypeSystem.Type, EventLocation As Location)
            If _ReturnType Is Nothing Then
                _ReturnType = Type
            ElseIf Not _ReturnType = Type Then
                Throw New TypeMismatchError(_ReturnType, Type, EventLocation)
            End If
        End Sub

    End Class

End Namespace