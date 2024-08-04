Imports System.Formats
Imports limc.LimSource
Imports MessagePack.Formatters

Public Class VariableNode
    Inherits ExpressionNode

    '===============================
    '======== VARIABLE NAME ========
    '===============================
    Private VariableName As String

    '=============================
    '======== CONSTRUCTOR ========
    '=============================
    Public Sub New(Value As Token)
        MyBase.New(Value.Location)

        'Set values
        Me.VariableName = Value.Value

    End Sub

    '=============================
    '======== RETURN TYPE ========
    '=============================
    Protected Overrides Function CalculateReturnType(Scope As Scope) As Type

        'Search variable
        For Each UpperScope As Scope In Scope.Uppers
            For Each Var As Variable In UpperScope.Variables
                If Var.Name = VariableName Then
                    Return Var.Type
                End If
            Next
        Next

        'Search for properties
        If Scope.HasAttachedClass Then
            For Each Propertie As Propertie In Scope.AttachedClass.Properties
                If Propertie.Name = VariableName Then
                    Return Propertie.Type
                End If
            Next
        End If

        'Search function just by name
        Try
            Return Me.Location.File.Function(VariableName, {}).SignatureType
        Catch ex As CannotFindFunctionException
            Throw New LocalizedException("The """ & VariableName & """ element cannot be found in this scope.", "No variable or function is named """ & VariableName & """. Check that the element is accessible.", Me.Location)
        End Try

    End Function

    Protected Overrides Function CanReturnType(Request As Type, Scope As Scope) As Boolean

        'Search variable
        For Each UpperScope As Scope In Scope.Uppers
            For Each Var As Variable In UpperScope.Variables
                If Var.Name = VariableName AndAlso Var.Type = Request Then
                    Return True
                End If
            Next
        Next

        'Search for properties
        If Scope.HasAttachedClass Then
            For Each Propertie As Propertie In Scope.AttachedClass.Properties
                If Propertie.Name = VariableName AndAlso Propertie.Type = Request Then
                    Return True
                End If
            Next
        End If

        'Search function
        If TypeOf Request Is FunctionSignatureType AndAlso Me.Location.File.FunctionExist(VariableName, {}, DirectCast(Request, FunctionSignatureType)) Then
            Return True
        End If

        'Cannot
        Return False

    End Function

    '==========================
    '======== ASSEMBLE ========
    '==========================
    Protected Overrides Function Assemble(Scope As Scope) As String

        'Search variable
        For Each UpperScope As Scope In Scope.Uppers
            For Each Var As Variable In UpperScope.Variables
                If Var.Name = VariableName Then
                    Return Var.CompiledName
                End If
            Next
        Next

        'Search for properties
        If Scope.HasAttachedClass Then
            For Each Propertie As Propertie In Scope.AttachedClass.Properties
                If Propertie.Name = VariableName Then
                    Return Propertie.Getter
                End If
            Next
        End If

        'Search function just by name
        Try

            'Get targeted function
            Dim TargetedFunction As CFunction = Me.Location.File.Function(VariableName, {})

            'Return variable
            Return TargetedFunction.SignatureType.NewFuncCompiledName & "(" & TargetedFunction.CompiledName & ")"

        Catch ex As CannotFindFunctionException
            Throw New LocalizedException("The """ & VariableName & """ element cannot be found in this scope.", "No variable or function is named """ & VariableName & """. Check that the element is accessible.", Me.Location)
        End Try

    End Function

    Protected Overrides Function Assemble(Scope As Scope, RequestedType As Type) As String

        'Search variable
        For Each UpperScope As Scope In Scope.Uppers
            For Each Var As Variable In UpperScope.Variables
                If Var.Name = VariableName AndAlso Var.Type = RequestedType Then
                    Return Var.CompiledName
                End If
            Next
        Next

        'Search for properties
        If Scope.HasAttachedClass Then
            For Each Propertie As Propertie In Scope.AttachedClass.Properties
                If Propertie.Name = VariableName AndAlso Propertie.Type = RequestedType Then
                    Return Propertie.Getter
                End If
            Next
        End If

        'Search function
        If TypeOf RequestedType Is FunctionSignatureType Then
            Try
                Dim Result As CFunction = Me.Location.File.Function(VariableName, {}, DirectCast(RequestedType, FunctionSignatureType))
                Return DirectCast(RequestedType, FunctionSignatureType).NewFuncCompiledName & "(" & Result.CompiledName & ")"
            Catch ex As CannotFindFunctionException
            End Try
        End If

        'Cannot
        Return Nothing

    End Function

End Class
