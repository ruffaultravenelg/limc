Namespace TypeSystem
    Public Interface ITypeWithBoneContext
        ReadOnly Property InnerContext As Context.Context
        ReadOnly Property BoneContext As Context.Context
    End Interface

End Namespace