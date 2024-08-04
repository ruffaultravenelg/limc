
'
' Represent a class's variable
'
Public Class Propertie

    '===============================
    '======== COMPILED NAME ========
    '===============================
    Public ReadOnly Property CompiledName As String

    '======================
    '======== NAME ========
    '======================
    Public ReadOnly Property Name As String

    '======================
    '======== TYPE ========
    '======================
    Public ReadOnly Property Type As Type

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(ParentType As Type, Name As String, Type As Type, CompiledName As String)

        'Set compiled name
        Me.CompiledName = CompiledName

        'Set name
        Me.Name = Name

        'Set type
        Me.Type = Type

        'Acessor
        Dim AcessorOperator As String = If(TypeOf ParentType Is HeapType, "->", ".")

        'Create getter
        Dim GetterName As String = $"{ParentType.Name}__{CompiledName}__get"
        Getter = GetterName & "(&self)"
        CSourceFunction.GenerateSourceFunction($"{Type.CompiledName} {GetterName}({ParentType.CompiledName}* self)", {
            $"return (*self){AcessorOperator}{CompiledName};"
        })

        'Create setter
        SetterName = $"{ParentType.Name}__{CompiledName}__set"
        CSourceFunction.GenerateSourceFunction($"void {SetterName}({ParentType.CompiledName}* self, {Type.CompiledName} newValue)", {
            $"(*self){AcessorOperator}{CompiledName} = newValue;"
        })

    End Sub

    '========================
    '======== GETTER ========
    '========================
    Public ReadOnly Property Getter As String

    '========================
    '======== SETTER ========
    '========================
    Private SetterName As String
    Public ReadOnly Property Setter(NewValue As String) As String
        Get
            Return SetterName & "(&self, " & NewValue & ")"
        End Get
    End Property

End Class
