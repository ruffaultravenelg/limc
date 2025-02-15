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
        Private Context As MethodContext


        ' Methods
        Private ReadOnly Property Methods As FunctionContainer Implements IHasSomeMethods.Methods
            Get
                Return Context.Functions
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

            'Set all "functions" to "method"
            For Each Func As Source.Function In Base.Methods
                Func.DefineAsMethod(Me)
            Next

            'Add method context
            Context = New MethodContext(Nothing, Base.Methods)

            'Add context types
            For i As Integer = 0 To PassedGenericTypes.Count - 1
                Context.GenericTypes.TryAdd(Base.GenericTypes(i).Name, PassedGenericTypes(i))
            Next

        End Sub

        Public Overrides Sub Compile()

            'Compile fields
            Dim FieldsCompiledNames As New List(Of String)
            For Each Field As Source.KeyNameType In DirectCast(Base, Source.Struct).Properties

                'Define a compiled name
                Dim FieldCompiledName As String = C.Generator.Namer.GenerateFieldName()

                'Get field type
                Dim FieldType As Lim.Type = Field.Type.GetTargetedType(Context)

                'Register a new getter
                Me.Getters.RegisterGetter(Field.Name, New Lim.StructureFieldGetterInvoker(FieldType, FieldCompiledName))

                'Create a new variable for internal methods
                Context.RegisterVariable(Field.Name, $"self.{FieldCompiledName}", FieldType)

                'Create C structure field
                FieldsCompiledNames.Add($"{FieldType.CompiledName} {FieldCompiledName}")

            Next

            'Compile struct
            C.Generator.AddStructure(New C.Structure(CompiledName, FieldsCompiledNames))

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

    End Class

End Namespace