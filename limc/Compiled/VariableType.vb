Namespace C
    Public Enum VariableType

        Primitive   '[stack]
        Struct      '[stack] mark()
        Leaf        '[heap]  free()
        Node        '[heap]  mark() free()

    End Enum

End Namespace