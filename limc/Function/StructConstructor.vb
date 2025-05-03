
Namespace Lim
    Public Class StructConstructor

        Inherits Lim.Function

        Private ParentStruct As Lim.StructType

        Public Sub New(Base As Source.Function, StructContext As Context, ParentStruct As StructType)
            MyBase.New(Base, {}, StructContext)
            Me.ParentStruct = ParentStruct

            If Base.ReturnType IsNot Nothing Then
                Throw New SyntaxException("A constructor cannot return a value.", Base.ReturnType.Location)
            ElseIf Base.ContainsReturnStatement Then
                Throw New SyntaxException("A constructor cannot return a value.", Base.Location)
            End If

        End Sub

        Protected Overrides Sub BeforeBody(Scope As Scope)
            Scope.WriteLine($"{ParentStruct.CompiledName} self;")
        End Sub

        Protected Overrides Sub AfterBody(Scope As Scope)
            Scope.WriteReturn("self")
        End Sub

        Public Overrides ReadOnly Property ReturnType As Type
            Get
                Return ParentStruct
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return ParentStruct.ToString & "." & MyBase.ToString()
        End Function

    End Class
End Namespace