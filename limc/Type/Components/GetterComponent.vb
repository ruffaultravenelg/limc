Namespace Lim

    'Base
    Public MustInherit Class GetterComponent

        'Properties
        Protected ParentType As Lim.Type
        Public Overridable ReadOnly Property Type As Lim.Type

        'Compile call
        Public MustOverride Function CompileCall(Obj As String) As String

        'Constructor
        Public Sub New(ParentType As Lim.Type, Type As Type)
            Me.ParentType = ParentType
            Me.Type = Type
        End Sub

    End Class

    'Hardcoded getter
    Public Class HardCodedGetterComponent
        Inherits GetterComponent

        'Body
        Private Body As IEnumerable(Of String)
        Private CompiledName As String

        'Constructor
        Public Sub New(ParentType As Lim.Type, Type As Type, Body As IEnumerable(Of String))
            MyBase.New(ParentType, Type)
            Me.Body = Body
            Me.CompiledName = Nothing
        End Sub

        'Compile call
        Public Overrides Function CompileCall(Obj As String) As String

            'If not already compiled
            If CompiledName = Nothing Then
                CompiledName = C.Generator.Namer.GenerateGetterName()
                C.Generator.AddFunction(New C.Function(Type.CompiledName & " " & CompiledName & "(" & ParentType.CompiledName & " self)", Body))
            End If

            'Compile
            Return CompiledName & "(" & Obj & ")"

        End Function

    End Class

    Public Class StructureFieldGetterComponent
        Inherits Lim.GetterComponent

        Private CompiledName As String

        Public Sub New(ParentType As Lim.Type, GetterReturnType As Lim.Type, CompiledName As String)
            MyBase.New(ParentType, GetterReturnType)
            Me.CompiledName = CompiledName
        End Sub

        Public Overrides Function CompileCall(Obj As String) As String
            Return $"({Obj}).{CompiledName}"
        End Function
    End Class

End Namespace