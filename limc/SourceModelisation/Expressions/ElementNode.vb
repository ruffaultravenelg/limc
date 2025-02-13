Namespace Source

    '
    ' A variable -> myVar
    ' A function -> myFunc
    ' A function -> lib::myFunc
    ' A structure -> myStruct 'TODO
    ' A structure -> lib::myStruct 'TODO
    ' A method -> myMethod 'TODO
    '
    Public Class ElementNode
        Inherits ExpressionNode
        Implements IStructureName

        'Value
        Private File As String
        Private Value As String

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
                Return New ElementNotFoundException($"No variable, function, method or structure named ""{Value}"" is accessible.", Location)
            Else
                Return New ElementNotFoundException($"No function or structure named ""{Value}"" is accessible in the ""{File}"" namespace.", Location)
            End If
        End Function

    End Class

End Namespace