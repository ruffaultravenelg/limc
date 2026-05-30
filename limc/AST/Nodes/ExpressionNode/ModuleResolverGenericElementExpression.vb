Namespace AST
    Public Class ModuleResolverGenericElementExpression
        Inherits GenericElementExpression

        Private ModuleName As String

        Public Sub New(ModuleName As String, ElementName As String, PassedGenericTypes As IEnumerable(Of TypeNode), Location As Location)
            MyBase.New(ElementName, PassedGenericTypes, Location)
            Me.ModuleName = ModuleName
        End Sub

        Protected Overrides Function GetMatch(Context As Context.Context) As SearchMatch

            For Each UseStatement In Location.File.AST.Include_Uses
                If UseStatement.ModuleName = ModuleName Then
                    Dim Results As SearchMatch
                    Try
                        Results = UseStatement.AssociatedFile.SearchMatchingElementsAtFileLevel(ElementName, PassedGenericTypes.Select(Function(g) g.GetAssociatedType(Context)), False).FirstOrDefault() 'TODO: .First() is weird
                    Catch ex As MissingLocationError
                        Throw ex.CreateError(Location)
                    End Try
                    If Results Is Nothing Then
                        Throw New UnknownOrUnreachableElementError(ElementName, Location)
                    Else
                        Return Results
                    End If
                End If
            Next

            Throw New SyntaxError($"This file does not contain a ""{ModuleName}"" module. Verify that it has been imported via ""use modulename"".", Location)

        End Function

        Protected Overrides Function TryGetEnumReference(Context As Context.Context) As TypeSystem.EnumType

            For Each UseStatement In Location.File.AST.Include_Uses
                If UseStatement.ModuleName = ModuleName Then

                    Dim GenericTypes = PassedGenericTypes.Select(Function(g) g.GetAssociatedType(Context))
                    Dim RetrievedType = UseStatement.AssociatedFile.RetrieveTypeOnlyExported(ElementName, GenericTypes)
                    If TypeOf RetrievedType Is TypeSystem.EnumType Then
                        Return RetrievedType
                    Else
                        Return Nothing
                    End If

                End If
            Next

            Return Nothing

        End Function

    End Class
End Namespace