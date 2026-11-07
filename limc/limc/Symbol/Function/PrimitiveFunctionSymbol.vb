Imports limc.HIR.Lowerer

Namespace Symbol
    Friend Class PrimitiveFunctionSymbol
        Inherits FunctionSymbol

        Private Sub New(LimName As String, Arguments As IEnumerable(Of VariableSymbol), ReturnType As TypeSymbol)
            MyBase.New(LimName, Array.Empty(Of TypeSymbol), Arguments, ReturnType)
        End Sub

        Public Overrides Sub Compile(Lowerer As IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder)
            ' do nothing => C function already exist in "runtime/" directory
        End Sub

        Public Class Builder
            Private LimName As String
            Private Arguments As New List(Of VariableSymbol)
            Private ReturnType As TypeSymbol = Nothing

            Public Function Build() As PrimitiveFunctionSymbol
                If String.IsNullOrEmpty(LimName) Then
                    Throw New InternalException("Cannot build a primitive function with no name. Use .SetName to add one")
                End If
                Return New PrimitiveFunctionSymbol(LimName, Arguments, ReturnType)
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