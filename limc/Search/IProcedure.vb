'
' Represents a procedure from lim's point of view, in other words a function or method
'
Public Module Procedure

    'Procedure interfaces
    Public Interface IProcedure
        ReadOnly Property Name As String

    End Interface
    Public Interface ICompiledProcedure
        Inherits IProcedure

        ReadOnly Property CompiledName As String
        ReadOnly Property GenericTypes As IEnumerable(Of Type)
        ReadOnly Property Arguments As IEnumerable(Of Type)
        ReadOnly Property ReturnType As Type

    End Interface
    Public Interface IUnCompiledProcedure
        Inherits IProcedure

        ReadOnly Property GenericTypes As List(Of GenericTypeNode)
        ReadOnly Property Arguments As List(Of FunctionArgumentNode)

        Function GenerateScope(GenericType As IEnumerable(Of Type), ParentClass As Scope) As Scope

    End Interface

    'Search [name] <generic_types...> (args...)
    Public Function SearchBestProcedure(Scope As Scope, CompiledProcedures As IEnumerable(Of ICompiledProcedure), UncompiledProcedures As IEnumerable(Of IUnCompiledProcedure), ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), ProvidedArguments As IEnumerable(Of ExpressionNode), CompileUncompiledFunction As Func(Of IUnCompiledProcedure, IEnumerable(Of Type), ICompiledProcedure), Optional CompiledScope As Scope = Nothing) As ICompiledProcedure

        'Search if there is a compiled procedure that already exist
        For Each Procedure As ICompiledProcedure In CompiledProcedures
            If CompiledProcedureCorrespond(Scope, Procedure, ProvidedName, ProvidedGenericType, ProvidedArguments) Then
                Return Procedure
            End If
        Next

        'Search if there is a uncompiled procedure that match
        For Each Procedure As IUnCompiledProcedure In UncompiledProcedures
            If UncompiledProcedureCorrespond(Scope, Procedure, ProvidedName, ProvidedGenericType, ProvidedArguments, CompiledScope) Then
                Return CompileUncompiledFunction(Procedure, ProvidedGenericType)
            End If
        Next

        'Nothing
        Throw New UnableToFindProcedure()

    End Function
    Public Function CompiledProcedureCorrespond(Scope As Scope, Procedure As ICompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), ProvidedArguments As IEnumerable(Of ExpressionNode)) As Boolean

        'Name
        If Not Procedure.Name = ProvidedName Then
            Return False
        End If

        'Generic types
        If Not ProvidedGenericType.Count = Procedure.GenericTypes.Count Then
            Return False
        End If

        'Passed generic types
        For i As Integer = 0 To ProvidedGenericType.Count - 1
            If Not Procedure.GenericTypes(i) = ProvidedGenericType Then
                Return False
            End If
        Next

        'Passed arguments types count
        If Not Procedure.Arguments.Count = ProvidedArguments.Count Then
            Return False
        End If

        'Passed arguments types
        For i As Integer = 0 To ProvidedArguments.Count - 1
            If Not ProvidedArguments(i).CanReturn(Procedure.Arguments(i), Scope) Then
                Return False
            End If
        Next

        'Everything looks ok
        Return True

    End Function
    Public Function UncompiledProcedureCorrespond(Scope As Scope, Procedure As IUnCompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), ProvidedArguments As IEnumerable(Of ExpressionNode), CompiledScope As Scope) As Boolean

        'Name
        If Not Procedure.Name = ProvidedName Then
            Return False
        End If

        'Passed generic types count
        If Not Procedure.GenericTypes.Count = ProvidedGenericType.Count Then
            Return False
        End If

        'Passed generic types
        For i As Integer = 0 To ProvidedGenericType.Count - 1
            If Not Procedure.GenericTypes(i).IsComptatible(ProvidedGenericType(i)) Then
                Return False
            End If
        Next

        'Passed arguments types count
        If Not Procedure.Arguments.Count = ProvidedArguments.Count Then
            Return False
        End If

        'Create scope
        Dim ProcedureScope As Scope = Procedure.GenerateScope(ProvidedGenericType, CompiledScope)

        'Passed arguments types
        For i As Integer = 0 To ProvidedArguments.Count - 1

            Dim WantedArgumentType As Type = Procedure.Arguments(i).Type.AssociatedType(ProcedureScope)

            If Not ProvidedArguments(i).CanReturn(WantedArgumentType, Scope) Then
                Return False
            End If

        Next

        'Everything looks ok
        Return True

    End Function


    'Search [name] <generic_types...>
    Public Function SearchBestProcedure(CompiledProcedures As IEnumerable(Of ICompiledProcedure), UncompiledProcedures As IEnumerable(Of IUnCompiledProcedure), ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), CompileUncompiledFunction As Func(Of IUnCompiledProcedure, IEnumerable(Of Type), ICompiledProcedure), Optional CompiledScope As Scope = Nothing) As ICompiledProcedure

        'Search if there is a compiled procedure that already exist
        Dim AvailableCompiledResults As New List(Of ICompiledProcedure)
        For Each Procedure As ICompiledProcedure In CompiledProcedures
            If CompiledProcedureCorrespond(Procedure, ProvidedName, ProvidedGenericType) Then
                AvailableCompiledResults.Add(Procedure)
            End If
        Next

        'If there is only one compatible procedure -> choose it
        If AvailableCompiledResults.Count = 1 Then
            Return AvailableCompiledResults(0)
        End If

        'Too many result
        If AvailableCompiledResults.Count > 1 Then
            Throw New UnableToChooseProcedure()
        End If

        'Search if there is a uncompiled procedure that match
        Dim AvailableUcompiledResults As New List(Of IUnCompiledProcedure)
        For Each Procedure As IUnCompiledProcedure In UncompiledProcedures
            If UncompiledProcedureCorrespond(Procedure, ProvidedName, ProvidedGenericType, CompiledScope) Then
                AvailableUcompiledResults.Add(Procedure)
            End If
        Next

        'One uncompiled procedure -> choose it
        If AvailableUcompiledResults.Count = 1 Then
            Return CompileUncompiledFunction(AvailableUcompiledResults(0), ProvidedGenericType)
        End If

        'Too many uncompiled procedures result
        If AvailableUcompiledResults.Count > 1 Then
            Throw New UnableToChooseProcedure()
        End If

        'No available uncompiled procedure
        Throw New UnableToFindProcedure()

    End Function
    Public Function CompiledProcedureCorrespond(Procedure As ICompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type)) As Boolean

        'Name
        If Not Procedure.Name = ProvidedName Then
            Return False
        End If

        'Generic types
        If Not ProvidedGenericType.Count = Procedure.GenericTypes.Count Then
            Return False
        End If

        'Passed generic types
        For i As Integer = 0 To ProvidedGenericType.Count - 1
            If Not Procedure.GenericTypes(i) = ProvidedGenericType(i) Then
                Return False
            End If
        Next

        'Everything looks ok
        Return True

    End Function
    Public Function UncompiledProcedureCorrespond(Procedure As IUnCompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), CompiledScope As Scope) As Boolean

        'Name
        If Not Procedure.Name = ProvidedName Then
            Return False
        End If

        'Passed generic types count
        If Not Procedure.GenericTypes.Count = ProvidedGenericType.Count Then
            Return False
        End If

        'Passed generic types
        For i As Integer = 0 To ProvidedGenericType.Count - 1
            If Not Procedure.GenericTypes(i).IsComptatible(ProvidedGenericType(i)) Then
                Return False
            End If
        Next

        'Everything looks ok
        Return True

    End Function

    'Search [name] <generic_types...> (args...)
    Public Function SearchBestProcedure(CompiledProcedures As IEnumerable(Of ICompiledProcedure), UncompiledProcedures As IEnumerable(Of IUnCompiledProcedure), ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), WantedSignature As FunctionSignatureType, CompileUncompiledFunction As Func(Of IUnCompiledProcedure, IEnumerable(Of Type), ICompiledProcedure), Optional CompiledScope As Scope = Nothing) As ICompiledProcedure

        'Search if there is a compiled procedure that already exist
        For Each Procedure As ICompiledProcedure In CompiledProcedures
            If CompiledProcedureCorrespond(Procedure, ProvidedName, ProvidedGenericType, WantedSignature) Then
                Return Procedure
            End If
        Next

        'Search if there is a uncompiled procedure that match
        For Each Procedure As IUnCompiledProcedure In UncompiledProcedures
            If UncompiledProcedureCorrespond(Procedure, ProvidedName, ProvidedGenericType, WantedSignature, CompiledScope) Then

                'Compile
                Dim CompiledProcedure As ICompiledProcedure = CompileUncompiledFunction(Procedure, ProvidedGenericType)

                'Check return type
                If CompiledProcedure.ReturnType = WantedSignature.ReturnType Then
                    Return CompiledProcedure
                End If

            End If
        Next

        'Nothing
        Throw New UnableToFindProcedure()

    End Function
    Public Function CompiledProcedureCorrespond(Procedure As ICompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), WantedSignature As FunctionSignatureType) As Boolean

        'Name
        If Not Procedure.Name = ProvidedName Then
            Return False
        End If

        'Generic types
        If Not ProvidedGenericType.Count = Procedure.GenericTypes.Count Then
            Return False
        End If

        'Passed generic types
        For i As Integer = 0 To ProvidedGenericType.Count - 1
            If Not Procedure.GenericTypes(i) = ProvidedGenericType(i) Then
                Return False
            End If
        Next

        'Passed arguments types count
        If Not Procedure.Arguments.Count = WantedSignature.ArgumentsTypes.Count Then
            Return False
        End If

        'Passed arguments types
        For i As Integer = 0 To WantedSignature.ArgumentsTypes.Count - 1
            If Not Procedure.Arguments(i) = WantedSignature.ArgumentsTypes(i) Then
                Return False
            End If
        Next

        'Return type
        If Not WantedSignature.ReturnType = Procedure.ReturnType Then
            Return False
        End If

        'Everything looks ok
        Return True

    End Function
    Public Function UncompiledProcedureCorrespond(Procedure As IUnCompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), WantedSignature As FunctionSignatureType, CompiledScope As Scope) As Boolean

        'Name
        If Not Procedure.Name = ProvidedName Then
            Return False
        End If

        'Passed generic types count
        If Not Procedure.GenericTypes.Count = ProvidedGenericType.Count Then
            Return False
        End If

        'Passed generic types
        For i As Integer = 0 To ProvidedGenericType.Count - 1
            If Not Procedure.GenericTypes(i).IsComptatible(ProvidedGenericType(i)) Then
                Return False
            End If
        Next

        'Passed arguments types count
        If Not Procedure.Arguments.Count = WantedSignature.ArgumentsTypes.Count Then
            Return False
        End If

        'Create scope
        Dim ProcedureScope As Scope = Procedure.GenerateScope(ProvidedGenericType, CompiledScope)

        'Passed arguments types
        For i As Integer = 0 To WantedSignature.ArgumentsTypes.Count - 1

            Dim WantedArgumentType As Type = Procedure.Arguments(i).Type.AssociatedType(ProcedureScope)
            Dim PassedArgumentType As Type = WantedSignature.ArgumentsTypes(i)

            If Not PassedArgumentType = WantedArgumentType Then 'TODO: Check type compatibility instead of equality
                Return False
            End If

        Next

        'Everything looks ok exept the return type
        Return True

    End Function

    'Errors
    Public Class SearchProcedureException
        Inherits CompilerException
        Public Sub New(Message As String)
            MyBase.New("Cannot find a procedure", Message)
        End Sub
    End Class
    Public Class UnableToFindProcedure
        Inherits SearchProcedureException
        Public Sub New()
            MyBase.New("Unable to find a procedure")
        End Sub
    End Class
    Public Class UnableToChooseProcedure
        Inherits SearchProcedureException
        Public Sub New()
            MyBase.New("Unable to choose between multiple procedures")
        End Sub
    End Class

End Module
