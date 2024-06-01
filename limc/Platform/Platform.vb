Module Platform

    Public ReadOnly Current As OSPlatform = GetCurrentPlatform()
    Public ReadOnly ExecutabeDirectory As String = AppDomain.CurrentDomain.BaseDirectory

    Public Enum OSPlatform
        WIN_X64
        WIN_ARM64
        LINUX_X86
        LINUX_ARM64
        MACOS
    End Enum

    Function GetCurrentPlatform() As OSPlatform
        If Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.Windows) Then
            If Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.X64 Then
                Return OSPlatform.WIN_X64
            ElseIf Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.Arm64 Then
                Return OSPlatform.WIN_ARM64
            End If
        ElseIf Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.Linux) Then
            If Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.X64 Then
                Return OSPlatform.LINUX_X86
            ElseIf Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.Arm64 Then
                Return OSPlatform.LINUX_ARM64
            End If
        ElseIf Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.OSX) Then
            Return OSPlatform.MACOS
        End If

        Throw New PlatformNotSupportedException()
    End Function

End Module
