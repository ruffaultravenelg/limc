Imports limc.HIR
Imports limc.HIR.Lowerer

Namespace Symbol
    Friend MustInherit Class TypeSymbol
        Inherits Symbol
        Implements ICompilableSymbol

        Public ReadOnly Property Env As Environment.TypeEnvironment
        Public Overridable ReadOnly Property IS_POINTER As Boolean = False

        Public Sub New(LimName As String, Env As Environment.Env)
            MyBase.New(LimName)
            Me.Env = Env
        End Sub

        ' Only used by Primitive (BoolTypeSymbol, FloatTypeSymbol, etc...) because they cannot generate their at thair constructor
        Public Sub SetEnv(_Env As Environment.Env)
            Me._Env = _Env
        End Sub

        ' ICompilable stuff
        Public Property Compiled As Boolean = False Implements ICompilableSymbol.Compiled
        Public MustOverride Sub Compile(Lowerer As IHIRLowerer, ProgramBuilder As ProgramNode.Builder) Implements ICompilableSymbol.Compile

        ' To string
        Public MustOverride Overrides Function ToString() As String

        ' Comparison
        Public Shared Operator =(a As TypeSymbol, b As TypeSymbol) As Boolean
            Return Object.ReferenceEquals(a, b)
        End Operator
        Public Shared Operator <>(a As TypeSymbol, b As TypeSymbol) As Boolean
            Return Not Object.ReferenceEquals(a, b)
        End Operator

    End Class
End Namespace