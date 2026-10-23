Namespace Symbol
    '
    ' Represent a procedure pointer
    '   procedure = function | method (with object contained)
    '   called "func" because it's more widly used than "procedure" or "proc" -> more of a internal term
    '
    Friend Class FuncTypeSymbol
        Inherits TypeSymbol

        Public ReadOnly Property ProcArgumentTypes As IReadOnlyList(Of TypeSymbol)
        Public ReadOnly Property ProcReturnType As TypeSymbol

        Public Sub New(ProcArgumentTypes As IEnumerable(Of TypeSymbol), ProcReturnType As TypeSymbol)
            MyBase.New("func")
            Me.ProcArgumentTypes = ProcArgumentTypes.ToList()
            Me.ProcReturnType = ProcReturnType
        End Sub

        Public Overrides Sub Compile(Lowerer As HIR.Lowerer.IHIRLowerer, ProgramBuilder As HIR.ProgramNode.Builder)
            'TODO
        End Sub

        Public Overrides Function ToString() As String
            Dim Args As String = ""
            If ProcArgumentTypes.Any Then
                Args = "<" & String.Join(", ", ProcArgumentTypes) & ">"
            End If
            Dim Ret As String = ""
            If ProcReturnType IsNot Nothing Then
                Ret = "<" & ProcReturnType.ToString() & ">"
            End If
            Return "func" & Args & Ret
        End Function

    End Class
End Namespace