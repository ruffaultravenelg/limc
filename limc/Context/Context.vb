Namespace Context
    Public Class Context

        Public ReadOnly Property Parent As Context

        Public Sub New(Parent As Context)
            Me.Parent = Parent
        End Sub

        ' Parent iterators
        Public ReadOnly Iterator Property AllParents As IEnumerable(Of Context)
            Get
                Dim Current As Context = Me
                While Current IsNot Nothing
                    Yield Current
                    Current = Current.Parent
                End While
            End Get
        End Property
        Public Iterator Function GetParents(Of T As Context)() As IEnumerable(Of T)
            Dim Current As Context = Me
            While Current IsNot Nothing
                If TypeOf Current Is T Then
                    Yield Current
                End If
                Current = Current.Parent
            End While
        End Function

        ' A Parent getter
        Public Function HasParent(Of T As Context)() As Boolean
            For Each Ctx As Context In AllParents
                If TypeOf Ctx Is T Then
                    Return True
                End If
            Next
            Return False
        End Function
        Public Function GetParent(Of T As Context)() As T
            For Each Ctx As Context In AllParents
                If TypeOf Ctx Is T Then
                    Return Ctx
                End If
            Next
            Return Nothing
        End Function

        'Get all name matching element from lower to upper context
        Public Function RetrieveMatchingElements(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As IEnumerable(Of SearchMatch)
            Dim Result As New List(Of SearchMatch)
            For Each Ctx As Context In AllParents
                Result.AddRange(Ctx.GetLocalMatchingElements(Name, GenericTypes))
            Next
            Return Result
        End Function
        Protected Overridable Function GetLocalMatchingElements(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As IEnumerable(Of SearchMatch)
            Return {}
        End Function

    End Class

End Namespace