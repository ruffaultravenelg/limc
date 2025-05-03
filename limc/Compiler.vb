Friend Module Compiler

    'TGC
    Public ReadOnly Property TGC_H_PATH As String = IO.Path.Combine(AppContext.BaseDirectory, "sources", "tgc.h")
    Public ReadOnly Property TGC_C_PATH As String = IO.Path.Combine(AppContext.BaseDirectory, "sources", "tgc.c")

    'Environnment folder
    Private ModulesDirectory As String = Nothing

    'Compile a file to a output source file
    Friend Sub Compile(Input As String, Output As String)

        'Get environment
        Dim ExpectedModuleFolder As String = IO.Path.Combine(IO.Path.GetDirectoryName(IO.Path.GetFullPath(Input)), "modules")
        If IO.Directory.Exists(ExpectedModuleFolder) Then
            ModulesDirectory = ExpectedModuleFolder
        End If

        'Parse file
        Dim Source As Lim.SourceFile = Lim.SourceFile.Load(Input)

        'Get "main" function
        Dim MainFunction As Lim.Function = Source.GetFunction("main", {}, {})

        'Init stuff
        C.Generator.InitAlwaysHereStuff()

        'Create main thread function (entry point)
        C.Generator.AddFunction(New C.Function("int main(int argc, char** argv)", {
            "",
            "tgc_t gc;",
            "tgc_start(&gc, &argc);",
            $"{C.Generator.CONTEXT_STRUCTURENAME} {C.Generator.CONTEXT_NAME} = {{.parent = NULL, .gc = &gc, .func_id = 0}};",
            $"{MainFunction.CompiledName}(&{C.Generator.CONTEXT_NAME});",
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

    'Get library filepath
    'Return Nothing if not found
    Public Function GetLibraryPath(Name As String)

        'Check in libs
        Dim LibPath As String = IO.Path.Combine(ArgumentHandler.LibsDirectory, Name & ".lim")
        If IO.File.Exists(LibPath) Then
            Return LibPath
        End If

        'No modules -> end here
        If ModulesDirectory = Nothing Then
            Return Nothing
        End If

        'Check in modules
        Dim ModuleFile As String = IO.Path.Combine(ModulesDirectory, Name, Name & ".lim")
        If IO.File.Exists(ModuleFile) Then
            Return ModuleFile
        End If

        ' Nothing found
        Return Nothing

    End Function

End Module
