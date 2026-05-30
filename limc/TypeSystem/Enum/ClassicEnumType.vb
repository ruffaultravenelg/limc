Imports limc.AST

Namespace TypeSystem
    Public Class ClassicEnumType
        Inherits EnumType

        Private ReadOnly EnumConstruct As AST.EnumConstruct
        Private ReadOnly Options As IEnumerable(Of EnumOption)

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

        Public Class EnumOption

            Public ReadOnly CompiledName As String
            Public ReadOnly Name As String

            Private Sub New(Name As String, CompiledName As String)
                Me.CompiledName = CompiledName
                Me.Name = Name
            End Sub

            Friend Shared Function FromFields(Fields As IEnumerable(Of EnumConstruct.Field), EnumCompiledName As String) As IEnumerable(Of EnumOption)
                Dim EnumOptions As New List(Of EnumOption)
                For i = 0 To Fields.Count() - 1
                    Dim Field = Fields(i)
                    If EnumOptions.Any(Function(o) o.Name = Field.Name) Then
                        Throw New ElementAlreadyExistError(Field.Name, Field.Location)
                    End If
                    EnumOptions.Add(New EnumOption(Field.Name, $"{EnumCompiledName}_{i}"))
                Next
                Return EnumOptions
            End Function

        End Class

    End Class
End Namespace