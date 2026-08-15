Imports limc.CodeGen

Namespace TypeSystem
    Public Class StrType
        Inherits Type

        Public Overrides ReadOnly Property cRepresentation As String = LIM_STR
        Public Overrides ReadOnly Property IsPointer As Boolean = True

        Public Overrides Function DefaultValue(Scope As Context.Scope) As String
            Return "NULL"
        End Function

        Public Overrides Function ToString() As String
            Return "str"
        End Function

        Public Sub Compile()

            ' Register string structure
            CodeGen.RegisterStruct(New CodeGen.Struct(LIM_STR, {"char* data;", "size_t len;"}, "lim string"))

            ' Register helper functions
            CodeGen.RegisterFunction(New BaseFunction($"{LIM_STR} {C_STR_TO_LIM_STR}(char* in)", {$"return ({LIM_STR}){{ .data = in, .len = strlen(in) }};"}, $"Convert (char*) to ({LIM_STR}), does not dup the string"))

            ' Register helper functions
            CodeGen.RegisterFunction(New BaseFunction($"char* {LIM_STR_TO_C_STR}({LIM_STR} in)", {$"return in.data;"}, $"Convert ({LIM_STR}) to (char*), you are the owner of the char*"))

            ' instance[index]
            RegisterRelation(Lazy.HardRelationBuilder.From(Me) _
                .WithType(RelationType.RELATION_BRACKETS) _
                .WithArgumentTypes({Type.Int}) _
                .WithArgumentNames({"index"}) _
                .WithReturnType(Type.Str) _
                .WithBody({
                    $"if (index < 0) index = {INSTANCE_ARGUMENT_NAME}.len + index;",
                    $"if (index >= {INSTANCE_ARGUMENT_NAME}.len || index < 0) {CodeGen.WritePanicCall("""Index out of range""")};",
                    $"char* result = {AllocateLeaf("char", "2")};",
                    $"result[0] = {INSTANCE_ARGUMENT_NAME}.data[index];",
                    "result[1] = '\0';",
                    $"return {Write_C_to_LimStr("result")};"
                }) _
                .Build()
            )

            ' instance + string
            RegisterRelation(Lazy.HardRelationBuilder.From(Me) _
                .WithType(RelationType.RELATION_ADD) _
                .WithArgumentTypes({Type.Str}) _
                .WithArgumentNames({"other"}) _
                .WithReturnType(Type.Str) _
                .WithBody({
                    $"size_t len = {INSTANCE_ARGUMENT_NAME}.len + other.len;",
                    $"char* result = {AllocateLeaf("char", "len + 1")};",
                    $"strcpy(result, {INSTANCE_ARGUMENT_NAME}.data);",
                    "strcat(result, other.data);",
                    $"return {Write_C_to_LimStr("result")};"
                }) _
                .Build()
            )

            ' string = string
            RegisterRelation(Lazy.HardRelationBuilder.From(Me) _
                .WithType(RelationType.RELATION_EQUAL) _
                .WithArgumentTypes({Type.Str}) _
                .WithArgumentNames({"other"}) _
                .WithReturnType(Type.Bool) _
                .WithBody({
                    $"return strcmp({INSTANCE_ARGUMENT_NAME}.data, other.data) == 0;"
                }) _
                .Build()
            )

            ' str -> int()
            RegisterMethod(New Lazy.HardMethod(Me, "int", {}, Type.Int, {
                "char* endptr;",
                "errno = 0;",
                $"long val = strtol({INSTANCE_ARGUMENT_NAME}.data, &endptr, 10);",
                $"if (errno != 0 || *endptr != '\0' || val > INT_MAX || val < INT_MIN) {CodeGen.WritePanicCall("""Cannot convert str to long""")};",
                "return (int)round(val);"
            }))

            ' substring(idx, len)
            RegisterMethod(New Lazy.HardMethod(Me, "substr", {Type.Int, Type.Int}, Type.Str, {
                $"int start = arg0;",
                $"int length = arg1;",
                $"if (start < 0) start = {INSTANCE_ARGUMENT_NAME}.len + start;",
                $"if (start >= {INSTANCE_ARGUMENT_NAME}.len || length <= 0) {CodeGen.WritePanicCall("""Invalid substring parameters""")};",
                $"char* result = {AllocateLeaf("char", "length + 1")};",
                $"strncpy(result, {INSTANCE_ARGUMENT_NAME}.data + start, length);",
                "result[length] = '\0';",
                $"return {Write_C_to_LimStr("result")};"
            }))

            RegisterGetter(New Lazy.DirectAccessGetter("len", Int, Function(instance) $"(int)strlen({instance})"))

        End Sub

    End Class
End Namespace