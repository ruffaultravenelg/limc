Imports limc.AST

Namespace TypeSystem

    Public Class EnumOption
        Public ReadOnly Property Name As String
        Public ReadOnly Property CompiledName As String

        ' Only for options that have a value
        Public Property Type As Type = Nothing
        Private ReadOnly _Typenode As TypeNode
        Public Property UnionValueCompiledName As String
        Private structCompiledName As String ' Unsed for value enum, to be able to create a struct value

        ' Return True if this option contains a value
        Public ReadOnly Property HasValue As Boolean
            Get
                Return _Typenode IsNot Nothing OrElse Type IsNot Nothing
            End Get
        End Property

        ' Constructors
        Public Sub New(Name As String, CompiledName As String)
            Me.Name = Name
            Me.CompiledName = CompiledName
        End Sub
        Public Sub New(Name As String, CompiledName As String, Typenode As TypeNode, structCompiledName As String)
            Me.New(Name, CompiledName)
            Me._Typenode = Typenode
            Me.UnionValueCompiledName = CompiledName & "_value"
            Me.structCompiledName = structCompiledName
        End Sub

        Public Overridable Function CompileValue() As String
            If HasValue Then
                Throw New InternalError("Trying to compile enum option with value without a value")
            Else
                Return CompiledName
            End If
        End Function

        Public Overridable Function CompileValue(OptionValue As ExpressionNode, Writer As CWriter, Scope As Context.Scope) As String
            If Not HasValue Then
                Throw New InternalError("Trying to compile class enum option with a value")
            End If

            ' Check if value type match option type
            If OptionValue.GetExpressionReturnType(Scope) <> Type Then
                Throw New TypeMismatchError(Type, OptionValue.GetExpressionReturnType(Scope), OptionValue.Location)
            End If

            ' Compile option value
            Dim CompiledValue As String = OptionValue.CompileExpression(Writer, Scope)

            ' Return object
            Return "(" & structCompiledName & "){ .discriminator = " & CompiledName & ", .values." & UnionValueCompiledName & " = " & CompiledValue & " }"

        End Function

        Friend Shared Function FromFields(Fields As IEnumerable(Of EnumConstruct.Field), EnumCompiledName As String) As IEnumerable(Of EnumOption)
            Dim EnumOptions As New List(Of EnumOption)
            For i = 0 To Fields.Count() - 1
                Dim Field = Fields(i)
                If EnumOptions.Any(Function(o) o.Name = Field.Name) Then
                    Throw New ElementAlreadyExistError(Field.Name, Field.Location)
                End If
                If Field.HasValue Then
                    EnumOptions.Add(New EnumOption(Field.Name, $"{EnumCompiledName}_{i}", Field.Type, EnumCompiledName))
                Else
                    EnumOptions.Add(New EnumOption(Field.Name, $"{EnumCompiledName}_{i}"))
                End If
            Next
            Return EnumOptions
        End Function

        Public Sub ResolveOptionType(Context As Context.Context)
            If _Typenode IsNot Nothing Then
                Me.Type = _Typenode.GetAssociatedType(Context)
            End If
        End Sub

    End Class

End Namespace