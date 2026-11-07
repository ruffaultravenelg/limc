Imports limc.Symbol

Friend Interface IPrimitives

    ReadOnly Property INT As IntTypeSymbol
    ReadOnly Property FLOAT As FloatTypeSymbol
    ReadOnly Property BOOL As BoolTypeSymbol
    ReadOnly Property STR As StrTypeSymbol

    Function Func(ArgumentTypes As IEnumerable(Of TypeSymbol), ReturnType As TypeSymbol) As FuncTypeSymbol

End Interface
