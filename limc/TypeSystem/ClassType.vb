Namespace TypeSystem
    Public Class ClassType
        Inherits CommonType
        Implements ITypeWithBoneContext

        Private ReadOnly Classe As AST.ClassConstruct
        Private ReadOnly Property InnerContext As Context.Context Implements ITypeWithBoneContext.InnerContext
        Private ReadOnly Property BoneContext As Context.Context Implements ITypeWithBoneContext.BoneContext

        Private ReadOnly ConstructorRepository As Repository.ConstructorRepository
        Public Overrides ReadOnly Property IsPointer As Boolean = True

        ' Constructor
        Public Sub New(Classe As AST.ClassConstruct, PassedGenericTypes As IEnumerable(Of TypeSystem.Type))
            MyBase.New(Classe.Name, PassedGenericTypes, Classe.Exported)
            Me.Classe = Classe
            structName = CodeGen.Namer.Classe(Classe.Name)
            cRepresentation = structName & "*"
            BoneContext = New Context.GenericContext(Classe.Location.File)
            InnerContext = New Context.TypeContext(BoneContext, Me)
            Me.ConstructorRepository = New Repository.ConstructorRepository(Me, Classe.Constructors)
        End Sub

        ' All recursion (like typeNode.AssociatedType) must be done in Compile()
        Public Overrides Sub Compile()

            ' Create context
            If Not Classe.GenericTypeNames.Count = GenericTypes.Count Then
                Throw New InternalError()
            End If
            For i As Integer = 0 To GenericTypes.Count - 1
                DirectCast(BoneContext, Context.GenericContext).RegisterGenericType(Classe.GenericTypeNames(i), GenericTypes(i))
            Next

            ' Compile fields
            Dim ClassFields As New List(Of String)
            For Each Field In Classe.Fields
                Dim Prop As New Propertie(Field.Name, Field.Type.GetAssociatedType(InnerContext))
                Properties.Add(Prop)
                ClassFields.Add($"{Prop.Type.cRepresentation} {Prop.CompiledName};")

                RegisterGetter(New Lazy.DirectAccessGetter(Field.Name, Prop.Type, Function(instance) $"{instance}->{Prop.CompiledName}", Function(instance) $"(&{instance}->{Prop.CompiledName})"))
                RegisterSetter(New Lazy.DirectAccessSetter(Field.Name, Prop.Type, Function(instance, newValue) $"{instance}->{Prop.CompiledName} = {newValue};"))

                'RegisterGetter(New Lazy.HardGetter(Me, Field.Name, Prop.Type, {
                '    $"if ({INSTANCE_ARGUMENT_NAME} == NULL) {CodeGen.WritePanicCall("""Null pointer exception""")};",
                '    $"return {INSTANCE_ARGUMENT_NAME}->{Prop.CompiledName};"
                '}))

                'RegisterSetter(New Lazy.HardSetter(Me, Field.Name, Prop.Type, {
                '    $"if ({INSTANCE_ARGUMENT_NAME} == NULL) {CodeGen.WritePanicCall("""Null pointer exception""")};",
                '    $"{INSTANCE_ARGUMENT_NAME}->{Prop.CompiledName} = newValue;"
                '}))
            Next

            ' Compile source fields
            For Each SourceField In Classe.SourceFields
                ClassFields.Add(CAPI.CompileSourceNoVariable(SourceField, BoneContext))
            Next

            ' Register struct
            CodeGen.RegisterStruct(New CodeGen.Struct(structName, ClassFields, ToString()))

            ' Register methods
            For Each MethodNode In Classe.Methods
                RegisterMethod(MethodNode)
            Next

        End Sub

        ' Get constructor
        Public Function GetConstructor(Arguments As IEnumerable(Of TypeSystem.Type)) As Lazy.Constructor
            Return ConstructorRepository.RetrieveConstructor(Arguments)
        End Function

        Public Overrides ReadOnly Property cRepresentation As String
        Private structName As String
        Protected Overrides ReadOnly Property Relations As IEnumerable(Of Lazy.Relation) = {}

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "NULL"
        End Function

        Private ReadOnly Properties As New List(Of Propertie)

        Private Class Propertie
            Public ReadOnly Property Name As String
            Public ReadOnly Property Type As Type
            Public ReadOnly Property CompiledName As String

            Public Sub New(Name As String, Type As Type)
                Me.Name = Name
                Me.Type = Type
                Me.CompiledName = CodeGen.Namer.Attribute(Name)
            End Sub

        End Class

    End Class
End Namespace