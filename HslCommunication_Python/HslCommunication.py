'''
                   GNU LESSER GENERAL PUBLIC LICENSE
                       Version 3, 29 June 2007

 Copyright (C) 2017 - 2018 Richard.Hu <http://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.


  This version of the GNU Lesser General Public License incorporates
the terms and conditions of version 3 of the GNU General Public
License, supplemented by the additional permissions listed below.

  0. Additional Definitions.

  As used herein, "this License" refers to version 3 of the GNU Lesser
General Public License, and the "GNU GPL" refers to version 3 of the GNU
General Public License.

  "The Library" refers to a covered work governed by this License,
other than an Application or a Combined Work as defined below.

  An "Application" is any work that makes use of an interface provided
by the Library, but which is not otherwise based on the Library.
Defining a subclass of a class defined by the Library is deemed a mode
of using an interface provided by the Library.

  A "Combined Work" is a work produced by combining or linking an
Application with the Library.  The particular version of the Library
with which the Combined Work was made is also called the "Linked
Version".

  The "Minimal Corresponding Source" for a Combined Work means the
Corresponding Source for the Combined Work, excluding any source code
for portions of the Combined Work that, considered in isolation, are
based on the Application, and not on the Linked Version.

  The "Corresponding Application Code" for a Combined Work means the
object code and/or source code for the Application, including any data
and utility programs needed for reproducing the Combined Work from the
Application, but excluding the System Libraries of the Combined Work.

  1. Exception to Section 3 of the GNU GPL.

  You may convey a covered work under sections 3 and 4 of this License
without being bound by section 3 of the GNU GPL.

  2. Conveying Modified Versions.

  If you modify a copy of the Library, and, in your modifications, a
facility refers to a function or data to be supplied by an Application
that uses the facility (other than as an argument passed when the
facility is invoked), then you may convey a copy of the modified
version:

   a) under this License, provided that you make a good faith effort to
   ensure that, in the event an Application does not supply the
   function or data, the facility still operates, and performs
   whatever part of its purpose remains meaningful, or

   b) under the GNU GPL, with none of the additional permissions of
   this License applicable to that copy.

  3. Object Code Incorporating Material from Library Header Files.

  The object code form of an Application may incorporate material from
a header file that is part of the Library.  You may convey such object
code under terms of your choice, provided that, if the incorporated
material is not limited to numerical parameters, data structure
layouts and accessors, or small macros, inline functions and templates
(ten or fewer lines in length), you do both of the following:

   a) Give prominent notice with each copy of the object code that the
   Library is used in it and that the Library and its use are
   covered by this License.

   b) Accompany the object code with a copy of the GNU GPL and this license
   document.

  4. Combined Works.

  You may convey a Combined Work under terms of your choice that,
taken together, effectively do not restrict modification of the
portions of the Library contained in the Combined Work and reverse
engineering for debugging such modifications, if you also do each of
the following:

   a) Give prominent notice with each copy of the Combined Work that
   the Library is used in it and that the Library and its use are
   covered by this License.

   b) Accompany the Combined Work with a copy of the GNU GPL and this license
   document.

   c) For a Combined Work that displays copyright notices during
   execution, include the copyright notice for the Library among
   these notices, as well as a reference directing the user to the
   copies of the GNU GPL and this license document.

   d) Do one of the following:

       0) Convey the Minimal Corresponding Source under the terms of this
       License, and the Corresponding Application Code in a form
       suitable for, and under terms that permit, the user to
       recombine or relink the Application with a modified version of
       the Linked Version to produce a modified Combined Work, in the
       manner specified by section 6 of the GNU GPL for conveying
       Corresponding Source.

       1) Use a suitable shared library mechanism for linking with the
       Library.  A suitable mechanism is one that (a) uses at run time
       a copy of the Library already present on the user's computer
       system, and (b) will operate properly with a modified version
       of the Library that is interface-compatible with the Linked
       Version.

   e) Provide Installation Information, but only if you would otherwise
   be required to provide such information under section 6 of the
   GNU GPL, and only to the extent that such information is
   necessary to install and execute a modified version of the
   Combined Work produced by recombining or relinking the
   Application with a modified version of the Linked Version. (If
   you use option 4d0, the Installation Information must accompany
   the Minimal Corresponding Source and Corresponding Application
   Code. If you use option 4d1, you must provide the Installation
   Information in the manner specified by section 6 of the GNU GPL
   for conveying Corresponding Source.)

  5. Combined Libraries.

  You may place library facilities that are a work based on the
Library side by side in a single library together with other library
facilities that are not Applications and are not covered by this
License, and convey such a combined library under terms of your
choice, if you do both of the following:

   a) Accompany the combined library with a copy of the same work based
   on the Library, uncombined with any other library facilities,
   conveyed under the terms of this License.

   b) Give prominent notice with the combined library that part of it
   is a work based on the Library, and explaining where to find the
   accompanying uncombined form of the same work.

  6. Revised Versions of the GNU Lesser General Public License.

  The Free Software Foundation may publish revised and/or new versions
of the GNU Lesser General Public License from time to time. Such new
versions will be similar in spirit to the present version, but may
differ in detail to address new problems or concerns.

  Each version is given a distinguishing version number. If the
Library as you received it specifies that a certain numbered version
of the GNU Lesser General Public License "or any later version"
applies to it, you have the option of following the terms and
conditions either of that published version or of any later version
published by the Free Software Foundation. If the Library as you
received it does not specify a version number of the GNU Lesser
General Public License, you may choose any version of the GNU Lesser
General Public License ever published by the Free Software Foundation.

  If the Library as you received it specifies that a proxy can decide
whether future versions of the GNU Lesser General Public License shall
apply, that proxy's public statement of acceptance of any version is
permanent authorization for you to choose that version for the
Library.

'''
import string
import uuid
import socket
import struct
import threading
import gzip
import datetime
import random
from time import sleep
from enum import Enum

class StringResources:
	'''系统的资源类'''
	@staticmethod
	def ConnectedFailed():
		return "连接失败"
	@staticmethod
	def UnknownError():
		return "未知错误"
	@staticmethod
	def ErrorCode():
		return "错误代号"
	@staticmethod
	def TextDescription():
		return "文本描述"
	@staticmethod
	def ExceptionMessage():
		return "错误信息："
	@staticmethod
	def ExceptionStackTrace():
		return "错误堆栈："
	@staticmethod
	def ExceptopnTargetSite():
		return "错误方法："
	@staticmethod
	def ExceprionCustomer():
		return "用户自定义方法出错："
	@staticmethod
	def TokenCheckFailed():
		return "令牌检查错误。"
	@staticmethod
	def SuccessText():
		return "Success"
	@staticmethod
	def NotSupportedDataType():
		return "输入的类型不支持，请重新输入"
	# Modbus相关
	@staticmethod
	def ModbusTcpFunctionCodeNotSupport():
		return "不支持的功能码"
	@staticmethod
	def ModbusTcpFunctionCodeOverBound():
		return "读取的数据越界"
	@staticmethod
	def ModbusTcpFunctionCodeQuantityOver():
		return "读取长度超过最大值"
	@staticmethod
	def ModbusTcpFunctionCodeReadWriteException():
		return "读写异常"
	@staticmethod
	def ModbusTcpReadCoilException():
		return "读取线圈异常"
	@staticmethod
	def ModbusTcpWriteCoilException():
		return "写入线圈异常"
	@staticmethod
	def ModbusTcpReadRegisterException():
		return "读取寄存器异常"
	@staticmethod
	def ModbusTcpWriteRegisterException():
		return "写入寄存器异常"
	@staticmethod
	def ModbusAddressMustMoreThanOne():
		return "地址值在起始地址为1的情况下，必须大于1"
	@staticmethod
	def MelsecPleaseReferToManulDocument():
		return "请查看三菱的通讯手册来查看报警的具体信息"
	@staticmethod
	def MelsecReadBitInfo():
		return "读取位变量数组只能针对位软元件，如果读取字软元件，请调用Read方法"
	@staticmethod
	def OmronStatus0():
		return "通讯正常"
	@staticmethod
	def OmronStatus1():
		return "消息头不是FINS"
	@staticmethod
	def OmronStatus2():
		return "数据长度太长"
	@staticmethod
	def OmronStatus3():
		return "该命令不支持"
	@staticmethod
	def OmronStatus20():
		return "超过连接上限"
	@staticmethod
	def OmronStatus21():
		return "指定的节点已经处于连接中"
	@staticmethod
	def OmronStatus22():
		return "尝试去连接一个受保护的网络节点，该节点还未配置到PLC中"
	@staticmethod
	def OmronStatus23():
		return "当前客户端的网络节点超过正常范围"
	@staticmethod
	def OmronStatus24():
		return "当前客户端的网络节点已经被使用"
	@staticmethod
	def OmronStatus25():
		return "所有的网络节点已经被使用"

class OperateResult:
	'''结果对象类，可以携带额外的数据信息'''
	def __init__(self, err = 0, msg = ""):
		self.ErrorCode = err
		self.Message = msg
	# 是否成功的标志
	IsSuccess = False
	# 操作返回的错误消息
	Message = StringResources.SuccessText()
	# 错误码
	ErrorCode = 0
	# 返回显示的文本
	def ToMessageShowString( self ):
		'''获取错误代号及文本描述'''
		return StringResources.ErrorCode() + ":" + str(self.ErrorCode) + "\r\n" + StringResources.TextDescription() + ":" + self.Message
	def CopyErrorFromOther(self, result):
		'''从另一个结果类中拷贝错误信息'''
		if result != None:
			self.ErrorCode = result.ErrorCode
			self.Message = result.Message
	@staticmethod
	def CreateFailedResult( result ):
		'''创建一个失败的结果对象'''
		failed = OperateResult()
		failed.ErrorCode = result.ErrorCode
		failed.Message = result.Message
		return failed
	@staticmethod
	def CreateSuccessResult(Content1=None,Content2=None,Content3=None,Content4=None,Content5=None,Content6=None,Content7=None,Content8=None,Content9=None,Content10=None):
		'''创建一个成功的对象'''
		success = OperateResult()
		success.IsSuccess = True
		success.Message = StringResources.SuccessText()
		if(Content2 == None and Content3 == None and Content4 == None and Content5 == None and Content6 == None and Content7 == None and Content8 == None and Content9 == None and Content10 == None) :
			success.Content = Content1
		else:
			success.Content1 = Content1
			success.Content2 = Content2
			success.Content3 = Content3
			success.Content4 = Content4
			success.Content5 = Content5
			success.Content6 = Content6
			success.Content7 = Content7
			success.Content8 = Content8
			success.Content9 = Content9
			success.Content10 = Content10
		return success

class SoftIncrementCount:
	'''一个简单的不持久化的序号自增类，采用线程安全实现，并允许指定最大数字，到达后清空从指定数开始'''
	start = 0
	current = 0
	maxValue = 100000000000000000000000000
	hybirdLock = threading.Lock()
	def __init__(self, maxValue, start):
		'''实例化一个自增信息的对象，包括最大值'''
		self.maxValue = maxValue
		self.start = start
	def GetCurrentValue( self ):
		'''获取自增信息'''
		value = 0
		self.hybirdLock.acquire()
		value = self.current
		self.current = self.current + 1
		if self.current > self.maxValue:
			self.current = 0
		self.hybirdLock.release()
		return value
	

class INetMessage:
	'''数据消息的基本基类'''
	def ProtocolHeadBytesLength(self):
		'''协议头数据长度，也即是第一次接收的数据长度'''
		return 0
	def GetContentLengthByHeadBytes(self):
		'''二次接收的数据长度'''
		return 0
	def CheckHeadBytesLegal(self,toke):
		'''令牌检查是否成功'''
		return False
	def GetHeadBytesIdentity(self):
		'''获取头子节里的消息标识'''
		return 0

	HeadBytes = bytes(0)
	ContentBytes = bytes(0)
	SendBytes = bytes(0)

class S7Message (INetMessage):
	'''西门子s7协议的消息接收规则'''
	def ProtocolHeadBytesLength(self):
		'''协议头数据长度，也即是第一次接收的数据长度'''
		return 4
	def GetContentLengthByHeadBytes(self):
		'''二次接收的数据长度'''
		if self.HeadBytes != None:
			return self.HeadBytes[2]*256 + self.HeadBytes[3]-4
		else:
			return 0
	def CheckHeadBytesLegal(self,token):
		'''令牌检查是否成功'''
		if self.HeadBytes != None:
			if self.HeadBytes[0] == 0x03 and self.HeadBytes[1] == 0x00:
				return True
			else:
				return False
		else:
			return False

class MelsecQnA3EBinaryMessage(INetMessage):
	'''三菱的Qna兼容3E帧协议解析规则'''
	def ProtocolHeadBytesLength(self):
		'''协议头数据长度，也即是第一次接收的数据长度'''
		return 9
	def GetContentLengthByHeadBytes(self):
		'''二次接收的数据长度'''
		if self.HeadBytes != None:
			return self.HeadBytes[8] * 256 + self.HeadBytes[7]
		else:
			return 0
	def CheckHeadBytesLegal(self,token):
		'''令牌检查是否成功'''
		if self.HeadBytes != None:
			if self.HeadBytes[0] == 0xD0 and self.HeadBytes[1] == 0x00:
				return True
			else:
				return False
		else:
			return False
class MelsecQnA3EAsciiMessage(INetMessage):
	'''三菱的Qna兼容3E帧的ASCII协议解析规则'''
	def ProtocolHeadBytesLength(self):
		'''协议头数据长度，也即是第一次接收的数据长度'''
		return 18
	def GetContentLengthByHeadBytes(self):
		'''二次接收的数据长度'''
		if self.HeadBytes != None:
			return int(self.HeadBytes[14:18].decode('ascii'),16)
		else:
			return 0
	def CheckHeadBytesLegal(self,token):
		'''令牌检查是否成功'''
		if self.HeadBytes != None:
			if self.HeadBytes[0] == ord('D') and self.HeadBytes[1] == ord('0') and self.HeadBytes[2] == ord('0') and self.HeadBytes[3] == ord('0'):
				return True
			else:
				return False
		else:
			return False

class ModbusTcpMessage (INetMessage):
	'''Modbus-Tcp协议的信息'''
	def ProtocolHeadBytesLength(self):
		'''协议头数据长度，也即是第一次接收的数据长度'''
		return 6
	def GetContentLengthByHeadBytes(self):
		'''二次接收的数据长度'''
		if self.HeadBytes != None:
			return self.HeadBytes[4] * 256 + self.HeadBytes[5]
		else:
			return 0
	def CheckHeadBytesLegal(self,token):
		'''令牌检查是否成功'''
		return True
	def GetHeadBytesIdentity(self):
		'''获取头子节里的消息标识'''
		return self.HeadBytes[0] * 256 + self.HeadBytes[1]

class HslMessage (INetMessage):
	'''本组件系统使用的默认的消息规则，说明解析和反解析规则的'''
	def ProtocolHeadBytesLength(self):
		'''协议头数据长度，也即是第一次接收的数据长度'''
		return 32
	def GetContentLengthByHeadBytes(self):
		'''二次接收的数据长度'''
		if self.HeadBytes != None:
			buffer = bytearray(4)
			buffer[0:4] = self.HeadBytes[28:32]
			return struct.unpack('<i',buffer)[0]
		else:
			return 0
	def GetHeadBytesIdentity(self):
		'''获取头子节里的消息标识'''
		if self.HeadBytes != None:
			buffer = bytearray(4)
			buffer[0:4] = self.HeadBytes[4:8]
			return struct.unpack('<i',buffer)[0]
		else:
			return 0
	def CheckHeadBytesLegal(self,token):
		'''令牌检查是否成功'''
		if self.HeadBytes == None:
			return False
		else:
			return SoftBasic.IsTwoBytesEquel(self.HeadBytes,12,token,0,16)
class DataFormat(Enum):
	'''应用于多字节数据的解析或是生成格式'''
	ABCD = 0
	BADC = 1
	CDAB = 2
	DCBA = 3


class ByteTransform:
	'''数据转换类的基础，提供了一些基础的方法实现.'''
	DataFormat = DataFormat.DCBA

	def TransBool(self, buffer, index ):
		'''将buffer数组转化成bool对象'''
		return ((buffer[index] & 0x01) == 0x01)
	def TransBoolArray(self, buffer, index, length ):
		'''将buffer数组转化成bool数组对象，需要转入索引，长度'''
		data = bytearray(length)
		for i in range(length):
			data[i]=buffer[i+index]
		return SoftBasic.ByteToBoolArray( data, length * 8 )

	def TransByte( self, buffer, index ):
		'''将buffer中的字节转化成byte对象，需要传入索引'''
		return buffer[index]
	def TransByteArray( self, buffer, index, length ):
		'''将buffer中的字节转化成byte数组对象，需要传入索引'''
		data = bytearray(length)
		for i in range(length):
			data[i]=buffer[i+index]
		return data

	def TransInt16( self, buffer, index ):
		'''从缓存中提取short结果'''
		data = self.TransByteArray(buffer,index,2)
		return struct.unpack('<h',data)[0]
	def TransInt16Array( self, buffer, index, length ):
		'''从缓存中提取short数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransInt16( buffer, index + 2 * i ))
		return tmp

	def TransUInt16(self, buffer, index ):
		'''从缓存中提取ushort结果'''
		data = self.TransByteArray(buffer,index,2)
		return struct.unpack('<H',data)[0]
	def TransUInt16Array(self, buffer, index, length ):
		'''从缓存中提取ushort数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransUInt16( buffer, index + 2 * i ))
		return tmp
	
	def TransInt32(self, buffer, index ):
		'''从缓存中提取int结果'''
		data = self.ByteTransDataFormat4(self.TransByteArray(buffer,index,4))
		return struct.unpack('<i',data)[0]
	def TransInt32Array(self, buffer, index, length ):
		'''从缓存中提取int数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransInt32( buffer, index + 4 * i ))
		return tmp

	def TransUInt32(self, buffer, index ):
		'''从缓存中提取uint结果'''
		data = self.ByteTransDataFormat4(self.TransByteArray(buffer,index,4))
		return struct.unpack('<I',data)[0]
	def TransUInt32Array(self, buffer, index, length ):
		'''从缓存中提取uint数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransUInt32( buffer, index + 4 * i ))
		return tmp
	
	def TransInt64(self, buffer, index ):
		'''从缓存中提取long结果'''
		data = self.ByteTransDataFormat8(self.TransByteArray(buffer,index,8))
		return struct.unpack('<q',data)[0]
	def TransInt64Array(self, buffer, index, length):
		'''从缓存中提取long数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransInt64( buffer, index + 8 * i ))
		return tmp
	
	def TransUInt64(self, buffer, index ):
		'''从缓存中提取ulong结果'''
		data = self.ByteTransDataFormat8(self.TransByteArray(buffer,index,8))
		return struct.unpack('<Q',data)[0]
	def TransUInt64Array(self, buffer, index, length):
		'''从缓存中提取ulong数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransUInt64( buffer, index + 8 * i ))
		return tmp
	
	def TransSingle(self, buffer, index ):
		'''从缓存中提取float结果'''
		data = self.ByteTransDataFormat4(self.TransByteArray(buffer,index,4))
		return struct.unpack('<f',data)[0]
	def TransSingleArray(self, buffer, index, length):
		'''从缓存中提取float数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransSingle( buffer, index + 4 * i ))
		return tmp
	
	def TransDouble(self, buffer, index ):
		'''从缓存中提取double结果'''
		data = self.ByteTransDataFormat8(self.TransByteArray(buffer,index,8))
		return struct.unpack('<d',data)[0]
	def TransDoubleArray(self, buffer, index, length):
		'''从缓存中提取double数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransDouble( buffer, index + 8 * i ))
		return tmp

	def TransString( self, buffer, index, length, encoding ):
		'''从缓存中提取string结果，使用指定的编码'''
		data = self.TransByteArray(buffer,index,length)
		return data.decode(encoding)

	def BoolArrayTransByte(self, values):
		'''bool数组变量转化缓存数据，需要传入bool数组'''
		if (values == None): return None
		return SoftBasic.BoolArrayToByte( values )
	def BoolTransByte(self, value):
		'''bool变量转化缓存数据，需要传入bool值'''
		return self.BoolArrayTransByte([value])

	def ByteTransByte(self, value ):
		'''byte变量转化缓存数据，需要传入byte值'''
		buffer = bytearray(1)
		buffer[0] = value
		return buffer

	def Int16ArrayTransByte(self, values ):
		'''short数组变量转化缓存数据，需要传入short数组'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 2)
		for i in range(len(values)):
			buffer[(i*2): (i*2+2)] = struct.pack('<h',values[i])
		return buffer
	def Int16TransByte(self, value ):
		'''short数组变量转化缓存数据，需要传入short值'''
		return self.Int16ArrayTransByte([value])

	def UInt16ArrayTransByte(self, values ):
		'''ushort数组变量转化缓存数据，需要传入ushort数组'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 2)
		for i in range(len(values)):
			buffer[(i*2): (i*2+2)] = struct.pack('<H',values[i])
		return buffer
	def UInt16TransByte(self, value ):
		'''ushort变量转化缓存数据，需要传入ushort值'''
		return self.UInt16ArrayTransByte([value])

	def Int32ArrayTransByte(self, values ):
		'''int数组变量转化缓存数据，需要传入int数组'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = self.ByteTransDataFormat4(struct.pack('<i',values[i]))
		return buffer
	def Int32TransByte(self, value ):
		'''int变量转化缓存数据，需要传入int值'''
		return self.Int32ArrayTransByte([value])

	def UInt32ArrayTransByte(self, values ):
		'''uint数组变量转化缓存数据，需要传入uint数组'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = self.ByteTransDataFormat4(struct.pack('<I',values[i]))
		return buffer
	def UInt32TransByte(self, value ):
		'''uint变量转化缓存数据，需要传入uint值'''
		return self.UInt32ArrayTransByte([value])

	def Int64ArrayTransByte(self, values ):
		'''long数组变量转化缓存数据，需要传入long数组'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = self.ByteTransDataFormat8(struct.pack('<q',values[i]))
		return buffer
	def Int64TransByte(self, value ):
		'''long变量转化缓存数据，需要传入long值'''
		return self.Int64ArrayTransByte([value])

	def UInt64ArrayTransByte(self, values ):
		'''ulong数组变量转化缓存数据，需要传入ulong数组'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = self.ByteTransDataFormat8(struct.pack('<Q',values[i]))
		return buffer
	def UInt64TransByte(self, value ):
		'''ulong变量转化缓存数据，需要传入ulong值'''
		return self.UInt64ArrayTransByte([value])

	def FloatArrayTransByte(self, values ):
		'''float数组变量转化缓存数据，需要传入float数组'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = self.ByteTransDataFormat4(struct.pack('<f',values[i]))
		return buffer
	def FloatTransByte(self, value ):
		'''float变量转化缓存数据，需要传入float值'''
		return self.FloatArrayTransByte([value])

	def DoubleArrayTransByte(self, values ):
		'''double数组变量转化缓存数据，需要传入double数组'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = self.ByteTransDataFormat8(struct.pack('<d',values[i]))
		return buffer
	def DoubleTransByte(self, value ):
		'''double变量转化缓存数据，需要传入double值'''
		return self.DoubleArrayTransByte([value])

	def StringTransByte(self, value:str, encoding:str ):
		'''使用指定的编码字符串转化缓存数据，需要传入string值及编码信息'''
		return value.encode(encoding)

	def ByteTransDataFormat4(self, value, index = 0 ):
		'''反转多字节的数据信息'''
		buffer = bytearray(4)
		if self.DataFormat == DataFormat.ABCD:
			buffer[0] = value[index + 3]
			buffer[1] = value[index + 2]
			buffer[2] = value[index + 1]
			buffer[3] = value[index + 0]
		elif self.DataFormat == DataFormat.BADC:
			buffer[0] = value[index + 2]
			buffer[1] = value[index + 3]
			buffer[2] = value[index + 0]
			buffer[3] = value[index + 1]
		elif self.DataFormat == DataFormat.CDAB:
			buffer[0] = value[index + 1]
			buffer[1] = value[index + 0]
			buffer[2] = value[index + 3]
			buffer[3] = value[index + 2]
		elif self.DataFormat == DataFormat.DCBA:
			buffer[0] = value[index + 0]
			buffer[1] = value[index + 1]
			buffer[2] = value[index + 2]
			buffer[3] = value[index + 3]
		return buffer

	def ByteTransDataFormat8(self, value, index = 0 ):
		'''反转多字节的数据信息'''
		buffer = bytearray(8)
		if self.DataFormat == DataFormat.ABCD:
			buffer[0] = value[index + 7]
			buffer[1] = value[index + 6]
			buffer[2] = value[index + 5]
			buffer[3] = value[index + 4]
			buffer[4] = value[index + 3]
			buffer[5] = value[index + 2]
			buffer[6] = value[index + 1]
			buffer[7] = value[index + 0]
		elif self.DataFormat == DataFormat.BADC:
			buffer[0] = value[index + 6]
			buffer[1] = value[index + 7]
			buffer[2] = value[index + 4]
			buffer[3] = value[index + 5]
			buffer[4] = value[index + 2]
			buffer[5] = value[index + 3]
			buffer[6] = value[index + 0]
			buffer[7] = value[index + 1]
		elif self.DataFormat == DataFormat.CDAB:
			buffer[0] = value[index + 1]
			buffer[1] = value[index + 0]
			buffer[2] = value[index + 3]
			buffer[3] = value[index + 2]
			buffer[4] = value[index + 5]
			buffer[5] = value[index + 4]
			buffer[6] = value[index + 7]
			buffer[7] = value[index + 6]
		elif self.DataFormat == DataFormat.DCBA:
			buffer[0] = value[index + 0]
			buffer[1] = value[index + 1]
			buffer[2] = value[index + 2]
			buffer[3] = value[index + 3]
			buffer[4] = value[index + 4]
			buffer[5] = value[index + 5]
			buffer[6] = value[index + 6]
			buffer[7] = value[index + 7]
		return buffer

class RegularByteTransform(ByteTransform):
	'''常规的字节转换类'''
	def __init__(self):
		return

class ReverseBytesTransform(ByteTransform):
	'''字节倒序的转换类'''
	def TransInt16(self, buffer, index ):
		'''从缓存中提取short结果'''
		data = self.TransByteArray(buffer,index,2)
		return struct.unpack('>h',data)[0]
	def TransUInt16(self, buffer, index ):
		'''从缓存中提取ushort结果'''
		data = self.TransByteArray(buffer,index,2)
		return struct.unpack('>H',data)[0]
	def TransInt32(self, buffer, index ):
		'''从缓存中提取int结果'''
		data = self.TransByteArray(buffer,index,4)
		return struct.unpack('>i',data)[0]
	def TransUInt32(self, buffer, index ):
		'''从缓存中提取uint结果'''
		data = self.TransByteArray(buffer,index,4)
		return struct.unpack('>I',data)[0]
	def TransInt64(self, buffer, index ):
		'''从缓存中提取long结果'''
		data = self.TransByteArray(buffer,index,8)
		return struct.unpack('>q',data)[0]
	def TransUInt64(self, buffer, index ):
		'''从缓存中提取ulong结果'''
		data = self.TransByteArray(buffer,index,8)
		return struct.unpack('>Q',data)[0]
	def TransSingle(self, buffer, index ):
		'''从缓存中提取float结果'''
		data = self.TransByteArray(buffer,index,4)
		return struct.unpack('>f',data)[0]
	def TransDouble(self, buffer, index ):
		'''从缓存中提取double结果'''
		data = self.TransByteArray(buffer,index,8)
		return struct.unpack('>d',data)[0]
	
	def Int16ArrayTransByte(self, values ):
		'''short数组变量转化缓存数据，需要传入short数组 -> bytearray'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 2)
		for i in range(len(values)):
			buffer[(i*2): (i*2+2)] = struct.pack('>h',values[i])
		return buffer
	def UInt16ArrayTransByte(self, values ):
		'''ushort数组变量转化缓存数据，需要传入ushort数组 -> bytearray'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 2)
		for i in range(len(values)):
			buffer[(i*2): (i*2+2)] = struct.pack('>H',values[i])
		return buffer
	def Int32ArrayTransByte(self, values ):
		'''int数组变量转化缓存数据，需要传入int数组 -> bytearray'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = struct.pack('>i',values[i])
		return buffer
	def UInt32ArrayTransByte(self, values ):
		'''uint数组变量转化缓存数据，需要传入uint数组 -> bytearray'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = struct.pack('>I',values[i])
		return buffer
	def Int64ArrayTransByte(self, values ):
		'''long数组变量转化缓存数据，需要传入long数组 -> bytearray'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = struct.pack('>q',values[i])
		return buffer
	def UInt64ArrayTransByte(self, values ):
		'''ulong数组变量转化缓存数据，需要传入ulong数组 -> bytearray'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = struct.pack('>Q',values[i])
		return buffer
	def FloatArrayTransByte(self, values ):
		'''float数组变量转化缓存数据，需要传入float数组 -> bytearray'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = struct.pack('>f',values[i])
		return buffer
	def DoubleArrayTransByte(self, values ):
		'''double数组变量转化缓存数据，需要传入double数组 -> bytearray'''
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = struct.pack('>d',values[i])
		return buffer

class ReverseWordTransform(ByteTransform):
	'''按照字节错位的数据转换类'''
	def __init__(self):
		'''初始化方法，重新设置DataFormat'''
		self.DataFormat = DataFormat.ABCD
	
	IsStringReverse = False

	def ReverseBytesByWord( self, buffer, index, length ):
		'''按照字节错位的方法 -> bytearray'''
		if buffer == None: return None
		data = self.TransByteArray(buffer,index,length)
		for i in range(len(data)//2):
			data[i*2+0],data[i*2+1]= data[i*2+1],data[i*2+0]
		return data
	def ReverseAllBytesByWord( self, buffer ):
		'''按照字节错位的方法 -> bytearray'''
		return self.ReverseBytesByWord(buffer,0,len(buffer))
	def TransInt16( self, buffer, index ):
		'''从缓存中提取short结果'''
		data = self.ReverseBytesByWord(buffer,index,2)
		return struct.unpack('<h',data)[0]
	def TransUInt16(self, buffer, index ):
		'''从缓存中提取ushort结果'''
		data = self.ReverseBytesByWord(buffer,index,2)
		return struct.unpack('<H',data)[0]
	def TransString( self, buffer, index, length, encoding ):
		'''从缓存中提取string结果，使用指定的编码'''
		data = self.TransByteArray(buffer,index,length)
		if self.IsStringReverse:
			return self.ReverseAllBytesByWord(data).decode(encoding)
		else:
			return data.decode(encoding)
	
	def Int16ArrayTransByte(self, values ):
		'''short数组变量转化缓存数据，需要传入short数组'''
		buffer = super().Int16ArrayTransByte(values)
		return self.ReverseAllBytesByWord(buffer)
	def UInt16ArrayTransByte(self, values ):
		'''ushort数组变量转化缓存数据，需要传入ushort数组'''
		buffer = super().UInt16ArrayTransByte(values)
		return self.ReverseAllBytesByWord(buffer)
	def StringTransByte(self, value, encoding ):
		'''使用指定的编码字符串转化缓存数据，需要传入string值及编码信息'''
		buffer = value.encode(encoding)
		buffer = SoftBasic.BytesArrayExpandToLengthEven(buffer)
		if self.IsStringReverse:
			return self.ReverseAllBytesByWord( buffer )
		else:
			return buffer

class ByteTransformHelper:
	'''所有数据转换类的静态辅助方法'''
	@staticmethod
	def GetBoolResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransBool(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetByteResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransByte(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetInt16ResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransInt16(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetUInt16ResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransUInt16(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetInt32ResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransInt32(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetUInt32ResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransUInt32(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetInt64ResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransInt64(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetUInt64ResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransUInt64(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetSingleResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransSingle(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetDoubleResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransDouble(result.Content , 0 ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))
	@staticmethod
	def GetStringResultFromBytes( result, byteTransform ):
		'''将指定的OperateResult类型转化'''
		try:
			if result.IsSuccess:
				return OperateResult.CreateSuccessResult(byteTransform.TransString(result.Content , 0, len(result.Content), 'ascii' ))
			else:
				return OperateResult.CreateFailedResult(result)
		except Exception as ex:
			return OperateResult( msg = "数据转化失败，源数据：" + SoftBasic.ByteToHexString( result.Content ) + " 消息：" + str(ex))


class DeviceAddressBase:
	'''所有设备通信类的地址基础类'''
	Address = 0
	def AnalysisAddress( self, address ):
		'''解析字符串的地址'''
		self.Address = int(address)


class SoftBasic:
	'''系统运行的基础方法，提供了一些基本的辅助方法'''
	@staticmethod
	def GetSizeDescription(size):
		'''获取指定数据大小的文本描述字符串'''
		if size < 1000:
			return str(size) + " B"
		elif size < (1000 * 1000):
			data = float(size) / 1024
			return '{:.2f}'.format(data) + " Kb"
		elif size < (1000 * 1000 * 1000):
			data = float(size) / 1024 / 1024
			return '{:.2f}'.format(data) + " Mb"
		else:
			data = float(size) / 1024 / 1024 / 1024
			return '{:.2f}'.format(data) + " Gb"
	@staticmethod
	def ByteToHexString(inBytes,segment=' '):
		'''将字节数组转换成十六进制的表示形式，需要传入2个参数，数据和分隔符，该方法还存在一点问题'''
		str_list = []
		for byte in inBytes:
			str_list.append('{:02X}'.format(byte))
		if segment != None: 
			return segment.join(str_list)
		else:
			return ''.join(str_list)
	@staticmethod
	def ByteToBoolArray( InBytes, length ):
		'''从字节数组中提取bool数组变量信息'''
		if InBytes == None:
			return None
		if length > len(InBytes) * 8:
			length = len(InBytes) * 8
		buffer = []
		for  i in range(length):
			index = i // 8
			offect = i % 8

			temp = 0
			if offect == 0 : temp = 0x01
			elif offect == 1 : temp = 0x02
			elif offect == 2 : temp = 0x04
			elif offect == 3 : temp = 0x08
			elif offect == 4 : temp = 0x10
			elif offect == 5 : temp = 0x20
			elif offect == 6 : temp = 0x40
			elif offect == 7 : temp = 0x80

			if (InBytes[index] & temp) == temp:
				buffer.append(True)
			else:
				buffer.append(False)
		return buffer
	@staticmethod
	def BoolArrayToByte( array ):
		'''从bool数组变量变成byte数组'''
		if (array == None) : return None

		length = 0
		if len(array) % 8 == 0:
			length = int(len(array) / 8)
		else:
			length = int(len(array) / 8) + 1
		buffer = bytearray(length)

		for i in range(len(array)):
			index = i // 8
			offect = i % 8

			temp = 0
			if offect == 0 : temp = 0x01
			elif offect == 1 : temp = 0x02
			elif offect == 2 : temp = 0x04
			elif offect == 3 : temp = 0x08
			elif offect == 4 : temp = 0x10
			elif offect == 5 : temp = 0x20
			elif offect == 6 : temp = 0x40
			elif offect == 7 : temp = 0x80

			if array[i] : buffer[index] += temp
		return buffer
	@staticmethod
	def HexStringToBytes( hex ):
		'''将hex字符串转化为byte数组'''
		return bytes.fromhex(hex)
	@staticmethod
	def BytesArrayExpandToLengthEven(array):
		'''扩充一个整型的数据长度为偶数个'''
		if len(array) % 2 == 1:
			array.append(0)
		return array
	@staticmethod
	def IsTwoBytesEquel( b1, start1, b2, start2, length ):
		'''判断两个字节的指定部分是否相同'''
		if b1 == None or b2 == None: return False
		for ii in range(length):
			if b1[ii+start1] != b2[ii+start2]: return False
		return True
	@staticmethod
	def TokenToBytes( token ):
		'''将uuid的token值转化成统一的bytes数组，方便和java，C#通讯'''
		buffer = bytearray(token.bytes)
		buffer[0],buffer[1],buffer[2],buffer[3] = buffer[3],buffer[2],buffer[1],buffer[0]
		buffer[4],buffer[5] = buffer[5],buffer[4]
		buffer[6],buffer[7] = buffer[7],buffer[6]
		return buffer
	@staticmethod
	def ArrayExpandToLength( value, length ):
		'''将数组扩充到指定的长度'''
		buffer = bytearray(length)
		if len(value) >= length:
			buffer[0:] = value[0:len(value)]
		else:
			buffer[0:len(value)] = value
		return buffer
	@staticmethod
	def ArrayExpandToLengthEven( value ):
		'''将数组扩充到偶数的长度'''
		if len(value) % 2 == 0:
			return value
		else:
			buffer = bytearray(len(value)+1)
			buffer[0:len(value)] = value
			return value
	@staticmethod
	def StringToUnicodeBytes( value ):
		'''获取字符串的unicode编码字符'''
		if value == None: return bytearray(0)

		buffer = value.encode('utf-16')
		if len(buffer) > 1 and buffer[0] == 255 and buffer[1] == 254:
			buffer = buffer[2:len(buffer)]
		return buffer
	@staticmethod
	def GetUniqueStringByGuidAndRandom():
		'''获取一串唯一的随机字符串，长度为20，由Guid码和4位数的随机数组成，保证字符串的唯一性'''
		return SoftBasic.ByteToHexString(SoftBasic.TokenToBytes(uuid.uuid1()), None) + str(random.randint(12, 20))

class HslSecurity:
	@staticmethod
	def ByteEncrypt( enBytes ):
		'''加密方法，只对当前的程序集开放'''
		if (enBytes == None) : return None
		result = bytearray(len(enBytes))
		for i in range(len(enBytes)):
			result[i] = enBytes[i] ^ 0xB5
		return result
	@staticmethod
	def ByteDecrypt( deBytes ):
		'''解密方法，只对当前的程序集开放'''
		return HslSecurity.ByteEncrypt(deBytes)

class SoftZipped:
	'''一个负责压缩解压数据字节的类'''
	@staticmethod
	def CompressBytes( inBytes ):
		'''压缩字节数据'''
		if inBytes == None : return None
		return gzip.compress( inBytes )
	@staticmethod
	def Decompress( inBytes ):
		'''解压字节数据'''
		if inBytes == None : return None
		return gzip.decompress( inBytes )

class HslProtocol:
	'''用于本程序集访问通信的暗号说明'''
	@staticmethod
	def HeadByteLength():
		'''规定所有的网络传输指令头都为32字节'''
		return 32
	@staticmethod
	def ProtocolBufferSize():
		'''所有网络通信中的缓冲池数据信息'''
		return 1024
	@staticmethod
	def ProtocolCheckSecends():
		'''用于心跳程序的暗号信息'''
		return 1
	@staticmethod
	def ProtocolClientQuit():
		'''客户端退出消息'''
		return 2
	@staticmethod
	def ProtocolClientRefuseLogin():
		'''因为客户端达到上限而拒绝登录'''
		return 3
	@staticmethod
	def ProtocolClientAllowLogin():
		return 4
	@staticmethod
	def ProtocolUserString():
		'''说明发送的只是文本信息'''
		return 1001
	@staticmethod
	def ProtocolUserBytes():
		'''发送的数据就是普通的字节数组'''
		return 1002
	@staticmethod
	def ProtocolUserBitmap():
		'''发送的数据就是普通的图片数据'''
		return 1003
	@staticmethod
	def ProtocolUserException():
		'''发送的数据是一条异常的数据，字符串为异常消息'''
		return 1004
	@staticmethod
	def ProtocolFileDownload():
		'''请求文件下载的暗号'''
		return 2001
	@staticmethod
	def ProtocolFileUpload():
		'''请求文件上传的暗号'''
		return 2002
	@staticmethod
	def ProtocolFileDelete():
		'''请求删除文件的暗号'''
		return 2003
	@staticmethod
	def ProtocolFileCheckRight():
		'''文件校验成功'''
		return 2004
	@staticmethod
	def ProtocolFileCheckError():
		'''文件校验失败'''
		return 2005
	@staticmethod
	def ProtocolFileSaveError():
		'''文件保存失败'''
		return 2006
	@staticmethod
	def ProtocolFileDirectoryFiles():
		'''请求文件列表的暗号'''
		return 2007
	@staticmethod
	def ProtocolFileDirectories():
		'''请求子文件的列表暗号'''
		return 2008
	@staticmethod
	def ProtocolProgressReport():
		'''进度返回暗号'''
		return 2009
	@staticmethod
	def ProtocolNoZipped():
		'''不压缩数据字节'''
		return 3001
	@staticmethod
	def ProtocolZipped():
		'''压缩数据字节'''
		return 3002
	@staticmethod
	def CommandBytesBase( command, customer, token, data ):
		'''生成终极传送指令的方法，所有的数据均通过该方法出来'''
		_zipped = HslProtocol.ProtocolNoZipped()
		buffer = None
		_sendLength = 0
		if data == None:
			buffer = bytearray(HslProtocol.HeadByteLength())
		else:
			data = HslSecurity.ByteEncrypt( data )
			if len(data) > 102400:
				data = SoftZipped.CompressBytes( data )
				_zipped = HslProtocol.ProtocolZipped()
			buffer = bytearray( HslProtocol.HeadByteLength() + len(data) )
			_sendLength = len(data)
		
		buffer[0:4] = struct.pack( '<i', command )
		buffer[4:8] = struct.pack( '<i', customer )
		buffer[8:12] = struct.pack( '<i', _zipped)
		buffer[12:28] = SoftBasic.TokenToBytes(token)
		buffer[28:32] = struct.pack( '<i', _sendLength)
		if _sendLength>0:
			buffer[32:_sendLength+32]=data
		return buffer
	@staticmethod
	def CommandAnalysis( head, content ):
		'''解析接收到数据，先解压缩后进行解密'''
		if content != None:
			_zipped = struct.unpack('<i', head[8:12])[0]
			if _zipped == HslProtocol.ProtocolZipped():
				content = SoftZipped.Decompress( content )
			return HslSecurity.ByteEncrypt(content)
		return bytearray(0)
	@staticmethod
	def CommandBytes( customer, token, data ):
		'''获取发送字节数据的实际数据，带指令头'''
		return HslProtocol.CommandBytesBase( HslProtocol.ProtocolUserBytes(), customer, token, data )
	@staticmethod
	def CommandString( customer, token, data ):
		'''获取发送字节数据的实际数据，带指令头'''
		if data == None: 
			return HslProtocol.CommandBytesBase( HslProtocol.ProtocolUserString(), customer, token, None )
		else:
			buffer = SoftBasic.StringToUnicodeBytes(data)
			return HslProtocol.CommandBytesBase( HslProtocol.ProtocolUserString(), customer, token, buffer )



class NetworkBase:
	'''网络基础类的核心'''
	Token = uuid.UUID('{00000000-0000-0000-0000-000000000000}')
	CoreSocket = socket.socket()
	def Receive(self,socket,length):
		'''接收固定长度的字节数组'''
		totle = 0
		data = bytearray()
		try:
			while totle < length:
				data.extend(socket.recv(length-totle))
				totle += len(data)
			return OperateResult.CreateSuccessResult(data)
		except Exception as e:
			result = OperateResult()
			result.Message = str(e)
			return result
	def Send(self,socket,data):
		'''发送消息给套接字，直到完成的时候返回'''
		try:
			socket.send(data)
			return OperateResult.CreateSuccessResult()
		except Exception as e:
			return OperateResult( msg = str(e))

	def CreateSocketAndConnect(self,ipAddress,port,timeout = 10000):
		'''创建一个新的socket对象并连接到远程的地址，默认超时时间为10秒钟'''
		try:
			socketTmp = socket.socket()
			socketTmp.connect((ipAddress,port))
			return OperateResult.CreateSuccessResult(socketTmp)
		except Exception as e:
			return OperateResult( msg = str(e))
	def ReceiveMessage( self, socket, timeOut, netMsg ):
		'''接收一条完整的数据，使用异步接收完成，包含了指令头信息'''
		result = OperateResult()
		headResult = self.Receive( socket, netMsg.ProtocolHeadBytesLength() )
		if headResult.IsSuccess == False:
			result.CopyErrorFromOther(headResult)
			return result
		netMsg.HeadBytes = headResult.Content
		if netMsg.CheckHeadBytesLegal( SoftBasic.TokenToBytes(self.Token) ) == False:
			# 令牌校验失败
			if socket != None: socket.close()
			result.Message = StringResources.TokenCheckFailed()
			return result

		contentLength = netMsg.GetContentLengthByHeadBytes( )
		if contentLength == 0:
			netMsg.ContentBytes = bytearray(0)
		else:
			contentResult = self.Receive( socket, contentLength )
			if contentResult.IsSuccess == False:
				result.CopyErrorFromOther( contentResult )
				return result
			netMsg.ContentBytes = contentResult.Content
		
		if netMsg.ContentBytes == None: netMsg.ContentBytes = bytearray(0)
		result.Content = netMsg
		result.IsSuccess = True
		return result

class NetworkDoubleBase(NetworkBase):
	'''支持长连接，短连接两个模式的通用客户端基类'''
	byteTransform = ByteTransform()
	ipAddress = "127.0.0.1"
	port = 10000
	isPersistentConn = False
	isSocketError = False
	receiveTimeOut = 10000
	isUseSpecifiedSocket = False
	interactiveLock = threading.Lock()
	iNetMessage = INetMessage()
	
	def SetPersistentConnection( self ):
		'''在读取数据之前可以调用本方法将客户端设置为长连接模式，相当于跳过了ConnectServer的结果验证，对异形客户端无效'''
		self.isPersistentConn = True
	def ConnectServer( self ):
		'''切换短连接模式到长连接模式，后面的每次请求都共享一个通道'''
		self.isPersistentConn = True
		result = OperateResult( )
		# 重新连接之前，先将旧的数据进行清空
		if self.CoreSocket != None: 
			self.CoreSocket.close()

		rSocket = self.CreateSocketAndInitialication( )
		if rSocket.IsSuccess == False:
			self.isSocketError = True
			rSocket.Content = None
			result.Message = rSocket.Message
		else:
			self.CoreSocket = rSocket.Content
			result.IsSuccess = True
		return result
	def ConnectClose( self ):
		'''在长连接模式下，断开服务器的连接，并切换到短连接模式'''
		result = OperateResult( )
		self.isPersistentConn = False

		self.interactiveLock.acquire()
		# 额外操作
		result = self.ExtraOnDisconnect( self.CoreSocket )
		# 关闭信息
		if self.CoreSocket != None : self.CoreSocket.close()
		self.CoreSocket = None
		self.interactiveLock.release( )
		return result
	

	# 初始化的信息方法和连接结束的信息方法，需要在继承类里面进行重新实现
	def InitializationOnConnect( self, socket ):
		'''连接上服务器后需要进行的初始化操作'''
		return OperateResult.CreateSuccessResult()
	def ExtraOnDisconnect( self, socket ):
		'''在将要和服务器进行断开的情况下额外的操作，需要根据对应协议进行重写'''
		return OperateResult.CreateSuccessResult()
	
	def GetAvailableSocket( self ):
		'''获取本次操作的可用的网络套接字'''
		if self.isPersistentConn :
			# 如果是异形模式
			if self.isUseSpecifiedSocket :
				if self.isSocketError:
					return OperateResult( msg = '连接不可用' )
				else:
					return OperateResult.CreateSuccessResult( self.CoreSocket )
			else:
				# 长连接模式
				if self.isSocketError or self.CoreSocket == None :
					connect = self.ConnectServer( )
					if connect.IsSuccess == False:
						self.isSocketError = True
						return OperateResult( msg = connect.Message )
					else:
						self.isSocketError = False
						return OperateResult.CreateSuccessResult( self.CoreSocket )
				else:
					return OperateResult.CreateSuccessResult( self.CoreSocket )
		else:
			# 短连接模式
			return self.CreateSocketAndInitialication( )

	def CreateSocketAndInitialication( self ):
		'''连接并初始化网络套接字'''
		result = self.CreateSocketAndConnect( self.ipAddress, self.port, 10000 )
		if result.IsSuccess:
			# 初始化
			initi = self.InitializationOnConnect( result.Content )
			if initi.IsSuccess == False:
				if result.Content !=None : result.Content.close( )
				result.IsSuccess = initi.IsSuccess
				result.CopyErrorFromOther( initi )
		return result

	def ReadFromCoreSocketServer( self, socket, send ):
		'''在其他指定的套接字上，使用报文来通讯，传入需要发送的消息，返回一条完整的数据指令'''
		read = self.ReadFromCoreServerBase( socket, send )
		if read.IsSuccess == False: return OperateResult.CreateFailedResult( read )

		# 拼接结果数据
		Content = bytearray(len(read.Content1) + len(read.Content2))
		if len(read.Content1) > 0 : 
			Content[0:len(read.Content1)] = read.Content1
		if len(read.Content2) > 0 : 
			Content[len(read.Content1):len(Content)] = read.Content2
		return OperateResult.CreateSuccessResult( Content )

	def ReadFromCoreServer( self, send ):
		'''使用底层的数据报文来通讯，传入需要发送的消息，返回一条完整的数据指令'''
		result = OperateResult( )
		self.interactiveLock.acquire()
		# 获取有用的网络通道，如果没有，就建立新的连接
		resultSocket = self.GetAvailableSocket( )
		if resultSocket.IsSuccess == False:
			self.isSocketError = True
			self.interactiveLock.release()
			result.CopyErrorFromOther( resultSocket )
			return result

		read = self.ReadFromCoreSocketServer( resultSocket.Content, send )
		if read.IsSuccess :
			self.isSocketError = False
			result.IsSuccess = read.IsSuccess
			result.Content = read.Content
			result.Message = StringResources.SuccessText
			# string tmp2 = BasicFramework.SoftBasic.ByteToHexString( result.Content, '-' )
		else:
			self.isSocketError = True
			result.CopyErrorFromOther( read )

		self.interactiveLock.release()
		if self.isPersistentConn==False:
			if resultSocket.Content != None:
				resultSocket.Content.close()
		return result
		
	def ReadFromCoreServerBase( self, socket, send ):
		'''使用底层的数据报文来通讯，传入需要发送的消息，返回最终的数据结果，被拆分成了头子节和内容字节信息'''
		self.iNetMessage.SendBytes = send
		sendResult = self.Send( socket, send )
		if sendResult.IsSuccess == False:
			if socket!= None : socket.close( )
			return OperateResult.CreateFailedResult( sendResult )

		# 接收超时时间大于0时才允许接收远程的数据
		if (self.receiveTimeOut >= 0):
			# 接收数据信息
			resultReceive = self.ReceiveMessage(socket, 10000, self.iNetMessage)
			if resultReceive.IsSuccess == False:
				socket.close( )
				return OperateResult( msg = "Receive data timeout: " + str(self.receiveTimeOut ) + " Msg:"+ resultReceive.Message)
			return OperateResult.CreateSuccessResult( resultReceive.Content.HeadBytes, resultReceive.Content.ContentBytes )
		else:
			return OperateResult.CreateSuccessResult( bytearray(0), bytearray(0) )

	def GetBoolResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetBoolResultFromBytes( result, self.byteTransform)
	def GetByteResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetByteResultFromBytes( result, self.byteTransform)
	def GetInt16ResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetInt16ResultFromBytes( result, self.byteTransform)
	def GetUInt16ResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetUInt16ResultFromBytes( result, self.byteTransform)
	def GetInt32ResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetInt32ResultFromBytes( result, self.byteTransform )
	def GetUInt32ResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetUInt32ResultFromBytes( result, self.byteTransform )
	def GetInt64ResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetInt64ResultFromBytes( result, self.byteTransform )
	def GetUInt64ResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetUInt64ResultFromBytes( result, self.byteTransform )
	def GetSingleResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetSingleResultFromBytes( result, self.byteTransform )
	def GetDoubleResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetDoubleResultFromBytes( result, self.byteTransform )
	def GetStringResultFromBytes( self, result ):
		'''将指定的OperateResult类型转化'''
		return ByteTransformHelper.GetStringResultFromBytes( result, self.byteTransform )

class NetworkDeviceBase(NetworkDoubleBase):
	'''设备类的基类，提供了基础的字节读写方法'''
	# 单个数据字节的长度，西门子为2，三菱，欧姆龙，modbusTcp就为1
	WordLength = 1
	def Read( self, address, length ):
		'''从设备读取原始数据'''
		return OperateResult( )
	def Write( self, address, value ):
		'''将原始数据写入设备'''
		return OperateResult()
	def ReadInt16( self, address, length = None ):
		'''读取设备的short类型的数据'''
		if(length == None):
			return self.GetInt16ResultFromBytes( self.Read( address, self.WordLength ) )
		else:
			read = self.Read(address,length*self.WordLength)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			return OperateResult.CreateSuccessResult(self.byteTransform.TransInt16Array(read.Content,0,length))
	def ReadUInt16( self, address, length = None ):
		'''读取设备的ushort数据类型的数据'''
		if length == None:
			return self.GetUInt16ResultFromBytes(self.Read(address,self.WordLength))
		else:
			read = self.Read(address,length*self.WordLength)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			return OperateResult.CreateSuccessResult(self.byteTransform.TransUInt16Array(read.Content,0,length))
	def ReadInt32( self, address, length = None ):
		'''读取设备的int类型的数据'''
		if length == None:
			return self.GetInt32ResultFromBytes( self.Read( address, 2 * self.WordLength ) )
		else:
			read = self.Read(address,2*length*self.WordLength)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			return OperateResult.CreateSuccessResult(self.byteTransform.TransInt32Array(read.Content,0,length))
	def ReadUInt32( self, address, length = None ):
		'''读取设备的uint数据类型的数据'''
		if length == None:
			return self.GetUInt32ResultFromBytes(self.Read(address,2 * self.WordLength))
		else:
			read = self.Read(address,2*length*self.WordLength)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			return OperateResult.CreateSuccessResult(self.byteTransform.TransUInt32Array(read.Content,0,length))
	def ReadFloat( self, address, length = None ):
		'''读取设备的float类型的数据'''
		if length == None:
			return self.GetSingleResultFromBytes( self.Read( address, 2 * self.WordLength ) )
		else:
			read = self.Read(address,2*length*self.WordLength)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			return OperateResult.CreateSuccessResult(self.byteTransform.TransSingleArray(read.Content,0,length))
	def ReadInt64( self, address, length = None ):
		'''读取设备的long类型的数组'''
		if length == None:
			return self.GetInt64ResultFromBytes( self.Read( address, 4 * self.WordLength) )
		else:
			read = self.Read(address,4*length*self.WordLength)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			return OperateResult.CreateSuccessResult(self.byteTransform.TransInt64Array(read.Content,0,length))
	def ReadUInt64( self, address, length = None ):
		'''读取设备的long类型的数组'''
		if length == None:
			return self.GetUInt64ResultFromBytes( self.Read( address, 4 * self.WordLength) )
		else:
			read = self.Read(address,4*length*self.WordLength)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			return OperateResult.CreateSuccessResult(self.byteTransform.TransUInt64Array(read.Content,0,length))
	def ReadDouble( self, address, length = None ):
		'''读取设备的long类型的数组'''
		if length == None:
			return self.GetDoubleResultFromBytes( self.Read( address, 4 * self.WordLength) )
		else:
			read = self.Read(address,4*length*self.WordLength)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			return OperateResult.CreateSuccessResult(self.byteTransform.TransDoubleArray(read.Content,0,length))
	def ReadString( self, address, length ):
		return self.GetStringResultFromBytes( self.Read( address, length ) )
	
	def WriteInt16( self, address, value ):
		'''向设备中写入short数据或是数组，返回是否写入成功'''
		if type(value) == list:
			return self.Write( address, self.byteTransform.Int16ArrayTransByte( value ) )
		else:
			return self.WriteInt16( address, [value] )
	def WriteUInt16( self, address, value ):
		'''向设备中写入short数据或是数组，返回是否写入成功'''
		if type(value) == list:
			return self.Write( address, self.byteTransform.UInt16ArrayTransByte( value ) )
		else:
			return self.WriteUInt16( address, [value] )
	def WriteInt32( self, address, value ):
		'''向设备中写入int数据，返回是否写入成功'''
		if type(value) == list:
			return self.Write( address, self.byteTransform.Int32ArrayTransByte(value) )
		else:
			return self.WriteInt32( address, [value])
	def WriteUInt32( self, address, value):
		'''向设备中写入uint数据，返回是否写入成功'''
		if type(value) == list:
			return self.Write( address, self.byteTransform.UInt32ArrayTransByte(value) )
		else:
			return self.WriteUInt32( address, [value] )
	def WriteFloat( self, address, value ):
		'''向设备中写入float数据，返回是否写入成功'''
		if type(value) == list:
			return self.Write( address, self.byteTransform.FloatArrayTransByte(value) )
		else:
			return self.WriteFloat(address, [value])
	def WriteInt64( self, address, value ):
		'''向设备中写入long数据，返回是否写入成功'''
		if type(value) == list:
			return self.Write( address,  self.byteTransform.Int64ArrayTransByte(value))
		else:
			return self.WriteInt64( address, [value] )
	def WriteUInt64( self, address, value ):
		'''向设备中写入ulong数据，返回是否写入成功'''
		if type(value) == list:
			return self.Write( address,  self.byteTransform.UInt64ArrayTransByte(value))
		else:
			return self.WriteUInt64( address, [value] )
	def WriteDouble( self, address, value ):
		'''向设备中写入double数据，返回是否写入成功'''
		if type(value) == list:
			return self.Write( address, self.byteTransform.DoubleArrayTransByte(value) )
		else:
			return self.WriteDouble( address, [value] )
	def WriteString( self, address, value, length = None ):
		'''向设备中写入string数据，编码为ascii，返回是否写入成功'''
		if length == None:
			return self.Write( address, self.byteTransform.StringTransByte( value, 'ascii' ) )
		else:
			return self.Write( address, SoftBasic.ArrayExpandToLength(self.byteTransform.StringTransByte( value, 'ascii' ), length))
	def WriteUnicodeString( self, address, value, length = None):
		'''向设备中写入string数据，编码为unicode，返回是否写入成功'''
		if length == None:
			temp = SoftBasic.StringToUnicodeBytes(value)
			return self.Write( address, temp )
		else:
			temp = SoftBasic.StringToUnicodeBytes(value)
			temp = SoftBasic.ArrayExpandToLength( temp, length * 2 )
			return self.Write( address, temp )

class ModbusInfo:
	'''Modbus协议相关的一些信息'''
	@staticmethod
	def ReadCoil():
		'''读取线圈功能码'''
		return 0x01
	@staticmethod
	def ReadDiscrete():
		'''读取寄存器功能码'''
		return 0x02
	@staticmethod
	def ReadRegister():
		'''读取寄存器功能码'''
		return 0x03
	@staticmethod
	def ReadInputRegister():
		'''读取输入寄存器'''
		return 0x04
	@staticmethod
	def WriteOneCoil():
		'''写单个寄存器'''
		return 0x05
	@staticmethod
	def WriteOneRegister():
		'''写单个寄存器'''
		return 0x06
	@staticmethod
	def WriteCoil():
		'''写多个线圈'''
		return 0x0F
	@staticmethod
	def WriteRegister():
		'''写多个寄存器'''
		return 0x10
	@staticmethod
	def FunctionCodeNotSupport():
		'''不支持该功能码'''
		return 0x01
	@staticmethod
	def FunctionCodeOverBound():
		'''该地址越界'''
		return 0x02
	@staticmethod
	def FunctionCodeQuantityOver():
		'''读取长度超过最大值'''
		return 0x03
	@staticmethod
	def FunctionCodeReadWriteException():
		'''读写异常'''
		return 0x04
	@staticmethod
	def PackCommandToTcp( value, id ):
		'''将modbus指令打包成Modbus-Tcp指令'''
		buffer = bytearray( len(value) + 6)
		buffer[0:2] = struct.pack('>H',id)
		buffer[4:6] = struct.pack('>H',len(value))
		buffer[6:len(buffer)] = value
		return buffer
	@staticmethod
	def GetDescriptionByErrorCode( code ):
		'''通过错误码来获取到对应的文本消息'''
		if code == 0x01: return StringResources.ModbusTcpFunctionCodeNotSupport()
		elif code == 0x02: return StringResources.ModbusTcpFunctionCodeOverBound()
		elif code == 0x03: return StringResources.ModbusTcpFunctionCodeQuantityOver()
		elif code == 0x04: return StringResources.ModbusTcpFunctionCodeReadWriteException()
		else: return StringResources.UnknownError
	@staticmethod
	def AnalysisReadAddress( address, isStartWithZero ):
		'''分析Modbus协议的地址信息，该地址适应于tcp及rtu模式'''
		try:
			mAddress = ModbusAddress(address)
			if isStartWithZero == False:
				if mAddress.Address < 1:
					raise RuntimeError(StringResources.ModbusAddressMustMoreThanOne())
				else:
					mAddress.Address = mAddress.Address - 1
			return OperateResult.CreateSuccessResult(mAddress)
		except Exception as ex:
			return OperateResult( msg = str(ex))
			
	

class ModbusAddress(DeviceAddressBase):
	'''Modbus协议的地址类'''
	Station = 0
	Function = ModbusInfo.ReadRegister()
	def __init__(self, address = "0"):
		self.Station = -1
		self.Function = ModbusInfo.ReadRegister()
		self.Address = 0
		self.AnalysisAddress(address)

	def AnalysisAddress( self, address = "0" ):
		'''解析Modbus的地址码'''
		if address.find(';')>=0:
			listAddress = address.split(";")
			for index in range(len(listAddress)):
				if listAddress[index][0] == 's' or listAddress[index][0] == 'S':
					self.Station = int(listAddress[index][2:])
				elif listAddress[index][0] == 'x' or listAddress[index][0] == 'X':
					self.Function = int(listAddress[index][2:])
				else:
					self.Address = int(listAddress[index])
		else:
			self.Address = int(address)
	
	def CreateReadCoils( self, station, length ):
		'''创建一个读取线圈的字节对象'''
		buffer = bytearray(6)
		if self.Station < 0 :
			buffer[0] = station
		else:
			buffer[0] = self.Station
		buffer[1] = ModbusInfo.ReadCoil()
		buffer[2:4] = struct.pack('>H', self.Address)
		buffer[4:6] = struct.pack('>H', length)
		return buffer
	def CreateReadDiscrete( self, station, length ):
		'''创建一个读取离散输入的字节对象'''
		buffer = bytearray(6)
		if self.Station < 0 :
			buffer[0] = station
		else:
			buffer[0] = self.Station
		buffer[1] = ModbusInfo.ReadDiscrete()
		buffer[2:4] = struct.pack('>H', self.Address)
		buffer[4:6] = struct.pack('>H', length)
		return buffer
	def CreateReadRegister( self, station, length ):
		'''创建一个读取寄存器的字节对象'''
		buffer = bytearray(6)
		if self.Station < 0 :
			buffer[0] = station
		else:
			buffer[0] = self.Station
		buffer[1] = self.Function
		buffer[2:4] = struct.pack('>H', self.Address)
		buffer[4:6] = struct.pack('>H', length)
		return buffer
	def CreateReadInputRegister( self, station, length ):
		'''创建一个读取寄存器的字节对象'''
		buffer = bytearray(6)
		if self.Station < 0 :
			buffer[0] = station
		else:
			buffer[0] = self.Station
		buffer[1] = ModbusInfo.ReadInputRegister()
		buffer[2:4] = struct.pack('>H', self.Address)
		buffer[4:6] = struct.pack('>H', length)
		return buffer
	def CreateWriteOneCoil(self, station, value):
		'''创建一个写入单个线圈的指令'''
		buffer = bytearray(6)
		if self.Station < 0 :
			buffer[0] = station
		else:
			buffer[0] = self.Station
		buffer[1] = ModbusInfo.WriteOneCoil()
		buffer[2:4] = struct.pack('>H', self.Address)
		if value == True:
			buffer[4] = 0xFF
		return buffer
	def CreateWriteOneRegister(self, station, values):
		'''创建一个写入单个寄存器的指令'''
		buffer = bytearray(6)
		if self.Station < 0 :
			buffer[0] = station
		else:
			buffer[0] = self.Station
		buffer[1] = ModbusInfo.WriteOneRegister()
		buffer[2:4] = struct.pack('>H', self.Address)
		buffer[4:6] = values
		return buffer
	def CreateWriteCoil(self, station, values):
		'''创建一个写入批量线圈的指令'''
		data = SoftBasic.BoolArrayToByte( values )
		buffer = bytearray(7 + len(data))
		if self.Station < 0 :
			buffer[0] = station
		else:
			buffer[0] = self.Station
		buffer[1] = ModbusInfo.WriteCoil()
		buffer[2:4] = struct.pack('>H', self.Address)
		buffer[4:6] = struct.pack('>H', len(values))
		buffer[6] = len(data)
		buffer[7:len(buffer)] = data
		return buffer
	def CreateWriteRegister(self, station, values):
		'''创建一个写入批量寄存器的指令'''
		buffer = bytearray(7 + len(values))
		if self.Station < 0 :
			buffer[0] = station
		else:
			buffer[0] = self.Station
		buffer[1] = ModbusInfo.WriteRegister()
		buffer[2:4] = struct.pack('>H', self.Address)
		buffer[4:6] = struct.pack('>H', len(values)//2)
		buffer[6] = len(values)
		buffer[7:len(buffer)] = values
		return buffer
	def AddressAdd(self, value):
		'''地址新增指定的数'''
		modbusAddress = ModbusAddress()
		modbusAddress.Station = self.Station
		modbusAddress.Function = self.Function
		modbusAddress.Address = self.Address+value
		return modbusAddress




class ModbusTcpNet(NetworkDeviceBase):
	'''Modbus-Tcp协议的客户端通讯类，方便的和服务器进行数据交互'''
	station = 1
	softIncrementCount = None
	isAddressStartWithZero = True
	def __init__(self, ipAddress = '127.0.0.1', port = 502, station = 1):
		'''实例化一个MOdbus-Tcp协议的客户端对象'''
		self.WordLength = 1
		self.softIncrementCount = SoftIncrementCount( 65536, 0 )
		self.station = station
		self.ipAddress = ipAddress
		self.port = port
		self.byteTransform = ReverseWordTransform()
		self.iNetMessage = ModbusTcpMessage()
	def SetDataFormat( self, value ):
		'''多字节的数据是否高低位反转，该设置的改变会影响Int32,UInt32,float,double,Int64,UInt64类型的读写'''
		self.byteTransform.DataFormat = value
	def GetDataFormat( self ):
		'''多字节的数据是否高低位反转，该设置的改变会影响Int32,UInt32,float,double,Int64,UInt64类型的读写'''
		return self.byteTransform.DataFormat
	def SetIsStringReverse( self, value ):
		'''字符串数据是否按照字来反转'''
		self.byteTransform.IsStringReverse = value
	def GetIsStringReverse( self ):
		'''字符串数据是否按照字来反转'''
		return self.byteTransform.IsStringReverse
	def BuildReadCoilCommand(self, address, length):
		'''生成一个读取线圈的指令头'''
		# 分析地址
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False: return OperateResult.CreateFailedResult(analysis)
		# 获取消息号
		messageId = self.softIncrementCount.GetCurrentValue()
		#生成最终的指令
		buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateReadCoils( self.station, length ), messageId)
		return OperateResult.CreateSuccessResult(buffer)
	def BuildReadDiscreteCommand(self, address, length):
		'''生成一个读取离散信息的指令头'''
		# 分析地址
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False: return OperateResult.CreateFailedResult(analysis)
		# 获取消息号
		messageId = self.softIncrementCount.GetCurrentValue()
		buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateReadDiscrete(self.station,length), messageId)
		return OperateResult.CreateSuccessResult(buffer)
	def BuildReadRegisterCommand(self, address, length):
		'''创建一个读取寄存器的字节对象'''
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False: return OperateResult.CreateFailedResult(analysis)
		# 获取消息号
		messageId = self.softIncrementCount.GetCurrentValue()
		buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateReadRegister(self.station,length), messageId)
		return OperateResult.CreateSuccessResult(buffer)
	def BuildReadInputRegisterCommand(self, address, length):
		'''创建一个读取寄存器的字节对象'''
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False: return OperateResult.CreateFailedResult(analysis)
		# 获取消息号
		messageId = self.softIncrementCount.GetCurrentValue()
		buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateReadInputRegister(self.station,length), messageId)
		return OperateResult.CreateSuccessResult(buffer)
	def BuildWriteOneCoilCommand(self, address,value):
		'''生成一个写入单线圈的指令头'''
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False: return OperateResult.CreateFailedResult(analysis)
		# 获取消息号
		messageId = self.softIncrementCount.GetCurrentValue()
		buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateWriteOneCoil(self.station,value), messageId)
		return OperateResult.CreateSuccessResult(buffer)
	def BuildWriteOneRegisterCommand(self, address, values):
		'''生成一个写入单个寄存器的报文'''
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False: return OperateResult.CreateFailedResult(analysis)
		# 获取消息号
		messageId = self.softIncrementCount.GetCurrentValue()
		buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateWriteOneRegister(self.station,values), messageId)
		return OperateResult.CreateSuccessResult(buffer)
	def BuildWriteCoilCommand(self, address, values):
		'''生成批量写入单个线圈的报文信息，需要传入bool数组信息'''
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False: return OperateResult.CreateFailedResult(analysis)
		# 获取消息号
		messageId = self.softIncrementCount.GetCurrentValue()
		buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateWriteCoil(self.station,values), messageId)
		return OperateResult.CreateSuccessResult(buffer)
	def BuildWriteRegisterCommand(self, address, values):
		'''生成批量写入寄存器的报文信息，需要传入byte数组'''
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False: return OperateResult.CreateFailedResult(analysis)
		# 获取消息号
		messageId = self.softIncrementCount.GetCurrentValue()
		buffer = ModbusInfo.PackCommandToTcp(analysis.Content.CreateWriteRegister(self.station,values), messageId)
		return OperateResult.CreateSuccessResult(buffer)
	def BuildReadModbusAddressCommand( self, address, length ):
		'''生成一个读取寄存器的指令头，address->ModbusAddress'''
		# 获取消息号
		messageId =  self.softIncrementCount.GetCurrentValue()
		# 生成最终tcp指令
		buffer = ModbusInfo.PackCommandToTcp( address.CreateReadRegister( self.station, length ), messageId )
		return OperateResult.CreateSuccessResult( buffer )
	def CheckModbusTcpResponse( self, send ):
		'''检查当前的Modbus-Tcp响应是否是正确的'''
		resultBytes = self.ReadFromCoreServer( send )
		if resultBytes.IsSuccess == True:
			if (send[7] + 0x80) == resultBytes.Content[7]:
				# 发生了错误
				resultBytes.IsSuccess = False
				resultBytes.Message = ModbusInfo.GetDescriptionByErrorCode( resultBytes.Content[8] )
				resultBytes.ErrorCode = resultBytes.Content[8]
		return resultBytes
	def ReadModBusBase( self, code, address, length ):
		'''检查当前的Modbus-Tcp响应是否是正确的'''
		command = None
		if code == ModbusInfo.ReadCoil():
			command = self.BuildReadCoilCommand( address, length )
		elif code == ModbusInfo.ReadDiscrete():
			command = self.BuildReadDiscreteCommand( address, length )
		elif code == ModbusInfo.ReadRegister():
			command = self.BuildReadRegisterCommand( address, length )
		elif code == ModbusInfo.ReadInputRegister():
			command = self.BuildReadInputRegisterCommand( address, length )
		else:
			command = OperateResult( msg = StringResources.ModbusTcpFunctionCodeNotSupport() )
		if command.IsSuccess == False : return OperateResult.CreateFailedResult( command )

		resultBytes = self.CheckModbusTcpResponse( command.Content )
		if resultBytes.IsSuccess == True:
			# 二次数据处理
			if len(resultBytes.Content) >= 9:
				buffer = bytearray(len(resultBytes.Content) - 9)
				buffer[0:len(buffer)] = resultBytes.Content[9:]
				resultBytes.Content = buffer
		return resultBytes
	def ReadModBusAddressBase( self, address, length = 1 ):
		'''读取服务器的数据，需要指定不同的功能码'''
		command = self.BuildReadModbusAddressCommand( address, length )
		if command.IsSuccess == False: return OperateResult.CreateFailedResult(command)

		resultBytes = self.CheckModbusTcpResponse( command.Content )
		if resultBytes.IsSuccess == True:
			# 二次数据处理
			if len(resultBytes.Content) >= 9:
				buffer = bytearray(len(resultBytes.Content) - 9)
				buffer[0:len(buffer)] = resultBytes.Content[9:]
				resultBytes.Content = buffer
		return resultBytes
	def ReadCoil( self, address, length = None):
		'''批量的读取线圈，需要指定起始地址，读取长度可选'''
		if length == None:
			read = self.ReadCoil( address, 1 )
			if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )
			return OperateResult.CreateSuccessResult( read.Content[0] )
		else:
			read = self.ReadModBusBase( ModbusInfo.ReadCoil(), address, length )
			if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

			return OperateResult.CreateSuccessResult( SoftBasic.ByteToBoolArray( read.Content, length ) )
	def ReadDiscrete( self, address, length = None):
		'''批量的读取输入点，需要指定起始地址，可选读取长度'''
		if length == None:
			read = self.ReadDiscrete( address, 1 )
			if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )
			return OperateResult.CreateSuccessResult( read.Content[0] )
		else:
			read = self.ReadModBusBase( ModbusInfo.ReadDiscrete(), address, length )
			if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )
			
			return OperateResult.CreateSuccessResult( SoftBasic.ByteToBoolArray( read.Content, length ) )
	def Read( self, address, length ):
		'''从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度'''
		analysis = ModbusInfo.AnalysisReadAddress( address, self.isAddressStartWithZero )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )
		return self.ReadModBusAddressBase( analysis.Content, length )
	def WriteOneRegister( self, address, value ):
		'''写一个寄存器数据'''
		if type(value) == list:
			command = self.BuildWriteOneRegisterCommand( address, value )
			if command.IsSuccess == False : return command
			return self.CheckModbusTcpResponse( command.Content )
		else:
			return self.WriteOneRegister(address, struct.pack('>H', value))
	def Write( self, address, value ):
		'''将数据写入到Modbus的寄存器上去，需要指定起始地址和数据内容'''
		command = self.BuildWriteRegisterCommand( address, value )
		if command.IsSuccess == False:
			return command

		return self.CheckModbusTcpResponse( command.Content )
	def WriteCoil( self, address, value ):
		'''批量写线圈信息，指定是否通断'''
		if type(value) == list:
			command = self.BuildWriteCoilCommand( address, value )
			if command.IsSuccess == False : return command
			return self.CheckModbusTcpResponse( command.Content )
		else:
			command = self.BuildWriteOneCoilCommand( address, value )
			if command.IsSuccess == False : return command
			return self.CheckModbusTcpResponse( command.Content )
	def WriteBool( self, address, values ):
		'''批量写寄存器的数据内容'''
		return self.Write( address, SoftBasic.BoolArrayToByte( values ) )

# 三菱的类库
class MelsecMcDataType:
	'''三菱PLC的数据类型，此处包含了几个常用的类型'''
	DataCode = 0
	DataType = 0
	AsciiCode = 0
	FromBase = 0
	def __init__(self, code, typeCode, asciiCode, fromBase):
		'''如果您清楚类型代号，可以根据值进行扩展'''
		self.DataCode = code
		self.AsciiCode = asciiCode
		self.FromBase = fromBase
		if typeCode < 2:
			self.DataType = typeCode

	@staticmethod
	def GetX():
		'''X输入寄存器'''
		return MelsecMcDataType(0x9C,0x01,'X*',16)
	@staticmethod
	def GetY():
		'''Y输出寄存器'''
		return MelsecMcDataType(0x9D,0x01,'Y*',16)
	@staticmethod
	def GetM():
		'''M中间寄存器'''
		return MelsecMcDataType(0x90,0x01,'M*',10)
	@staticmethod
	def GetD():
		'''D数据寄存器'''
		return MelsecMcDataType(0xA8,0x00,'D*',10)
	@staticmethod
	def GetW():
		'''W链接寄存器'''
		return MelsecMcDataType(0xB4,0x00,'W*',16)
	@staticmethod
	def GetL():
		'''L锁存继电器'''
		return MelsecMcDataType(0x92,0x01,'L*',10)
	@staticmethod
	def GetF():
		'''F报警器'''
		return MelsecMcDataType(0x93,0x01,'F*',10)
	@staticmethod
	def GetV():
		'''V边沿继电器'''
		return MelsecMcDataType(0x93,0x01,'V*',10)
	@staticmethod
	def GetB():
		'''B链接继电器'''
		return MelsecMcDataType(0xA,0x01,'B*',16)
	@staticmethod
	def GetR():
		'''R文件寄存器'''
		return MelsecMcDataType(0xAF,0x00,'R*',10)
	@staticmethod
	def GetS():
		'''S步进继电器'''
		return MelsecMcDataType(0x98,0x01,'S*',10)
	@staticmethod
	def GetZ():
		'''变址寄存器'''
		return MelsecMcDataType(0xCC,0x00,'Z*',10)
	@staticmethod
	def GetT():
		'''定时器的值'''
		return MelsecMcDataType(0xC2,0x00,'TN',10)
	@staticmethod
	def GetC():
		'''计数器的值'''
		return MelsecMcDataType(0xC5,0x00,'CN',10)

class MelsecHelper:
	'''所有三菱通讯类的通用辅助工具类，包含了一些通用的静态方法，可以使用本类来获取一些原始的报文信息。详细的操作参见例子'''
	@staticmethod
	def McAnalysisAddress( address = "0" ):
		result = OperateResult()
		try:
			if address.startswith("M") or address.startswith("m"):
				result.Content1 = MelsecMcDataType.GetM()
				result.Content2 = int(address[1:], MelsecMcDataType.GetM().FromBase)
			elif address.startswith("X") or address.startswith("x"):
				result.Content1 = MelsecMcDataType.GetX()
				result.Content2 = int(address[1:], MelsecMcDataType.GetX().FromBase)
			elif address.startswith("Y") or address.startswith("y"):
				result.Content1 = MelsecMcDataType.GetY()
				result.Content2 = int(address[1:], MelsecMcDataType.GetY().FromBase)
			elif address.startswith("D") or address.startswith("d"):
				result.Content1 = MelsecMcDataType.GetD()
				result.Content2 = int(address[1:], MelsecMcDataType.GetD().FromBase)
			elif address.startswith("W") or address.startswith("w"):
				result.Content1 = MelsecMcDataType.GetW()
				result.Content2 = int(address[1:], MelsecMcDataType.GetW().FromBase)
			elif address.startswith("L") or address.startswith("l"):
				result.Content1 = MelsecMcDataType.GetL()
				result.Content2 = int(address[1:], MelsecMcDataType.GetL().FromBase)
			elif address.startswith("F") or address.startswith("f"):
				result.Content1 = MelsecMcDataType.GetF()
				result.Content2 = int(address[1:], MelsecMcDataType.GetF().FromBase)
			elif address.startswith("V") or address.startswith("v"):
				result.Content1 = MelsecMcDataType.GetV()
				result.Content2 = int(address[1:], MelsecMcDataType.GetV().FromBase)
			elif address.startswith("B") or address.startswith("b"):
				result.Content1 = MelsecMcDataType.GetB()
				result.Content2 = int(address[1:], MelsecMcDataType.GetB().FromBase)
			elif address.startswith("R") or address.startswith("r"):
				result.Content1 = MelsecMcDataType.GetR()
				result.Content2 = int(address[1:], MelsecMcDataType.GetR().FromBase)
			elif address.startswith("S") or address.startswith("s"):
				result.Content1 = MelsecMcDataType.GetS()
				result.Content2 = int(address[1:], MelsecMcDataType.GetS().FromBase)
			elif address.startswith("Z") or address.startswith("z"):
				result.Content1 = MelsecMcDataType.GetZ()
				result.Content2 = int(address[1:], MelsecMcDataType.GetZ().FromBase)
			elif address.startswith("T") or address.startswith("t"):
				result.Content1 = MelsecMcDataType.GetT()
				result.Content2 = int(address[1:], MelsecMcDataType.GetT().FromBase)
			elif address.startswith("C") or address.startswith("c"):
				result.Content1 = MelsecMcDataType.GetC()
				result.Content2 = int(address[1:], MelsecMcDataType.GetC().FromBase)
			else:
				raise Exception("type not supported!")
		except Exception as ex:
			result.Message = str(ex)
			return result
		
		result.IsSuccess = True
		result.Message = StringResources.SuccessText()
		return result
	@staticmethod
	def BuildBytesFromData( value, length = None ):
		'''从数据构建一个ASCII格式地址字节'''
		if length == None:
			return ('{:02X}'.format(value)).encode('ascii')
		else:
			return (('{:0'+ str(length) +'X}').format(value)).encode('ascii')
	@staticmethod
	def BuildBytesFromAddress( address, dataType ):
		'''从三菱的地址中构建MC协议的6字节的ASCII格式的地址'''
		if dataType.FromBase == 10:
			return ('{:06d}'.format(address)).encode('ascii')
		else:
			return ('{:06X}'.format(address)).encode('ascii')
	@staticmethod
	def FxCalculateCRC( data ):
		'''计算Fx协议指令的和校验信息'''
		sum = 0
		index = 1
		while index < (len(data) - 2):
			sum += data[index]
			index=index+1
		return MelsecHelper.BuildBytesFromData( sum )
	@staticmethod
	def CheckCRC( data ):
		'''检查指定的和校验是否是正确的'''
		crc = MelsecHelper.FxCalculateCRC( data )
		if (crc[0] != data[data.Length - 2]) : return False
		if (crc[1] != data[data.Length - 1]) : return False
		return True
class MelsecMcNet(NetworkDeviceBase):
	'''三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯'''
	NetworkNumber = 0
	NetworkStationNumber = 0
	def __init__(self,ipAddress= "127.0.0.1",port = 0):
		'''实例化一个三菱的Qna兼容3E帧协议的通讯对象'''
		self.iNetMessage = MelsecQnA3EBinaryMessage()
		self.byteTransform = RegularByteTransform()
		self.ipAddress = ipAddress
		self.port = port
		self.WordLength = 1
	@staticmethod
	def BuildReadCommand(address,length,networkNumber = 0,networkStationNumber = 0):
		'''根据类型地址长度确认需要读取的指令头'''
		analysis = MelsecHelper.McAnalysisAddress( address )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )

		_PLCCommand = bytearray(21)
		_PLCCommand[0]  = 0x50                                   # 副标题
		_PLCCommand[1]  = 0x00
		_PLCCommand[2]  = networkNumber                          # 网络号
		_PLCCommand[3]  = 0xFF                                   # PLC编号
		_PLCCommand[4]  = 0xFF                                   # 目标模块IO编号
		_PLCCommand[5]  = 0x03
		_PLCCommand[6]  = networkStationNumber                   # 目标模块站号
		_PLCCommand[7]  = 0x0C                                   # 请求数据长度
		_PLCCommand[8]  = 0x00
		_PLCCommand[9]  = 0x0A                                   # CPU监视定时器
		_PLCCommand[10] = 0x00
		_PLCCommand[11] = 0x01                                   # 批量读取数据命令
		_PLCCommand[12] = 0x04
		_PLCCommand[13] = analysis.Content1.DataType             # 以点为单位还是字为单位成批读取
		_PLCCommand[14] = 0x00
		_PLCCommand[15] = analysis.Content2 % 256                # 起始地址的地位
		_PLCCommand[16] = analysis.Content2 // 256
		_PLCCommand[17] = 0x00
		_PLCCommand[18] = analysis.Content1.DataCode             # 指明读取的数据
		_PLCCommand[19] = length % 256                           # 软元件长度的地位
		_PLCCommand[20] = length // 256

		return OperateResult.CreateSuccessResult( _PLCCommand )
	@staticmethod
	def BuildWriteCommand( address, value, networkNumber = 0, networkStationNumber = 0 ):
		'''根据类型地址以及需要写入的数据来生成指令头'''
		analysis = MelsecHelper.McAnalysisAddress( address )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )
		
		length = -1
		if analysis.Content1.DataType == 1:
			# 按照位写入的操作，数据需要重新计算
			length2 =  len(value) // 2 + 1
			if len(value) % 2 == 0 : 
				length2 = len(value) // 2
			buffer = bytearray(length2)

			for i in range(length2):
				if value[i * 2 + 0] != 0x00 :
					buffer[i] += 0x10
				if (i * 2 + 1) < len(value) :
					if value[i * 2 + 1] != 0x00 :
						buffer[i] += 0x01
			length = len(value)
			value = buffer
		
		_PLCCommand = bytearray(21 + len(value))
		_PLCCommand[0]  = 0x50                                          # 副标题
		_PLCCommand[1]  = 0x00
		_PLCCommand[2]  = networkNumber                                 # 网络号
		_PLCCommand[3]  = 0xFF                                          # PLC编号
		_PLCCommand[4]  = 0xFF                                          # 目标模块IO编号
		_PLCCommand[5]  = 0x03
		_PLCCommand[6]  = networkStationNumber                          # 目标模块站号
		_PLCCommand[7]  = (len(_PLCCommand) - 9) % 256                  # 请求数据长度
		_PLCCommand[8]  = (len(_PLCCommand) - 9) // 256
		_PLCCommand[9]  = 0x0A                                          # CPU监视定时器
		_PLCCommand[10] = 0x00
		_PLCCommand[11] = 0x01                                          # 批量读取数据命令
		_PLCCommand[12] = 0x14
		_PLCCommand[13] = analysis.Content1.DataType                    # 以点为单位还是字为单位成批读取
		_PLCCommand[14] = 0x00
		_PLCCommand[15] = analysis.Content2 % 256                       # 起始地址的地位
		_PLCCommand[16] = analysis.Content2 // 256
		_PLCCommand[17] = 0x00
		_PLCCommand[18] = analysis.Content1.DataCode                    # 指明写入的数据

		# 判断是否进行位操作
		if analysis.Content1.DataType == 1:
			if length > 0:
				_PLCCommand[19] = length % 256                          # 软元件长度的地位
				_PLCCommand[20] = length // 256
			else:
				_PLCCommand[19] = len(value) * 2 % 256                  # 软元件长度的地位
				_PLCCommand[20] = len(value) * 2 // 256 
		else:
			_PLCCommand[19] = len(value) // 2 % 256                     # 软元件长度的地位
			_PLCCommand[20] = len(value) // 2 // 256
		_PLCCommand[21:] = value

		return OperateResult.CreateSuccessResult( _PLCCommand )
	@staticmethod
	def ExtractActualData( response, isBit ):
		''' 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取'''
		if isBit == True:
			# 位读取
			Content = bytearray((len(response) - 11) * 2)
			i = 11
			while i < len(response):
				if (response[i] & 0x10) == 0x10:
					Content[(i - 11) * 2 + 0] = 0x01
				if (response[i] & 0x01) == 0x01:
					Content[(i - 11) * 2 + 1] = 0x01
				i = i + 1

			return OperateResult.CreateSuccessResult( Content )
		else:
			# 字读取
			Content = bytearray(len(response) - 11)
			Content[0:] = response[11:]

			return OperateResult.CreateSuccessResult( Content )
	def Read( self, address, length ):
		'''从三菱PLC中读取想要的数据，返回读取结果'''
		# 获取指令
		command = MelsecMcNet.BuildReadCommand( address, length, self.NetworkNumber, self.NetworkStationNumber )
		if command.IsSuccess == False :
			return OperateResult.CreateFailedResult( command )

		# 核心交互
		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

		# 错误代码验证
		errorCode = read.Content[9] * 256 + read.Content[10]
		if errorCode != 0 : return OperateResult(err=errorCode, msg=StringResources.MelsecPleaseReferToManulDocument())

		# 数据解析，需要传入是否使用位的参数
		return MelsecMcNet.ExtractActualData( read.Content, command.Content[13] == 1 )
	def ReadBool( self, address, length = None ):
		'''从三菱PLC中批量读取位软元件，返回读取结果'''
		if length == None:
			read = self.ReadBool(address,1)
			if read.IsSuccess == False:
				return OperateResult.CreateFailedResult(read)
			else:
				return OperateResult.CreateSuccessResult(read.Content[0])
		else:
			# 解析地址
			analysis = MelsecHelper.McAnalysisAddress( address )
			if analysis.IsSuccess == False : 
				return OperateResult.CreateFailedResult( analysis )

			# 位读取校验
			if analysis.Content1.DataType == 0x00 : 
				return OperateResult( msg = StringResources.MelsecReadBitInfo() )

			# 核心交互
			read = self.Read( address, length )
			if read.IsSuccess == False : 
				return OperateResult.CreateFailedResult( read )

			# 转化bool数组
			content = []
			for i in range(length):
				if read.Content[i] == 0x01:
					content.append(True)
				else:
					content.append(False)
			return OperateResult.CreateSuccessResult( content )
	def Write( self, address, value ):
		'''向PLC写入数据，数据格式为原始的字节类型'''
		# 解析指令
		command = MelsecMcNet.BuildWriteCommand( address, value, self.NetworkNumber, self.NetworkStationNumber )
		if command.IsSuccess == False : return command

		# 核心交互
		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return read

		# 错误码校验
		errorCode = read.Content[9] * 256 + read.Content[10]
		if errorCode != 0 : return OperateResult(err=errorCode, msg=StringResources.MelsecPleaseReferToManulDocument())

		# 成功
		return OperateResult.CreateSuccessResult( )
	def WriteBool( self, address, values ):
		'''向PLC中位软元件写入bool数组或是值，返回值说明，比如你写入M100,values[0]对应M100'''
		if type(values) == list:
			buffer = bytearray(len(values))
			for i in range(len(values)):
				if values[i] == True:
					buffer[i] = 0x01
			return self.Write(address, buffer)
		else:
			return self.WriteBool(address,[values])

class MelsecMcAsciiNet(NetworkDeviceBase):
	'''三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式'''
	NetworkNumber = 0
	NetworkStationNumber = 0
	def __init__(self,ipAddress= "127.0.0.1",port = 0):
		'''实例化一个三菱的Qna兼容3E帧协议的通讯对象'''
		self.iNetMessage = MelsecQnA3EAsciiMessage()
		self.byteTransform = RegularByteTransform()
		self.ipAddress = ipAddress
		self.port = port
		self.WordLength = 1
	@staticmethod
	def BuildReadCommand( address, length, networkNumber = 0, networkStationNumber = 0 ):
		'''根据类型地址长度确认需要读取的报文'''
		analysis = MelsecHelper.McAnalysisAddress( address )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )

		# 默认信息----注意：高低字节交错
		_PLCCommand = bytearray(42)
		_PLCCommand[ 0] = 0x35                                                               # 副标题
		_PLCCommand[ 1] = 0x30
		_PLCCommand[ 2] = 0x30
		_PLCCommand[ 3] = 0x30
		_PLCCommand[ 4] = MelsecHelper.BuildBytesFromData( networkNumber )[0]                # 网络号
		_PLCCommand[ 5] = MelsecHelper.BuildBytesFromData( networkNumber )[1]
		_PLCCommand[ 6] = 0x46                                                               # PLC编号
		_PLCCommand[ 7] = 0x46
		_PLCCommand[ 8] = 0x30                                                               # 目标模块IO编号
		_PLCCommand[ 9] = 0x33
		_PLCCommand[10] = 0x46
		_PLCCommand[11] = 0x46
		_PLCCommand[12] = MelsecHelper.BuildBytesFromData( networkStationNumber )[0]         # 目标模块站号
		_PLCCommand[13] = MelsecHelper.BuildBytesFromData( networkStationNumber )[1]
		_PLCCommand[14] = 0x30                                                               # 请求数据长度
		_PLCCommand[15] = 0x30
		_PLCCommand[16] = 0x31
		_PLCCommand[17] = 0x38
		_PLCCommand[18] = 0x30                                                               # CPU监视定时器
		_PLCCommand[19] = 0x30
		_PLCCommand[20] = 0x31
		_PLCCommand[21] = 0x30
		_PLCCommand[22] = 0x30                                                               # 批量读取数据命令
		_PLCCommand[23] = 0x34
		_PLCCommand[24] = 0x30
		_PLCCommand[25] = 0x31
		_PLCCommand[26] = 0x30                                                               # 以点为单位还是字为单位成批读取
		_PLCCommand[27] = 0x30
		_PLCCommand[28] = 0x30
		_PLCCommand[29] = 0x30 if analysis.Content1.DataType == 0 else 0x31
		_PLCCommand[30] = analysis.Content1.AsciiCode.encode('ascii')[0]                     # 软元件类型
		_PLCCommand[31] = analysis.Content1.AsciiCode.encode('ascii')[1]
		_PLCCommand[32:38] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )           # 起始地址的地位
		_PLCCommand[38:42] = MelsecHelper.BuildBytesFromData( length, 4 )                    # 软元件点数

		return OperateResult.CreateSuccessResult( _PLCCommand )
	@staticmethod
	def BuildWriteCommand( address, value, networkNumber = 0, networkStationNumber = 0 ):
		'''根据类型地址以及需要写入的数据来生成报文'''
		analysis = MelsecHelper.McAnalysisAddress( address )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )

		# 预处理指令
		if analysis.Content1.DataType == 0x01:
			# 位写入
			buffer = bytearray(len(value))
			for i in range(len(buffer)):
				buffer[i] = 0x30 if value[i] == 0x00 else 0x31
			value = buffer
		else:
			# 字写入
			buffer = bytearray(len(value) * 2)
			for i in range(len(value) // 2):
				tmp = value[i*2]+ value[i*2+1]*256
				buffer[4*i:4*i+4] = MelsecHelper.BuildBytesFromData( tmp, 4 )
			value = buffer

		# 默认信息----注意：高低字节交错

		_PLCCommand = bytearray(42 + len(value))

		_PLCCommand[ 0] = 0x35                                                                              # 副标题
		_PLCCommand[ 1] = 0x30
		_PLCCommand[ 2] = 0x30
		_PLCCommand[ 3] = 0x30
		_PLCCommand[ 4] = MelsecHelper.BuildBytesFromData( networkNumber )[0]                               # 网络号
		_PLCCommand[ 5] = MelsecHelper.BuildBytesFromData( networkNumber )[1]
		_PLCCommand[ 6] = 0x46                                                                              # PLC编号
		_PLCCommand[ 7] = 0x46
		_PLCCommand[ 8] = 0x30                                                                              # 目标模块IO编号
		_PLCCommand[ 9] = 0x33
		_PLCCommand[10] = 0x46
		_PLCCommand[11] = 0x46
		_PLCCommand[12] = MelsecHelper.BuildBytesFromData( networkStationNumber )[0]                        # 目标模块站号
		_PLCCommand[13] = MelsecHelper.BuildBytesFromData( networkStationNumber )[1]
		_PLCCommand[14] = MelsecHelper.BuildBytesFromData( len(_PLCCommand) - 18, 4 )[0]           # 请求数据长度
		_PLCCommand[15] = MelsecHelper.BuildBytesFromData( len(_PLCCommand) - 18, 4 )[1]
		_PLCCommand[16] = MelsecHelper.BuildBytesFromData( len(_PLCCommand) - 18, 4 )[2]
		_PLCCommand[17] = MelsecHelper.BuildBytesFromData( len(_PLCCommand) - 18, 4 )[3]
		_PLCCommand[18] = 0x30                                                                              # CPU监视定时器
		_PLCCommand[19] = 0x30
		_PLCCommand[20] = 0x31
		_PLCCommand[21] = 0x30
		_PLCCommand[22] = 0x31                                                                              # 批量写入的命令
		_PLCCommand[23] = 0x34
		_PLCCommand[24] = 0x30
		_PLCCommand[25] = 0x31
		_PLCCommand[26] = 0x30                                                                              # 子命令
		_PLCCommand[27] = 0x30
		_PLCCommand[28] = 0x30
		_PLCCommand[29] = 0x30 if analysis.Content1.DataType == 0 else 0x31
		_PLCCommand[30] = analysis.Content1.AsciiCode.encode('ascii')[0]                         # 软元件类型
		_PLCCommand[31] = analysis.Content1.AsciiCode.encode('ascii')[1]
		_PLCCommand[32] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[0]     # 起始地址的地位
		_PLCCommand[33] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[1]
		_PLCCommand[34] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[2]
		_PLCCommand[35] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[3]
		_PLCCommand[36] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[4]
		_PLCCommand[37] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[5]

		# 判断是否进行位操作
		if (analysis.Content1.DataType == 1):
			_PLCCommand[38] = MelsecHelper.BuildBytesFromData( len(value), 4 )[0]                    # 软元件点数
			_PLCCommand[39] = MelsecHelper.BuildBytesFromData( len(value), 4 )[1]
			_PLCCommand[40] = MelsecHelper.BuildBytesFromData( len(value), 4 )[2]
			_PLCCommand[41] = MelsecHelper.BuildBytesFromData( len(value), 4 )[3]
		else:
			_PLCCommand[38] = MelsecHelper.BuildBytesFromData( len(value) // 4, 4 )[0]              # 软元件点数
			_PLCCommand[39] = MelsecHelper.BuildBytesFromData( len(value) // 4, 4 )[1]
			_PLCCommand[40] = MelsecHelper.BuildBytesFromData( len(value) // 4, 4 )[2]
			_PLCCommand[41] = MelsecHelper.BuildBytesFromData( len(value) // 4, 4 )[3]
		_PLCCommand[42:] = value

		return OperateResult.CreateSuccessResult( _PLCCommand )

	@staticmethod
	def ExtractActualData( response, isBit ):
		if isBit == True:
			# 位读取
			Content = bytearray(len(response) - 22)
			for i in range(22,len(response)):
				Content[i - 22] = 0x00 if response[i] == 0x30 else 0x01

			return OperateResult.CreateSuccessResult( Content )
		else:
			# 字读取
			Content = bytearray((len(response) - 22) // 2)
			for i in range(len(Content)//2):
				tmp = int(response[i * 4 + 22:i * 4 + 26].decode('ascii'),16)
				Content[i * 2:i * 2+2] = struct.pack('<H',tmp)

			return OperateResult.CreateSuccessResult( Content )
	def Read( self, address, length ):
		'''从三菱PLC中读取想要的数据，返回读取结果'''
		# 获取指令
		command = MelsecMcAsciiNet.BuildReadCommand( address, length, self.NetworkNumber, self.NetworkStationNumber )
		if command.IsSuccess == False : return OperateResult.CreateFailedResult( command )

		# 核心交互
		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

		# 错误代码验证
		errorCode = int( read.Content[18:22].decode('ascii'), 16 )
		if errorCode != 0 : return OperateResult( err= errorCode, msg = StringResources.MelsecPleaseReferToManulDocument() )

		# 数据解析，需要传入是否使用位的参数
		return MelsecMcAsciiNet.ExtractActualData( read.Content, command.Content[29] == 0x31 )
	def ReadBool( self, address, length = None ):
		if length == None:
			read = self.ReadBool( address, 1 )
			if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

			return OperateResult.CreateSuccessResult( read.Content[0] )
		else:
			# 解析地址
			analysis = MelsecHelper.McAnalysisAddress( address )
			if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )

			# 位读取校验
			if analysis.Content1.DataType == 0x00 : return OperateResult( msg = StringResources.MelsecReadBitInfo )

			# 核心交互
			read = self.Read( address, length )
			if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

			# 转化bool数组
			content = []
			for i in range(len(read.Content)):
				if read.Content[i] == 0x01:
					content.append(True)
				else:
					content.append(False)
			return OperateResult.CreateSuccessResult( content )
	def Write( self, address, value ):
		'''向PLC写入数据，数据格式为原始的字节类型'''
		# 解析指令
		command = MelsecMcAsciiNet.BuildWriteCommand( address, value, self.NetworkNumber, self.NetworkStationNumber )
		if command.IsSuccess == False : return command

		# 核心交互
		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return read

		# 错误码验证
		errorCode = int( read.Content[18:22].decode('ascii'), 16 )
		if errorCode != 0 : return OperateResult( err = errorCode, msg = StringResources.MelsecPleaseReferToManulDocument() )

		# 写入成功
		return OperateResult.CreateSuccessResult( )
	def WriteBool( self, address, values ):
		'''向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100'''
		if type(values) == list:
			buffer = bytearray(len(values))
			for i in range(len(buffer)):
				buffer[i] = 0x01 if values[i] == True else 0x00
			return self.Write( address, buffer )
		else:
			return self.WriteBool( address, [values] )
        

# 西门子的数据类
class SiemensPLCS(Enum):
	'''西门子PLC的类对象'''
	S1200 = 0
	S300 = 1
	S1500 = 2
	S200Smart = 3
class SiemensS7Net(NetworkDeviceBase):
	'''一个西门子的客户端类，使用S7协议来进行数据交互'''
	CurrentPlc = SiemensPLCS.S1200
	plcHead1 = bytearray([0x03,0x00,0x00,0x16,0x11,0xE0,0x00,0x00,0x00,0x01,0x00,0xC0,0x01,0x0A,0xC1,0x02,0x01,0x02,0xC2,0x02,0x01,0x00])
	plcHead2 = bytearray([0x03,0x00,0x00,0x19,0x02,0xF0,0x80,0x32,0x01,0x00,0x00,0x04,0x00,0x00,0x08,0x00,0x00,0xF0,0x00,0x00,0x01,0x00,0x01,0x01,0xE0])
	plcOrderNumber = bytearray([0x03,0x00,0x00,0x21,0x02,0xF0,0x80,0x32,0x07,0x00,0x00,0x00,0x01,0x00,0x08,0x00,0x08,0x00,0x01,0x12,0x04,0x11,0x44,0x01,0x00,0xFF,0x09,0x00,0x04,0x00,0x11,0x00,0x00])
	plcHead1_200smart = bytearray([0x03,0x00,0x00,0x16,0x11,0xE0,0x00,0x00,0x00,0x01,0x00,0xC1,0x02,0x10,0x00,0xC2,0x02,0x03,0x00,0xC0,0x01,0x0A])
	plcHead2_200smart = bytearray([0x03,0x00,0x00,0x19,0x02,0xF0,0x80,0x32,0x01,0x00,0x00,0xCC,0xC1,0x00,0x08,0x00,0x00,0xF0,0x00,0x00,0x01,0x00,0x01,0x03,0xC0])
	def __init__(self, siemens, ipAddress = "127.0.0.1"):
		'''实例化一个西门子的S7协议的通讯对象并指定Ip地址'''
		self.WordLength = 2
		self.ipAddress = ipAddress
		self.port = 102
		self.CurrentPlc = siemens
		self.iNetMessage = S7Message()
		self.byteTransform = ReverseBytesTransform()

		if siemens == SiemensPLCS.S1200:
			self.plcHead1[21] = 0
		elif siemens == SiemensPLCS.S300:
			self.plcHead1[21] = 2
		elif siemens == SiemensPLCS.S1500:
			self.plcHead1[21] = 0
		elif siemens == SiemensPLCS.S200Smart:
			self.plcHead1 = self.plcHead1_200smart
			self.plcHead2 = self.plcHead2_200smart
		else:
			self.plcHead1[18] = 0
	@staticmethod
	def CalculateAddressStarted( address = "M0" ):
		'''计算特殊的地址信息'''
		if address.find('.') >= 0:
			temp = address.split(".")
			return int(temp[0]) * 8 + int(temp[1])
		else:
			return int( address ) * 8
	@staticmethod
	def AnalysisAddress( address = 'M0' ):
		'''解析数据地址，解析出地址类型，起始地址，DB块的地址'''
		result = OperateResult( )
		try:
			result.Content3 = 0
			if address[0] == 'I':
				result.Content1 = 0x81
				result.Content2 = SiemensS7Net.CalculateAddressStarted( address[1:] )
			elif address[0] == 'Q':
				result.Content1 = 0x82
				result.Content2 = SiemensS7Net.CalculateAddressStarted( address[1:] )
			elif address[0] == 'M':
				result.Content1 = 0x83
				result.Content2 = SiemensS7Net.CalculateAddressStarted( address[1:] )
			elif address[0] == 'D' or address[0:2] == "DB":
				result.Content1 = 0x84
				adds = address.split(".")
				if address[1] == 'B':
					result.Content3 = int( adds[0][2:] )
				else:
					result.Content3 = int( adds[0][1:] )
				result.Content2 = SiemensS7Net.CalculateAddressStarted( address[ (address.find( '.' ) + 1):]) 
			elif address[0] == 'T':
				result.Content1 = 0x1D
				result.Content2 = SiemensS7Net.CalculateAddressStarted( address[1:] )
			elif address[0] == 'C':
				result.Content1 = 0x1C
				result.Content2 = SiemensS7Net.CalculateAddressStarted( address[1:] )
			elif address[0] == 'V':
				result.Content1 = 0x84
				result.Content3 = 1
				result.Content2 = SiemensS7Net.CalculateAddressStarted( address[1:] )
			else:
				result.Message = StringResources.NotSupportedDataType()
				result.Content1 = 0
				result.Content2 = 0
				result.Content3 = 0
				return result
		except Exception as ex:
			result.Message = str(ex)
			return result

		result.IsSuccess = True
		return result
	@staticmethod
	def BuildReadCommand( address, length ):
		'''生成一个读取字数据指令头的通用方法'''
		if address == None : raise Exception( "address" )
		if length == None : raise Exception( "count" )
		if len(address) != len(length) : raise Exception( "两个参数的个数不统一" )
		if len(length) > 19 : raise Exception( "读取的数组数量不允许大于19" )

		readCount = len(length)
		_PLCCommand = bytearray(19 + readCount * 12)
		# ======================================================================================
		_PLCCommand[0] = 0x03                                                # 报文头
		_PLCCommand[1] = 0x00
		_PLCCommand[2] = len(_PLCCommand) // 256                           # 长度
		_PLCCommand[3] = len(_PLCCommand) % 256
		_PLCCommand[4] = 0x02                                                # 固定
		_PLCCommand[5] = 0xF0
		_PLCCommand[6] = 0x80
		_PLCCommand[7] = 0x32                                                # 协议标识
		_PLCCommand[8] = 0x01                                                # 命令：发
		_PLCCommand[9] = 0x00                                                # redundancy identification (reserved): 0x0000;
		_PLCCommand[10] = 0x00                                               # protocol data unit reference; it’s increased by request event;
		_PLCCommand[11] = 0x00
		_PLCCommand[12] = 0x01                                               # 参数命令数据总长度
		_PLCCommand[13] = (len(_PLCCommand) - 17) // 256
		_PLCCommand[14] = (len(_PLCCommand) - 17) % 256
		_PLCCommand[15] = 0x00                                               # 读取内部数据时为00，读取CPU型号为Data数据长度
		_PLCCommand[16] = 0x00
		# =====================================================================================
		_PLCCommand[17] = 0x04                                               # 读写指令，04读，05写
		_PLCCommand[18] = readCount                                    # 读取数据块个数

		for ii in range(readCount):
			#===========================================================================================
			# 指定有效值类型
			_PLCCommand[19 + ii * 12] = 0x12
			# 接下来本次地址访问长度
			_PLCCommand[20 + ii * 12] = 0x0A
			# 语法标记，ANY
			_PLCCommand[21 + ii * 12] = 0x10
			# 按字为单位
			_PLCCommand[22 + ii * 12] = 0x02
			# 访问数据的个数
			_PLCCommand[23 + ii * 12] = length[ii] // 256
			_PLCCommand[24 + ii * 12] = length[ii] % 256
			# DB块编号，如果访问的是DB块的话
			_PLCCommand[25 + ii * 12] = address[ii].Content3 // 256
			_PLCCommand[26 + ii * 12] = address[ii].Content3 % 256
			# 访问数据类型
			_PLCCommand[27 + ii * 12] = address[ii].Content1
			# 偏移位置
			_PLCCommand[28 + ii * 12] = address[ii].Content2 // 256 // 256 % 256
			_PLCCommand[29 + ii * 12] = address[ii].Content2 // 256 % 256
			_PLCCommand[30 + ii * 12] = address[ii].Content2 % 256

		return OperateResult.CreateSuccessResult( _PLCCommand )
	@staticmethod
	def BuildBitReadCommand( address ):
		'''生成一个位读取数据指令头的通用方法'''
		analysis = SiemensS7Net.AnalysisAddress( address )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )

		_PLCCommand = bytearray(31)
		# 报文头
		_PLCCommand[0] = 0x03
		_PLCCommand[1] = 0x00
		# 长度
		_PLCCommand[2] = len(_PLCCommand) // 256
		_PLCCommand[3] = len(_PLCCommand) % 256
		# 固定
		_PLCCommand[4] = 0x02
		_PLCCommand[5] = 0xF0
		_PLCCommand[6] = 0x80
		_PLCCommand[7] = 0x32
		# 命令：发
		_PLCCommand[8] = 0x01
		# 标识序列号
		_PLCCommand[9] = 0x00
		_PLCCommand[10] = 0x00
		_PLCCommand[11] = 0x00
		_PLCCommand[12] = 0x01
		# 命令数据总长度
		_PLCCommand[13] = (len(_PLCCommand) - 17) // 256
		_PLCCommand[14] = (len(_PLCCommand) - 17) % 256

		_PLCCommand[15] = 0x00
		_PLCCommand[16] = 0x00

		# 命令起始符
		_PLCCommand[17] = 0x04
		# 读取数据块个数
		_PLCCommand[18] = 0x01

		#===========================================================================================
		# 读取地址的前缀
		_PLCCommand[19] = 0x12
		_PLCCommand[20] = 0x0A
		_PLCCommand[21] = 0x10
		# 读取的数据时位
		_PLCCommand[22] = 0x01
		# 访问数据的个数
		_PLCCommand[23] = 0x00
		_PLCCommand[24] = 0x01
		# DB块编号，如果访问的是DB块的话
		_PLCCommand[25] = analysis.Content3 // 256
		_PLCCommand[26] = analysis.Content3 % 256
		# 访问数据类型
		_PLCCommand[27] = analysis.Content1
		# 偏移位置
		_PLCCommand[28] = analysis.Content2 // 256 // 256 % 256
		_PLCCommand[29] = analysis.Content2 // 256 % 256
		_PLCCommand[30] = analysis.Content2 % 256

		return OperateResult.CreateSuccessResult( _PLCCommand )
	@staticmethod
	def BuildWriteByteCommand( address, data ):
		'''生成一个写入字节数据的指令'''
		if data == None : data = bytearray(0)
		analysis = SiemensS7Net.AnalysisAddress( address )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult(analysis)

		_PLCCommand = bytearray(35 + len(data))
		_PLCCommand[0] = 0x03
		_PLCCommand[1] = 0x00
		# 长度
		_PLCCommand[2] = (35 + len(data)) // 256
		_PLCCommand[3] = (35 + len(data)) % 256
		# 固定
		_PLCCommand[4] = 0x02
		_PLCCommand[5] = 0xF0
		_PLCCommand[6] = 0x80
		_PLCCommand[7] = 0x32
		# 命令 发
		_PLCCommand[8] = 0x01
		# 标识序列号
		_PLCCommand[9] = 0x00
		_PLCCommand[10] = 0x00
		_PLCCommand[11] = 0x00
		_PLCCommand[12] = 0x01
		# 固定
		_PLCCommand[13] = 0x00
		_PLCCommand[14] = 0x0E
		# 写入长度+4
		_PLCCommand[15] = (4 + len(data)) // 256
		_PLCCommand[16] = (4 + len(data)) % 256
		# 读写指令
		_PLCCommand[17] = 0x05
		# 写入数据块个数
		_PLCCommand[18] = 0x01
		# 固定，返回数据长度
		_PLCCommand[19] = 0x12
		_PLCCommand[20] = 0x0A
		_PLCCommand[21] = 0x10
		# 写入方式，1是按位，2是按字
		_PLCCommand[22] = 0x02
		# 写入数据的个数
		_PLCCommand[23] = len(data) // 256
		_PLCCommand[24] = len(data) % 256
		# DB块编号，如果访问的是DB块的话
		_PLCCommand[25] = analysis.Content3 // 256
		_PLCCommand[26] = analysis.Content3 % 256
		# 写入数据的类型
		_PLCCommand[27] = analysis.Content1
		# 偏移位置
		_PLCCommand[28] = analysis.Content2 // 256 // 256 % 256
		_PLCCommand[29] = analysis.Content2 // 256 % 256
		_PLCCommand[30] = analysis.Content2 % 256
		# 按字写入
		_PLCCommand[31] = 0x00
		_PLCCommand[32] = 0x04
		# 按位计算的长度
		_PLCCommand[33] = len(data) * 8 // 256
		_PLCCommand[34] = len(data) * 8 % 256

		_PLCCommand[35:] = data

		return OperateResult.CreateSuccessResult(_PLCCommand)
	@staticmethod
	def BuildWriteBitCommand( address, data ):
		analysis = SiemensS7Net.AnalysisAddress( address )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult(analysis)

		buffer = bytearray(1)
		if data == True : buffer[0] = 0x01

		_PLCCommand = bytearray(35 + len(buffer))
		_PLCCommand[0] = 0x03
		_PLCCommand[1] = 0x00
		# 长度
		_PLCCommand[2] = (35 + len(buffer)) // 256
		_PLCCommand[3] = (35 + len(buffer)) % 256
		# 固定
		_PLCCommand[4] = 0x02
		_PLCCommand[5] = 0xF0
		_PLCCommand[6] = 0x80
		_PLCCommand[7] = 0x32
		# 命令 发
		_PLCCommand[8] = 0x01
		# 标识序列号
		_PLCCommand[9] = 0x00
		_PLCCommand[10] = 0x00
		_PLCCommand[11] = 0x00
		_PLCCommand[12] = 0x01
		# 固定
		_PLCCommand[13] = 0x00
		_PLCCommand[14] = 0x0E
		# 写入长度+4
		_PLCCommand[15] = (4 + len(buffer)) // 256
		_PLCCommand[16] = (4 + len(buffer)) % 256
		# 命令起始符
		_PLCCommand[17] = 0x05
		# 写入数据块个数
		_PLCCommand[18] = 0x01
		_PLCCommand[19] = 0x12
		_PLCCommand[20] = 0x0A
		_PLCCommand[21] = 0x10
		# 写入方式，1是按位，2是按字
		_PLCCommand[22] = 0x01
		# 写入数据的个数
		_PLCCommand[23] = len(buffer) // 256
		_PLCCommand[24] = len(buffer) % 256
		# DB块编号，如果访问的是DB块的话
		_PLCCommand[25] = analysis.Content3 // 256
		_PLCCommand[26] = analysis.Content3 % 256
		# 写入数据的类型
		_PLCCommand[27] = analysis.Content1
		# 偏移位置
		_PLCCommand[28] = analysis.Content2 // 256 // 256
		_PLCCommand[29] = analysis.Content2 // 256
		_PLCCommand[30] = analysis.Content2 % 256
		# 按位写入
		_PLCCommand[31] = 0x00
		_PLCCommand[32] = 0x03
		# 按位计算的长度
		_PLCCommand[33] = len(buffer) // 256
		_PLCCommand[34] = len(buffer) % 256

		_PLCCommand[35:] = buffer

		return OperateResult.CreateSuccessResult(_PLCCommand)
	def InitializationOnConnect( self, socket ):
		'''连接上服务器后需要进行的二次握手操作'''
		# msg = SoftBasic.ByteToHexString(self.plcHead1, ' ')
		# 第一次握手
		read_first = self.ReadFromCoreServerBase( socket, self.plcHead1 )
		if read_first.IsSuccess == False : return read_first

		# 第二次握手
		read_second = self.ReadFromCoreServerBase( socket, self.plcHead2 )
		if read_second.IsSuccess == False : return read_second

		# 返回成功的信号
		return OperateResult.CreateSuccessResult( )
	def ReadOrderNumber( self ):
		'''从PLC读取订货号信息'''
		read = self.ReadFromCoreServer( self.plcOrderNumber )
		if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

		return OperateResult.CreateSuccessResult( read.Content[71:92].decode('ascii') )
	def __ReadBase( self, address, length ):
		'''基础的读取方法，外界不应该调用本方法'''
		command = SiemensS7Net.BuildReadCommand( address, length )
		if command.IsSuccess == False : return command

		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return read

		# 分析结果
		receiveCount = 0
		for i in range(len(length)):
			receiveCount += length[i]

		if len(read.Content) >= 21 and read.Content[20] == len(length) :
			buffer = bytearray(receiveCount)
			kk = 0
			ll = 0
			ii = 21
			while ii < len(read.Content):
				if ii + 1 < len(read.Content):
					if read.Content[ii] == 0xFF and read.Content[ii + 1] == 0x04:
						# 有数据
						buffer[ll : ll + length[kk]] = read.Content[ii+4 : ii+4+length[kk]]
						ii += length[kk] + 3
						ll += length[kk]
						kk += 1
				ii += 1
			return OperateResult.CreateSuccessResult( buffer )
		else :
			result = OperateResult()
			result.ErrorCode = read.ErrorCode
			result.Message = "数据块长度校验失败"
			return result
	
	def Read( self, address, length ):
		'''从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100以字节为单位'''
		if type(address) == list and type(length) == list:
			addressResult = []
			for i in range(length):
				tmp = SiemensS7Net.AnalysisAddress( address[i] )
				if tmp.IsSuccess == False : return OperateResult.CreateFailedResult( addressResult[i] )

				addressResult.append( tmp )
			return self.__ReadBase( addressResult, length )
		else:
			addressResult = SiemensS7Net.AnalysisAddress( address )
			if addressResult.IsSuccess == False : return OperateResult.CreateFailedResult( addressResult )

			bytesContent = bytearray()
			alreadyFinished = 0
			while alreadyFinished < length :
				readLength = min( length - alreadyFinished, 200 )
				read = self.__ReadBase( [ addressResult ], [ readLength ] )
				if read.IsSuccess == True :
					bytesContent.extend( read.Content )
				else:
					return read

				alreadyFinished += readLength
				addressResult.Content2 += readLength * 8

			return OperateResult.CreateSuccessResult( bytesContent )
	def __ReadBitFromPLC( self, address ):
		'''从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以位为单位'''
		# 指令生成
		command = SiemensS7Net.BuildBitReadCommand( address )
		if command.IsSuccess == False : return OperateResult.CreateFailedResult( command )

		# 核心交互
		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return read

		# 分析结果
		receiveCount = 1
		if len(read.Content) >= 21 and read.Content[20] == 1 :
			buffer = bytearray(receiveCount)
			if 22 < len(read.Content) :
				if read.Content[21] == 0xFF and read.Content[22] == 0x03:
					# 有数据
					buffer[0] = read.Content[25]
			return OperateResult.CreateSuccessResult( buffer )
		else:
			result = OperateResult()
			result.ErrorCode = read.ErrorCode
			result.Message = "数据块长度校验失败"
			return result
	def ReadBool( self, address ):
		'''读取指定地址的bool数据'''
		return self.GetBoolResultFromBytes( self.__ReadBitFromPLC( address ) )
	def ReadByte( self, address ):
		'''读取指定地址的byte数据'''
		return self.GetByteResultFromBytes( self.Read( address, 1 ) )
	def __WriteBase( self, entireValue ):
		'''基础的写入数据的操作支持'''
		write = self.ReadFromCoreServer( entireValue )
		if write.IsSuccess == False : return write

		if write.Content[len(write.Content) - 1] != 0xFF :
			# 写入异常
			return OperateResult( msg = "写入数据异常", err = write.Content[write.Content.Length - 1])
		else:
			return OperateResult.CreateSuccessResult( )
	def Write( self, address, value ):
		'''将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位'''
		command = self.BuildWriteByteCommand( address, value )
		if command.IsSuccess == False : return command

		return self.__WriteBase( command.Content )
	def WriteBool( self, address, value ):
		'''写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0'''
		# 生成指令
		command = SiemensS7Net.BuildWriteBitCommand( address, value )
		if command.IsSuccess == False : return command

		return self.__WriteBase( command.Content )
	def WriteByte( self, address, value ):
		'''向PLC中写入byte数据，返回值说明'''
		return self.Write( address, [value] )
class SiemensFetchWriteNet(NetworkDeviceBase):
	'''使用了Fetch/Write协议来和西门子进行通讯，该种方法需要在PLC侧进行一些配置'''
	def __init__( self, ipAddress = '127.0.0.1', port = 1000 ):
		''' 实例化一个西门子的Fetch/Write协议的通讯对象，可以指定ip地址及端口号'''
		self.ipAddress = ipAddress
		self.port = port
		self.WordLength = 2
	@staticmethod
	def CalculateAddressStarted( address = "M100" ):
		'''计算特殊的地址信息'''
		if address.find( '.' ) < 0:
			return int( address )
		else:
			temp = address.split( '.' )
			return int( temp[0] )
	@staticmethod
	def AnalysisAddress( address = "M100" ):
		'''解析数据地址，解析出地址类型，起始地址，DB块的地址'''
		result = OperateResult( )
		try:
			result.Content3 = 0
			if address[0] == 'I':
				result.Content1 = 0x03
				result.Content2 = SiemensFetchWriteNet.CalculateAddressStarted( address[1:] )
			elif address[0] == 'Q':
				result.Content1 = 0x04
				result.Content2 = SiemensFetchWriteNet.CalculateAddressStarted( address[1:] )
			elif address[0] == 'M':
				result.Content1 = 0x02
				result.Content2 = SiemensFetchWriteNet.CalculateAddressStarted( address[1:] )
			elif address[0] == 'D' or address.startswith("DB"):
				result.Content1 = 0x01
				adds = address.split( '.' )
				if address[1] == 'B':
					result.Content3 = int( adds[0][2:] )
				else:
					result.Content3 = int( adds[0][1:] )

				if result.Content3 > 255:
					result.Message = "DB块数据无法大于255"
					return result

				result.Content2 = SiemensFetchWriteNet.CalculateAddressStarted( address[ address.find( '.' ) + 1:] )
			elif address[0] == 'T':
				result.Content1 = 0x07
				result.Content2 = SiemensFetchWriteNet.CalculateAddressStarted( address[1:] )
			elif address[0] == 'C':
				result.Content1 = 0x06
				result.Content2 = SiemensFetchWriteNet.CalculateAddressStarted( address[1:])
			else:
				result.Message = StringResources.NotSupportedDataType()
				result.Content1 = 0
				result.Content2 = 0
				result.Content3 = 0
				return result
		except Exception as ex:
			result.Message = str(ex)
			return result

		result.IsSuccess = True
		return result
	@staticmethod
	def BuildReadCommand( address, count ):
		'''生成一个读取字数据指令头的通用方法'''
		result = OperateResult( )

		analysis = SiemensFetchWriteNet.AnalysisAddress( address )
		if analysis.IsSuccess == False :
			result.CopyErrorFromOther( analysis )
			return result

		_PLCCommand = bytearray(16)
		_PLCCommand[0] = 0x53
		_PLCCommand[1] = 0x35
		_PLCCommand[2] = 0x10
		_PLCCommand[3] = 0x01
		_PLCCommand[4] = 0x03
		_PLCCommand[5] = 0x05
		_PLCCommand[6] = 0x03
		_PLCCommand[7] = 0x08

		# 指定数据区
		_PLCCommand[8] = analysis.Content1
		_PLCCommand[9] = analysis.Content3

		# 指定数据地址
		_PLCCommand[10] =analysis.Content2 // 256
		_PLCCommand[11] = analysis.Content2 % 256

		if analysis.Content1 == 0x01 or analysis.Content1 == 0x06 or analysis.Content1 == 0x07:
			if count % 2 != 0:
				result.Message = "读取的数据长度必须为偶数"
				return result
			else:
				# 指定数据长度
				_PLCCommand[12] = count // 2 // 256
				_PLCCommand[13] = count // 2 % 256
		else:
			# 指定数据长度
			_PLCCommand[12] = count // 256
			_PLCCommand[13] = count % 256

		_PLCCommand[14] = 0xff
		_PLCCommand[15] = 0x02

		result.Content = _PLCCommand
		result.IsSuccess = True
		return result
	@staticmethod
	def BuildWriteCommand( address, data ):
		'''生成一个写入字节数据的指令'''
		if data == None : data = bytearray(0)
		result = OperateResult( )

		analysis = SiemensFetchWriteNet.AnalysisAddress( address )
		if analysis.IsSuccess == False:
			result.CopyErrorFromOther( analysis )
			return result

		_PLCCommand = bytearray(16 + len(data))
		_PLCCommand[0] = 0x53
		_PLCCommand[1] = 0x35
		_PLCCommand[2] = 0x10
		_PLCCommand[3] = 0x01
		_PLCCommand[4] = 0x03
		_PLCCommand[5] = 0x03
		_PLCCommand[6] = 0x03
		_PLCCommand[7] = 0x08

		# 指定数据区
		_PLCCommand[8] = analysis.Content1
		_PLCCommand[9] = analysis.Content3

		# 指定数据地址
		_PLCCommand[10] = analysis.Content2 // 256
		_PLCCommand[11] = analysis.Content2 % 256

		if analysis.Content1 == 0x01 or analysis.Content1 == 0x06 or analysis.Content1 == 0x07:
			if data.Length % 2 != 0:
				result.Message = "写入的数据长度必须为偶数"
				return result
			else:
				# 指定数据长度
				_PLCCommand[12] = data.Length // 2 // 256
				_PLCCommand[13] = data.Length // 2 % 256
		else:
			# 指定数据长度
			_PLCCommand[12] = data.Length // 256
			_PLCCommand[13] = data.Length % 256
		_PLCCommand[14] = 0xff
		_PLCCommand[15] = 0x02

		# 放置数据
		_PLCCommand[16:16+len(data)] = data

		result.Content = _PLCCommand
		result.IsSuccess = True
		return result
	def Read( self, address, length ):
		'''从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100，以字节为单位'''
		# 指令解析 -> Instruction parsing
		command = SiemensFetchWriteNet.BuildReadCommand( address, length )
		if command.IsSuccess == False : return command

		# 核心交互 -> Core Interactions
		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return read

		# 错误码验证 -> Error code Verification
		if read.Content[8] != 0x00 : return OperateResult(read.Content[8],"发生了异常，具体信息查找Fetch/Write协议文档")

		# 读取正确 -> Read Right
		buffer = bytearray(len(read.Content) - 16)
		buffer[0:len(buffer)] = read.Content[16:16+len(buffer)]
		return OperateResult.CreateSuccessResult( buffer )
	def ReadByte( self, address ):
		'''读取指定地址的byte数据'''
		return self.GetByteResultFromBytes( self.Read( address, 1 ) )
	def Write( self, address, value ):
		'''将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位'''
		# 指令解析 -> Instruction parsing
		command = SiemensFetchWriteNet.BuildWriteCommand( address, value )
		if command.IsSuccess == False : return command

		# 核心交互 -> Core Interactions
		write = self.ReadFromCoreServer( command.Content )
		if write.IsSuccess == False : return write

		# 错误码验证 -> Error code Verification
		if (write.Content[8] != 0x00) : OperateResult(err = write.Content[8], msg = "西门子PLC写入失败！")

		# 写入成功 -> Write Right
		return OperateResult.CreateSuccessResult( )
	def WriteBool( self, address, values):
		'''向PLC中写入byte数据，返回是否写入成功 -> Writes byte data to the PLC and returns whether the write succeeded'''
		if type(values) == list:
			return self.Write( address, SoftBasic.BoolArrayToByte( values ) )
		else:
			return self.WriteBool( address, [ values ] )

# Omron PLC 通讯类
class OmronFinsDataType:
	'''欧姆龙的Fins协议的数据类型'''
	BitCode = 0
	WordCode = 0
	def __init__(self, bitCode = 0, wordCode = 0):
		'''实例化一个Fins的数据类型'''
		self.BitCode = bitCode
		self.WordCode = wordCode
	@staticmethod
	def DM():
		'''DM Area'''
		return OmronFinsDataType( 0x02, 0x82 )
	@staticmethod
	def CIO():
		'''CIO Area'''
		return OmronFinsDataType( 0x30, 0xB0 )
	@staticmethod
	def WR():
		'''Work Area'''
		return OmronFinsDataType( 0x31, 0xB1 )
	@staticmethod
	def HR():
		'''Holding Bit Area'''
		return OmronFinsDataType( 0x32, 0xB2 )
	@staticmethod
	def AR():
		'''Auxiliary Bit Area'''
		return OmronFinsDataType( 0x33, 0xB3 )

class OmronFinsNet(NetworkDoubleBase):
	'''欧姆龙PLC通讯类，采用Fins-Tcp通信协议实现'''
	def __init__(self,ipAddress="127.0.0.1",port = 1000):
		'''实例化一个欧姆龙PLC Fins帧协议的通讯对象'''
		self.ipAddress = ipAddress
		self.port = port
	ICF = 0
	RSV = 0
	GCT = 0
	DNA = 0
	DA1 = 0
	DA2 = 0
	SNA = 0
	SA1 = 0
	SA2 = 0
	SID = 0
	def SetSA1(self, value):
		'''设置SA1的方法'''
		self.SA1 = value
		self.handSingle[19] = value
	handSingle = bytearray([0x46, 0x49, 0x4E, 0x53,0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01])
       
	@staticmethod
	def AnalysisAddress( address, isBit ):
		result = OperateResult( )
		try:
			if address[0] == 'D' or address[0] == 'd':
				# DM区数据
				result.Content1 = OmronFinsDataType.DM
			elif address[0] == 'C' or address[0] == 'c':
				# CIO区数据
				result.Content1 = OmronFinsDataType.CIO
			elif address[0] == 'W' or address[0] == 'w':
				# WR区
				result.Content1 = OmronFinsDataType.WR
			elif address[0] == 'H' or address[0] == 'h':
				# HR区
				result.Content1 = OmronFinsDataType.HR
			elif address[0] == 'A' or address[0] == 'a':
				# AR区
				result.Content1 = OmronFinsDataType.AR
			else:
				raise RuntimeError( StringResources.NotSupportedDataType() )

			if isBit == True:
				# 位操作
				splits = address[1:].split('.')
				addr = int( splits[0] )
				result.Content2 = bytearray(3)
				result.Content2[0] = struct.pack('<H', addr )[1]
				result.Content2[1] = struct.pack('<H', addr )[0]

				if len(splits) > 1:
					result.Content2[2] = int(splits[1])
					if result.Content2[2] > 15:
						raise RuntimeError( "欧姆龙位地址必须0-15之间" )
			else:
				# 字操作
				addr = int( address[1:] )
				result.Content2 = bytearray(3)
				result.Content2[0] = struct.pack('<H', addr )[1]
				result.Content2[1] = struct.pack('<H', addr )[0]
		except Exception as ex:
			result.Message = str(ex)
			return result

		result.IsSuccess = True
		return result
	@staticmethod
	def ResponseValidAnalysis( response, isRead ):
		# 数据有效性分析
		if len(response) >= 16:
			# 提取错误码
			buffer = bytearray(4)
			buffer[0] = response[15]
			buffer[1] = response[14]
			buffer[2] = response[13]
			buffer[3] = response[12]

			err = struct.unpack( '<i' , buffer )[0]
			if err > 0 : return OperateResult( err = err, msg = OmronFinsNet.GetStatusDescription( err ) )

			if response.Length >= 30:
				err = response[28] * 256 + response[29]
				if err > 0 : return OperateResult( err = err, msg = "欧姆龙数据接收出错" )

				if isRead == False : return OperateResult.CreateSuccessResult( bytearray(0) )
				# 读取操作
				content = bytearray(len(response) - 30)
				if len(content) > 0 :
					content[0:len(content)] = response[30:]
				return OperateResult.CreateSuccessResult( content )

		return OperateResult( msg = "欧姆龙数据接收出错" )
	@staticmethod
	def GetStatusDescription( err ):
		'''获取错误信息的字符串描述文本'''
		if err == 0: return StringResources.OmronStatus0()
		elif err == 1: return StringResources.OmronStatus1()
		elif err == 2: return StringResources.OmronStatus2()
		elif err == 3: return StringResources.OmronStatus3()
		elif err == 20: return StringResources.OmronStatus20()
		elif err == 21: return StringResources.OmronStatus21()
		elif err == 22: return StringResources.OmronStatus22()
		elif err == 23: return StringResources.OmronStatus23()
		elif err == 24: return StringResources.OmronStatus24()
		elif err == 25: return StringResources.OmronStatus25()
		else: return StringResources.UnknownError()
	def PackCommand( self, cmd ):
		'''将普通的指令打包成完整的指令'''
		buffer = bytearray(26 + len(cmd))
		buffer[0:4] = self.handSingle[0:4]

		tmp = struct.pack('>i', len(buffer) - 8 )
		buffer[4:8] = tmp
		buffer[11] = 0x02
		buffer[16] = self.ICF
		buffer[17] = self.RSV
		buffer[18] = self.GCT
		buffer[19] = self.DNA
		buffer[20] = self.DA1
		buffer[21] = self.DA2
		buffer[22] = self.SNA
		buffer[23] = self.SA1
		buffer[24] = self.SA2
		buffer[25] = self.SID
		buffer[26:] = cmd
		return buffer
	def BuildReadCommand( self, address, length , isBit):
		'''根据类型地址长度确认需要读取的指令头'''
		analysis = OmronFinsNet.AnalysisAddress( address, isBit )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )

		_PLCCommand = bytearray(8)
		_PLCCommand[0] = 0x01
		_PLCCommand[1] = 0x01
		if isBit == True:
			_PLCCommand[2] = analysis.Content1.BitCode
		else:
			_PLCCommand[2] = analysis.Content1.WordCode

		_PLCCommand[3:6] = analysis.Content2
		_PLCCommand[6] = length / 256
		_PLCCommand[7] = length % 256

		return OperateResult.CreateSuccessResult( self.PackCommand( _PLCCommand ) )
	def BuildWriteCommand( self, address, value, isBit ):
		'''根据类型地址以及需要写入的数据来生成指令头'''
		analysis = self.AnalysisAddress( address, isBit )
		if analysis.IsSuccess == False : return OperateResult.CreateFailedResult( analysis )

		_PLCCommand = bytearray(8 + len(value))
		_PLCCommand[0] = 0x01
		_PLCCommand[1] = 0x02

		if isBit == True:
			_PLCCommand[2] = analysis.Content1.BitCode
		else:
			_PLCCommand[2] = analysis.Content1.WordCode

		_PLCCommand[3:6] = analysis.Content2
		if isBit == True:
			_PLCCommand[6] = len(value) // 256
			_PLCCommand[7] = len(value) % 256
		else:
			_PLCCommand[6] = len(value) // 2 // 256
			_PLCCommand[7] = len(value) // 2 % 256

		_PLCCommand[8:] = value
		return OperateResult.CreateSuccessResult( self.PackCommand( _PLCCommand ) )
	def InitializationOnConnect( self, socket ):
		'''在连接上欧姆龙PLC后，需要进行一步握手协议'''
		# 握手信号
		read = self.ReadFromCoreServerBase( socket, self.handSingle )
		if read.IsSuccess == False : return read

		# 检查返回的状态
		buffer = bytearray(4)
		buffer[0] = read.Content2[7]
		buffer[1] = read.Content2[6]
		buffer[2] = read.Content2[5]
		buffer[3] = read.Content2[4]

		status = struct.unpack( '<i',buffer )[0]
		if status != 0 : return OperateResult( err = status, msg = OmronFinsNet.GetStatusDescription( status ) )

		# 提取PLC的节点地址
		if read.Content2.Length >= 16 : self.DA1 = read.Content2[15]

		return OperateResult.CreateSuccessResult( )
	def Read( self, address, length ):
		'''从欧姆龙PLC中读取想要的数据，返回读取结果，读取单位为字'''
		# 获取指令
		command = self.BuildReadCommand( address, length, False )
		if command.IsSuccess == False : return OperateResult.CreateFailedResult( command )

		# 核心数据交互
		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

		# 数据有效性分析
		valid = OmronFinsNet.ResponseValidAnalysis( read.Content, True )
		if valid.IsSuccess == False : return OperateResult.CreateFailedResult( valid )

		# 读取到了正确的数据
		return OperateResult.CreateSuccessResult( valid.Content )
	def ReadBool( self, address, length = None ):
		'''从欧姆龙PLC中批量读取位软元件，返回读取结果'''
		if length == None:
			read = self.ReadBool( address, 1 )
			if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

			return OperateResult.CreateSuccessResult( read.Content[0] )
		else:
			# 获取指令
			command = self.BuildReadCommand( address, length, True )
			if command.IsSuccess == False : return OperateResult.CreateFailedResult( command )

			# 核心数据交互
			read = self.ReadFromCoreServer( command.Content )
			if read.IsSuccess == False : return OperateResult.CreateFailedResult( read )

			# 数据有效性分析
			valid = OmronFinsNet.ResponseValidAnalysis( read.Content, True )
			if valid.IsSuccess == False : return OperateResult.CreateFailedResult( valid )

			# 返回正确的数据信息
			content = []
			for i in range(len(read.Content)):
				if read.Content[i] == 0x01:
					content.append(True)
				else:
					content.append(False)
			return OperateResult.CreateSuccessResult( content )
	def Write( self, address, value ):
		'''向PLC中位软元件写入bool数组，返回值说明，比如你写入D100,values[0]对应D100.0'''
		# 获取指令
		command = self.BuildWriteCommand( address, value, False )
		if command.IsSuccess == False : return command

		# 核心数据交互
		read = self.ReadFromCoreServer( command.Content )
		if read.IsSuccess == False : return read

		# 数据有效性分析
		valid = OmronFinsNet.ResponseValidAnalysis( read.Content, False )
		if valid.IsSuccess == False : return valid

		# 成功
		return OperateResult.CreateSuccessResult( )
	def WriteBool( self, address, values ):
		'''向PLC中位软元件写入bool数组，返回值说明，比如你写入D100,values[0]对应D100.0'''
		if type(values) == list:
			# 获取指令
			content = bytearray(len(values))
			for i in range(len(values)):
				if values[i] == True:
					content[i] = 0x01
				else:
					content[i] = 0x00
			command = self.BuildWriteCommand( address, content, True )
			if command.IsSuccess == False : return command

			# 核心数据交互
			read = self.ReadFromCoreServer( command.Content )
			if read.IsSuccess == False : return read

			# 数据有效性分析
			valid = OmronFinsNet.ResponseValidAnalysis( read.Content, False )
			if valid.IsSuccess == False : return valid

			# 写入成功
			return OperateResult.CreateSuccessResult( )
		else:
			return self.WriteBool( address, [values] )

# NetSimplifyClient类
class NetSimplifyClient(NetworkDoubleBase):
	'''异步访问数据的客户端类，用于向服务器请求一些确定的数据信息'''
	def __init__(self, ipAddress, port):
		'''实例化一个客户端的对象，用于和服务器通信'''
		self.iNetMessage = HslMessage()
		self.byteTransform = RegularByteTransform()
		self.ipAddress = ipAddress
		self.port = port
	def ReadBytesFromServer( self, customer, send = None):
		'''客户端向服务器进行请求，请求字节数据'''
		return self.__ReadFromServerBase( HslProtocol.CommandBytes( customer, self.Token, send))

	def ReadStringFromServer( self, customer, send = None):
		'''客户端向服务器进行请求，请求字符串数据'''
		read = self.__ReadFromServerBase(  HslProtocol.CommandString( customer, self.Token, send))
		if read.IsSuccess == False:
			return OperateResult.CreateFailedResult( read )
		
		return OperateResult.CreateSuccessResult( read.Content.decode('utf-16') )

	def __ReadFromServerBase( self, send):
		'''需要发送的底层数据'''
		read = self.ReadFromCoreServer( send )
		if read.IsSuccess == False:
			return read

		headBytes = bytearray(HslProtocol.HeadByteLength())
		contentBytes = bytearray(len(read.Content) - HslProtocol.HeadByteLength())

		headBytes[0:HslProtocol.HeadByteLength()] = read.Content[0:HslProtocol.HeadByteLength()]
		if len(contentBytes) > 0:
			contentBytes[0:len(contentBytes)] = read.Content[HslProtocol.HeadByteLength():len(read.Content)]

		contentBytes = HslProtocol.CommandAnalysis( headBytes, contentBytes )
		return OperateResult.CreateSuccessResult( contentBytes )


class AppSession:
	'''网络会话信息'''
	IpAddress = "127.0.0.1"
	Port = 12345
	LoginAlias = ""
	HeartTime = None
	ClientType = ""
	ClientUniqueID = ""
	BytesHead = bytearray(32)
	BytesContent = bytearray(0)
	KeyGroup = ""
	WorkSocket = socket.socket()
	HybirdLockSend = threading.Lock()
	def __init__( self ):
		self.ClientUniqueID = SoftBasic.GetUniqueStringByGuidAndRandom()
		self.HeartTime = datetime.datetime.now()
	def Clear( self ):
		self.BytesHead = bytearray(HslProtocol.HeadByteLength())
		self.BytesContent = None

class NetworkXBase(NetworkBase):
	'''多功能网络类的基类'''
	ThreadBack = None
	def __init__(self):
		return
	def SendBytesAsync( self, session, content ):
		'''发送数据的方法'''
		if content == None : return
			
		session.HybirdLockSend.acquire()
		self.Send( session.WorkSocket, content )
		session.HybirdLockSend.release()
	def ThreadBackground( self, session ):
		while True:
			if session.WorkSocket == None : break
			readHeadBytes = self.Receive(session.WorkSocket,HslProtocol.HeadByteLength())
			if readHeadBytes.IsSuccess == False :
				self.SocketReceiveException( session )
				return

			length = struct.unpack( '<i', readHeadBytes.Content[28:32])[0]
			readContent = self.Receive(session.WorkSocket,length)
			if readContent.IsSuccess == False :
				self.SocketReceiveException( session )
				return

			if self.CheckRemoteToken( readHeadBytes.Content ):
				head = readHeadBytes.Content
				content = HslProtocol.CommandAnalysis(head,readContent.Content)
				protocol = struct.unpack('<i', head[0:4])[0]
				customer = struct.unpack('<i', head[4:8])[0]

				self.DataProcessingCenter(session,protocol,customer,content)
			else:
				self.AppSessionRemoteClose( session )
	def BeginReceiveBackground( self, session ):
		ThreadBack = threading.Thread(target=self.ThreadBackground,args=[session])
		ThreadBack.start()
	def DataProcessingCenter( self, session, protocol, customer, content ):
		'''数据处理中心，应该继承重写'''
		return
	def CheckRemoteToken( self, headBytes ):
		'''检查当前的头子节信息的令牌是否是正确的'''
		return SoftBasic.IsTwoBytesEquel( headBytes,12, SoftBasic.TokenToBytes(self.Token), 0, 16 )
	def SocketReceiveException( self, session ):
		'''接收出错的时候进行处理'''
		return
	def AppSessionRemoteClose( self, session ):
		'''当远端的客户端关闭连接时触发'''
		return
	def SendBaseAndCheckReceive( self, socket, headcode, customer, send ):
		'''[自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯'''
		# 数据处理
		send = HslProtocol.CommandBytesBase( headcode, customer, self.Token, send )

		sendResult = self.Send( socket, send )
		if sendResult.IsSuccess == False:  return sendResult

		# 检查对方接收完成
		checkResult = self.ReceiveLong( socket )
		if checkResult.IsSuccess == False: return checkResult

		# 检查长度接收
		if checkResult.Content != len(send):
			self.CloseSocket(socket)
			return OperateResult( msg = "接收的数据数据长度验证失败")

		return checkResult
	def SendBytesAndCheckReceive( self, socket, customer, send ):
		'''[自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯'''
		return self.SendBaseAndCheckReceive( socket, HslProtocol.ProtocolUserBytes(), customer, send )
	def SendStringAndCheckReceive( self, socket, customer, send ):
		'''[自校验] 直接发送字符串数据并确认对方接收完成数据，如果结果异常，则结束通讯'''
		data = SoftBasic.StringToUnicodeBytes(send)

		return self.SendBaseAndCheckReceive( socket, HslProtocol.ProtocolUserString(), customer, data )
	def ReceiveAndCheckBytes( self, socket, timeout ):
		'''[自校验] 接收一条完整的同步数据，包含头子节和内容字节，基础的数据，如果结果异常，则结束通讯'''
		# 30秒超时接收验证
		# if (timeout > 0) ThreadPool.QueueUserWorkItem( new WaitCallback( ThreadPoolCheckTimeOut ), hslTimeOut );

		# 接收头指令
		headResult = self.Receive(socket, HslProtocol.HeadByteLength())
		if headResult.IsSuccess == False:
			return OperateResult.CreateFailedResult(headResult)

		# 检查令牌
		if self.CheckRemoteToken(headResult.Content) == False:
			self.CloseSocket(socket)
			return OperateResult( msg = StringResources.TokenCheckFailed() )

		contentLength = struct.unpack( '<i', headResult.Content[(HslProtocol.HeadByteLength() - 4):])[0]
		# 接收内容
		contentResult = self.Receive(socket, contentLength)
		if contentResult.IsSuccess == False:
			return OperateResult.CreateFailedResult( contentResult )

		# 返回成功信息
		checkResult = self.SendLong(socket, HslProtocol.HeadByteLength() + contentLength)
		if checkResult.IsSuccess == False:
			return OperateResult.CreateFailedResult( checkResult )

		head = headResult.Content
		content = contentResult.Content
		content = HslProtocol.CommandAnalysis(head, content)
		return OperateResult.CreateSuccessResult(head, content)
	def ReceiveStringContentFromSocket( self, socket ):
		'''[自校验] 从网络中接收一个字符串数据，如果结果异常，则结束通讯'''
		receive = self.ReceiveAndCheckBytes(socket, 10000)
		if receive.IsSuccess == False: return OperateResult.CreateFailedResult(receive)

		# 检查是否是字符串信息
		if struct.unpack('<i',receive.Content1[0:4])[0] != HslProtocol.ProtocolUserString():
			self.CloseSocket(socket)
			return OperateResult( msg = "ReceiveStringContentFromSocket异常" )

		if receive.Content2 == None: receive.Content2 = bytearray(0)
		# 分析数据
		return OperateResult.CreateSuccessResult(struct.unpack('<i', receive.Content1[4:8])[0], receive.Content2.decode('utf-16'))
	def ReceiveBytesContentFromSocket( self, socket ):
		'''[自校验] 从网络中接收一串字节数据，如果结果异常，则结束通讯'''
		receive = self.ReceiveAndCheckBytes( socket, 10000 )
		if receive.IsSuccess == False: return OperateResult.CreateFailedResult(receive)

		# 检查是否是字节信息
		if struct.unpack('<i', receive.Content1[0:4])[0] != HslProtocol.ProtocolUserBytes():
			self.CloseSocket(socket)
			return OperateResult( msg = "字节内容检查失败" )
		
		# 分析数据
		return OperateResult.CreateSuccessResult( struct.unpack('<i', receive.Content1[4:8])[0], receive.Content2 )
	def ReceiveLong( self, socket ):
		'''从网络中接收Long数据'''
		read = self.Receive(socket, 8)
		if read.IsSuccess == False: return OperateResult.CreateFailedResult(read)

		return OperateResult.CreateSuccessResult(struct.unpack('<Q', read.Content)[0])
	def SendLong( self, socket, value ):
		'''将Long数据发送到套接字'''
		return self.Send( socket, struct.pack( '<Q', value ) )
	def CloseSocket(self, socket):
		'''关闭网络'''
		if socket != None:
			socket.close()

class NetPushClient(NetworkXBase):
	'''发布订阅类的客户端，使用指定的关键订阅相关的数据推送信息'''
	IpAddress = "127.0.0.1"
	Port = 12345
	keyWord = "A"
	ReConnectTime = 10
	action = None
	def __init__( self, ipAddress, port, key):
		'''实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字'''
		self.IpAddress = ipAddress
		self.Port = port
		self.keyWord = key
	def DataProcessingCenter( self, session, protocol, customer, content ):
		if protocol == HslProtocol.ProtocolUserString():
			if self.action != None: self.action( self.keyWord, content.decode('utf-16') )
	def SocketReceiveException( self, session ):
		# 发生异常的时候需要进行重新连接
		while True:
			print('NetPushClient wait 10s to reconnect server')
			sleep( self.ReConnectTime )

			if self.CreatePush( ).IsSuccess == True:
				break
	def CreatePush( self, pushCallBack = None ):
		'''创建数据推送服务'''
		if pushCallBack == None:
			if self.CoreSocket != None: self.CoreSocket.close( )
			connect = self.CreateSocketAndConnect( self.IpAddress, self.Port, 5000 )
			if connect.IsSuccess == False: return connect

			send = self.SendStringAndCheckReceive( connect.Content, 0, self.keyWord )
			if send.IsSuccess == False: return send

			receive = self.ReceiveStringContentFromSocket( connect.Content )
			if receive.IsSuccess == False : return receive

			if receive.Content1 != 0: return OperateResult( msg = receive.Content2 )

			appSession = AppSession( )
			self.CoreSocket = connect.Content
			appSession.WorkSocket = connect.Content
			self.BeginReceiveBackground( appSession )

			return OperateResult.CreateSuccessResult( )
		else:
			self.action = pushCallBack
			return self.CreatePush( )
	def ClosePush( self ):
		'''关闭消息推送的界面'''
		self.action = None
		if self.CoreSocket != None:
			self.Send(self.CoreSocket, struct.pack('<i', 100 ) )

		self.CloseSocket(self.CoreSocket)
