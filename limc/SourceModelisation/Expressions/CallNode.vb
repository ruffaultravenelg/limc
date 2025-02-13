Namespace Source

    Public Class CallNode
        Inherits ExpressionNode

        '
        ' Could be :
        '   - A call to a function
        '   - A structure initialization
        '

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

            '--------------------------------
            '--- Structure initialisation ---
            '--------------------------------

            If TypeOf Target Is IStructureName Then

                'Cast
                Dim Struct As Lim.StructType = DirectCast(Target, IStructureName).ResolveStructure(Context)

                If Struct IsNot Nothing Then
                    Return Struct
                End If

            End If

            '---------------------
            '--- Function Call ---
            '---------------------

            Dim TargetReturnType As Lim.Type = Target.GetReturnType(Context)

            If TypeOf TargetReturnType Is Lim.FuncType Then

                'Cast
                Dim TargetedFunction As Lim.FuncType = TargetReturnType

                'Check return type
                If TargetedFunction.ReturnType = Nothing Then
                    Throw New TypeException("This procedure can't be called as an expression, since it doesn't return a value.", Target.Location)
                End If

                'Return Target return value
                Return TargetedFunction.ReturnType

            End If

            '---------------------
            '--- Nothing found ---
            '---------------------
            Throw New TypeException("A procedure or a structure name was expected here.", Target.Location)

        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String
            Return Compile(Scope, True)
        End Function
        Public Overloads Function Compile(Scope As Scope, CareAboutReturn As Boolean) As String

            '--------------------------------
            '--- Structure initialisation ---
            '--------------------------------

            If TypeOf Target Is IStructureName Then

                'Cast
                Dim Struct As Lim.StructType = DirectCast(Target, IStructureName).ResolveStructure(Scope)

                If Struct IsNot Nothing Then

                    'Compile struct values
                    Dim Args As String = CompileArguments(Struct.FieldsTypes(), Scope)
                    If Args.StartsWith(", ") Then
                        Args = Args.Substring(2)
                    End If

                    'Instanciate
                    Return "(" & Struct.CompiledName & "){" & Args & "}"

                End If

            End If

            '---------------------
            '--- Function Call ---
            '---------------------

            Dim TargetReturnType As Lim.Type = Target.GetReturnType(Scope)

            If TypeOf TargetReturnType Is Lim.FuncType Then

                'Cast
                Dim TargetedFunction As Lim.FuncType = TargetReturnType

                'Check if the fun return something
                If CareAboutReturn AndAlso TargetedFunction.ReturnType = Nothing Then
                    Throw New TypeException("This procedure can't be called as an expression, since it doesn't return a value.", Target.Location)
                End If

                'Compile all arguments
                Dim Args As String = CompileArguments(TargetedFunction.PassedGenericTypes, Scope)

                'Return a call
                Return $"{Target.Compile(Scope)}.fn(&ctx{Args})"

            End If

            '---------------------
            '--- Nothing found ---
            '---------------------
            Throw New TypeException("A procedure or a structure name was expected here.", Target.Location)

        End Function

        'Compile arguments
        Private Function CompileArguments(Model As IEnumerable(Of Lim.Type), Context As Context) As String

            'Argument count
            If Not PassedArguments.Count = Model.Count Then
                Throw New SyntaxException($"{Model.Count} arguments were expected where you gave {PassedArguments.Count}.", Location)
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