Public MustInherit Class ExpressionNode
    Inherits Node

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Protected Sub New(Location As Location)
        MyBase.New(Location)
    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Public ReadOnly Property ReturnType(Scope As Scope) As Type
        Get
            Return CalculateReturnType(Scope)
        End Get
    End Property
    Public ReadOnly Property CanReturn(Request As Type, Scope As Scope) As Boolean
        Get

            'If node can convert
            If CanReturnType(Request, Scope) Then
                Return True
            End If

            'Check if the return type can be converted
            Dim ReturnedType As Type = CalculateReturnType(Scope)
            Return ReturnedType.CanConvertTo(Request)

        End Get
    End Property

    'Calculate return type
    Protected MustOverride Function CalculateReturnType(Scope As Scope) As Type

    'Can return type ?
    Protected Overridable Function CanReturnType(Request As Type, Scope As Scope) As Boolean
        Return False
    End Function

    '=========================
    '======== COMPILE ========
    '=========================
    Public Function Compile(Scope As Scope) As String
        Return Assemble(Scope)
    End Function
    Public Function Compile(Scope As Scope, RequestedType As Type) As String

        'Try assembling when Assemble(Request) is overriden
        Dim Result As String = Assemble(Scope, RequestedType)
        If Not Result = Nothing Then
            Return Result
        End If

        'If Assemble iss not overriden then convert using @converter
        Return ReturnType(Scope).ConvertTo(Compile(Scope), RequestedType)

    End Function

    'Assemble
    Protected MustOverride Function Assemble(Scope As Scope) As String
    Protected Overridable Function Assemble(Scope As Scope, RequestedType As Type) As String
        Return Nothing
    End Function

End Class
