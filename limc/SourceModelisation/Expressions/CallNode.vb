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

            '-----------------------------
            '--- Direct procedure call ---
            '-----------------------------
            If TypeOf Target Is IProcedureDirectAccess Then

                Dim ArgumentTypes As IEnumerable(Of Lim.Type) = PassedArguments.Select(Function(Expr As ExpressionNode) Expr.GetReturnType(Context))
                Dim FuncReturnType As Lim.Type = DirectCast(Target, IProcedureDirectAccess).GetProcedureReturnedType(Context, ArgumentTypes)
                If FuncReturnType IsNot Nothing Then
                    Return FuncReturnType
                End If

            End If

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

            '----------------------------
            '--- Direct function call ---
            '----------------------------
            If TypeOf Target Is IProcedureDirectAccess Then

                Dim CompiledFuncCall As String = DirectCast(Target, IProcedureDirectAccess).CompileProcedureCall(Scope, PassedArguments)
                If CompiledFuncCall IsNot Nothing Then
                    Return CompiledFuncCall
                End If

            End If

            '--------------------------------
            '--- Structure initialisation ---
            '--------------------------------
            If TypeOf Target Is IStructureName Then

                'Cast
                Dim Struct As Lim.StructType = DirectCast(Target, IStructureName).ResolveStructure(Scope)

                If Struct IsNot Nothing Then
                    Return Struct.Constuct(PassedArguments, Scope, Location)
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
                Dim Args As String = Source.CallNode.CompileArguments(TargetedFunction.PassedGenericTypes, PassedArguments, Scope, Location)

                'Return a call
                Return C.Function.WriteCall($"{Target.Compile(Scope)}.fn", Args)

            End If

            '---------------------
            '--- Nothing found ---
            '---------------------
            Throw New TypeException("A procedure or a structure name was expected here.", Target.Location)

        End Function

        'Compile arguments
        Public Shared Function CompileArguments(Model As IEnumerable(Of Lim.Type), PassedArguments As IEnumerable(Of ExpressionNode), Scope As Scope, NodeLocation As Location) As String

            'Argument count
            If Not PassedArguments.Count = Model.Count Then
                Throw New SyntaxException($"{Model.Count} arguments were expected where you gave {PassedArguments.Count}.", NodeLocation)
            End If

            'Compile each arguments
            Dim Result As String = ""
            For i As Integer = 0 To Model.Count - 1

                If Not Model(i) = PassedArguments(i).GetReturnType(Scope) Then
                    Throw New TypeException($"The specified argument is of type ""{PassedArguments(i).GetReturnType(Scope)}"" whereas a ""{Model(i)}"" type was expected.", PassedArguments(i).Location)
                End If

                Result &= ", " & PassedArguments(i).Compile(Scope)

            Next

            'Return result
            Return Result

        End Function

        Public Shared Function CompileArguments(PassedArguments As IEnumerable(Of ExpressionNode), Scope As Scope) As String

            Dim Result As String = ""

            For Each Arg As ExpressionNode In PassedArguments
                Result &= ", " & Arg.Compile(Scope)
            Next

            Return Result

        End Function

    End Class

End Namespace