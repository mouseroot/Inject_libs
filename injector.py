from ctypes import *
import struct
import time

class RECT(Structure):
    _fields_ = [('left', c_long),
                ('top', c_long),
                ('right', c_long),
                ('bottom', c_long)]

class PAINTSTRUCT(Structure):
    _fields_ = [('hdc', c_int),
                ('fErase', c_int),
                ('rcPaint', RECT),
                ('fRestore', c_int),
                ('fIncUpdate', c_int),
                ('rgbReserved', c_char * 32)]

class POINT(Structure):
    _fields_ = [('x', c_long),
                ('y', c_long)]


class Injector:
	def __init__(self,classname=None,title=None,pid=None):
		self.PROC_ALL_ACCESS = 0x001F0FFF
		self.MEM_CREATE = 0x00001000 | 0x00002000
		self.PAGE_RW = 0x00000004
		self.PAGE_EX = 0x40
		self.kern32 = windll.kernel32
		self.user32 = windll.user32
		self.title = title
		self.classname = classname
		self.RECT = RECT
		self.POINT = POINT
		self.PAINTSTRUCT = PAINTSTRUCT
		self.pid = c_ulong()
		try:
			if classname or title:
				print "Finding:",classname,title
				self.handle = self.getProcess(self.classname,self.title)
		except Exception as e:
			print e
		
	def allocRemoteString(self,s):
		size = len(s)
		alloc = self.kern32.VirtualAllocEx(self.handle,None,c_int(size),self.MEM_CREATE,self.PAGE_EX)
		self.writeMemory(alloc,s)
		return alloc
	
	def getProcess(self,classname=None,title=None):
		hwnd = windll.user32.FindWindowA(c_char_p(classname),c_char_p(title))
		self.user32.GetWindowThreadProcessId(hwnd,byref(self.pid))
		handle = self.kern32.OpenProcess(self.PROC_ALL_ACCESS,0,self.pid)
		return handle
		
	def getAddress(self,module,func):
		mod = self.kern32.GetModuleHandleA(module)
		addr = self.kern32.GetProcAddress(mod,func)
		return addr
		
	def allocString(self,size):
		return create_string_buffer(size)
		
		
	def readMemory(self,addr,size):
		buf = self.allocString(size)
		self.kern32.ReadProcessMemory(self.handle,c_long(addr),buf,size,None)
		return buf
		
	def writeMemory(self,addr,s):
		sz = len(s)
		self.kern32.WriteProcessMemory(self.handle,addr,s,sz,None)
		
	def readInt(self,buffer):
		return ord(struct.unpack("<c",buffer[0])[0])
		
	def readString(self,buffer):
		o = ""
		for b in buffer:
			o += struct.unpack("<c",b)[0]
		return o
		
	def injectDll(self,name):
		dll = self.allocRemoteString(name)
		loadlib = self.getAddress("kernel32.dll","LoadLibraryA")
		thread = self.kern32.CreateRemoteThread(self.handle,None,None,c_long(loadlib),c_long(dll),None,None)
		return thread

	
