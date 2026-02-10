Imports System.IO
Imports limc.AST
Imports limc.Repository
Imports limc.TypeSystem

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
            SourceInstance.AST = AbstractSyntaxTree.LoadFile(Tokens)
            SourceInstance.AST.Include_Imports.Add(New AST.ImportLibNode(New Location(SourceInstance, 0, 0, 0), "std")) 'Import std lib to every source file

            'Create function repository
            SourceInstance.FunctionRepository = New FunctionRepository(SourceInstance.AST.Functions, SourceInstance)

            'Create type repository
            SourceInstance.TypeRepository = New CommonTypeRepository(SourceInstance.AST.TypeConstructs)

            'Compile constants
            For Each ConstantsConstruct In SourceInstance.AST.Constants
                ConstantsConstruct.Compile(SourceInstance.ConstantStore)
            Next

            'Return object instance
            Return SourceInstance

        End Function

        '=====================
        '===== FUNCTIONS =====
        '=====================
        Public Property FunctionRepository As FunctionRepository

        '=====================
        '===== CONSTANTS =====
        '=====================
        Private ConstantStore As New Dictionary(Of String, DeclareConstantWithValueConstruct)

        '========================
        '===== CUSTOM TYPES =====
        '========================
        Private TypeRepository As CommonTypeRepository

        Public Function RetrieveTypeFromLocal(TypeName As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As TypeSystem.Type

            ' Search in current file
            Dim LocalType As TypeSystem.Type = TypeRepository.RetrieveType(TypeName, GenericTypes, False)
            If LocalType IsNot Nothing Then
                Return LocalType
            End If

            ' Search in imported files
            For Each Include In AST.Include_Imports
                Dim Result As TypeSystem.Type = Include.AssociatedFile.RetrieveTypeOnlyExported(TypeName, GenericTypes)
                If Result IsNot Nothing Then
                    Return Result
                End If
            Next

            ' Nothing found
            Return Nothing

        End Function
        Public Function RetrieveTypeOnlyExported(TypeName As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As TypeSystem.Type
            Return TypeRepository.RetrieveType(TypeName, GenericTypes, True)
        End Function

        '==========================
        '===== SEARCH ELEMENT =====
        '==========================

        ' With generics
        Protected Overrides Function GetLocalMatchingElements(Name As String, GenericTypes As IEnumerable(Of Type)) As IEnumerable(Of SearchMatch)
            Return SearchMatchingElementsAtFileLevel(Name, GenericTypes, True)
        End Function
        Public Function SearchMatchingElementsAtFileLevel(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type), FromInside As Boolean) As IEnumerable(Of SearchMatch)
            Dim Matchs As New List(Of SearchMatch)

            Matchs.AddRange(FunctionRepository.RetrieveFunctions(Name, GenericTypes).Select(Function(fn) New SearchMatch(fn)))

            If ConstantStore.ContainsKey(Name) AndAlso (FromInside OrElse ConstantStore(Name).Exported) Then
                Matchs.Add(New SearchMatch(ConstantStore(Name).Data))
            End If

            If FromInside Then
                For Each Include In AST.Include_Imports
                    Matchs.AddRange(Include.AssociatedFile.SearchMatchingElementsAtFileLevel(Name, GenericTypes, False))
                Next
            End If

            Return Matchs
        End Function

    End Class

End Namespace