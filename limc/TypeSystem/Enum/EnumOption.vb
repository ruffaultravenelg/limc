Imports limc.AST

Namespace TypeSystem

    Public Class EnumOption
        Public ReadOnly Property Name As String
        Public ReadOnly Property CompiledName As String

        Public ReadOnly Property HasValue As Boolean
            Get
                Return Type IsNot Nothing
            End Get
        End Property

        Public Sub New(Name As String, CompiledName As String)
            Me.Name = Name
            Me.CompiledName = CompiledName
        End Sub

        Public Overridable Function CompileValue() As String
            Return CompiledName
        End Function

        Public Overridable Function CompileValue(OptionValue As ExpressionNode, Writer As CWriter, Scope As Context.Scope) As String
            Throw New InternalError("Trying to compiel class enum option with a value")
        End Function

    End Class

    Public Class EnumOptionWithValue
        Inherits EnumOption
        Public ReadOnly Property Type As Type

        Public Sub New(Name As String, CompiledName As String, Type As Type)
            MyBase.New(Name, CompiledName, Type)
        End Sub

        Public Overrides Function CompileValue() As String

            If Type IsNot Nothing Then
                Throw New InternalError("Trying to compile enum option with value without a value")
            End If


        End Function

        Public Overrides Function CompileValue(OptionValue As ExpressionNode, Writer As CWriter, Scope As Context.Scope) As String

            ' Check if value type match option type
            If OptionValue.GetExpressionReturnType(Scope) <> Type Then
                Throw New TypeMismatchError(Type, OptionValue.GetExpressionReturnType(Scope), OptionValue.Location)
            End If

            ' Compile option value
            Dim CompiledValue As String = OptionValue.CompileExpression(Writer, Scope)

            ' Return object


        End Function

    End Class

End Namespace