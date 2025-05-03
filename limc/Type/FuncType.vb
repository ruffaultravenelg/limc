Namespace Lim

    Public Class FuncType
        Inherits Lim.Type

        'Name
        Public Overrides ReadOnly Property Name As String = "fun"

        'Passed generic types
        Public Overrides ReadOnly Property PassedGenericTypes As IEnumerable(Of Lim.Type) = {}

        'The return type of the fun type (fun<exmpl, exmpl><[HERE]>)
        Public ReadOnly Property ReturnType As Lim.Type

        'Constructor
        Private Sub New(GenericTypes As IEnumerable(Of Lim.Type), ReturnType As Lim.Type)
            MyBase.New(Nothing) 'Se if this thing make crash lol
            Me.PassedGenericTypes = GenericTypes
            Me.ReturnType = ReturnType
        End Sub

        'Compile
        Public Overrides Sub Compile()

            'Set arguments
            Dim Arguments As String = C.Generator.CONTEXT_STRUCTURENAME & "*"
            For Each Arg As Lim.Type In PassedGenericTypes
                Arguments &= ", " & Arg.CompiledName
            Next

            'Generate structure
            C.Generator.AddStructure(New C.Structure(CompiledName, {
                If(ReturnType Is Nothing, "void", ReturnType.CompiledName) & " (*fn)(" & Arguments & ")"
            }))

            'Generate default function
            C.Generator.AddFunction(New C.Function(CompiledName & "_default", {CompiledName & " unused"}, If(ReturnType Is Nothing, "void", ReturnType.CompiledName), {
                "",
                $"lim_panic(&{C.Generator.CONTEXT_NAME}, ""Call on a null ""{ToString()}"" );"
            }))

        End Sub

        'Default value
        Public Overrides Function DefaultValue() As String
            Return Wrap(CompiledName & "_default")
        End Function

        'Compile a static function into a FuncType value
        Public Function Wrap(Fn As String) As String
            Return "((" & CompiledName & "){.fn = " & Fn & "})"
        End Function

        'Assignation
        Public Overrides Function Assignation(Variable As String, Value As String) As String
            Return $"{Variable} = {Value};"
        End Function

        'To string
        Public Overrides Function ToString() As String
            If ReturnType Is Nothing Then
                Return MyBase.ToString()
            Else
                Return MyBase.ToString() & "<" & ReturnType.ToString() & ">"
            End If
        End Function

        'All existing func types
        Private Shared ExistingTypes As New HashSet(Of FuncType)

        'From
        Public Shared Function From(Arguments As IEnumerable(Of Lim.Type), ReturnType As Lim.Type) As FuncType

            'If already exist
            For Each i As FuncType In ExistingTypes
                If SameType(i, Arguments, ReturnType) Then
                    Return i
                End If
            Next

            'Do not exist -> new one
            Dim Type As FuncType = New FuncType(Arguments, ReturnType)
            ExistingTypes.Add(Type)
            Type.Compile()
            Return Type

        End Function

        'Compare functype
        Private Shared Function SameType(Target As FuncType, Arguments As IEnumerable(Of Lim.Type), ReturnType As Lim.Type) As Boolean

            'Compare argument count
            If Not Target.PassedGenericTypes.Count = Arguments.Count Then
                Return False
            End If

            'Compare arguments
            For i As Integer = 0 To Arguments.Count - 1
                If Not Target.PassedGenericTypes(i) = Arguments(i) Then
                    Return False
                End If
            Next

            'Compare return type
            If Not Target.ReturnType = ReturnType Then
                Return False
            End If

            'Every test pass
            Return True

        End Function

    End Class


End Namespace