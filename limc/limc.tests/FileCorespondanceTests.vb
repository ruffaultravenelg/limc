Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace limc.tests
    <TestClass>
    Public Class FileCorespondanceTests

        Private Const TESTCASE_DIR_PATH As String = "TestCases"

        Private Structure FileCase
            Public ReadOnly Property InFile As String
            Public ReadOnly Property OutFile As String
            Public Sub New(Infile As String)
                Me.InFile = Infile
                Me.OutFile = Path.ChangeExtension(Infile, "out")
            End Sub

        End Structure

        Private Shared Iterator Function GetDirectoryFilePair(Dirname As String) As IEnumerable(Of FileCase)
            Dim folderPath As String = Path.Combine(TESTCASE_DIR_PATH, Dirname)

            If Not Directory.Exists(folderPath) Then
                Assert.Inconclusive($"Unreachable directory : {folderPath}")
                Exit Function
            End If

            For Each File In Directory.GetFiles(folderPath, "*.lim")
                Yield New FileCase(File)
            Next
        End Function

        <TestMethod>
        Sub LexerCases()

            Dim Failures As New List(Of String)

            For Each TestCase In GetDirectoryFilePair("Lexer")
                Try
                    Using sr As New StreamReader(TestCase.InFile)
                        Dim Tokens = Lexer.Tokenizer.Parse(sr, TestCase.InFile)

                        ' To string
                        Using sw As New OutputStream.StringOutputStream
                            Printer.TokenPrinter.PrintTokens(sw, Tokens)

                            Dim actual As String = sw.GetResult()
                            Dim expected As String = File.ReadAllText(TestCase.OutFile)

                            ' Comparaison
                            Assert.AreEqual(expected, actual, $"[LEXER] FAIL ""{Path.GetFileName(TestCase.InFile)}""")
                        End Using
                    End Using
                Catch ex As Exception
                    Failures.Add($"[LEXER] EXCEPTION ""{Path.GetFileName(TestCase.InFile)}"" : {ex.Message}")
                End Try
            Next

            ' Fail if there is at least one failure
            If Failures.Count > 0 Then
                Assert.Fail("Lexer test failed :" & System.Environment.NewLine & String.Join(System.Environment.NewLine, Failures))
            End If

        End Sub

        <TestMethod>
        Sub ParserCases()

            Dim Failures As New List(Of String)

            For Each TestCase In GetDirectoryFilePair("Parser")
                Try
                    Using sr As New StreamReader(TestCase.InFile)
                        Dim Tokens = Lexer.Tokenizer.Parse(sr, TestCase.InFile)
                        Dim AbstractSyntaxTree = AST.FileParser.Parse(Tokens, TestCase.InFile)

                        ' To string
                        Using sw As New OutputStream.StringOutputStream
                            Printer.ASTPrinter.PrintAST(sw, AbstractSyntaxTree)

                            Dim actual As String = sw.GetResult()
                            Dim expected As String = File.ReadAllText(TestCase.OutFile)

                            ' Comparaison
                            Assert.AreEqual(expected, actual, $"[PARSER] FAIL ""{Path.GetFileName(TestCase.InFile)}""")
                        End Using
                    End Using
                Catch ex As Exception
                    Failures.Add($"[PARSER] EXCEPTION ""{Path.GetFileName(TestCase.InFile)}"" : {ex.Message}")
                End Try
            Next

            ' Fail if there is at least one failure
            If failures.Count > 0 Then
                Assert.Fail("Parser test failed :" & System.Environment.NewLine & String.Join(System.Environment.NewLine, Failures))
            End If

        End Sub

    End Class

End Namespace

