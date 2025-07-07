Namespace Lim

    Public Class StructType
        Inherits Lim.Type
        Implements IHasSomeMethods

        'Name of the struct
        Public Overrides ReadOnly Property Name As String
            Get
                Return Base.Name
            End Get
        End Property

        'Context
        Private GenericTypesContext As Context
        Private Context As MethodContext

        ' Methods
        Private ReadOnly Property Methods As FunctionContainer Implements IHasSomeMethods.Methods
            Get
                Return Context.Functions
            End Get
        End Property
        Private Constructors As FunctionContainer

        ' Inline properties
        Private ReadOnly Property IsInlineProperties As Boolean
            Get
                Return DirectCast(Base, Source.Struct).InlineProperties.Count > 0
            End Get
        End Property

        'Structure variables types
        Public Overrides ReadOnly Property PassedGenericTypes As IEnumerable(Of Lim.Type)

        'Field types
        Public Iterator Function FieldsTypes() As IEnumerable(Of Lim.Type)
            For Each Variable As Lim.Variable In Context.LocalVariables.Values
                Yield Variable.Type
            Next
        End Function

        'Constructor
        Public Sub New(Base As Source.Struct, PassedGenericTypes As IEnumerable(Of Lim.Type))
            MyBase.New(Base)
            Me.PassedGenericTypes = PassedGenericTypes

            'Add method context
            GenericTypesContext = New Context(Nothing)
            Context = New MethodContext(GenericTypesContext, Base.Methods, AddressOf MethodBuilder)

            'Add context types
            For i As Integer = 0 To PassedGenericTypes.Count - 1
                GenericTypesContext.GenericTypes.TryAdd(Base.GenericTypes(i).Name, PassedGenericTypes(i))
            Next

            'Add constructors
            Me.Constructors = New FunctionContainer(Base.Constructors, Context, AddressOf ConstructorBuilder)

        End Sub

        'Constructor builder
        'PassedGenericTypes will always be empty cause this is only constructors, context = Me.Context in this situation
        Private Function ConstructorBuilder(Source As Source.Function, PassedGenericTypes As IEnumerable(Of Lim.Type), Context As Context) As Lim.Function
            Return New Lim.StructConstructor(Source, Me.Context, Me)
        End Function

        'Method builder
        Private Function MethodBuilder(Source As Source.Function, PassedGenericTypes As IEnumerable(Of Lim.Type), Context As Context) As Lim.Function
            Return New Lim.StructMethod(Source, PassedGenericTypes, Me.Context, Me)
        End Function

        Public Overrides Sub Compile()

            'Struct source
            Dim StructSource As Source.Struct = DirectCast(Base, Source.Struct)

            'Compile fields
            Dim CompiledCFields As New List(Of String)
            DefineProperties(StructSource.InlineProperties, CompiledCFields)
            DefineProperties(StructSource.Properties, CompiledCFields)

            'Compile getters (TODO: lazy compile for getters & setters)
            For Each Getter As Source.Getter In StructSource.Getters
                RegisterGetter(New StructureHandwritenGetter(Getter, CompiledName, Context, ToString()))
            Next
            For Each Setter As Source.Setter In StructSource.Setters
                RegisterSetter(New StructureHandwritenSetter(Setter, CompiledName, Context, ToString()))
            Next

            'Compile struct
            C.Generator.AddStructure(New C.Structure(CompiledName, CompiledCFields))

        End Sub

        ' Take a list of source properties and compile them
        Private Sub DefineProperties(Properties As IEnumerable(Of Source.IPropertieDefinition), CompiledCFields As List(Of String))

            For Each Field As Source.IPropertieDefinition In Properties

                'Define a compiled name
                Dim FieldCompiledName As String = C.Generator.Namer.GenerateFieldName()

                'Get field type
                Dim FieldType As Lim.Type = Field.Type.GetTargetedType(GenericTypesContext)

                'Register a new getter & setter
                If Field.GET Then
                    RegisterGetter(New StructureFieldGetter(Field.Name, FieldType, FieldCompiledName))
                End If
                If Field.SET Then
                    RegisterSetter(New StructureFieldSetter(Field.Name, FieldType, FieldCompiledName))
                End If

                'Create a new variable for internal methods
                Context.RegisterVariable(Field.Name, $"self->{FieldCompiledName}", FieldType)

                'Create C structure field
                CompiledCFields.Add($"{FieldType.CompiledName} {FieldCompiledName}")

            Next

        End Sub

        Public Overrides Function DefaultValue() As String
            Dim ElementsDefaultsValues As New List(Of String)
            For Each Type As Lim.Type In FieldsTypes()
                ElementsDefaultsValues.Add(Type.DefaultValue())
            Next
            Return "(" & CompiledName & "){" & String.Join(", ", ElementsDefaultsValues) & "}"
        End Function

        Public Overrides Function Assignation(Variable As String, Value As String) As String
            Return $"{Variable} = {Value};"
        End Function

        ' Compile inisialiation -> ~ a call to new
        Public Function Constuct(PassedArguments As IEnumerable(Of ExpressionNode), Scope As Scope, Location As Location) As String
            If IsInlineProperties Then

                'Compile struct values
                Dim Args As String = Source.CallNode.CompileArguments(FieldsTypes(), PassedArguments, Scope, Location)
                If Args.StartsWith(", ") Then
                    Args = Args.Substring(2)
                End If

                'Instanciate
                Return "(" & CompiledName & "){" & Args & "}"

            Else

                'Get the constructor
                Dim Constructor As Lim.Function = Constructors.GetCorrespondance("new", {}, ExpressionNode.GetTypesOfExpressions(PassedArguments, Scope))
                If Constructor Is Nothing Then
                    Throw New SyntaxException($"No constructors has this signature in the ""{ToString()}"" structure.", Location)
                End If

                'Compile all arguments
                Dim Args As String = Source.CallNode.CompileArguments(Constructor.Arguments, PassedArguments, Scope, Location)

                'Return a call
                Return C.Function.WriteCall(Constructor.CompiledName, Args)

            End If
        End Function

    End Class

End Namespace