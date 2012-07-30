Injection Libs
==============

## About
This is a small collection of libraries to call win32 functions
like ReadProcessMemory and WriteProcessMemory and everything you
need to write trainer like programs for the following languages

+ __vb.net__
+ __python w/ ctypes__

## Useage
methods are exposed via the Injector class for all languages
to simply useage

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

