Imports limc.Lim

Namespace Source

    '
    ' A Method -> parent.method<T, D>
    '
    Public Class GenericChildNode
        Inherits ExpressionNode
        Implements IProcedureDirectAccess

        'Value
        Private Parent As ExpressionNode
        Private Propertie As String
        Private PassedGenericTypes As IEnumerable(Of Source.Type)

        'Constructor
        Public Sub New(Location As Location, Parent As ExpressionNode, Propertie As String, PassedGenericTypes As IEnumerable(Of Source.Type))
            MyBase.New(Location)
            Me.Parent = Parent
            Me.Propertie = Propertie
            Me.PassedGenericTypes = PassedGenericTypes
        End Sub

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type
            Throw New SyntaxException("A procedure cannot be used as an expression.", Location)
        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String
            Throw New SyntaxException("A procedure cannot be used as an expression.", Location)
        End Function


        ' If this is a procedure, get it's return type
        Private Function GetProcedureReturnedType(Context As Context, PassedArguments As IEnumerable(Of Lim.Type)) As Lim.Type Implements IProcedureDirectAccess.GetProcedureReturnedType
            Return If(GetMethod(Context, PassedArguments)?.ReturnType, Nothing)
        End Function

        Private Function CompileProcedureCall(Scope As Scope, Passedarguments As IEnumerable(Of ExpressionNode)) As String Implements IProcedureDirectAccess.CompileProcedureCall

            ' Compiple types
            Dim ArgumentTypes As IEnumerable(Of Lim.Type) = Passedarguments.Select(Function(Expr As ExpressionNode) Expr.GetReturnType(Scope))

            ' Get method
            Dim Method As Lim.Function = GetMethod(Scope, ArgumentTypes)
            If Method Is Nothing Then
                Return Nothing
            End If

            ' Compile
            Dim Args As String = Source.CallNode.CompileArguments(Method.Arguments, Passedarguments, Scope, Location)
            Return C.Function.WriteCall(Method.CompiledName, Parent.Compile(Scope), Args)

        End Function

        Private Function GetMethod(Context As Context, PassedArguments As IEnumerable(Of Lim.Type)) As Lim.Function

            ' Get parent type [HERE].method
            Dim ParentType As Lim.Type = Parent.GetReturnType(Context)

            ' Check if parent type contain methods
            If TypeOf ParentType IsNot IHasSomeMethods Then
                Return Nothing
            End If

            ' Get method context
            Dim FunctionContainer As FunctionContainer = DirectCast(ParentType, IHasSomeMethods).Methods

            ' Get method
            Dim GenericTypes As IEnumerable(Of Lim.Type) = CompilePassedGenericTypes(Context)
            Dim Method As Lim.Function = FunctionContainer.GetCorrespondance(Propertie, GenericTypes, PassedArguments)

            'If method is not found
            If Method Is Nothing Then
                Return Nothing
            End If

            ' Check if method is exported
            If Not Method.Base.Exported Then
                Return Nothing
            End If

            'Nothing  
            Return Method

        End Function

        'Compile passed generic types
        Private Function CompilePassedGenericTypes(Context As Context) As IEnumerable(Of Lim.Type)
            Return Me.PassedGenericTypes.Select(Function(TypeNode As Source.Type) TypeNode.GetTargetedType(Context))
        End Function

    End Class

End Namespace