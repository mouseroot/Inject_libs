using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace InjectionLibrary
{

    class Injection
    {
        public const long PROCESS_ALL_ACCESS = 0x001F0FFF;
        public const long MEM_CREATE = 0x00001000 | 0x00002000;
        public const long MEM_COMMIT = 0x1000;
        public const long PAGE_RW = 0x4;
        public const long PAGE_EX = 0x40;

        //FindWindow
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //VirtualAllocEx
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "VirtualAllocEx")]
        public static extern IntPtr VirtualAllocEx(IntPtr handle, IntPtr addr, int size, int type, int protocal);

        //GetWindowThreadProcessId
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowThreadProcessId")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hwnd,out int id);

        //OpenProcess
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "OpenProcess")]
        public static extern IntPtr OpenProcess(int access, bool inherit, int id);

        //GetModuleHandle
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetModuleHandle")]
        public static extern IntPtr GetModuleHandle(string name);

        //GetProcAddress
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetProcAddress")]
        public static extern IntPtr GetProcAddress(IntPtr m, string name);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        //CreateRemoteThread
        [DllImport("kernel32.dll",SetLastError = true, EntryPoint = "CreateRemoteThread")]
        public static extern IntPtr CreateRemoteThread(IntPtr proc,IntPtr attributes,int size,IntPtr address,long flags,int id);

        //


        private IntPtr handle;
        public int pid;

        public Injection() { }

        public Injection(string processName)
        {

        }

        public Injection(int pid)
        {

        }

        //Get Process by window classname or caption/title
        public IntPtr getProcess(string classname, string title)
        {
            IntPtr windowHandle = FindWindow(classname, title);
            GetWindowThreadProcessId(windowHandle,out this.pid);
            this.handle = OpenProcess((int)PROCESS_ALL_ACCESS, false, this.pid);
            return this.handle;
        }

        public IntPtr allocateRemoteString(IntPtr handle, string name)
        {
            IntPtr alloc = VirtualAllocEx(handle, IntPtr.Zero, name.Length,(int) MEM_COMMIT,(int) PAGE_RW);
            UTF8Encoding encoding = new UTF8Encoding();
            IntPtr bytesout;
            //WriteProcessMemory(handle, alloc, encoding.GetBytes(name), name.Length,0);
            WriteProcessMemory(handle, (IntPtr)alloc, encoding.GetBytes(name), (UIntPtr)name.Length, out bytesout);
            return alloc;
        }

        public static IntPtr getAddress(string module, string functionName)
        {
            IntPtr mod = GetModuleHandle(module);
            IntPtr func = GetProcAddress(mod, functionName);
            return func;
        }

        public void attachTest()
        {
            IntPtr sonicDX = getProcess(null, "Sonic Adventure DX");
            IntPtr dllName = allocateRemoteString(sonicDX, "InjectME.dll");

            IntPtr LoadLibraryA = getAddress("kernel32.dll", "LoadLibraryA");
            IntPtr thread = CreateRemoteThread(sonicDX,(IntPtr) null,0, LoadLibraryA,(long)dllName,0);
        }



    }
}
