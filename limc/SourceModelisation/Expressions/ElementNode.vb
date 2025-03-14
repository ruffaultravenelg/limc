Namespace Source

    '
    ' A variable -> myVar
    ' A function -> myFunc
    ' A function -> lib::myFunc
    ' A structure -> myStruct
    ' A structure -> lib::myStruct
    ' A method -> myMethod
    '
    Public Class ElementNode
        Inherits ExpressionNode
        Implements IStructureName
        Implements IProcedureDirectAccess

        'Value
        Public ReadOnly Property File As String
        Public ReadOnly Property Value As String

        'Constructor
        Public Sub New(Location As Location, File As String, Value As String)
            MyBase.New(Location)
            Me.File = File
            Me.Value = Value
        End Sub

        ' Get variable
        Private Function GetVariable(Context As Context) As Lim.Variable

            'If there is a file, this is not a variable
            If File <> "" Then
                Return Nothing
            End If

            'Check all variables in parent context
            Dim Variable As Lim.Variable = Context.Variable(Value)
            If Variable IsNot Nothing Then
                Return Variable
            End If

            'Nothing found
            Return Nothing

        End Function

        ' Get a function
        Private Function GetFunction(Context As Context) As Lim.Function

            If File = "" Then

                'Get correspondance in current file
                Dim Correspondances As IEnumerable(Of Lim.Function) = Location.File.GetFunctions(Value, {})

                'No corresponding function found
                If Correspondances.Count = 0 Then
                    Return Nothing
                End If

                'Too many functions
                If Correspondances.Count > 1 Then
                    Throw New SyntaxException("More than one functions can be access by this statement, please specifie wich one", Location)
                End If

                'Return function pointer
                Return Correspondances.First()

            Else

                'Get correspondance in selected file
                Dim FileCorrespondances As IEnumerable(Of Lim.Function) = Location.File.GetFunctions(File, Value, {})

                'File is not imported
                If FileCorrespondances Is Nothing Then
                    Throw New SyntaxException("""" & File & """ is not imported.", Location)
                End If

                'No corresponding function found
                If FileCorrespondances.Count = 0 Then
                    Return Nothing
                End If

                'Too many functions
                If FileCorrespondances.Count > 1 Then
                    Throw New SyntaxException("More than one functions can be access by this statement, please specifie wich one", Location)
                End If

                'Return function pointer
                Return FileCorrespondances.First()

            End If

        End Function

        ' Get a structure
        Private Function GetStructure(Context As Context) As Lim.StructType Implements IStructureName.ResolveStructure

            Dim Correspondance As Lim.Type

            If File = "" Then
                Correspondance = Location.File.GetAType(Value, {})
            Else
                Correspondance = Location.File.GetAType(File, Value, {})
            End If

            'No type found
            If Correspondance Is Nothing Then
                Return Nothing
            End If

            'Type is not a structure
            If TypeOf Correspondance IsNot Lim.StructType Then
                Return Nothing
            End If

            'Return struct
            Return Correspondance

        End Function

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type

            '---------------------------
            '--- A variable -> myVar ---
            '---------------------------
            Dim Variable As Lim.Variable = GetVariable(Context)
            If Variable IsNot Nothing Then
                Return Variable.Type
            End If

            '------------------------------------------
            '--- A function -> myFunc / lib::myFunc ---
            '------------------------------------------
            Dim Func As Lim.Function = GetFunction(Context)
            If Func IsNot Nothing Then
                Return Func.PointerType
            End If

            '-------------------------------------
            '--- Nothing found, throw an error ---
            '-------------------------------------
            Throw MakeNotFoundExcepetion()

        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String

            '---------------------------
            '--- A variable -> myVar ---
            '---------------------------
            Dim Variable As Lim.Variable = GetVariable(Scope)
            If Variable IsNot Nothing Then
                Return Variable.CompiledName
            End If

            '------------------------------------------
            '--- A function -> myFunc / lib::myFunc ---
            '------------------------------------------
            Dim Func As Lim.Function = GetFunction(Scope)
            If Func IsNot Nothing Then
                Return Func.PointerType.Wrap(Func.CompiledName)
            End If

            '-------------------------------------
            '--- Nothing found, throw an error ---
            '-------------------------------------
            Throw MakeNotFoundExcepetion()

        End Function

        'Make not found excepetion
        Private Function MakeNotFoundExcepetion() As DisplayableException
            If File = "" Then
                Return New ElementNotFoundException($"No variable, function or structure named ""{Value}"" is accessible.", Location)
            Else
                Return New ElementNotFoundException($"No function or structure named ""{Value}"" is accessible in the ""{File}"" namespace.", Location)
            End If
        End Function

        ' If this is a procedure, get it's return type
        Function GetProcedureReturnedType(Context As Context, PassedArguments As IEnumerable(Of Lim.Type)) As Lim.Type Implements IProcedureDirectAccess.GetProcedureReturnedType

            ' Try getting a method
            Dim Method As Lim.Function = GetMethodWitharguments(Context, PassedArguments)
            If Method IsNot Nothing Then
                Return Method.ReturnType
            End If

            ' Try getting a function
            Dim Func As Lim.Function = GetFunctionWithArguments(Context, PassedArguments)
            If Func IsNot Nothing Then
                Return Func.ReturnType
            End If

            ' Nothing found 
            Return Nothing

        End Function

        Function CompileProcedureCall(Scope As Scope, Passedarguments As IEnumerable(Of ExpressionNode)) As String Implements IProcedureDirectAccess.CompileProcedureCall

            ' Get passed arguments types
            Dim ArgumentTypes As IEnumerable(Of Lim.Type) = Passedarguments.Select(Function(Expr As ExpressionNode) Expr.GetReturnType(Scope))

            ' Try getting a method
            Dim Method As Lim.Function = GetMethodWitharguments(Scope, ArgumentTypes)
            If Method IsNot Nothing Then
                Dim Args As String = Source.CallNode.CompileArguments(Method.Arguments, Passedarguments, Scope, Location)
                Return Method.CompiledName & "(&ctx, self" & Args & ")"
            End If


            ' Try getting a function
            Dim Func As Lim.Function = GetFunctionWithArguments(Scope, ArgumentTypes)
            If Func IsNot Nothing Then
                Dim Args As String = Source.CallNode.CompileArguments(Func.Arguments, Passedarguments, Scope, Location)
                Return Func.CompiledName & "(&ctx" & Args & ")"
            End If

            ' Nothing founnd
            Return Nothing

        End Function

        'Get functions with arguments
        Private Function GetFunctionWithArguments(Context As Context, Arguments As IEnumerable(Of Lim.Type)) As Lim.Function

            If File = "" Then
                Return Location.File.GetFunction(Value, {}, Arguments)
            Else
                Return Location.File.GetFunction(File, Value, {}, Arguments)
            End If

        End Function

        ' Get a method with arguments
        Private Function GetMethodWitharguments(Context As Context, Arguments As IEnumerable(Of Lim.Type)) As Lim.Function

            ' Make no sens to have a file indicator for a method
            If File <> "" Then
                Return Nothing
            End If

            ' Get method context
            Dim MethodContext As MethodContext = Context.GetScope(Of MethodContext)
            If MethodContext Is Nothing Then
                Return Nothing
            End If

            ' Return correspondance
            Return MethodContext.Functions.GetCorrespondance(Value, {}, Arguments)

        End Function

    End Class

End Namespace