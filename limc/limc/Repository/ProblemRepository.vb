Namespace Repository
    Public Class ProblemRepository

        ' Contains all problems
        Private ReadOnly Problems As New Dictionary(Of String, Lazy.Problem)
        Private ReadOnly ProblemConstructs As IEnumerable(Of AST.ProblemConstruct)

        ' Constructor
        Public Sub New(ProblemConstructs As IEnumerable(Of AST.ProblemConstruct))
            Me.ProblemConstructs = ProblemConstructs
        End Sub

        ' General repository endpoint
        Public Function RetrieveProblem(Name As String) As Lazy.Problem

            ' Search if it already exists
            If Problems.ContainsKey(Name) Then
                Return Problems(Name)
            End If

            ' Search in constructs
            Dim Construct = ProblemConstructs.FirstOrDefault(Function(PC) PC.Name = Name)
            If Construct IsNot Nothing Then
                Dim Problem = New Lazy.Problem(Construct)
                Problems(Name) = Problem
                Return Problem
            End If

            ' Not found
            Return Nothing

        End Function

    End Class
End Namespace