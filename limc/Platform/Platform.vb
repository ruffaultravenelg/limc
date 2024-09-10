Imports System.IO

Module Platform

    ' Properties
    Public ReadOnly CurrentOS As OSPlatform = GetCurrentOS()
    Public ReadOnly CurrentArch As ArchPlatform = GetCurrentArch()
    Public ReadOnly ExecutableDirectory As String = AppDomain.CurrentDomain.BaseDirectory
    Public ReadOnly LibDirectory As String = Path.Combine(ExecutableDirectory, "libs")

    ' Enum for Operating Systems
    Public Enum OSPlatform
        WINDOWS
        LINUX
        MACOS
    End Enum

    ' Enum for CPU Architectures
    Public Enum ArchPlatform
        X86
        X64
        ARM64
    End Enum

    ' Function to detect the current Operating System
    Private Function GetCurrentOS() As OSPlatform
        If Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.Windows) Then
            Return OSPlatform.WINDOWS
        ElseIf Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.Linux) Then
            Return OSPlatform.LINUX
        ElseIf Runtime.InteropServices.RuntimeInformation.IsOSPlatform(Runtime.InteropServices.OSPlatform.OSX) Then
            Return OSPlatform.MACOS
        End If

        Throw New PlatformNotSupportedException("Unsupported Operating System")
    End Function

    ' Function to detect the current CPU Architecture
    Private Function GetCurrentArch() As ArchPlatform
        Select Case Runtime.InteropServices.RuntimeInformation.OSArchitecture
            Case Runtime.InteropServices.Architecture.X86
                Return ArchPlatform.X86
            Case Runtime.InteropServices.Architecture.X64
                Return ArchPlatform.X64
            Case Runtime.InteropServices.Architecture.Arm64
                Return ArchPlatform.ARM64
            Case Else
                Throw New PlatformNotSupportedException("Unsupported CPU Architecture")
        End Select
    End Function

End Module
