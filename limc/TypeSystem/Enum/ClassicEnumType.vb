Imports limc.AST

Namespace TypeSystem
    Public Class ClassicEnumType
        Inherits EnumType

        Public Overrides ReadOnly Property cRepresentation As String
        Private ReadOnly EnumConstruct As AST.EnumConstruct
        Private ReadOnly Options As New List(Of ClassicEnumOption)
        Public Overrides ReadOnly Property IsPointer As Boolean = False

        ' Constructor
        Public Sub New(EnumConstruct As AST.EnumConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type))
            MyBase.New(EnumConstruct.Name, PassedGenericTypes, EnumConstruct.Exported)
            Me.EnumConstruct = EnumConstruct

            ' Create a name for the enum
            cRepresentation = CodeGen.Namer.Enum(Name)

            ' Create options
            For i = 0 To EnumConstruct.Fields.Count - 1
                Dim EnumOptionCompiledName = $"{cRepresentation}_{i}"
                Options.Add(New ClassicEnumOption(EnumConstruct.Fields(i).Name, EnumOptionCompiledName))
            Next

        End Sub

        ' All recursion (like typeNode.AssociatedType) must be done in Compile()
        Public Overrides Sub Compile()

            ' Register the new enum
            CodeGen.RegisterEnum(New CodeGen.Enum(
                cRepresentation,                            ' enum name
                Options.Select(Function(o) o.CompiledName), ' enum options
                ToString()                                  ' comment (enum type as string)
            ))

            ' a = b
            RegisterRelation(New Lazy.HardRelation(Me, RelationType.RELATION_EQUAL, {Me}, {"b"}, Type.Bool, {
                $"return {INSTANCE_ARGUMENT_NAME} == b;"
            }))

        End Sub

        Public Overrides Function GetOptionByName(Name As String) As EnumOption
            Return Options.FirstOrDefault(Function(o) o.Name = Name)
        End Function

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "(" & cRepresentation & "){" & "TODO" & "}"
        End Function

        ' Implementation of the enum options
        Private Class ClassicEnumOption
            Inherits EnumOption

            Public ReadOnly CompiledName As String
            Public ReadOnly Name As String

            Public Overrides ReadOnly Property HasValue As Boolean = False

            Public Sub New(Name As String, CompiledName As String)
                Me.Name = Name
                Me.CompiledName = CompiledName
            End Sub

            Public Overrides Function CompileValue() As String
                Return CompiledName
            End Function

            Public Overrides Function CompileValue(Writer As CWriter, Context As Context.Context, Value As ExpressionNode) As String
                Throw New InternalError("Trying to compile classic enum option using a value")
            End Function

        End Class

    End Class
End Namespace