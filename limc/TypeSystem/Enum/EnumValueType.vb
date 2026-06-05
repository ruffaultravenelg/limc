Namespace TypeSystem
    Public Class EnumValueType
        Inherits EnumType

        Private ReadOnly EnumConstruct As AST.EnumConstruct
        Private ReadOnly Property BoneContext As Context.Context

        Public Overrides ReadOnly Property IsPointer As Boolean = False

        ' Constructor
        Public Sub New(EnumConstruct As AST.EnumConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type))
            MyBase.New(EnumConstruct.Name, PassedGenericTypes, EnumConstruct.Exported)
            Me.EnumConstruct = EnumConstruct
            cRepresentation = CodeGen.Namer.Struct(Name)
            BoneContext = New Context.GenericContext(EnumConstruct.Location.File)
            Options = EnumOption.FromFields(EnumConstruct.Fields, cRepresentation) ' Create options
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
            CodeGen.RegisterUnion(New CodeGen.Union(UnionName, Options.Where(Function(o) o.HasValue).Select(Function(o) $"{o.Type.cRepresentation} {o.UnionValueCompiledName}"), ToString()))

            ' Create enum struct (to store enum state & union values)
            CodeGen.RegisterStruct(New CodeGen.Struct(cRepresentation, {
                $"{EnumName} discriminator,",
                $"{UnionName} values"
            }, ToString()))

        End Sub

        Public Overrides ReadOnly Property cRepresentation As String
        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "(" & cRepresentation & "){" & "TODO" & "}"
        End Function

    End Class
End Namespace