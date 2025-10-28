Imports System.IO
Imports limc.AST
Imports limc.Repository

Namespace Context
    Public Class SourceFile
        Inherits Context

        '======================================
        '===== ALL REGISTERED SOURCE FILE =====
        '======================================
        Private Shared SolutionFiles As New List(Of SourceFile)
        Private Shared MainFile As SourceFile = Nothing

        '======================
        '===== PROPERTIES =====
        '======================
        Public ReadOnly Property Filepath As String
        Public Property AST As AST.AbstractSyntaxTree

        Public ReadOnly Property Filename As String
            Get
                Return Path.GetFileName(Filepath)
            End Get
        End Property
        Public ReadOnly Property RelativePath As String
            Get
                Return Path.GetRelativePath(Path.GetDirectoryName(MainFile.Filepath), Filepath)
            End Get
        End Property

        '=======================
        '===== CONSTRUCTOR =====
        '=======================
        Private Sub New(Filepath As String)
            MyBase.New(Nothing) 'Root of the context tree
            Me.Filepath = Filepath
            If MainFile Is Nothing Then
                MainFile = Me
            End If
        End Sub

        Public Shared Function FromFile(Filepath As String) As SourceFile

            'Sanatize path
            Filepath = Path.GetFullPath(Filepath)

            'Search if file is already registered, return it
            For Each SolutionFile As SourceFile In SourceFile.SolutionFiles
                If SolutionFile.Filepath = Filepath Then
                    Return SolutionFile
                End If
            Next

            'Create and register instance
            Dim SourceInstance As New SourceFile(Filepath)
            SolutionFiles.Add(SourceInstance)

            'Read file
            Dim Tokens As IEnumerable(Of Token) = Tokenizer.TokenizeFile(SourceInstance)

            'Generate AST
            SourceInstance.AST = New AbstractSyntaxTree(Tokens)

            'Create function repository
            SourceInstance.FunctionRepository = New FunctionRepository(SourceInstance.AST.Functions, SourceInstance)

            'Return object instance
            Return SourceInstance

        End Function

        '=====================
        '===== FUNCTIONS =====
        '=====================
        Public Property FunctionRepository As FunctionRepository

        '==========================
        '===== SEARCH ELEMENT =====
        '==========================
        Protected Overrides Function GetLocalMatchingElement(Name As String) As IEnumerable(Of SearchMatch)
            Return SearchMatchingElements(Name, False)
        End Function
        Private Function SearchMatchingElements(Name As String, FromOutside As Boolean) As IEnumerable(Of SearchMatch)
            Dim Matchs As New List(Of SearchMatch)

            Dim Func As Lazy.Function = FunctionRepository.RetrieveFunction(Name)
            If Func IsNot Nothing AndAlso (Not FromOutside OrElse Func.Exported) Then
                Matchs.Add(New SearchMatch(Func))
            End If

            If Not FromOutside Then
                For Each Include In AST.Include_Imports
                    Matchs.AddRange(Include.AssociatedFile.SearchMatchingElements(Name, True))
                Next
            End If

            Return Matchs
        End Function

    End Class

End Namespace