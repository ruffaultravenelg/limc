Imports limc.C.Naming
Imports limc.Environment
Imports limc.Symbol

Friend Class ClassicLimPrimitives
    Implements IPrimitives

    ' ==== COMMON TYPES ====
    Public ReadOnly Property INT As New IntTypeSymbol() Implements IPrimitives.INT
    Public ReadOnly Property FLOAT As New FloatTypeSymbol() Implements IPrimitives.FLOAT
    Public ReadOnly Property BOOL As New BoolTypeSymbol() Implements IPrimitives.BOOL
    Public ReadOnly Property STR As New StrTypeSymbol() Implements IPrimitives.STR

    Public ReadOnly Property PUTS As PrimitiveFunctionSymbol Implements IPrimitives.PUTS

    ' func<a, b><r>
    Private FuncRepo As New Repository.FuncTypeRepository()
    Public Function Func(ArgumentTypes As IEnumerable(Of TypeSymbol), ReturnType As TypeSymbol) As FuncTypeSymbol Implements IPrimitives.Func
        Return FuncRepo.ResolveFuncType(ArgumentTypes, ReturnType)
    End Function

    ' ==== CONSTRUCTOR ====
    ' shared cause it shows that it will generate things & use the namer, simple "New" constructor wasn't that explicit..
    Public Shared Function Generate(Namer As ISymbolNamer) As ClassicLimPrimitives
        Return New ClassicLimPrimitives(Namer)
    End Function
    Private Sub New(Namer As ISymbolNamer)

        ' ==== CREATE TYPES ENVS ====
        PopulateInt(Namer)
        PopulateFloat(Namer)

        Namer.BindKnownSymbol(BOOL, "bool")
        Namer.BindKnownSymbol(STR, "lim_str")

        ' puts(value:str)
        PUTS = New PrimitiveFunctionSymbol.Builder() _
            .SetName("puts") _
            .AddArgument("value", STR) _
            .NoReturn() _
            .Build()
        Namer.BindKnownSymbol(PUTS, "lim_puts")

    End Sub

    ' Generate all INT related primitives
    Private Sub PopulateInt(Namer As ISymbolNamer)
        Namer.BindKnownSymbol(INT, "int")

        ' 5.float()
        Dim int2float = New PrimitiveMethodSymbol.Builder() _
            .SetAttachedType(INT) _
            .SetName("float") _
            .SetReturnType(FLOAT) _
            .Build()
        Namer.BindKnownSymbol(int2float, "lim_int2float")

        ' create INT env
        INT.SetEnv(New PrimitiveTypeEnv.Builder() _
            .AddMethod(int2float) _
            .Build()
        )

    End Sub

    ' Generate all FLOAT related primitives
    Private Sub PopulateFloat(Namer As ISymbolNamer)
        Namer.BindKnownSymbol(FLOAT, "double")

        ' 5.float()
        Dim float2int = New PrimitiveMethodSymbol.Builder() _
            .SetAttachedType(FLOAT) _
            .SetName("int") _
            .SetReturnType(INT) _
            .Build()
        Namer.BindKnownSymbol(float2int, "lim_float2int")

        ' create INT env
        FLOAT.SetEnv(New PrimitiveTypeEnv.Builder() _
            .AddMethod(float2int) _
            .Build()
        )

    End Sub

End Class
