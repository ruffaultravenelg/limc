Namespace AST
    Public Class ModuleResolverExpression
        Inherits ElementExpression

        Private ModuleName As String

        Public Sub New(ModuleName As String, ElementName As String, Location As Location)
            MyBase.New(ElementName, Location)
            Me.ModuleName = ModuleName
        End Sub

        Protected Overrides Function GetMatchs(Context As Context.Context) As IEnumerable(Of SearchMatch)

            For Each UseStatement In Location.File.AST.Include_Uses
                If UseStatement.ModuleName = ModuleName Then
                    Dim Results As IEnumerable(Of SearchMatch)
                    Try
                        Results = UseStatement.AssociatedFile.SearchMatchingElementsAtFileLevel(ElementName, {}, False)
                    Catch ex As MissingLocationError
                        Throw ex.CreateError(Location)
                    End Try
                    If Results.Count = 0 Then
                        Throw New UnknownOrUnreachableElementError(ElementName, Location)
                    Else
                        Return Results
                    End If
                End If
            Next

            Throw New SyntaxError($"This file does not contain a ""{ModuleName}"" module. Verify that it has been imported via ""use modulename"".", Location)

        End Function

    End Class
End Namespace