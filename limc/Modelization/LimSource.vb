Imports System.IO
Public Class LimSource

    '====================================
    '======== EVERY PARSED FILES ========
    '====================================
    Private Shared ReadOnly EveryParsedFiles As New List(Of LimSource) From {}

    '============================
    '======== MAIN FILES ========
    '============================
    Public Shared ReadOnly STD As LimSource = New LimSource(Path.Combine(Platform.LibDirectory, "std.lim"))

    '===========================
    '======== FILE PATH ========
    '===========================
    Public ReadOnly Property Filepath As String

    '===============================
    '======== RELATIVE PATH ========
    '===============================
    Public ReadOnly Property RelativePath As String
        Get
            Return Path.GetRelativePath(".", Me.Filepath)
        End Get
    End Property

    '=======================
    '======== SCOPE ========
    '=======================
    Public ReadOnly Property Scope As Scope = New Scope()

    '=========================
    '======== CONTENT ========
    '=========================
    Private ReadOnly Exceptions As New List(Of ExceptionConstructNode)
    Private ReadOnly Functions As New List(Of FunctionConstructNode)
    Private ReadOnly Classs As New List(Of ClassConstructNode)

    '=========================
    '======== IMPORTS ========
    '=========================
    Public ReadOnly ImportedFiles As New List(Of LimSource)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Filepath As String)

        'Add itself to the list of compiled file
        EveryParsedFiles.Add(Me)

        'Get full path
        Filepath = Path.GetFullPath(Filepath)
        Me.Filepath = Filepath

        'File doesn't exist
        If Not File.Exists(Filepath) Then
            Throw New FileDoNotExistException("File """ & Path.GetRelativePath(".", Filepath) & """ doesn't exist.")
        End If

        'Get lines
        Dim Lines As List(Of LimSourceLine) = LineParser.Parse(Filepath)

        'Convert to tokens
        Dim Tokens As List(Of Token) = TokenParser.Parse(Me, Lines)

        'Parse nodes
        NodeParser.Parse(Me, Tokens, Exceptions, Functions, Classs)

        'Export all constructs if no one is exported
        Dim ExportedConstructFound As Boolean = False
        For Each elm As ConstructNode In Constructs
            If elm.Exported Then
                ExportedConstructFound = True
                Exit For
            End If
        Next
        If Not ExportedConstructFound Then
            For Each elm As ConstructNode In Constructs
                elm.Exported = True
            Next
        End If

        'Import std.lim if the current file is not std.lim
        If Not (HasTheSamePathAs(Path.Combine(Platform.LibDirectory, "std.lim"))) Then
            ImportedFiles.Add(LimSource.STD)
        End If

        'Compilation target
        If Program.CompilationTarget = CompilationType.Libs Then
            Throw New NotImplementedException
            End
        ElseIf Program.CompilationTarget = CompilationType.Json Then
            Throw New NotImplementedException
            End
        End If

    End Sub

    '===============================
    '======== IMPORT A FILE ========
    '===============================
    Private Sub Import(Filename As String)

        'Get full path
        Filename = Path.GetFullPath(Filename)

        'Search if file is already imported in imported files
        For Each File As LimSource In ImportedFiles
            If File.HasTheSamePathAs(Filename) Then
                Return
            End If
        Next

        'Search if the file is already compiled
        For Each File As LimSource In LimSource.EveryParsedFiles
            If File.HasTheSamePathAs(Filename) Then
                ImportedFiles.Add(File)
                Return
            End If
        Next

        'Compile the file
        ImportedFiles.Add(New LimSource(Filename))

    End Sub

    '=============================
    '======== IS THE SAME ========
    '=============================
    Private Function HasTheSamePathAs(Other As String)
        Return Filepath = Other
    End Function

    '===================================
    '======== FILE DO NOT EXIST ========
    '===================================
    Public Class FileDoNotExistException
        Inherits CompilerException

        Public Sub New(Message As String)
            MyBase.New("File doesn't exist", Message)
        End Sub

    End Class

    '====================================
    '======== CONSTRUCT ITERATOR ========
    '====================================
    Private ReadOnly Iterator Property Constructs As IEnumerable(Of ConstructNode)
        Get
            For Each elm In Exceptions
                Yield elm
            Next
            For Each elm In Functions
                Yield elm
            Next
            For Each elm In Classs
                Yield elm
            Next
        End Get
    End Property

    '==============================
    '======== GET FUNCTION ========
    '==============================
    Public ReadOnly Property [Function](Name As String, GenericTypes As IEnumerable(Of Type)) As CFunction
        Get

            'Search best function in current file
            Try
                Return Procedure.SearchBestProcedure(Of CFunction)(Me.CompiledFunctions, Me.Functions, Name, GenericTypes, Function(X, Y) New CFunction(X, Y))
            Catch ex As UnableToFindProcedure
            End Try

            'Search best function in all files
            For Each ImportedFile As LimSource In Me.ImportedFiles
                Try
                    Return Procedure.SearchBestProcedure(Of CFunction)(ImportedFile.CompiledFunctions, ImportedFile.Functions, Name, GenericTypes, Function(X, Y) New CFunction(X, Y))
                Catch ex As UnableToFindProcedure
                End Try
            Next

            'Error
            Throw New UnableToFindProcedure()

        End Get
    End Property
    Public ReadOnly Property [Function](Name As String, GenericTypes As IEnumerable(Of Type), ArgumentsTypes As IEnumerable(Of Type)) As CFunction
        Get

            'Search best function in current file
            Try
                Return Procedure.SearchBestProcedure(Of CFunction)(Me.CompiledFunctions, Me.Functions, Name, GenericTypes, ArgumentsTypes, Function(X, Y) New CFunction(X, Y))
            Catch ex As UnableToFindProcedure
            End Try

            'Search best function in all files
            For Each ImportedFile As LimSource In Me.ImportedFiles
                Try
                    Return Procedure.SearchBestProcedure(Of CFunction)(ImportedFile.CompiledFunctions, ImportedFile.Functions, Name, GenericTypes, ArgumentsTypes, Function(X, Y) New CFunction(X, Y))
                Catch ex As UnableToFindProcedure
                End Try
            Next

            'Error
            Throw New UnableToFindProcedure()

        End Get
    End Property
    Public ReadOnly Property [Function](Name As String, GenericTypes As IEnumerable(Of Type), Signature As FunctionSignatureType) As CFunction
        Get

            'Search best function in current file
            Try
                Return Procedure.SearchBestProcedure(Of CFunction)(Me.CompiledFunctions, Me.Functions, Name, GenericTypes, Signature, Function(X, Y) New CFunction(X, Y))
            Catch ex As UnableToFindProcedure
            End Try

            'Search best function in all files
            For Each ImportedFile As LimSource In Me.ImportedFiles
                Try
                    Return Procedure.SearchBestProcedure(Of CFunction)(ImportedFile.CompiledFunctions, ImportedFile.Functions, Name, GenericTypes, Signature, Function(X, Y) New CFunction(X, Y))
                Catch ex As UnableToFindProcedure
                End Try
            Next

            'Error
            Throw New UnableToFindProcedure()

        End Get
    End Property
    Public Function HasFunction(Name As String, GenericTypes As IEnumerable(Of Type), Signature As FunctionSignatureType) As Boolean

        'Search best function in current file
        Try
            Procedure.SearchBestProcedure(Of CFunction)(Me.CompiledFunctions, Me.Functions, Name, GenericTypes, Signature, Function(X, Y) New CFunction(X, Y))
            Return True
        Catch ex As UnableToFindProcedure
        End Try

        'Search best function in all files
        For Each ImportedFile As LimSource In Me.ImportedFiles
            Try
                Procedure.SearchBestProcedure(Of CFunction)(ImportedFile.CompiledFunctions, ImportedFile.Functions, Name, GenericTypes, Signature, Function(X, Y) New CFunction(X, Y))
                Return True
            Catch ex As UnableToFindProcedure
            End Try
        Next

        'Error
        Return False

    End Function

    ' Already compiled functions
    Private CompiledFunctions As New List(Of CFunction)

    ' Notice a new compiled type
    Public Sub NoticeNewCompiledFunction(Fun As CFunction)
        CompiledFunctions.Add(Fun)
    End Sub

    '==========================
    '======== GET TYPE ========
    '==========================
    Private Function LocalType(Name As String, GenericTypes As IEnumerable(Of Type)) As Type

        ' Search compiled types in cu   rrent file
        For Each CT As Type In CompiledTypes
            If CT.LooksLike(Name, GenericTypes) Then
                Return CT
            End If
        Next

        ' Search in current file
        For Each CL As ClassConstructNode In Me.Classs
            If CL.CouldBe(Name, GenericTypes) Then
                If CL.Primitive Then
                    Return New PrimitiveClassType(CL, GenericTypes)
                Else
                    Throw New NotImplementedException
                    'Return New ClassType(Me)
                End If
            End If
        Next

        'Not found
        Return Nothing

    End Function
    Public ReadOnly Property Type(Name As String, GenericTypes As IEnumerable(Of Type)) As Type
        Get

            ' Search locally
            Dim Result As Type = LocalType(Name, GenericTypes)
            If Result IsNot Nothing Then
                Return Result
            End If

            ' Search in imported files
            For Each ImportedFile As LimSource In Me.ImportedFiles
                Result = ImportedFile.LocalType(Name, GenericTypes)
                If Result IsNot Nothing Then
                    Return Result
                End If
            Next

            ' Cannot find function
            Throw New CannotFindTypeException(Name, GenericTypes, Me)

        End Get
    End Property

    ' Already compiled functions
    Private CompiledTypes As New List(Of Type)

    ' Notice a new compiled type
    Public Sub NoticeNewCompiledType(Type As Type)
        CompiledTypes.Add(Type)
    End Sub

    'Types
    Public ReadOnly Iterator Property Types As IEnumerable(Of Type)
        Get
            For Each Type As Type In CompiledTypes
                Yield Type
            Next
        End Get
    End Property

    ' Type not found exception
    Public Class CannotFindTypeException
        Inherits CompilerException
        Public Sub New(TypeName As String, GenericArguments As IEnumerable(Of Type), File As LimSource)
            MyBase.New("Cannot find type", "Cannot find """ & TypeName & If(GenericArguments.Count > 0, limc.Type.StringifyListOfType(GenericArguments), "") & """ type in """ & File.Filepath & """.")
        End Sub
    End Class

End Class
