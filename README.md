Injection Libs
==============

### Incomplete.

## About
This is a small collection of libraries to call win32 functions
like ReadProcessMemory and WriteProcessMemory and everything you
need to write trainer like programs for the following languages

+ __vb.net__
+ __python w/ ctypes__

## Useage
methods are exposed via the Injector class for all languages
to simply useage
* [Extended Documentation](https://github.com/mouseroot/Inject_libs/wiki)

_vb.net_
```vb.net
Imports Namespace.Injector
Imports System.IO

Public Sub hack()
  Dim injector as New Injector()
  Dim handle as IntPtr = injector.getProcess(Nothing,"Command Prompt")
  Console.WriteLine("Read address: " & injector.readInt(handle,"0x4D709E0"))
End Sub
```
_python_
```python
from injector import *

def main():
	i = Injector(None,"Command Prompt")
	number = i.readMemory(c_ulong(0x4D709E0),1)
	print number
if __name__ == "__main__":
	main()
```

## Dll Functions
The following win32 functions are implimented.

+ FindWindow
+ VirtualAllocEx
+ GetWindowThreadProcessId
+ OpenProcess
+ GetModuleHandle
+ GetProcAddress
+ CreateFile
+ ReadProcessMemory
+ WriteProcessMemory
+ CreateRemoteThread

## vb.net class functions

+ getProcess
+ allocRemoteString
+ getAddress
+ readInt
+ writeInt

## python class functions

+ allocRemoteString
+ getProcess
+ getAddress
+ allocString
+ readMemory
+ writeMemory
+ readInt
+ readString
+ injectDLL

