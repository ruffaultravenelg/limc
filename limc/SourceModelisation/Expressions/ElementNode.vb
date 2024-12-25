Namespace Source

    '
    ' A variable -> myVar
    ' A function -> myFunc
    ' A function -> lib::myFunc
    '
    Public Class ElementNode
        Inherits ExpressionNode

        'Value
        Private File As String
        Private Value As String

        'Constructor
        Public Sub New(Location As Location, File As String, Value As String)
            MyBase.New(Location)
            Me.File = File
            Me.Value = Value
        End Sub

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type

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
                Return FileCorrespondances.First().PointerType

            End If

            'Check for varaibles
            Dim Variable As Lim.Variable = Context.Variable(Value)
            If Variable IsNot Nothing Then
                Return Variable.Type
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
    End Class

End Namespace