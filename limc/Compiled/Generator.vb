Namespace C
    Public Module Generator

        'Context name
        Public Const CONTEXT_NAME As String = "ctx"
        Public Const CONTEXT_STRUCTURENAME As String = CONTEXT_NAME & "_t"

        'Naming shema
        Public ReadOnly Namer As New CounterNamer()

        'Content of the file
        Private Import As New HashSet(Of String)
        Private Defines As New HashSet(Of String)
        Private Functions As New HashSet(Of C.Function)
        Private Enums As New HashSet(Of C.Enum)
        Private Structures As New HashSet(Of C.Structure)

        'Append function
        Public Sub AddFunction(Fn As C.Function)
            Functions.Add(Fn)
        End Sub

        'Append import
        Public Sub AddImport(Import As String)
            If Not Generator.Import.Contains(Import) Then
                Generator.Import.Add(Import)
            End If
        End Sub

        'Append enum
        Public Sub AddEnum(E As C.Enum)
            Enums.Add(E)
        End Sub

        'Append structure
        Public Sub AddStructure(Struct As C.Structure)
            Structures.Add(Struct)
        End Sub

        'Append define
        Public Sub AddDefine(Define As String)
            Defines.Add(Define)
        End Sub

        'Write
        Public Sub Write(Stream As IO.StreamWriter)

            'Write file header
            Stream.WriteLine("/*")
            Stream.WriteLine("")
            Stream.WriteLine(vbTab & "File compiled by Lim compiler.")
            Stream.WriteLine(vbTab & "Written by Gémino Ruffault--Ravenel the 22/12/2024")
            Stream.WriteLine("")
            Stream.WriteLine(vbTab & "You are the only responsible for this file and his content.")
            Stream.WriteLine("")
            Stream.WriteLine(vbTab & "Compile using :")
            Stream.WriteLine(vbTab & vbTab & "gcc source.c -o prog")
            Stream.WriteLine("")
            Stream.WriteLine("*/")

            'Write imports
            WriteTitle(Stream, "Imports")
            For Each Import As String In Generator.Import
                Stream.WriteLine(Import)
            Next

            'Write defines
            WriteTitle(Stream, "Defines")
            For Each Define As String In Defines
                Stream.WriteLine(Define)
            Next

            'Structure signature
            WriteTitle(Stream, "Forward declaration")
            For Each Struct As C.Structure In Structures
                Struct.WriteTypedefSignature(Stream)
            Next

            'Write enums
            WriteTitle(Stream, "Enums")
            For Each E As C.Enum In Generator.Enums
                E.WriteTypedef(Stream)
            Next

            'Structure definition
            WriteTitle(Stream, "Complete definition")
            For Each Struct As C.Structure In Structures
                Struct.WriteDefinition(Stream)
                Stream.WriteLine()
            Next

            'Functions id (for stacktrace)
            WriteTitle(Stream, "Function names", False)
            C.Function.WriteFunctionMap(Stream)

            'Write functions signatures
            WriteTitle(Stream, "Functions signatures")
            For Each Func As C.Function In Functions
                Func.WriteSignature(Stream)
            Next

            'Write functions bodies
            WriteTitle(Stream, "Functions bodies")
            For Each Func As C.Function In Functions
                Func.WriteBody(Stream)
                Stream.WriteLine()
            Next

        End Sub

        'Write title
        Private Sub WriteTitle(Stream As IO.StreamWriter, Title As String, Optional NewLine As Boolean = True)

            'Generate title
            Dim Main As String = "//// " & Title.ToUpper() & " ////"
            Dim Around As String = StrDup(Main.Length, "/")

            'Write
            If NewLine Then
                Stream.WriteLine()
            End If
            Stream.WriteLine(Around)
            Stream.WriteLine(Main)
            Stream.WriteLine(Around)

        End Sub

        'Always here stuff
        Public Sub InitAlwaysHereStuff()

            'Add general imports
            AddImport("#include <stdio.h>")
            AddImport("#include <stdlib.h>")
            AddImport("#include <string.h>")
            AddImport("#include <stdbool.h>")
            AddImport("#include <stdint.h>")
            AddImport("#include """ & Compiler.TGC_PATH & """")

            'Add context structure
            AddStructure(New C.Structure(CONTEXT_STRUCTURENAME, {
                CONTEXT_STRUCTURENAME & "* parent",
                "uint32_t func_id",
                "tgc_t* gc"
            }))

            'Add context init define
            AddDefine("#define INIT_" & CONTEXT_NAME.ToUpper() & "(dad, id) \" & Environment.NewLine & vbTab & CONTEXT_STRUCTURENAME & " " & CONTEXT_NAME & " = {.parent = dad, .func_id = id, .gc = dad ? dad->gc : NULL}")

            'Add function id map entry structure
            AddStructure(New C.Structure("fn_entry", {
                "uint32_t id",
                "const char* name"
            }))

            'Add retrieve function name from func_id function
            AddFunction(New C.Function("const char* get_function_name(uint32_t id)", {
                "",
                "for (size_t i = 0; i < sizeof(fn_name_map) / sizeof(fn_name_map[0]); i++)",
                vbTab & "if (fn_name_map[i].id == id)",
                vbTab & vbTab & "return fn_name_map[i].name;",
                "return ""<unknown>"";"
            }))

            'Add panic function
            AddFunction(New C.Function("void lim_panic(" & CONTEXT_STRUCTURENAME & "* ctx, const char* message)", {
                "",
                "printf(""LIM RUNTIME ERROR: %s\n"", message);",
                "printf(""stacktrace:\n"");",
                "while (ctx){",
                vbTab & "const char* funcname = get_function_name(ctx->func_id);",
                vbTab & "printf("" > %s\n"", funcname);",
                vbTab & "ctx = ctx->parent;",
                "}",
                "printf(""Program aborted.\n"");"
            }))

            'Add allocator function
            AddFunction(New C.Function("void* lim_alloc(" & CONTEXT_STRUCTURENAME & "* ctx, size_t size)", {
                "",
                "void* addr = tgc_alloc(ctx->gc, size);",
                "if (addr == NULL) lim_panic(ctx, ""Not enough memory"");",
                "return addr;"
            }))

        End Sub

    End Module
End Namespace
