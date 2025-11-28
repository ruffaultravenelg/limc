Imports System.Text
Imports limc.Context

Namespace TypeSystem
    Public Class RecordType
        Inherits CommonType

        Private ReadOnly Record As AST.RecordConstruct
        Private ReadOnly BoneContext As Context.GenericContext

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

                Dim FieldType As TypeSystem.Type = Field.Type.GetAssociatedType(BoneContext)
                _FieldTypes.Add(FieldType)
                Dim Tmp As String = CodeGen.Namer.Temp()
                StructFields.Add($"{FieldType.cRepresentation} {Tmp};")

            Next
            Me.FieldTypes = _FieldTypes

            ' Register struct
            CodeGen.RegisterStruct(New CodeGen.Struct(cRepresentation, StructFields, ToString()))

        End Sub

        Public Overrides ReadOnly Property cRepresentation As String

        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation) = {}

        Private FieldTypes As IEnumerable(Of Type) = Nothing
        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            If FieldTypes Is Nothing Then
                Throw New ResourceUsedTooQuicklyError(Scope.Location)
            End If
            Return "(" & cRepresentation & "){" & String.Join(", ", FieldTypes.Select(Function(t) t.DefaultValue(Scope))) & "}"
        End Function

        Public Overrides Function RetrieveElements(Name As String) As IEnumerable(Of SearchMatch)
            Return Array.Empty(Of SearchMatch)
        End Function

    End Class
End Namespace