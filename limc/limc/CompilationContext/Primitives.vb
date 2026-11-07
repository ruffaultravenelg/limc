Imports limc.Environment
Imports limc.Symbol

Friend Class Primitives
    Implements IPrimitives

    ' ==== COMMON TYPES ====
    Public ReadOnly Property INT As New IntTypeSymbol() Implements IPrimitives.INT
    Public ReadOnly Property FLOAT As New FloatTypeSymbol() Implements IPrimitives.FLOAT
    Public ReadOnly Property BOOL As New BoolTypeSymbol() Implements IPrimitives.BOOL
    Public ReadOnly Property STR As New StrTypeSymbol() Implements IPrimitives.STR

    ' func<a, b><r>
    Private FuncRepo As New Repository.FuncTypeRepository()

    Public Function Func(ArgumentTypes As IEnumerable(Of TypeSymbol), ReturnType As TypeSymbol) As FuncTypeSymbol Implements IPrimitives.Func
        Return FuncRepo.ResolveFuncType(ArgumentTypes, ReturnType)
    End Function

    ' constructor
    Public Sub New()

        ' ==== CREATE TYPES ENVS ====

        ' int env
        INT.SetEnv(New PrimitiveTypeEnv.Builder().Build())

    End Sub

End Class
