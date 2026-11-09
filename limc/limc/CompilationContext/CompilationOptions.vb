Friend Class CompilationOptions

    Public Property Primitives As IPrimitives

    Public Property Namer As C.Naming.ISymbolNamer
    Public Property CCEngine As C.IArgumentGenerator

    Private Sub New()
        ' empty constructor so no one could construct this class without default values => Shared DefaultOptions()
    End Sub

    ' ==== CONSTRUCTORS ====

    Public Shared Function DefaultOptions() As CompilationOptions
        Return New CompilationOptions() With {
            .Namer = New C.Naming.ManglerNamer(),
            .Primitives = ClassicLimPrimitives.Generate(.Namer),
            .CCEngine = New C.ClangArgumentGenerator()
        }
    End Function

End Class
