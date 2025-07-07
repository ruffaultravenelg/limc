Namespace Lim
    Public Class HardcodedGetter
        Implements Lim.IGetter

        Public ReadOnly Property Name As String Implements IGetter.Name

        Public ReadOnly Property Type As Type Implements IGetter.Type

        'Body
        Private Body As IEnumerable(Of String)
        Private GetterFunction_CompiledName As String
        Private SelfType As Lim.Type

        'Constructor
        Public Sub New(Name As String, Type As Lim.Type, SelfType As Lim.Type, Body As IEnumerable(Of String))
            Me.Name = Name
            Me.Type = Type
            Me.Body = Body
            Me.SelfType = SelfType
            Me.GetterFunction_CompiledName = Nothing
        End Sub

        'Compile call
        Public Function CompileCall(Scope As Scope, ParentObject As ExpressionNode) As String Implements IGetter.CompileCall

            'If not already compiled
            If GetterFunction_CompiledName = Nothing Then

                'Generate name
                GetterFunction_CompiledName = C.Generator.Namer.GenerateGetterName()

                'Generate signature
                Dim GetterFunction_Signature As String = $"{Type.CompiledName} {GetterFunction_CompiledName}({SelfType.CompiledName} self)"

                'Add function to the C file
                C.Generator.AddFunction(New C.Function(GetterFunction_Signature, Body))

            End If

            'Compile
            Return GetterFunction_CompiledName & "(" & ParentObject.Compile(Scope) & ")"

        End Function

    End Class
End Namespace