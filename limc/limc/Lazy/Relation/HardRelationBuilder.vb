Namespace Lazy
    Public Class HardRelationBuilder

        Private ParentType As TypeSystem.Type
        Private Type As TypeSystem.RelationType
        Private ArgumentTypes As IEnumerable(Of TypeSystem.Type)
        Private ArgumentNames As IEnumerable(Of String)
        Private ReturnType As TypeSystem.Type
        Private Body As IEnumerable(Of String)

        Private Sub New(ParentType As TypeSystem.Type)
            Me.ParentType = ParentType
        End Sub

        Public Shared Function From(ParentType As TypeSystem.Type) As HardRelationBuilder
            Return New HardRelationBuilder(ParentType)
        End Function

        Public Function WithType(Type As TypeSystem.RelationType) As HardRelationBuilder
            Me.Type = Type
            Return Me
        End Function

        Public Function WithArgumentTypes(ArgumentTypes As IEnumerable(Of TypeSystem.Type)) As HardRelationBuilder
            Me.ArgumentTypes = ArgumentTypes
            Return Me
        End Function

        Public Function WithArgumentNames(ArgumentNames As IEnumerable(Of String)) As HardRelationBuilder
            Me.ArgumentNames = ArgumentNames
            Return Me
        End Function

        Public Function WithReturnType(ReturnType As TypeSystem.Type) As HardRelationBuilder
            Me.ReturnType = ReturnType
            Return Me
        End Function

        Public Function WithBody(Body As IEnumerable(Of String)) As HardRelationBuilder
            Me.Body = Body
            Return Me
        End Function

        Public Function Build() As HardRelation
            Return New HardRelation(ParentType, Type, ArgumentTypes, ArgumentNames, ReturnType, Body)
        End Function

    End Class
End Namespace