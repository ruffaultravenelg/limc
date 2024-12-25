Friend Module Compiler

    'TGC
    Public ReadOnly Property TGC_H_PATH As String = IO.Path.Combine(AppContext.BaseDirectory, "sources", "tgc.h")
    Public ReadOnly Property TGC_C_PATH As String = IO.Path.Combine(AppContext.BaseDirectory, "sources", "tgc.c")

    'Compile a file to a output source file
    Friend Sub Compile(Input As String, Output As String)

        'Parse file
        Dim Source As Lim.SourceFile = Lim.SourceFile.Load(Input)

        'Get "main" function
        Dim MainFunction As Lim.Function = Source.GetFunction("main", {}, {})

        'Init stuff
        C.Generator.InitAlwaysHereStuff()

        'Create main thread function (entry point)
        C.Generator.AddFunction(New C.Function("int main(int argc, char** arv)", {
            "",
            "tgc_t gc;",
            "tgc_start(&gc, &argc);",
            C.Generator.CONTEXT_STRUCTURENAME & " ctx = {.parent = NULL, .gc = &gc, .func_id = 0};",
            MainFunction.CompiledName & "(&ctx);",
            "tgc_stop(&gc);",
            "return 0;"
        }))

        'Set compile commande
        C.Generator.SetCompileCommand(GetCompileCommand(Output))

        'Write file to output
        Dim Writer As New IO.StreamWriter(Output)
        C.Generator.Write(Writer)
        Writer.Close()

    End Sub

    'Get compile command
    Public Function GetCompileCommand(SourceFile As String) As String

        'Gcc path
        Dim GCC As String = ArgumentHandler.GCC

        'Add each file
        Dim FilesToCompiles As String = String.Join(" ", {SourceFile, TGC_C_PATH})

        'Dim output
        Dim Output As String = "prog"

        'Return compile commande
        Return $"{GCC} {FilesToCompiles} -o {Output}"

    End Function

End Module
