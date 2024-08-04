Imports System.IO

Public Module FileBuilder

    '============================
    '======== BUILD FILE ========
    '============================
    Public Sub BuildFile(Writer As StreamWriter)

        ' Build type declarations
        WriteHeaderComment(Writer, "types declarations")
        For Each Struct As BuildableStructure In Structures
            Writer.WriteLine(Struct.BuildTypeForward())
        Next

        ' Space between
        Writer.WriteLine()

        ' Build structure definition
        WriteHeaderComment(Writer, "structure definition")
        For Each Struct As BuildableStructure In Structures

            ' Write structure definition
            For Each Line As String In Struct.BuildStructureDefinition()
                Writer.WriteLine(Line)
            Next

            ' Spacing
            Writer.WriteLine()

        Next

        ' Space between
        Writer.WriteLine()

        ' Build functions prototypes
        WriteHeaderComment(Writer, "functions prototypes")
        For Each Fn As BuildableFunction In Functions
            Writer.WriteLine(Fn.BuildPrototypeWithoutSemiColon() & ";")
        Next

        ' Space between
        Writer.WriteLine()

        ' Build functions
        WriteHeaderComment(Writer, "functions")
        For Each Fn As BuildableFunction In Functions

            'Write function signature
            Writer.WriteLine(Fn.BuildPrototypeWithoutSemiColon() & "{")

            'Write function body
            For Each Line As String In Fn.BuildLogic()
                Writer.Write(vbTab)
                Writer.WriteLine(Line)
            Next

            'Write function end
            Writer.WriteLine("}")
            Writer.WriteLine()

        Next

    End Sub

    '======================================
    '======== WRITE HEADER COMMENT ========
    '======================================
    Private Sub WriteHeaderComment(Writer As StreamWriter, Title As String)

        ' Create strings
        Dim Inner As String = "//// " & Title.ToUpper() & " ////"
        Dim Outer As String = StrDup(Inner.Length, "/")

        ' Write strings
        Writer.WriteLine(Outer)
        Writer.WriteLine(Inner)
        Writer.WriteLine(Outer)

    End Sub

    '============================
    '======== STRUCTURES ========
    '============================
    Private Structures As New List(Of BuildableStructure)
    Public Sub NotifyNewStructure(Elm As BuildableStructure)
        Structures.Add(Elm)
    End Sub

    '===========================
    '======== FUNCTIONS ========
    '===========================
    Private Functions As New List(Of BuildableFunction)
    Public Sub NotifyNewFunction(Elm As BuildableFunction)
        Functions.Add(Elm)
    End Sub

End Module
