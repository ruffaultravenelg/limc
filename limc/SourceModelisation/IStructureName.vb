Namespace Source

    Public Interface IStructureName

        '
        ' Used in ElementNode & GenericElementNode to search for a structure
        '   point(5, 4) could be a call to the "point" function, but also a instanciation of the point structure
        '

        Function ResolveStructure(Context As Context) As Lim.StructType

    End Interface

End Namespace