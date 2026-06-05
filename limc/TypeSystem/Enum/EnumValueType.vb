Imports limc.AST

Namespace TypeSystem
    Public Class EnumValueType
        Inherits EnumType

        Public Overrides ReadOnly Property cRepresentation As String
        Private ReadOnly EnumConstruct As AST.EnumConstruct
        Private ReadOnly Property BoneContext As Context.Context
        Private ReadOnly Options As New List(Of ValueEnumOption)
        Public Overrides ReadOnly Property IsPointer As Boolean = False

        ' Constructor
        Public Sub New(EnumConstruct As AST.EnumConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type))
            MyBase.New(EnumConstruct.Name, PassedGenericTypes, EnumConstruct.Exported)
            Me.EnumConstruct = EnumConstruct

            ' Create a name for the enum struct (to store enum state & union values)
            cRepresentation = CodeGen.Namer.Struct(Name)

            ' Create context
            BoneContext = New Context.GenericContext(EnumConstruct.Location.File)

            ' Create options
            For i = 0 To EnumConstruct.Fields.Count - 1
                Dim Field = EnumConstruct.Fields(i)
                Dim FieldCompiledName = $"{cRepresentation}_{i}"
                If Field.HasValue Then
                    Options.Add(New ValueEnumOption(Field.Name, FieldCompiledName, cRepresentation, Field.Type))
                Else
                    Options.Add(New ValueEnumOption(Field.Name, FieldCompiledName, cRepresentation))
                End If
            Next

        End Sub

        ' All recursion (like typeNode.AssociatedType) must be done in Compile()
        Public Overrides Sub Compile()

            ' Create context
            If Not EnumConstruct.GenericTypeNames.Count = GenericTypes.Count Then
                Throw New InternalError("Enum generic argument count missmatch")
            End If
            For i As Integer = 0 To GenericTypes.Count - 1
                DirectCast(BoneContext, Context.GenericContext).RegisterGenericType(EnumConstruct.GenericTypeNames(i), GenericTypes(i))
            Next

            ' Resolve options types
            For Each Opt In Options
                Opt.ResolveOptionType(BoneContext)
            Next

            ' We need to create enum, struct(enum, union)
            Dim EnumName = CodeGen.Namer.Enum(Name)
            Dim UnionName = CodeGen.Namer.Union(Name)
            Dim StructName = cRepresentation

            ' Step 1 : enum
            CodeGen.RegisterEnum(New CodeGen.Enum(EnumName, Options.Select(Function(o) o.CompiledName), ToString()))

            ' Step 2 : union
            CodeGen.RegisterUnion(New CodeGen.Union(UnionName, Options.Where(Function(o) o.HasValue).Select(Function(o) o.ToUnionFieldDeclaration()), ToString()))

            ' Create enum struct (to store enum state & union values)
            CodeGen.RegisterStruct(New CodeGen.Struct(cRepresentation, {
                $"{EnumName} discriminator;",
                $"{UnionName} values;"
            }, ToString()))

        End Sub

        Public Overrides Function GetOptionValueByName(Name As String) As EnumOption
            Return Options.FirstOrDefault(Function(o) o.Name = Name)
        End Function

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "(" & cRepresentation & "){" & "TODO" & "}"
        End Function

        ' Implementation of the enum option
        Private Class ValueEnumOption
            Inherits EnumOption

            Public ReadOnly Name As String
            Public ReadOnly CompiledName As String
            Private ReadOnly UnionFieldCompiledName As String
            Private Typenode As AST.TypeNode
            Private EnumStructName As String
            Private Type As Type = Nothing

            Public Overrides ReadOnly Property HasValue As Boolean
                Get
                    Return Typenode IsNot Nothing OrElse Type IsNot Nothing
                End Get
            End Property

            Public Sub New(Name As String, CompiledName As String, EnumStructName As String)
                Me.Name = Name
                Me.CompiledName = CompiledName
                Me.EnumStructName = EnumStructName
            End Sub

            Public Sub New(Name As String, CompiledName As String, EnumStructName As String, Typenode As AST.TypeNode)
                Me.New(Name, CompiledName, EnumStructName)
                Me.Typenode = Typenode
                UnionFieldCompiledName = $"{CompiledName}_value"
            End Sub

            Public Sub ResolveOptionType(Context As Context.Context)
                If Typenode IsNot Nothing Then
                    Me.Type = Typenode.GetAssociatedType(Context)
                End If
            End Sub

            Public Function ToUnionFieldDeclaration() As String
                Return $"{Type.cRepresentation} {UnionFieldCompiledName};"
            End Function

            Public Overrides Function CompileValue() As String

                ' If there is a value, wrong call
                If HasValue Then
                    Throw New InternalError("Trying to compile enum option with value using no value")
                End If

                ' Just compile the struct with discriminator
                Return "(" & EnumStructName & "){ .discriminator = " & CompiledName & "}"

            End Function

            Public Overrides Function CompileValue(Writer As CWriter, Context As Context.Context, Value As AST.ExpressionNode) As String

                ' If the option doesn't need a value to be provided
                If Not HasValue Then
                    Throw New InternalError("Trying to compile a classic enum option with a value (not needed)")
                End If

                ' Check if value type match option type
                If Value.GetExpressionReturnType(Context) <> Type Then
                    Throw New TypeMismatchError(Type, Value.GetExpressionReturnType(Context), Value.Location)
                End If

                ' Compile option value
                Dim CompiledValue As String = Value.CompileExpression(Writer, Context)

                ' Return object
                Return "(" & EnumStructName & "){ .discriminator = " & CompiledName & ", .values." & UnionFieldCompiledName & " = " & CompiledValue & " }"

            End Function

        End Class

    End Class
End Namespace