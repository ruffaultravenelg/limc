Imports limc.AST

Namespace TypeSystem
    Public Class ClassicEnumType
        Inherits EnumType

        Private ReadOnly EnumConstruct As AST.EnumConstruct

        Public Overrides ReadOnly Property IsPointer As Boolean = False

        ' Constructor
        Public Sub New(EnumConstruct As AST.EnumConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type))
            MyBase.New(EnumConstruct.Name, PassedGenericTypes, EnumConstruct.Exported)
            Me.EnumConstruct = EnumConstruct
            cRepresentation = CodeGen.Namer.Enum(Name)
            Options = EnumOption.FromFields(EnumConstruct.Fields, cRepresentation) ' Create options
        End Sub

        ' All recursion (like typeNode.AssociatedType) must be done in Compile()
        Public Overrides Sub Compile()

            ' Create enum
            CodeGen.RegisterEnum(New CodeGen.Enum(cRepresentation, Options.Select(Function(o) o.CompiledName), ToString()))

        End Sub


        Public Overrides ReadOnly Property cRepresentation As String
        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "(" & cRepresentation & "){" & "TODO" & "}"
        End Function

    End Class
End Namespace