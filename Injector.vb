Imports Microsoft.Win32
Imports System
Imports System.Runtime.InteropServices
Imports EasyHook
Imports SlimDX
Imports SlimDX.Direct3D9

Namespace Epic

    Public Class Injector

        Public Sub Injector()

        End Sub


        Const PROCESS_ALL_ACCESS As Long = &H1F0FFF
        'Const MEM_CREATE As Long = &H100 Or &H200
        Const MEM_COMMIT As Integer = &H1000
        Const PAGE_EXECUTE_READWRITE As Integer = &H40

        Const PAGE_RW As Long = &H4
        Const PAGE_EX As Long = &H40

        'FindWindow
        <DllImport("user32.dll", EntryPoint:="FindWindow", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Shared Function FindWindow(ByVal zero As IntPtr, ByVal lpWindowName As String) As IntPtr
        End Function

        'VirtualAllocEx
        <DllImport("kernel32.dll", EntryPoint:="VirtualAllocEx", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Private Shared Function VirtualAllocEx(ByVal zero As IntPtr, ByVal addr As IntPtr, ByVal size As Integer, ByVal allocType As Integer, ByVal prot As Integer) As IntPtr
        End Function

        'GetWindowThreadProcessId
        <DllImport("user32.dll", EntryPoint:="GetWindowThreadProcessId", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Private Shared Function GetWindowThreadProcessId(ByVal hwnd As Integer, <Out()> ByRef id As Integer) As IntPtr
        End Function

        'OpenProcess
        <DllImport("kernel32.dll", EntryPoint:="OpenProcess")> _
        Private Shared Function OpenProcess(ByVal acc As Integer, ByVal inherit As Boolean, ByVal id As Integer) As IntPtr
        End Function

        'GetModuleHandle
        <DllImport("kernel32.dll", EntryPoint:="GetModuleHandle")> _
        Private Shared Function GetModuleHandle(ByVal name As String) As IntPtr
        End Function

        'GetProcAddress
        <DllImport("kernel32.dll", EntryPoint:="GetProcAddress")> _
        Private Shared Function GetProcAddress(ByVal m As IntPtr, ByVal name As String) As IntPtr
        End Function

        'CreateFile
        <DllImport("kernel32.dll", EntryPoint:="CreateFile")> _
        Public Shared Function CreateFile(ByVal lpFileName As String, _
    ByVal dwDesiredAccess As String, ByVal dwShareMode As UInt32, ByVal lpSecurityAttributes As IntPtr, _
    ByVal dwCreationDisposition As UInt32, ByVal dwFlagsAndAttributes As UInt32, ByVal hTemplateFile As IntPtr) As IntPtr
        End Function

        'ReadProcessMemory
        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Shared Function ReadProcessMemory( _
        ByVal hProcess As IntPtr, _
        ByVal lpBaseAddress As IntPtr, _
        <Out()> ByVal lpBuffer() As Byte, _
        ByVal dwSize As Integer, _
        ByRef lpNumberOfBytesRead As Integer
        ) As Boolean
        End Function

        'WriteProcessMemory
        <DllImport("Kernel32.dll", EntryPoint:="WriteProcessMemory")> _
        Private Shared Function WriteProcessMemory(ByVal proc As IntPtr, ByVal addr As IntPtr, ByVal buff() As Byte, ByVal size As Integer, ByRef out As Integer) As IntPtr
        End Function

        'CreateRemoteThread
        <DllImport("kernel32.dll", EntryPoint:="CreateRemoteThread")> _
        Private Shared Function CreateRemoteThread(ByVal proc As IntPtr, ByVal attr As IntPtr, ByVal size As Integer, ByVal addr As IntPtr, ByVal param As IntPtr, ByVal flags As Integer, ByVal id As Integer) As IntPtr

        End Function

        'getProcess
        Public Function getProcess(ByVal className As String, ByVal titleName As String)
            Dim wndHandle As IntPtr = FindWindow(className, titleName)
            Dim handle As IntPtr
            Dim pid As Integer
            GetWindowThreadProcessId(wndHandle, pid)
            handle = OpenProcess(PROCESS_ALL_ACCESS, Nothing, pid)
            Return handle
        End Function

        'allocRemoteString
        Public Function allocRemoteString(ByVal handle As IntPtr, ByVal name As String)
            Dim alloc As IntPtr = VirtualAllocEx(handle, Nothing, New IntPtr(name.Length), MEM_COMMIT, PAGE_EXECUTE_READWRITE)
            Dim encoding As New System.Text.UTF8Encoding()
            WriteProcessMemory(handle, alloc, encoding.GetBytes(name), name.Length, Nothing)
            Return alloc
        End Function

        'getAddress
        Public Function getAddress(ByVal mo As String, ByVal func As String) As IntPtr
            Dim m As Integer = GetModuleHandle(mo)
            Dim addr As Integer = GetProcAddress(m, func)
            Return addr
        End Function

        'readMemory
        Public Function readInt(ByVal handle As IntPtr, ByVal addr As String) As Integer
            Dim s(4) As Byte
            'Dim s As Char
            ReadProcessMemory(handle, "&H" & addr, s, 4, Nothing)
            Return BitConverter.ToInt32(s, Nothing)
        End Function

        'writeMeory
        Public Sub writeInt(ByVal handle As IntPtr, ByVal addr As String, ByVal i As Integer)
            WriteProcessMemory(handle, "&H" & addr, BitConverter.GetBytes(i), 4, Nothing)
        End Sub

        Public Sub testRemote(ByVal handle As IntPtr, ByVal s As String)
            Dim alloc As IntPtr = allocRemoteString(handle, s)
            Dim loadlib As IntPtr = getAddress("kernel32.dll", "LoadLibraryA")
            CreateRemoteThread(handle, Nothing, Nothing, loadlib, alloc, Nothing, Nothing)
        End Sub

    End Class

End Namespace