Namespace Lim

    '
    ' A type represent a way to store data and interact with it.
    '
    Public MustInherit Class Type

        'Main types
        Private Shared _Int As Type = Nothing
        Public Shared ReadOnly Property Int As Type
            Get
                If _Int Is Nothing Then
                    _Int = Lim.SourceFile.STD.GetAType("int", {})
                    If _Int Is Nothing Then
                        Throw New SimpleException("Incomplete library", "Int type not found in the standard library (" & IO.Path.Combine(ArgumentHandler.LibsDirectory, "std.lim") & ").")
                    End If
                End If
                Return _Int
            End Get
        End Property

        Private Shared _Float As Type = Nothing
        Public Shared ReadOnly Property Float As Type
            Get
                If _Float Is Nothing Then
                    _Float = Lim.SourceFile.STD.GetAType("int", {})
                    If _Float Is Nothing Then
                        Throw New SimpleException("Incomplete library", "Float type not found in the standard library (" & IO.Path.Combine(ArgumentHandler.LibsDirectory, "std.lim") & ").")
                    End If
                End If
                Return _Float
            End Get
        End Property

        Private Shared _Str As Type = Nothing
        Public Shared ReadOnly Property Str As Type
            Get
                If _Str Is Nothing Then
                    _Str = Lim.SourceFile.STD.GetAType("str", {})
                    If _Str Is Nothing Then
                        Throw New SimpleException("Incomplete library", "Str type not found in the standard library (" & IO.Path.Combine(ArgumentHandler.LibsDirectory, "std.lim") & ").")
                    End If
                End If
                Return _Str
            End Get
        End Property

        Private Shared _Bool As Type = Nothing
        Public Shared ReadOnly Property Bool As Type
            Get
                If _Bool Is Nothing Then
                    _Bool = Lim.SourceFile.STD.GetAType("bool", {})
                    If _Bool Is Nothing Then
                        Throw New SimpleException("Incomplete library", "Bool type not found in the standard library (" & IO.Path.Combine(ArgumentHandler.LibsDirectory, "std.lim") & ").")
                    End If
                End If
                Return _Bool
            End Get
        End Property

        'Compiled name
        Private _CompiledName As String
        Public ReadOnly Property CompiledName As String
            Get
                Return _CompiledName
            End Get
        End Property
        Protected Sub SetCompiledName(Value As String)
            _CompiledName = Value
        End Sub

        'Name
        Public MustOverride ReadOnly Property Name As String

        'Generic types
        Public MustOverride ReadOnly Property PassedGenericTypes As IEnumerable(Of Lim.Type)

        'TypeID
        Public ReadOnly Property TypeID As Integer
        Private Shared TypesIDs As Integer = 0

        'Base
        Public ReadOnly Property Base As TypeConstruct

        'Constructor
        Public Sub New(Base As TypeConstruct)

            'Set base
            Me.Base = Base

            'Create TypeID
            Lim.Type.TypesIDs += 1
            Me.TypeID = Lim.Type.TypesIDs

            'Create compiled name
            SetCompiledName(C.Generator.Namer.GenerateTypeName())

        End Sub

        'Compile type
        Public MustOverride Sub Compile()

        'Equality
        Public Shared Operator =(a As Type, b As Type) As Boolean
            If a Is Nothing Then
                If b Is Nothing Then
                    Return True
                Else
                    Return False
                End If
            ElseIf b Is Nothing Then
                Return False
            End If
            Return a.TypeID = b.TypeID
        End Operator
        Public Shared Operator <>(a As Type, b As Type) As Boolean
            Return Not a = b
        End Operator

        'To string
        Public Overrides Function ToString() As String
            If PassedGenericTypes.Count = 0 Then
                Return Name
            Else
                Return Name & "<" & String.Join(", ", PassedGenericTypes) & ">"
            End If
        End Function

        'Default value
        Public MustOverride Function DefaultValue() As String

        'Assignation
        Public MustOverride Function Assignation(Variable As String, Value As String) As String

        'Getter
        Private Getters As New Dictionary(Of String, GetterComponent)

        'Compile a call to a getter
        Public Function CallGetter(Name As String, Obj As String) As String
            Return Getters(Name).CompileCall(Obj)
        End Function

        'Do getter exist
        Public Function HasGetter(Name As String) As Boolean
            Return Getters.ContainsKey(Name)
        End Function

        'Get getter type
        Public Function GetGetterType(Name As String) As Lim.Type
            Return Getters(Name).Type
        End Function

        'Register a new getter
        Protected Sub RegisterGetter(Name As String, Getter As GetterComponent)
            Getters.Add(Name, Getter)
        End Sub

    End Class

End Namespace