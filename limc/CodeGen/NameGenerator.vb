Namespace CodeGen
    Public Class NameGenerator

        '=====================
        '===== FUNCTIONS =====
        '=====================
        Public Overridable Function Variable(Optional Info As String = "") As String
            Return GenerateName() & "_v"
        End Function
        Public Overridable Function Attribute(Optional Info As String = "") As String
            Return GenerateName() & "_a"
        End Function
        Public Overridable Function Temp() As String
            Return GenerateName() & "_tmp"
        End Function
        Public Overridable Function [Function](Optional Info As String = "") As String
            Return GenerateName() & "_f"
        End Function
        Public Overridable Function [Method](Optional Info As String = "") As String
            Return GenerateName() & "_m"
        End Function
        Public Overridable Function [Struct](Optional Info As String = "") As String
            Return GenerateName() & "_t"
        End Function
        Public Overridable Function Constant(Optional Info As String = "") As String
            Return GenerateName() & "_c"
        End Function
        Public Function Rack(Info As String) As String
            Return GenerateName() & "_r" ' Keep ending _r for RackType.ValidateStructFieldDefinition
        End Function

        '============================
        '===== SIMPLE GENERATOR =====
        '============================

        Private ElementCounter As Integer = 0
        Private Function GenerateName() As String
            ElementCounter += 1
            Const Chars As String = "abcdefghijklmnopqrstuvwxyz"
            Dim Result As String = ""

            Dim n As Integer = ElementCounter
            While n > 0
                n -= 1
                Dim index As Integer = n Mod 26
                Result = Chars(index) & Result
                n \= 26
            End While

            Return Result
        End Function

    End Class

End Namespace