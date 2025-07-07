Imports limc.Lim

Namespace Source
    Public Class ChildNode
        Inherits ExpressionNode
        Implements IProcedureDirectAccess

        'Value
        Public ReadOnly Property Parent As ExpressionNode
        Public ReadOnly Property Propertie As String

        'Constructor
        Public Sub New(Location As Location, Parent As ExpressionNode, Propertie As String)
            MyBase.New(Location)
            Me.Parent = Parent
            Me.Propertie = Propertie
        End Sub

        'Get the return type
        Public Overrides Function GetReturnType(Context As Context) As Lim.Type

            'Get type
            Dim ParentType As Lim.Type = Parent.GetReturnType(Context)

            'No getter
            If ParentType.Getter(Propertie) Is Nothing Then
                Throw New SyntaxException($"The ""{ParentType}"" type has no getter named ""{Propertie}"".", Location)
            End If

            'Return getter type
            Return ParentType.Getter(Propertie).Type

        End Function

        'Compile the expression
        Public Overrides Function Compile(Scope As Scope) As String

            'Get type
            Dim ParentType As Lim.Type = Parent.GetReturnType(Scope)

            'No getter
            If ParentType.Getter(Propertie) Is Nothing Then
                Throw New SyntaxException($"The ""{ParentType}"" type has no getter named ""{Propertie}"".", Location)
            End If

            'Return getter call
            Return ParentType.Getter(Propertie).CompileCall(Scope, Parent)

        End Function

        ' If this is a procedure, get it's return type
        Private Function GetProcedureReturnedType(Context As Context, PassedArguments As IEnumerable(Of Lim.Type)) As Lim.Type Implements IProcedureDirectAccess.GetProcedureReturnedType
            Return If(GetMethod(Context, PassedArguments)?.ReturnType, Nothing)
        End Function

        Private Function CompileProcedureCall(Scope As Scope, Passedarguments As IEnumerable(Of ExpressionNode)) As String Implements IProcedureDirectAccess.CompileProcedureCall

            ' Get arguments types
            Dim ArgumentTypes As IEnumerable(Of Lim.Type) = ExpressionNode.GetTypesOfExpressions(Passedarguments, Scope)

            ' Get method
            Dim Method As Lim.Method = GetMethod(Scope, ArgumentTypes)
            If Method Is Nothing Then
                Return Nothing
            End If

            'Return compiled call
            Return Method.CompileMethodCall(Scope, Parent, Passedarguments)

        End Function

        Private Function GetMethod(Context As Context, PassedArguments As IEnumerable(Of Lim.Type)) As Lim.Method

            ' Get parent type [HERE].method
            Dim ParentType As Lim.Type = Parent.GetReturnType(Context)

            ' Check if parent type contain methods
            If TypeOf ParentType IsNot IHasSomeMethods Then
                Return Nothing
            End If

            ' Get method context
            Dim FunctionContainer As FunctionContainer = DirectCast(ParentType, IHasSomeMethods).Methods

            ' Get method
            Dim Method As Lim.Function = FunctionContainer.GetCorrespondance(Propertie, {}, PassedArguments)

            ' Method not found
            If Method Is Nothing Then
                Return Nothing
            End If

            ' Check if method is exported
            If Not Method.Base.Exported Then
                Return Nothing
            End If

            ' Check if it is a method
            If TypeOf Method IsNot Lim.Method Then
                Throw New NotImplementedException()
            End If

            'Return method  
            Return Method

        End Function

    End Class

End Namespace