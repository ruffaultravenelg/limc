Namespace Lazy
    Public Class HardRelation
        Inherits Relation

        Public Overrides ReadOnly Property Type As TypeSystem.RelationType
        Public Overrides ReadOnly Property ArgumentsTypes As IEnumerable(Of TypeSystem.Type)
        Public Overrides ReadOnly Property ReturnType As TypeSystem.Type


        Private _GeneratedFunction As CodeGen.ContextedFunction = Nothing
        Public Overrides ReadOnly Property GeneratedFunction As CodeGen.ContextedFunction
            Get
                If _GeneratedFunction Is Nothing Then
                    If ArgumentsTypes.Count <> ArgumentNames.Count Then
                        Throw New InternalError()
                    End If

                    Dim Args As New List(Of String) From {$"{ParentType.cRepresentation} {Constants.INSTANCE_ARGUMENT_NAME}"}
                    For i As Integer = 0 To ArgumentsTypes.Count - 1
                        Args.Add($"{ArgumentsTypes(i).cRepresentation} {ArgumentNames(i)}")
                    Next

                    _GeneratedFunction = New CodeGen.ContextedFunction(Args, ReturnType.cRepresentation, Body, $"{ParentType.ToString()} -> {Type.ToString()}({String.Join(", ", ArgumentsTypes.Select(Function(arg) arg.ToString()))}):{ReturnType.ToString()}")
                    CodeGen.RegisterFunction(_GeneratedFunction)
                End If
                Return _GeneratedFunction
            End Get
        End Property

        Private Body As IEnumerable(Of String)
        Private ArgumentNames As IEnumerable(Of String)

        Public Sub New(ParentType As TypeSystem.Type, Type As TypeSystem.RelationType, ArgumentTypes As IEnumerable(Of TypeSystem.Type), ArgumentNames As IEnumerable(Of String), ReturnType As TypeSystem.Type, Body As IEnumerable(Of String))
            MyBase.New(ParentType)
            Me.Type = Type
            Me.ArgumentsTypes = ArgumentTypes
            Me.ArgumentNames = ArgumentNames
            Me.ReturnType = ReturnType
            Me.Body = Body
        End Sub

    End Class
End Namespace