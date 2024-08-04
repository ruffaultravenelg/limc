Public Class ClassConstructNode
    Inherits ConstructNode

    '===========================
    '======== PRIMITIVE ========
    '===========================
    Public ReadOnly Primitive As Boolean

    '============================
    '======== CLASS NAME ========
    '============================
    Public ReadOnly Name As String

    '===============================
    '======== GENERIC TYPES ========
    '===============================
    Private ReadOnly GenericTypes As List(Of GenericTypeNode)

    '=========================
    '======== CONTENT ========
    '=========================
    Public ReadOnly DeclareVariables As New List(Of DeclareVariableStatementNode)
    Public ReadOnly Getters As New List(Of GetterConstructNode)
    Public ReadOnly Setters As New List(Of SetterConstructNode)
    Public ReadOnly Methods As New List(Of MethodConstructNode)
    Public ReadOnly Constructors As New List(Of MethodConstructNode)
    Public ReadOnly DefaultState As MethodConstructNode

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Private ReadOnly ReturnType As TypeNode

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, Primitive As Boolean, Name As String, GenericTypes As List(Of GenericTypeNode), Content As List(Of Node))
        MyBase.New(Location)

        'Set properties
        Me.Primitive = Primitive
        Me.Name = Name
        Me.GenericTypes = GenericTypes

        'Set content
        For Each Node As Node In Content

            If TypeOf Node Is DeclareVariableStatementNode Then
                DeclareVariables.Add(Node)
            ElseIf TypeOf node Is GetterConstructNode Then
                Getters.Add(Node)
            ElseIf TypeOf Node Is SetterConstructNode Then
                Setters.Add(Node)
            ElseIf TypeOf Node Is MethodConstructNode Then

                'Cast method
                Dim Method As MethodConstructNode = Node

                'Constructor ?
                If Method.Name = "new" Then

                    'Check return
                    If Method.HasReturn Then
                        Throw New SyntaxErrorException("A constructor cannot return a value. However, this method contains one or more ""return"" statement(s).", Method.Location)
                    End If

                    'Add it
                    Constructors.Add(Node)
                    Continue For

                End If

                'Default ?
                If Method.Name = "default" Then

                    'Check return
                    If Method.HasReturn Then
                        Throw New SyntaxErrorException("The default function cannot return a value. However, this method contains one or more ""return"" statement(s).", Method.Location)
                    End If

                    'Add it
                    If DefaultState IsNot Nothing Then
                        Throw New SyntaxErrorException("A class can only have one ""default"" method.", Method.Location)
                    End If
                    DefaultState = Method
                    Continue For

                End If

                'Normal method
                Methods.Add(Node)

            Else
                Throw New UnanticipatedException()
            End If

        Next

    End Sub

    '======================================
    '======== CREATE GENERIC TYPES ========
    '======================================
    Public Function CreateGenericTypes(PassedGenericTypes As IEnumerable(Of Type)) As List(Of GenericType)

        Dim Result As New List(Of GenericType)
        For i As Integer = 0 To GenericTypes.Count - 1
            Result.Add(New GenericType(GenericTypes(i).Name, PassedGenericTypes(i)))
        Next
        Return Result

    End Function

    '==========================
    '======== COULD BE ========
    '==========================
    Public Function CouldBe(Name As String, GenericTypes As IEnumerable(Of Type)) As Boolean

        'Name
        If Not Name = Me.Name Then
            Return False
        End If

        'Generic types count
        If Not GenericTypes.Count = Me.GenericTypes.Count Then
            Return False
        End If

        'Generic types validates contracts
        For i As Integer = 0 To GenericTypes.Count - 1
            If Not Me.GenericTypes(i).IsComptatible(GenericTypes(i)) Then
                Return False
            End If
        Next

        'Corporate says there are the sames
        Return True

    End Function

End Class
