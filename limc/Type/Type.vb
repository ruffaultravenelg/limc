Namespace Lim

    '
    ' A type represent a way to store data and interact with it.
    '
    Public MustInherit Class Type

        '---------------------
        '--- GENERIC TYPES ---
        '---------------------
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

        '-----------------------
        '--- TYPE PROPERTIES ---
        '-----------------------

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

        '-------------------
        '--- CONSTRUCTOR ---
        '-------------------
        Public Sub New(Base As TypeConstruct)

            'Set base
            Me.Base = Base

            'Create TypeID
            Lim.Type.TypesIDs += 1
            Me.TypeID = Lim.Type.TypesIDs

            'Create compiled name
            SetCompiledName(C.Generator.Namer.GenerateTypeName())

        End Sub

        '-------------------------
        '--- COMPILATION RULES ---
        '-------------------------
        Public MustOverride Sub Compile()

        '-------------------------
        '--- EQUALITY OPERATOR ---
        '-------------------------
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

        '-----------------
        '--- TO STRING ---
        '-----------------
        Public Overrides Function ToString() As String
            If PassedGenericTypes.Count = 0 Then
                Return Name
            Else
                Return Name & "<" & String.Join(", ", PassedGenericTypes) & ">"
            End If
        End Function

        '---------------------
        '--- DEFAULT VALUE ---
        '---------------------
        'Returns a C expression that is the default value of this type
        '   Exemple :
        '           int     -> 0
        '           user_t* -> NULL
        Public MustOverride Function DefaultValue() As String

        '----------------------------
        '--- VARIABLE ASSIGNATION ---
        '----------------------------
        ' Returns a string that is a C line that assign "Value" to the variable named "Variable"
        Public MustOverride Function Assignation(Variable As String, Value As String) As String

        '---------------
        '--- GETTERS ---
        '---------------
        Public ReadOnly Property Getters As New GetterComponent()

        '---------------
        '--- SETTERS ---
        '---------------
        Public ReadOnly Property Setters As New SetterComponent()

    End Class

End Namespace