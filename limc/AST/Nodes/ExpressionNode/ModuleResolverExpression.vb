Namespace AST
    Public Class ModuleResolverExpression
        Inherits ElementExpression

        Private ModuleName As String

        Public Sub New(ModuleName As String, ElementName As String, Location As Location)
            MyBase.New(ElementName, Location)
            Me.ModuleName = ModuleName
        End Sub

        Protected Overrides Function GetMatch(Context As Context.Context) As SearchMatch

            For Each UseStatement In Location.File.AST.Include_Uses
                If UseStatement.ModuleName = ModuleName Then
                    Dim Results As IEnumerable(Of SearchMatch) = UseStatement.AssociatedFile.SearchMatchingElementsAtFileLevel(ElementName, True)
                    If Results.Count = 0 Then
                        Throw New UnknownOrUnreachableElementError(ElementName, Location)
                    Else
                        Return Results(0)
                    End If
                End If
            Next

            Throw New SyntaxError($"This file does not contain a ""{ModuleName}"" module. Verify that it has been imported via ""use modulename"".", Location)

        End Function

    End Class
End Namespace