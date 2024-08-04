Imports limc.LimSource

Public Class FunctionReferenceNode
    Inherits ExpressionNode

    '===============================
    '======== FUNCTION NAME ========
    '===============================
    Private FunctionName As String

    '==========================================
    '======== PASSED GENERIC ARGUMENTS ========
    '==========================================
    Private PassedGenericArguments As List(Of TypeNode)

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Location As Location, FunctionName As String, PassedGenericArguments As List(Of TypeNode))
        MyBase.New(Location)

        'Set values
        Me.FunctionName = FunctionName
        Me.PassedGenericArguments = PassedGenericArguments

    End Sub

    '=============================================
    '======== COMPILE PASSED GENERIC TYPE ========
    '=============================================
    Private Function CompilePassedGenericTypes(Scope As Scope) As List(Of Type)

        'Create the result
        Dim Result As New List(Of Type)

        'Compile each argument
        For Each Argument As TypeNode In PassedGenericArguments
            Result.Add(Argument.AssociatedType(Scope))
        Next

        'Return result
        Return Result

    End Function

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        'Get generic types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function
        Try
            Return Me.Location.File.Function(FunctionName, GenericTypes).SignatureType
        Catch ex As CannotFindFunctionException
            Throw New LocalizedException($"The ""{FunctionName}{Type.StringifyListOfType(GenericTypes)}"" function/method cannot be found in this scope.", $"No function or method named ""{FunctionName}"" exist or have {GenericTypes.Count} generic types.", Me.Location)
        End Try

    End Function

    Protected Overrides Function CanReturnType(Request As Type, Scope As Scope) As Boolean

        'Get generic types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function
        If TypeOf Request Is FunctionSignatureType AndAlso Me.Location.File.FunctionExist(FunctionName, GenericTypes, DirectCast(Request, FunctionSignatureType)) Then
            Return True
        End If

        'Cannot
        Return False

    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        'Get generic types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function just by name
        Try

            'Get targeted function
            Dim TargetedFunction As CFunction = Me.Location.File.Function(FunctionName, GenericTypes)

            'Return variable
            Return TargetedFunction.SignatureType.NewFuncCompiledName & "(" & TargetedFunction.CompiledName & ")"

        Catch ex As CannotFindFunctionException
            Throw New LocalizedException($"The ""{FunctionName}{Type.StringifyListOfType(GenericTypes)}"" function/method cannot be found in this scope.", $"No function or method named ""{FunctionName}"" exist or have {GenericTypes.Count} generic types.", Me.Location)
        End Try

    End Function

    Protected Overrides Function Assemble(Scope As Scope, RequestedType As Type) As String

        'Get generic types
        Dim GenericTypes As List(Of Type) = CompilePassedGenericTypes(Scope)

        'Search function
        If TypeOf RequestedType Is FunctionSignatureType Then
            Try
                Dim Result As CFunction = Me.Location.File.Function(FunctionName, GenericTypes, DirectCast(RequestedType, FunctionSignatureType))
                Return DirectCast(RequestedType, FunctionSignatureType).NewFuncCompiledName & "(" & Result.CompiledName & ")"
            Catch ex As CannotFindFunctionException
            End Try
        End If

        'Cannot
        Return Nothing

    End Function


End Class
