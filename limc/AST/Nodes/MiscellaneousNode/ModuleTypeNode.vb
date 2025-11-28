Imports System.Data

Namespace AST
    Public Class ModuleTypeNode
        Inherits AST.TypeNode

        Private ReadOnly ModuleName As String
        Private ReadOnly TypeName As String
        Private ReadOnly TypeGenericTypes As IEnumerable(Of AST.TypeNode)

        Public Sub New(ModuleName As String, TypeName As String, TypeGenericTypes As IEnumerable(Of AST.TypeNode), Location As Location)
            MyBase.New(Location)
            Me.ModuleName = ModuleName
            Me.TypeName = TypeName
            Me.TypeGenericTypes = TypeGenericTypes
        End Sub

        Public Overrides Function GetAssociatedType(Context As Context.Context) As TypeSystem.Type

            'Search for type
            For Each Use In Location.File.AST.Include_Uses
                If Use.ModuleName = ModuleName Then

                    Dim CommonType As TypeSystem.Type = Use.AssociatedFile.RetrieveTypeOnlyExported(TypeName, TypeGenericTypes.Select(Function(t) t.GetAssociatedType(Context)))
                    If CommonType IsNot Nothing Then
                        Return CommonType
                    End If

                    Throw New UnknownOrUnreachableElementError(TypeName, Location)

                End If
            Next

            ' Not found
            Throw New SyntaxError($"This file does not contain a ""{ModuleName}"" module. Verify that it has been imported via ""use modulename"".", Location)

        End Function

    End Class

End Namespace