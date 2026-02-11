Namespace AST
    Public Class RaiseStatement
        Inherits StatementNode

        Private ReadOnly ModuleName As String
        Private ReadOnly ProblemName As String

        Public Sub New(ModuleName As String, ProblemName As String, Location As Location)
            MyBase.New(Location)
            Me.ModuleName = ModuleName
            Me.ProblemName = ProblemName
        End Sub
        Public Sub New(ProblemName As String, Location As Location)
            Me.New("", ProblemName, Location)
        End Sub

        Private Function GetProblem() As Lazy.Problem

            If ModuleName <> "" Then

                For Each UseStatement In Location.File.AST.Include_Uses
                    If UseStatement.ModuleName = ModuleName Then
                        Return UseStatement.AssociatedFile.RetrieveExportedProblem(ProblemName)
                    End If
                Next
                Throw New SyntaxError($"This file does not contain a ""{ModuleName}"" module. Verify that it has been imported via ""use modulename"".", Location)

            Else

                Return Location.File.RetrieveProblem(ProblemName)

            End If

        End Function

        Public Overrides Sub Compile(Writer As CWriter, Scope As Context.Scope)

            ' Retrieve problem
            Dim Problem = GetProblem()

            ' Write problem
            Writer.WriteLine($"if ({RUNTIME_CONTEXT_VARIABLE_NAME}.upper->accept_problem) {{")
            Writer.WriteLine(vbTab & $"{RUNTIME_CONTEXT_VARIABLE_NAME}.upper->problem = {Problem.MessageConstCompiledName};")
            Dim ReturnScope = Scope.GetParent(Of Context.MustReturnScope)()
            If ReturnScope Is Nothing Then
                Writer.WriteLine(vbTab & $"return;")
            Else
                Writer.WriteLine(vbTab & $"return {ReturnScope.ReturnType.DefaultValue(Scope)};")
            End If
            Writer.WriteLine("} else {")
            Writer.WriteLine(vbTab & CodeGen.WritePanicCall(Problem.MessageConstCompiledName) & ";")
            Writer.WriteLine("}")

        End Sub

    End Class

End Namespace