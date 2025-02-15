Namespace Source

    '
    ' A function -> myFunc<T, D>
    ' A function -> lib::myFunc<T, D>
    ' A Structure -> myStruct<T, D>
    ' A Structure -> lib::myStruct<T, D>
    ' A Method -> myMethod<T, D>
    '
    Public Class GenericElementNode
        Inherits ExpressionNode
        Implements IStructureName
        Implements IProcedureDirectAccess

        'Value
        Private File As String
        Private Value As String
        Private PassedGenericTypes As IEnumerable(Of Source.Type)

        'Constructor
        Public Sub New(Location As Location, File As String, Value As String, PassedGenericTypes As IEnumerable(Of Source.Type))
            MyBase.New(Location)
            Me.File = File
            Me.Value = Value
            Me.PassedGenericTypes = PassedGenericTypes
        End Sub

        ' Get a function
        Private Function GetFunction(Context As Context, PassedGenericTypes As IEnumerable(Of Lim.Type)) As Lim.Function

            If File = "" Then

                'Get correspondance in current file
                Dim Correspondances As IEnumerable(Of Lim.Function) = Location.File.GetFunctions(Value, PassedGenericTypes)

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
                Dim FileCorrespondances As IEnumerable(Of Lim.Function) = Location.File.GetFunctions(File, Value, PassedGenericTypes)

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
                Correspondance = Location.File.GetAType(Value, CompilePassedTypes(Context))
            Else
                Correspondance = Location.File.GetAType(File, Value, CompilePassedTypes(Context))
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

            'Compile passed generic types
            Dim PassedTypes As IEnumerable(Of Lim.Type) = CompilePassedTypes(Context)

            '----------------------------------------------------
            '--- A function -> myFunc<int> / lib::myFunc<int> ---
            '----------------------------------------------------
            Dim Func As Lim.Function = GetFunction(Context, PassedTypes)
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

            'Compile passedgeneric types
            Dim PassedTypes As IEnumerable(Of Lim.Type) = CompilePassedTypes(Scope)

            '------------------------------------------
            '--- A function -> myFunc / lib::myFunc ---
            '------------------------------------------
            Dim Func As Lim.Function = GetFunction(Scope, PassedGenericTypes)
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
                Return New ElementNotFoundException($"No function, method or structure named ""{Value}"" is accessible.", Location)
            Else
                Return New ElementNotFoundException($"No function or structure named ""{Value}"" is accessible in the ""{File}"" namespace.", Location)
            End If
        End Function

        'Compile passed types
        Private Function CompilePassedTypes(Context As Context) As IEnumerable(Of Lim.Type)

            'Create result
            Dim Result As New List(Of Lim.Type)

            'Search each type in context
            For Each TypeNode As Source.Type In PassedGenericTypes
                Result.Add(TypeNode.GetTargetedType(Context))
            Next

            'Return result
            Return Result

        End Function

        ' If this is a procedure, get it's return type
        Function GetProcedureReturnedType(Context As Context, PassedArguments As IEnumerable(Of Lim.Type)) As Lim.Type Implements IProcedureDirectAccess.GetProcedureReturnedType

            'Get passed generic types
            Dim PassedGenericTypes As IEnumerable(Of Lim.Type) = CompilePassedTypes(Context)

            'Try getting a method
            Dim Method As Lim.Function = GetMethodWitharguments(Context, PassedGenericTypes, PassedArguments)
            If Method IsNot Nothing Then
                Return Method.ReturnType
            End If

            'Try getting a function
            Dim Func As Lim.Function = GetFunctionWithArguments(Context, PassedGenericTypes, PassedArguments)
            If Func IsNot Nothing Then
                Return Func.ReturnType
            End If

            'Nothing found 
            Return Nothing

        End Function

        Function CompileProcedureCall(Scope As Scope, Passedarguments As IEnumerable(Of ExpressionNode)) As String Implements IProcedureDirectAccess.CompileProcedureCall

            'Get passed generic types
            Dim PassedGenericTypes As IEnumerable(Of Lim.Type) = CompilePassedTypes(Scope)
            Dim ArgumentTypes As IEnumerable(Of Lim.Type) = Passedarguments.Select(Function(Expr As ExpressionNode) Expr.GetReturnType(Scope))

            'Try getting a method
            Dim Method As Lim.Function = GetMethodWitharguments(Scope, PassedGenericTypes, ArgumentTypes)
            If Method IsNot Nothing Then
                Dim Args As String = Source.CallNode.CompileArguments(Method.Arguments, Passedarguments, Scope, Location)
                Return Method.CompiledName & "(&ctx, self" & Args & ")"
            End If

            'Try getting a function
            Dim Func As Lim.Function = GetFunctionWithArguments(Scope, PassedGenericTypes, ArgumentTypes)
            If Func IsNot Nothing Then
                Dim Args As String = Source.CallNode.CompileArguments(Func.Arguments, Passedarguments, Scope, Location)
                Return Func.CompiledName & "(&ctx" & Args & ")"
            End If

            'Nothing founnd
            Return Nothing

        End Function

        'Get functions with arguments
        Private Function GetFunctionWithArguments(Context As Context, PassedGenericTypes As IEnumerable(Of Lim.Type), Arguments As IEnumerable(Of Lim.Type)) As Lim.Function

            If File = "" Then
                Return Location.File.GetFunction(Value, PassedGenericTypes, Arguments)
            Else
                Return Location.File.GetFunction(File, Value, PassedGenericTypes, Arguments)
            End If

        End Function

        ' Get a method with arguments
        Private Function GetMethodWitharguments(Context As Context, PassedGenericTypes As IEnumerable(Of Lim.Type), Arguments As IEnumerable(Of Lim.Type)) As Lim.Function

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
            Return MethodContext.Functions.GetCorrespondance(Value, PassedGenericTypes, Arguments)

        End Function

    End Class

End Namespace