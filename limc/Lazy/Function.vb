Imports System.Text
Imports limc.AST

Namespace Lazy
    Public Class [Function]
        Implements CompiledProcedure

        Private FunctionNode As FunctionConstruct
        Private FunctionContext As ReturnableScope

        Public ReadOnly Property Name As String Implements CompiledProcedure.Name
            Get
                Return FunctionNode.Name
            End Get
        End Property
        Public ReadOnly Property ArgumentTypes As IEnumerable(Of TypeSystem.Type) = New List(Of TypeSystem.Type) Implements CompiledProcedure.ArgumentTypes
        Public ReadOnly Property ReturnType As TypeSystem.Type
            Get
                If FunctionContext.ReturnType IsNot Nothing Then
                    Return FunctionContext.ReturnType
                End If
                If IsCompiling Then
                    Throw New ProcedureDontReturnValueError(FunctionNode.Location, Name)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly CompiledFunctionName As String
        Private IsCompiling As Boolean = True

        Public ReadOnly Property AssociatedFunType As TypeSystem.FunType
            Get
                Return TypeSystem.FunType.From(ArgumentTypes, ReturnType)
            End Get
        End Property

        Public Sub New(FunctionNode As FunctionConstruct, FileContext As Context)

            ' Initialize properties
            Me.FunctionNode = FunctionNode
            Me.FunctionContext = New ReturnableScope(FileContext, FunctionNode.Location)

            ' Create a compiled name
            Me.CompiledFunctionName = CodeGen.Namer.Function(Name)

            ' Get compiled argument type (we can do this here because arguments are already compiled from ProcedureRepository.FindProcedure method)
            Me.ArgumentTypes = FunctionNode.GetArgumentTypes(FileContext)

        End Sub

        Public Sub Compile() Implements CompiledProcedure.Compile

            ' Handle return type
            If FunctionNode.ReturnType IsNot Nothing Then
                FunctionContext.ReturnType = FunctionNode.ReturnType.GetAssociatedType(FunctionContext)
            End If

            ' Compile body
            For Each Statement In FunctionNode.Body
                Statement.Compile(FunctionContext)
            Next
            IsCompiling = False

            ' Register function
            CodeGen.RegisterFunction(New CodeGen.Function(GetSignature(), FunctionContext.GetLines()))

        End Sub

        Private Function GetSignature() As String
            Dim Signature As New StringBuilder()

            'Return type
            If ReturnType Is Nothing Then
                Signature.Append("void")
            Else
                Signature.Append(ReturnType.cRepresentation)
            End If
            Signature.Append(" ")

            ' Function name
            Signature.Append(CompiledFunctionName)

            'Arguments
            Signature.Append("(")
            For i As Integer = 0 To ArgumentTypes.Count - 1
                If i > 0 Then
                    Signature.Append(", ")
                End If
                Signature.Append(ArgumentTypes(i).cRepresentation)
                Signature.Append(CodeGen.Namer.Variable(FunctionNode.Arguments(i).ArgumentName))
            Next
            Signature.Append(")")

            Return Signature.ToString()
        End Function

    End Class

End Namespace