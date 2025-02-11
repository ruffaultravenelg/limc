Namespace Source

    '
    ' A function -> myFunc<T, D>
    ' A function -> lib::myFunc<T, D>
    ' A Structure -> myStruct<T, D>
    ' A Structure -> lib::myStruct<T, D>
    ' A Method -> myMethod<T, D> 'TODO
    '
    Public Class GenericElementNode
        Inherits ExpressionNode

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

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type

            'Compile arguments
            Dim PassedTypes As IEnumerable(Of Lim.Type) = CompilePassedTypes(Context)

            'If there is a file -> it's a function or a struct
            If Not File = "" Then

                'Get correspondance
                Dim FileCorrespondances As IEnumerable(Of Lim.Function) = Location.File.GetFunctions(File, Value, PassedTypes)

                'File is not imported
                If FileCorrespondances Is Nothing Then
                    Throw New SyntaxException("""" & File & """ is not imported.", Location)
                End If

                'Too many functions
                If FileCorrespondances.Count > 1 Then
                    Throw New SyntaxException("More than one functions can be access by this statement, please specifie wich one", Location)
                End If

                'One function
                If FileCorrespondances.Count = 1 Then
                    Return FileCorrespondances.First().PointerType 'Return function pointer
                End If

                'No function found -> search for structures
                Dim TypesCorrespondances As IEnumerable(Of Lim.Type) = Location.File.GetAType(File, Value, PassedTypes)

                'Filter only structs ones

                Throw New SyntaxException("No functions named """ & Value & """ is accecible.", Location)



            End If

            'Get correspondance
            Dim Correspondances As IEnumerable(Of Lim.Function) = Location.File.GetFunctions(Value, PassedTypes)

            'Empty file
            If Correspondances.Count = 0 Then
                Throw New SyntaxException("No functions or variable named """ & Value & """ is accecible.", Location)
            End If

            'Too many functions
            If Correspondances.Count > 1 Then
                Throw New SyntaxException("More than one functions can be access by this statement, please specifie wich one", Location)
            End If

            'Return function pointer
            Return Correspondances.First().PointerType

        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String

            'If there is a file -> it's a function
            If Not File = "" Then

                'Get correspondance
                Dim FileCorrespondances As IEnumerable(Of Lim.Function) = Location.File.GetFunctions(File, Value, {})

                'File is not imported
                If FileCorrespondances Is Nothing Then
                    Throw New SyntaxException("""" & File & """ is not imported.", Location)
                End If

                'Empty file
                If FileCorrespondances.Count = 0 Then
                    Throw New SyntaxException("No functions named """ & Value & """ is accecible.", Location)
                End If

                'Too many functions
                If FileCorrespondances.Count > 1 Then
                    Throw New SyntaxException("More than one functions can be access by this statement, please specifie wich one", Location)
                End If

                'Return function pointer
                Return FileCorrespondances.First().PointerType.Wrap(FileCorrespondances.First().CompiledName)

            End If

            'Check for varaibles
            Dim Variable As Lim.Variable = Scope.Variable(Value)
            If Variable IsNot Nothing Then
                Return Variable.CompiledName
            End If

            'Check for functions

            'Get correspondance
            Dim Correspondances As IEnumerable(Of Lim.Function) = Location.File.GetFunctions(Value, {})

            'Empty file
            If Correspondances.Count = 0 Then
                Throw New SyntaxException("No functions or variable named """ & Value & """ is accecible.", Location)
            End If

            'Too many functions
            If Correspondances.Count > 1 Then
                Throw New SyntaxException("More than one functions can be access by this statement, please specifie wich one", Location)
            End If

            'Return function pointer
            Return Correspondances.First().PointerType.Wrap(Correspondances.First().CompiledName)

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

    End Class

End Namespace