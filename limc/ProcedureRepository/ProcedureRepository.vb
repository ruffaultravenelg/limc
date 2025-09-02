Public Class ProcedureRepository

    Private UncompiledProcedures As IEnumerable(Of UncompiledProcedure)
    Private CompiledProcedures As New List(Of CompiledProcedure)
    Private CompilingContext As Context

    Public Sub New(UncompiledProcedures As IEnumerable(Of UncompiledProcedure), CompilingContext As Context)
        Me.UncompiledProcedures = UncompiledProcedures
        Me.CompilingContext = CompilingContext
    End Sub

    Public Function FindProcedure(Name As String, ArgumentTypes As IEnumerable(Of TypeSystem.Type)) As CompiledProcedure

        'Search in compiled procedures
        For Each Proc As CompiledProcedure In Me.CompiledProcedures
            If Proc.Name = Name AndAlso Proc.ArgumentTypes.SequenceEqual(ArgumentTypes) Then
                Return Proc
            End If
        Next

        'Search in uncompiled procedures
        For Each UncompiledProc As UncompiledProcedure In Me.UncompiledProcedures
            If UncompiledProc.Name = Name AndAlso UncompiledProc.GetArgumentTypes(CompilingContext).SequenceEqual(ArgumentTypes) Then

                'Compile procedure
                Dim CompiledProc As CompiledProcedure = UncompiledProc.CompileProcedure(CompilingContext)

                'Register compiled procedure
                Me.CompiledProcedures.Add(CompiledProc)

                'Finish compiling
                CompiledProc.Compile()

                'Return compiled procedure
                Return CompiledProc

            End If
        Next

        'Not found
        Return Nothing

    End Function

End Class
