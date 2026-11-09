Namespace Symbol
    Friend Class PrimitiveMethodSymbol
        Inherits MethodSymbol

        Private Sub New(LimName As String, AttachedType As TypeSymbol, Arguments As IEnumerable(Of VariableSymbol), ReturnType As TypeSymbol)
            MyBase.New(LimName, AttachedType, Array.Empty(Of TypeSymbol), Arguments, ReturnType)
        End Sub

        Public Overrides Sub Compile(Lowerer As HIR.Lowerer.IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder)
            ' do nothing => C function already exist in "runtime/" directory
        End Sub

        Public Class Builder
            Private AttachedType As TypeSymbol = Nothing
            Private LimName As String
            Private Arguments As New List(Of VariableSymbol)
            Private ReturnType As TypeSymbol = Nothing

            Public Function Build() As PrimitiveMethodSymbol
                If String.IsNullOrEmpty(LimName) Then
                    Throw New InternalException("Cannot build a primitive method with no name. Use .SetName to add one")
                ElseIf AttachedType = Nothing Then
                    Throw New InternalException("Cannot build a primitive method with no attached name. Use .SetAttachedType to add one")
                End If
                Return New PrimitiveMethodSymbol(LimName, AttachedType, Arguments, ReturnType)
            End Function

            Public Function SetAttachedType(Type As TypeSymbol) As Builder
                Me.AttachedType = Type
                Return Me
            End Function

            Public Function SetName(Name As String) As Builder
                LimName = Name
                Return Me
            End Function
            Public Function SetReturnType(Type As TypeSymbol) As Builder
                ReturnType = Type
                Return Me
            End Function
            Public Function NoReturn() As Builder
                ReturnType = Nothing
                Return Me
            End Function
            Public Function AddArgument(ArgumentLimName As String, ArgumentType As TypeSymbol) As Builder
                Arguments.Add(New VariableSymbol(ArgumentLimName, ArgumentType))
                Return Me
            End Function

        End Class

    End Class
End Namespace