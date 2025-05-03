Namespace Lim

    Public Class SourceFile

        'STD file
        Private Shared _STD As Lim.SourceFile = Nothing
        Public Shared ReadOnly Property STD As Lim.SourceFile
            Get
                If _STD Is Nothing Then
                    Dim STDPath As String = IO.Path.Combine(ArgumentHandler.LibsDirectory, "std.lim")
                    If Not IO.File.Exists(STDPath) Then
                        Throw New SimpleException("std.lim missing", "The standard library file is missing from """ & STDPath & """.")
                    End If
                    _STD = Lim.SourceFile.Load(STDPath)
                End If
                Return _STD
            End Get
        End Property

        'Store all sources files
        Private Shared ReadOnly ProjectFiles As New List(Of Lim.SourceFile)

        'File path
        Public ReadOnly Property FullFilePath As String

        'File name
        Public ReadOnly Property Filename As String
            Get
                Return IO.Path.GetFileName(FullFilePath)
            End Get
        End Property

        'Relative path
        Public ReadOnly Property RelativePath As String
            Get
                Return IO.Path.GetRelativePath(IO.Directory.GetCurrentDirectory(), FullFilePath)
            End Get
        End Property

        'Imports
        Public ReadOnly Property ImportedFiles As New HashSet(Of Lim.SourceFile)
        Public ReadOnly Property NammedImportedFiles As New Dictionary(Of String, Lim.SourceFile)

        'Functions
        Private ReadOnly Functions As FunctionContainer

        'Types
        Private ReadOnly Types As TypeContainer

        'Constructor
        Private Sub New(Filename As String)

            'Set path
            Me.FullFilePath = IO.Path.GetFullPath(Filename)

            'Add to project files
            ProjectFiles.Add(Me)

            'Import the standard library
            ImportedFiles.Add(STD)

            'Parse text
            Dim Lines As IEnumerable(Of SourceLine) = SourceLine.Load(Me)

            'Parse tokens
            Dim Content As FileAST = AST.GenerateAST(Lines)

            'Get imports
            HandleImports(Content.GetConstructs(Of Source.Import))

            'Get functions
            Me.Functions = New FunctionContainer(Content.GetConstructs(Of Source.Function), New Context())

            'Get types
            Me.Types = New TypeContainer(Content.GetConstructs(Of TypeConstruct))

        End Sub

        'Handle imports
        Private Sub HandleImports(ImportsStatements As IEnumerable(Of Source.Import))
            For Each Statement As Source.Import In ImportsStatements

                'Get the full filepath
                Dim Filepath As String
                If Statement.Library Then
                    Filepath = Compiler.GetLibraryPath(Statement.Filename)
                    If Filepath Is Nothing Then
                        Throw New ImportException($"Unable to locate the '{Statement.Filename}' module, try installing it with `lim install {Statement.Filename}`.", Statement.Location)
                    End If
                Else
                    Filepath = IO.Path.GetFullPath(Statement.Filename, IO.Path.GetDirectoryName(FullFilePath))
                    If Not IO.File.Exists(Filepath) Then
                        Throw New ImportException($"Cannot locate ""{Statement.Filename}"" from ""{Filename}""", Statement.Location)
                    End If
                End If

                'Load the file
                Dim File As Lim.SourceFile = Lim.SourceFile.Load(Filepath)

                'Add it to imports
                Select Case Statement.Naming

                    Case ""
                        ImportedFiles.Add(File)

                    Case "."
                        Dim Name As String = IO.Path.GetFileNameWithoutExtension(Filepath)
                        If Not NammedImportedFiles.TryAdd(Name, File) Then
                            Throw New SyntaxException("A import nammed """ & Name & """ is already existing, please change import name", Statement.Location)
                        End If

                    Case Else
                        If Not NammedImportedFiles.TryAdd(Statement.Naming, File) Then
                            Throw New SyntaxException("A import nammed """ & Statement.Naming & """ is already existing, please change import name", Statement.Location)
                        End If

                End Select

            Next
        End Sub

        'Load source file
        Public Shared Function Load(Filename As String) As SourceFile

            'Check if the filepath already exist
            Dim Fullpath As String = IO.Path.GetFullPath(Filename)

            'Search if the file was already parsed
            For Each File As Lim.SourceFile In ProjectFiles
                If File.FullFilePath = Fullpath Then
                    Return File
                End If
            Next

            'No files found -> parse this new one
            Return New SourceFile(Filename)

        End Function

        'Search for a function in the file or a exported one in the imports
        Public Function GetFunctions(Name As String, GenericTypes As IEnumerable(Of Lim.Type)) As IEnumerable(Of Lim.Function)

            'Search in the current file
            Dim Correspondances As List(Of Lim.Function) = Functions.GetCorrespondances(Name, GenericTypes)

            'Search in the imports
            For Each File As Lim.SourceFile In ImportedFiles
                For Each Correspondance As Lim.Function In File.Functions.GetCorrespondances(Name, GenericTypes)
                    If Correspondance.Base.Exported Then
                        Correspondances.Add(Correspondance)
                    End If
                Next
            Next

            'Return correspondances
            Return Correspondances

        End Function
        Public Function GetFunctions(ImportName As String, Name As String, GenericTypes As IEnumerable(Of Lim.Type)) As IEnumerable(Of Lim.Function)

            'Search if the import exist
            If Not NammedImportedFiles.ContainsKey(ImportName) Then
                Return Nothing
            End If

            'Check for exported results
            Dim Result As New List(Of Lim.Function)
            For Each Func As Lim.Function In NammedImportedFiles(ImportName).Functions.GetCorrespondances(Name, GenericTypes)
                If Func.Base.Exported Then
                    Result.Add(Func)
                End If
            Next

            'Return the result
            Return Result

        End Function

        Public Function GetFunction(Name As String, GenericTypes As IEnumerable(Of Lim.Type), Arguments As IEnumerable(Of Lim.Type)) As Lim.Function

            'Search in the current file
            Dim Correspondance As Lim.Function = Functions.GetCorrespondance(Name, GenericTypes, Arguments)
            If Correspondance IsNot Nothing Then
                Return Correspondance
            End If

            'Search in the imports
            For Each File As Lim.SourceFile In ImportedFiles
                Correspondance = File.Functions.GetCorrespondance(Name, GenericTypes, Arguments)
                If Correspondance IsNot Nothing AndAlso Correspondance.Base.Exported Then
                    Return Correspondance
                End If
            Next

            'Not found
            Return Nothing

        End Function
        Public Function GetFunction(ImportName As String, Name As String, GenericTypes As IEnumerable(Of Lim.Type), Arguments As IEnumerable(Of Lim.Type)) As Lim.Function

            'Search if the import exist
            If Not NammedImportedFiles.ContainsKey(ImportName) Then
                Return Nothing
            End If

            'Search function in the imported file
            Dim Func As Lim.Function = NammedImportedFiles(ImportName).Functions.GetCorrespondance(Name, GenericTypes, Arguments)
            If Func IsNot Nothing AndAlso Func.Base.Exported Then
                Return Func
            End If

            'Not found
            Return Nothing

        End Function

        'Search for a type in the file or a exported one in the imports
        Public Function GetAType(TypeName As String, PassedGenericTypes As IEnumerable(Of Lim.Type)) As Lim.Type

            'Search in the current file
            Dim Result As Lim.Type = Types.GetCorrespondance(TypeName, PassedGenericTypes)
            If Result IsNot Nothing Then
                Return Result
            End If

            'Search in the imports
            For Each File As Lim.SourceFile In ImportedFiles
                Result = File.Types.GetCorrespondance(TypeName, PassedGenericTypes)
                If Result IsNot Nothing AndAlso Result.Base.Exported Then
                    Return Result
                End If
            Next

            'Not found
            Return Nothing

        End Function
        Public Function GetAType(ImportName As String, TypeName As String, PassedGenericTypes As IEnumerable(Of Lim.Type)) As Lim.Type

            'Search if the import exist
            If Not NammedImportedFiles.ContainsKey(ImportName) Then
                Return Nothing
            End If

            'Search in the import
            Dim Result As Lim.Type = NammedImportedFiles(ImportName).Types.GetCorrespondance(TypeName, PassedGenericTypes)

            'If result if null or not exported, return null
            If Result Is Nothing OrElse Not Result.Base.Exported Then
                Return Nothing
            End If

            'Return the result
            Return Result

        End Function

    End Class

End Namespace