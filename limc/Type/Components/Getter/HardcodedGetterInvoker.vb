Namespace Lim

    'Hardcoded getter
    Public Class HardcodedGetterInvoker
        Inherits GetterInvoker

        'Body
        Private Body As IEnumerable(Of String)
        Private GetterFunction_CompiledName As String
        Private ObjectType As Lim.Type

        'Constructor
        Public Sub New(ObjectType As Lim.Type, ReturnedType As Type, Body As IEnumerable(Of String))
            MyBase.New(ReturnedType)
            Me.Body = Body
            Me.ObjectType = ObjectType
            Me.GetterFunction_CompiledName = Nothing
        End Sub

        'Compile call
        Public Overrides Function CompileCall(Obj As String) As String

            'If not already compiled
            If GetterFunction_CompiledName = Nothing Then

                'Generate name
                GetterFunction_CompiledName = C.Generator.Namer.GenerateGetterName()

                'Generate signature
                Dim GetterFunction_Signature As String = $"{Type.CompiledName} {GetterFunction_CompiledName}({ObjectType.CompiledName} self)"

                'Add function to the C file
                C.Generator.AddFunction(New C.Function(GetterFunction_Signature, Body))

            End If

            'Compile
            Return GetterFunction_CompiledName & "(" & Obj & ")"

        End Function

    End Class

End Namespace