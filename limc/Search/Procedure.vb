'
' Represents a procedure from lim's point of view, in other words a function or method
'
Public Module Procedure

    'Procedure interfaces
    Public Interface Procedure
        ReadOnly Property Name As String

    End Interface
    Public Interface CompiledProcedure
        Inherits Procedure

        ReadOnly Property CompiledName As String
        ReadOnly Property GenericTypes As IEnumerable(Of Type)
        ReadOnly Property Arguments As IEnumerable(Of Type)
        ReadOnly Property ReturnType As Type

    End Interface
    Public Interface UnCompiledProcedure
        Inherits Procedure

        ReadOnly Property GenericTypes As List(Of GenericTypeNode)
        ReadOnly Property Arguments As List(Of FunctionArgumentNode)

        Function GenerateScope(GenericType As IEnumerable(Of Type)) As Scope

    End Interface

    'Search [name] <generic_types...> (args...)
    Public Function SearchBestProcedure(Of T As Procedure)(Scope As Scope, CompiledProcedures As IEnumerable(Of CompiledProcedure), UncompiledProcedures As IEnumerable(Of UnCompiledProcedure), ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), ProvidedArguments As IEnumerable(Of ExpressionNode), CompileUncompiledFunction As Func(Of UnCompiledProcedure, IEnumerable(Of Type), CompiledProcedure)) As T

        'Search if there is a compiled procedure that already exist
        For Each Procedure As CompiledProcedure In CompiledProcedures
            If CompiledProcedureCorrespond(Scope, Procedure, ProvidedName, ProvidedGenericType, ProvidedArguments) Then
                Return Procedure
            End If
        Next

        'Search if there is a uncompiled procedure that match
        For Each Procedure As UnCompiledProcedure In UncompiledProcedures
            If UncompiledProcedureCorrespond(Scope, Procedure, ProvidedName, ProvidedGenericType, ProvidedArguments) Then
                Return CompileUncompiledFunction(Procedure, ProvidedGenericType)
            End If
        Next

        'Nothing
        Throw New UnableToFindProcedure()

    End Function
    Public Function CompiledProcedureCorrespond(Scope As Scope, Procedure As CompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), ProvidedArguments As IEnumerable(Of ExpressionNode)) As Boolean

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
    Public Function UncompiledProcedureCorrespond(Scope As Scope, Procedure As UnCompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), ProvidedArguments As IEnumerable(Of ExpressionNode)) As Boolean

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
        Dim ProcedureScope As Scope = Procedure.GenerateScope(ProvidedGenericType)

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
    Public Function SearchBestProcedure(Of T As Procedure)(CompiledProcedures As IEnumerable(Of CompiledProcedure), UncompiledProcedures As IEnumerable(Of UnCompiledProcedure), ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), CompileUncompiledFunction As Func(Of UnCompiledProcedure, IEnumerable(Of Type), CompiledProcedure)) As T

        'Store each corresponding functions
        Dim Results As New List(Of Procedure)

        'Search if there is a compiled procedure that already exist
        For Each Procedure As CompiledProcedure In CompiledProcedures
            If CompiledProcedureCorrespond(Procedure, ProvidedName, ProvidedGenericType) Then
                Results.Add(Procedure)
            End If
        Next

        'Search if there is a uncompiled procedure that match
        For Each Procedure As UnCompiledProcedure In UncompiledProcedures
            If UncompiledProcedureCorrespond(Procedure, ProvidedName, ProvidedGenericType) Then
                Results.Add(Procedure)
            End If
        Next

        'No result
        If Results.Count = 0 Then
            Throw New UnableToFindProcedure()
        End If

        'Too many result
        If Results.Count > 1 Then
            Throw New UnableToChooseProcedure()
        End If

        'Return result
        If TypeOf Results(0) Is UnCompiledProcedure Then
            Return CompileUncompiledFunction(Results(0), ProvidedGenericType)
        Else
            Return Results(0)
        End If

    End Function
    Public Function CompiledProcedureCorrespond(Procedure As CompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type)) As Boolean

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
    Public Function UncompiledProcedureCorrespond(Procedure As UnCompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type)) As Boolean

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
    Public Function SearchBestProcedure(Of T As Procedure)(CompiledProcedures As IEnumerable(Of CompiledProcedure), UncompiledProcedures As IEnumerable(Of UnCompiledProcedure), ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), WantedSignature As FunctionSignatureType, CompileUncompiledFunction As Func(Of UnCompiledProcedure, IEnumerable(Of Type), CompiledProcedure)) As T

        'Search if there is a compiled procedure that already exist
        For Each Procedure As CompiledProcedure In CompiledProcedures
            If CompiledProcedureCorrespond(Procedure, ProvidedName, ProvidedGenericType, WantedSignature) Then
                Return Procedure
            End If
        Next

        'Search if there is a uncompiled procedure that match
        For Each Procedure As UnCompiledProcedure In UncompiledProcedures
            If UncompiledProcedureCorrespond(Procedure, ProvidedName, ProvidedGenericType, WantedSignature) Then

                'Compile
                Dim CompiledProcedure As CompiledProcedure = CompileUncompiledFunction(Procedure, ProvidedGenericType)

                'Check return type
                If CompiledProcedure.ReturnType = WantedSignature.ReturnType Then
                    Return CompiledProcedure
                End If

            End If
        Next

        'Nothing
        Throw New UnableToFindProcedure()

    End Function
    Public Function CompiledProcedureCorrespond(Procedure As CompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), WantedSignature As FunctionSignatureType) As Boolean

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
    Public Function UncompiledProcedureCorrespond(Procedure As UnCompiledProcedure, ProvidedName As String, ProvidedGenericType As IEnumerable(Of Type), WantedSignature As FunctionSignatureType) As Boolean

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
        Dim ProcedureScope As Scope = Procedure.GenerateScope(ProvidedGenericType)

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
