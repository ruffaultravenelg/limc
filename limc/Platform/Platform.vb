Module Platform

    Public ReadOnly Current As Platform = GetCurrentPlatform()
    Public ReadOnly ExecutabeDirectory As String = AppDomain.CurrentDomain.BaseDirectory

    Public Enum Platform
        WIN
        WINARM
        LINUX
        LINUXARM
        MACOS
    End Enum

    Function GetCurrentPlatform() As Platform
        If Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.Windows) Then
            If Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.X64 OrElse Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.X86 Then
                Return Platform.WIN
            ElseIf Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.Arm64 Then
                Return Platform.WINARM
            End If
        ElseIf Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.Linux) Then
            If Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.X64 OrElse Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.X86 Then
                Return Platform.LINUX
            ElseIf Runtime.InteropServices.RuntimeInformation.OSArchitecture = Runtime.InteropServices.Architecture.Arm64 Then
                Return Platform.LINUXARM
            End If
        ElseIf Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.OSX) Then
            Return Platform.MACOS
        End If

        Throw New PlatformNotSupportedException()
    End Function

End Module
