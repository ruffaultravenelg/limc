Imports System.IO

Namespace C
    Friend Class RuntimeFiles

        Private Shared ReadOnly EXEDIR As String = AppDomain.CurrentDomain.BaseDirectory
        Private Shared ReadOnly RUNTIME_HEADERS As String = Path.Combine(EXEDIR, "runtime", "headers")
        Private Shared ReadOnly RUNTIME_SRC As String = Path.Combine(EXEDIR, "runtime", "src")

        Public Shared Function GetHeaderDirectory() As String
            Return RUNTIME_HEADERS
        End Function
        Public Shared Function GetHeaders() As IReadOnlyList(Of String)
            Return Directory.EnumerateFiles(RUNTIME_HEADERS, "*.h", SearchOption.AllDirectories) _ ' List all .h files
                            .Select(Function(f) Path.GetRelativePath(RUNTIME_HEADERS, f)).ToList() ' Set relative to header dir
        End Function
        Public Shared Function GetSrcFiles() As IReadOnlyList(Of String)
            Return Directory.GetFiles(RUNTIME_SRC, "*.c", SearchOption.AllDirectories) ' Get all c files
        End Function

    End Class
End Namespace