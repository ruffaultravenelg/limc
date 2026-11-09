Namespace Environment
    Friend MustInherit Class TypeEnvironment
        Inherits Env
        Public Sub New(Parent As Env)
            MyBase.New(Parent)
        End Sub

        Public Overridable Function GetRelation()
            Throw New NotImplementedException() 'TODO
        End Function

    End Class
End Namespace