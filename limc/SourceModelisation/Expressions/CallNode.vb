Namespace Source

    Public Class CallNode
        Inherits ExpressionNode

        'Value
        Private Target As ExpressionNode
        Private PassedArguments As IEnumerable(Of ExpressionNode)

        'Constructor
        Public Sub New(Location As Location, Target As ExpressionNode, PassedArguments As IEnumerable(Of ExpressionNode))
            MyBase.New(Location)
            Me.Target = Target
            Me.PassedArguments = PassedArguments
        End Sub

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type

            'Get target return type
            Dim TargetReturnType As Lim.Type = Target.GetReturnType(Context)

            'Check if function
            If TypeOf TargetReturnType IsNot FuncType Then
                Throw New TypeException("A procedure was expected here.", Target.Location)
            End If
            Dim TargetedFunction As FuncType = TargetReturnType

            'Check if the fun return something
            If TargetedFunction.ReturnType = Nothing Then
                Throw New TypeException("This procedure can't be called as an expression, since it doesn't return a value.", Target.Location)
            End If

            'Return Target return value
            Return TargetedFunction.ReturnType

        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String
            Return Compile(Scope, True)
        End Function
        Public Overloads Function Compile(Scope As Scope, CareAboutReturn As Boolean) As String

            'Get target return type
            Dim TargetReturnType As Lim.Type = Target.GetReturnType(Scope)

            'Check if function
            If TypeOf TargetReturnType IsNot FuncType Then
                Throw New TypeException("A procedure was expected here.", Target.Location)
            End If
            Dim TargetedFunction As FuncType = TargetReturnType

            'Check if the fun return something
            If CareAboutReturn AndAlso TargetedFunction.ReturnType = Nothing Then
                Throw New TypeException("This procedure can't be called as an expression, since it doesn't return a value.", Target.Location)
            End If

            'Compile all arguments
            Dim Args As String = CompileArguments(TargetedFunction.ArgumentTypes, Scope)

            'Return a call
            Return $"{Target.Compile(Scope)}.fn(&ctx{Args})"

        End Function

        'Compile arguments
        Private Function CompileArguments(Model As IEnumerable(Of Lim.Type), Context As Context) As String

            'Argument count
            If Not PassedArguments.Count = Model.Count Then
                Throw New SyntaxException($"The procedure requires {Model.Count} arguments, whereas you provide {PassedArguments.Count}.", Location)
            End If

            'Compile each arguments
            Dim Result As String = ""
            For i As Integer = 0 To Model.Count - 1

                If Not Model(i) = PassedArguments(i).GetReturnType(Context) Then
                    Throw New TypeException($"The specified argument is of type ""{PassedArguments(i).GetReturnType(Context)}"" whereas a ""{Model(i)}"" type was expected.", PassedArguments(i).Location)
                End If

                Result &= ", " & PassedArguments(i).Compile(Context)

            Next

            'Return result
            Return Result

        End Function

    End Class

End Namespace