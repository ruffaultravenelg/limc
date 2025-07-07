Imports System.Text.RegularExpressions

Namespace C
    Module Utils

        Public Function IsLeftHandedReference(chaine As String) As Boolean
            Dim pattern As String = "^\w+(?:(?:\.|->)\w+)*$"
            Return Regex.IsMatch(chaine, pattern)
        End Function

        Public Function ResolvePointerOfStaticObject(Scope As Scope, StaticObject As String, StaticObjectCompiledTypeName As String) As String
            If (IsLeftHandedReference(StaticObject)) Then
                'If left-handed -> direct access -> just use &variable
                Return "&(" & StaticObject & ")"
            Else
                'Not left-handed -> access from a function returns value or something -> store it into a tmp variable and use &tmp
                Dim Tmp As String = Scope.GetTempVariable()
                Scope.WriteLine($"{StaticObjectCompiledTypeName} {Tmp} = {StaticObject};")
                Return "&" & Tmp
            End If
        End Function

    End Module
End Namespace