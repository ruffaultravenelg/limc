Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions

Public Module CAPI

    ' C developer API
    ' Miscellaneous helper functions

    ' Do a line should be kept by it's platform directive name
    Public Function DoKeepLineFromPlatformName(PlatformName As String, Location As Location)

        Select Case PlatformName
            Case "win"
                Return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)

            Case "unix"
                Return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) OrElse RuntimeInformation.IsOSPlatform(OSPlatform.OSX) OrElse RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)

            Case Else
                Throw New SyntaxError($"The ""{PlatformName}"" platform is unknown. Uses ""win"" or ""unix"".", Location)

        End Select

    End Function

    ' Include a $include
    Public Sub HandleInclude(Header As String, Location As Location)

        ' Just a lib
        If Header.StartsWith("<") Then
            CodeGen.RegisterInclude(Header)
            Exit Sub
        End If

        ' A file -> get full path
        Dim FullFilePath As String = IO.Path.GetFullPath(Header, Location.File.Filepath)
        If Not IO.File.Exists(FullFilePath) Then
            Throw New IncludedFileDoesntExistError(Location, Header, FullFilePath)
        End If
        CodeGen.RegisterInclude("""" & FullFilePath & """")

        ' If ends by ".h", try to find ".c"
        Dim ImplementationFile As String = IO.Path.ChangeExtension(FullFilePath, "c")
        If IO.File.Exists(ImplementationFile) Then
            Compiler.CFilesToInclude.Add(ImplementationFile)
        End If

    End Sub

    ' Compile source string
    Public Function CompileSourceString(Source As String, Scope As Context.Scope) As String
        Return ReplaceConstants(ReplaceTypes(ReplaceVariable(Source, Scope), Scope))
    End Function
    Public Function CompileSourceStringWithoutContext(Source As String) As String
        Return ReplaceConstants(Source)
    End Function
    Public Function CompileSourceNoVariable(Source As String, Context As Context.Context) As String
        Return ReplaceConstants(ReplaceTypes(Source, Context))
    End Function

    ' Replace constants
    Private Function ReplaceConstants(input As String) As String
        Dim constantsType As Type = GetType(limc.Constants)
        Dim fields = constantsType.GetFields(BindingFlags.Public Or BindingFlags.Static)
        Dim pattern As String = "@(\w+)"  ' Match @CONSTANT_NAME

        Dim result As String = Regex.Replace(input, pattern, Function(m)
                                                                 Dim constName = m.Groups(1).Value
                                                                 Dim field = fields.FirstOrDefault(Function(f) f.Name = constName)
                                                                 If field IsNot Nothing Then
                                                                     Return field.GetValue(Nothing).ToString()
                                                                 Else
                                                                     Return m.Value 'Do nothing if no field found
                                                                 End If
                                                             End Function)

        Return result
    End Function

    ' Replace types
    Private Function ReplaceTypes(Source As String, Scope As Context.Context)
        Return Regex.Replace(Source, "\$:([\w<>,]+)", Function(m As Match)
                                                          Dim Expression As String = m.Groups(1).Value
                                                          Dim Tokens = Tokenizer.TokenizeExpression(Expression, Scope.ParentFile)
                                                          Dim Typenode = AST.AbstractSyntaxTree.ParseType(Tokens)
                                                          Return Typenode.GetAssociatedType(Scope).cRepresentation
                                                      End Function)
    End Function

    ' Replace variables
    Private Function ReplaceVariable(Source As String, Scope As Context.Scope) As String
        Return Regex.Replace(Source, "\$(\w+)", Function(m As Match)
                                                    Dim VariableName As String = m.Groups(1).Value
                                                    Dim Results As IEnumerable(Of SearchMatch) = Scope.RetrieveMatchingElements(VariableName, {})
                                                    For Each Result In Results
                                                        If Result.Type = SearchMatch.MatchType.MATCH_VARIABLE Then
                                                            Return Result.MatchingVariable.CompiledName
                                                        ElseIf Result.Type = SearchMatch.MatchType.MATCH_CONSTANT Then
                                                            Return Result.MatchingConstant.CompiledName
                                                        ElseIf Result.Type = SearchMatch.MatchType.MATCH_SCOPE_GETTER Then
                                                            Return Result.MatchingScopeGetter.CompileCall()
                                                        End If
                                                    Next
                                                    Throw New SyntaxError($"The variable ""{VariableName}"" could not be identified in this context. Check its name and visibility.", Scope.Location)
                                                End Function)
    End Function

    ' String to 

End Module