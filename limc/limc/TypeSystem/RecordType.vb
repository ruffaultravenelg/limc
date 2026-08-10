Imports limc.AST

Namespace TypeSystem
    Public Class RecordType
        Inherits CommonType

        Private ReadOnly Record As AST.RecordConstruct
        Public ReadOnly BoneContext As Context.GenericContext
        Public Overrides ReadOnly Property IsPointer As Boolean = False

        ' Constructor
        Public Sub New(Record As AST.RecordConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type))
            MyBase.New(Record.Name, PassedGenericTypes, Record.Exported)
            Me.Record = Record
            cRepresentation = CodeGen.Namer.Struct(Record.Name)
            BoneContext = New Context.GenericContext(Record.Location.File)
        End Sub

        ' All recursion (like typeNode.AssociatedType) must be done in Compile()
        Public Overrides Sub Compile()

            ' Create context
            If Not Record.GenericTypeNames.Count = GenericTypes.Count Then
                Throw New InternalError()
            End If
            For i As Integer = 0 To GenericTypes.Count - 1
                BoneContext.RegisterGenericType(Record.GenericTypeNames(i), GenericTypes(i))
            Next

            ' Compile fields
            Dim StructFields As New List(Of String)
            Dim _FieldTypes As New List(Of TypeSystem.Type)
            For Each Field In Record.Fields

                Dim FieldType As TypeSystem.Type
                If Field.Type IsNot Nothing AndAlso Field.DefaultValue Is Nothing Then
                    FieldType = Field.Type.GetAssociatedType(BoneContext)
                ElseIf Field.Type Is Nothing AndAlso Field.DefaultValue IsNot Nothing Then
                    FieldType = Field.DefaultValue.GetExpressionReturnType(BoneContext)
                Else
                    FieldType = Field.Type.GetAssociatedType(BoneContext)
                    If FieldType <> Field.DefaultValue.GetExpressionReturnType(BoneContext) Then
                        Throw New TypeMismatchError(FieldType, Field.DefaultValue.GetExpressionReturnType(BoneContext), Field.DefaultValue.Location)
                    End If
                End If

                _FieldTypes.Add(FieldType)
                Dim CompiledAttributeName As String = CodeGen.Namer.Attribute()
                StructFields.Add($"{FieldType.cRepresentation} {CompiledAttributeName};")
                RegisterGetter(New Lazy.DirectAccessGetter(Field.Name, FieldType, Function(instance) $"{instance}.{CompiledAttributeName}"))
                'Record don't have setters, that intended btw

            Next
            Me.FieldTypes = _FieldTypes

            ' Register struct
            CodeGen.RegisterStruct(New CodeGen.Struct(cRepresentation, StructFields, ToString()))

        End Sub

        Public Overrides ReadOnly Property cRepresentation As String

        Private FieldTypes As IEnumerable(Of Type) = Nothing
        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            If FieldTypes Is Nothing Then
                Throw New ResourceUsedTooQuicklyError(Scope.Location)
            End If
            Return "(" & cRepresentation & "){" & String.Join(", ", FieldTypes.Select(Function(t) t.DefaultValue(Scope))) & "}"
        End Function

        Public Function GetConstructionFields(CallLocation As Location) As IEnumerable(Of ConstructionField)
            If FieldTypes Is Nothing Then
                Throw New ResourceUsedTooQuicklyError(CallLocation)
            End If
            Dim Fields As New List(Of ConstructionField)
            For i As Integer = 0 To Record.Fields.Count - 1
                Fields.Add(New ConstructionField(Record.Fields(i).Name, FieldTypes(i), Record.Fields(i).DefaultValue))
            Next
            Return Fields
        End Function

        Public Class ConstructionField

            Public ReadOnly Property Name As String
            Public ReadOnly Property Type As TypeSystem.Type
            Public ReadOnly Property DefaultValue As ExpressionNode

            Public Sub New(Name As String, Type As TypeSystem.Type, DefaultValue As ExpressionNode)
                Me.Name = Name
                Me.Type = Type
                Me.DefaultValue = DefaultValue
            End Sub

        End Class

    End Class
End Namespace