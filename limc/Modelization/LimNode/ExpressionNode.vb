Imports System.Diagnostics.CodeAnalysis

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
    Public ReadOnly Property ReturnType(Context As Context) As Type
        Get
            Return CalculateReturnType(Context)
        End Get
    End Property
    Public ReadOnly Property CanReturn(Request As Type, Context As Context) As Boolean
        Get

            'If node can convert
            If CanReturnType(Request, Context) Then
                Return True
            End If

            'Check if the return type can be converted
            Dim ReturnedType As Type = CalculateReturnType(Context)
            Return ReturnedType.CanConvertTo(Request)

        End Get
    End Property

    'Calculate return type
    Protected MustOverride Function CalculateReturnType(Context As Context) As Type

    'Can return type ?
    Protected Overridable Function CanReturnType(Request As Type, Context As Context) As Boolean
        Return False
    End Function

    '=========================
    '======== COMPILE ========
    '=========================
    Public Function Compile(Context As Context) As CExpression
        Return Assemble(Context)
    End Function
    Public Function Compile(Request As Type, Context As Context) As CExpression

        'Try assembling when Assemble(Request) is overriden
        Dim Result As CExpression = Assemble(Request, Context)
        If Result IsNot Nothing Then
            Return Result
        End If

        'If Assemble is not overriden then convert using @converter
        Return ReturnType(Context).ConvertTo(Compile(Context), Request)

    End Function

    'Assemble
    Protected MustOverride Function Assemble(Context As Context) As CExpression
    Protected Overridable Function Assemble(Request As Type, Result As Context) As CExpression
        Return Nothing
    End Function

End Class
