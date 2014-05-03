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
        public const int NOP = 0x90;


        #region Win32 API Calls

        /// <summary>
        /// Win32 API FindWindow - Returns a handle based on the window caption or the window`s registered classname
        /// </summary>
        /// <param name="lpClassName">The classname of the window</param>
        /// <param name="lpWindowName">The Caption/Title of the window</param>
        /// <returns>Returns the Handle to the window</returns>
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Win32 API VirtualAllocEx - Allocates memory in another process
        /// </summary>
        /// <param name="handle">The handle to the process returned by OpenProcess</param>
        /// <param name="inheritedProtection">The protection struct to inherit from (use IntPtr.Zero)</param>
        /// <param name="size">The size to allocate</param>
        /// <param name="type">The type of allocation (PAGE_RW | PAGE_EX)</param>
        /// <param name="protocal">Unused (use 0)</param>
        /// <returns>Returns a handle to the address the memory is allocated</returns>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "VirtualAllocEx")]
        public static extern IntPtr VirtualAllocEx(IntPtr handle, IntPtr interitedProtection, int size, int type, int protocal);

        //GetWindowThreadProcessId
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowThreadProcessId")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hwnd,out int id);

        /// <summary>
        /// Win32 API OpenProcess - Opens the specified process so memory can be read/write/allocated to
        /// </summary>
        /// <param name="access">Process Access Rights (PROCESS_ALL_ACCESS)</param>
        /// <param name="inherit">whether to inherit the security of process that called this function</param>
        /// <param name="id">The process id to open</param>
        /// <returns>Returns the handle to the opened process</returns>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "OpenProcess")]
        public static extern IntPtr OpenProcess(int access, bool inherit, int id);

        /// <summary>
        /// Win32 API GetModuleHandle - Returns the handle the specified module
        /// </summary>
        /// <param name="name">The module name, (eg: kernel32.dll,user32.dll,etc)</param>
        /// <returns>Returns a handle to the module</returns>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetModuleHandle")]
        public static extern IntPtr GetModuleHandle(string name);

        /// <summary>
        /// Win32 API GetProcAddress - Returns the address of the function from the specified module
        /// </summary>
        /// <param name="module">The module that holds the function</param>
        /// <param name="name">The function name to find</param>
        /// <returns>Returns a handle to the functions address</returns>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetProcAddress")]
        public static extern IntPtr GetProcAddress(IntPtr module, string name);

        /// <summary>
        /// Win32 API WritePRocessMemory - Writes the byte[] array to the address in memory
        /// </summary>
        /// <param name="hProcess">The process to write to, returned by OpenProcess</param>
        /// <param name="lpBaseAddress">The address to write the byte[] array to</param>
        /// <param name="lpBuffer">The byte[] array to write</param>
        /// <param name="nSize">the size of the array to write</param>
        /// <param name="lpNumberOfBytesWritten">Returns the number of bytes actually written</param>
        /// <returns>Returns true if data was written, false otherwise</returns>
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        /// <summary>
        /// Win32 API ReadProcessMemory - Reads the byte[] array from the address in memory
        /// </summary>
        /// <param name="hProcess">The process to read from, returned by OpenProcess</param>
        /// <param name="lpBaseAddress">The address to read the byte[] array from</param>
        /// <param name="lpBuffer">the byte[] array to read</param>
        /// <param name="nSize">the size of the array to read</param>
        /// <param name="lpNumberOfBytesRead">Returns the number of bytes actually read</param>
        /// <returns>Returns true if data was read, false otherwise</returns>
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// Win32 API CreateRemoteThread - Creates a thread in the remote process
        /// </summary>
        /// <param name="proc">The process to create the remote thread in</param>
        /// <param name="attributes">The security structure to use when createding the thread</param>
        /// <param name="size">Unknown size (use 0)</param>
        /// <param name="address">The address of the remote thread</param>
        /// <param name="flags">Data to pass to the remote thread</param>
        /// <param name="id">Unknown id (use 0)</param>
        /// <returns>Returns the handle the remote thread if succsesful</returns>
        [DllImport("kernel32.dll",SetLastError = true, EntryPoint = "CreateRemoteThread")]
        public static extern IntPtr CreateRemoteThread(IntPtr proc,IntPtr attributes,int size,IntPtr address,long flags,int id);
        #endregion

        #region Get Process Methods
        /// <summary>
        /// Finds the window by classname and/or title and returns the handle by OpenProcess
        /// </summary>
        /// <param name="classname">The window classname</param>
        /// <param name="title">The window title/caption</param>
        /// <returns></returns>
        public static IntPtr getProcess(string classname, string title)
        {
            int pid;
            IntPtr handle;
            IntPtr windowHandle = FindWindow(classname, title);
            GetWindowThreadProcessId(windowHandle,out pid);
            handle = OpenProcess((int)PROCESS_ALL_ACCESS, false, pid);
            return handle;
        }

        /// <summary>
        /// Find the window by caption/title only and returns the handle by OpenProcess
        /// </summary>
        /// <param name="title">The window title/caption</param>
        /// <returns></returns>
        public static IntPtr getProcessByTitle(string title)
        {
            int pid;
            IntPtr handle = IntPtr.Zero;
            IntPtr windowHandle = FindWindow(null, title);
            GetWindowThreadProcessId(windowHandle, out pid);
            handle = OpenProcess((int)PROCESS_ALL_ACCESS, false, pid);
            return handle;
        }

        /// <summary>
        /// Find the window by classname and returns the handle by OpenProcess
        /// </summary>
        /// <param name="className">The window classname</param>
        /// <returns></returns>
        public static IntPtr getProcessByClassName(string className)
        {
            int pid;
            IntPtr handle = IntPtr.Zero;
            IntPtr windowHandle = FindWindow(className, null);
            GetWindowThreadProcessId(windowHandle, out pid);
            handle = OpenProcess((int)PROCESS_ALL_ACCESS, false, pid);
            return handle;
        }
        #endregion

        #region Allocate Memory Methods
        /// <summary>
        /// Allocates memory in the specified process
        /// </summary>
        /// <param name="procHandle">The handle to the process returned by OpenProcess</param>
        /// <param name="size">The size of the memory to allocate</param>
        /// <returns></returns>
        public static IntPtr allocMemory(IntPtr procHandle, int size)
        {
            IntPtr allocMem = VirtualAllocEx(procHandle, IntPtr.Zero, size, (int)MEM_COMMIT, (int)PAGE_RW);
            return allocMem;
        }
        /// <summary>
        /// Allocates a string in memory of the specified process
        /// </summary>
        /// <param name="handle">The handle to the process returned by OpenProcess</param>
        /// <param name="name">The string to write</param>
        /// <returns></returns>
        public static IntPtr allocateRemoteString(IntPtr handle, string name)
        {
            IntPtr alloc = VirtualAllocEx(handle, IntPtr.Zero, name.Length,(int) MEM_COMMIT,(int) PAGE_RW);
            UTF8Encoding encoding = new UTF8Encoding();
            IntPtr bytesout;
            WriteProcessMemory(handle, (IntPtr)alloc, encoding.GetBytes(name), (UIntPtr)name.Length, out bytesout);
            return alloc;
        }
        #endregion

        #region Write Memory Methods

        /// <summary>
        /// Writes a Int to the specified address in memory
        /// </summary>
        /// <param name="procHandle">Handle to the process returned by OpenProcess</param>
        /// <param name="address">Address to write the Int</param>
        /// <param name="data">The Int to write</param>
        /// <returns>Returns true if data was written, false otherwise</returns>
        public static bool writeInt(IntPtr procHandle, IntPtr address, int data)
        {
            IntPtr bytesout;
            WriteProcessMemory(procHandle, (IntPtr)address,BitConverter.GetBytes(data), (UIntPtr)1, out bytesout);
            if (bytesout != IntPtr.Zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes a String to the specifed address in memory
        /// </summary>
        /// <param name="procHandle">Handle to the process returned by OpenProcess</param>
        /// <param name="address">Address to write the Int</param>
        /// <param name="data">The String to write</param>
        /// <returns>Returns true if data was written, false otherwise</returns>
        public static bool writeString(IntPtr procHandle, IntPtr address, string data)
        {
            IntPtr bytesout;
            WriteProcessMemory(procHandle, (IntPtr)address, ASCIIEncoding.ASCII.GetBytes(data), (UIntPtr)data.Length, out bytesout);
            if (bytesout != IntPtr.Zero)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        public static Process[] getProcesses()
        {
            return Process.GetProcesses();
        }

        public static Process getProcessByName(string name)
        {
            return Process.GetProcessesByName(name)[0];
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
            //IntPtr thread = CreateRemoteThread(sonicDX,(IntPtr) null,0, LoadLibraryA,(long)dllName,0);
        }

        public static IntPtr injectDLL(string titleName,string dllName)
        {
            IntPtr winHandle = getProcessByTitle(titleName);
            IntPtr dllNameHandle = allocateRemoteString(winHandle,dllName);
            return CreateRemoteThread(winHandle,(IntPtr)null,0,getAddress("kernel32.dll","LoadLibraryA"),(long)dllNameHandle,0);
        }

        //"255" -> 0xFF
        public IntPtr convertDecValue(string val)
        {
            int intVal = int.Parse(val);
            string fix = String.Format("{0:X8}", intVal);
            return new IntPtr(Convert.ToInt32(fix, 16));
        }

        public string convertHexValue(string val)
        {
            int intVal = int.Parse(val);
            return String.Format("{0:X8}", intVal);
        }


    }
}
