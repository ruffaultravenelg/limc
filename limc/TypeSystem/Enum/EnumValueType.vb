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
            cRepresentation = CodeGen.Namer.Enum(Name)
            BoneContext = New Context.GenericContext(EnumConstruct.Location.File)
        End Sub

        ' All recursion (like typeNode.AssociatedType) must be done in Compile()
        Public Overrides Sub Compile()

            ' Create context
            If Not EnumConstruct.GenericTypeNames.Count = GenericTypes.Count Then
                Throw New InternalError()
            End If
            For i As Integer = 0 To GenericTypes.Count - 1
                DirectCast(BoneContext, Context.GenericContext).RegisterGenericType(EnumConstruct.GenericTypeNames(i), GenericTypes(i))
            Next



        End Sub

        Public Overrides ReadOnly Property cRepresentation As String
        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "(" & cRepresentation & "){" & "TODO" & "}"
        End Function

    End Class
End Namespace