import string
import uuid
import socket
import struct

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
	def SuccessText():
		return "Success"


class OperateResult:
	'''结果对象类，可以携带额外的数据信息'''
	# 是否成功的标志
	IsSuccess = False
	# 操作返回的错误消息
	Message = StringResources.SuccessText()
	# 错误码
	ErrorCode = 0
	# 返回显示的文本
	def ToMessageShowString(self):
		return StringResources.ErrorCode() + ":" + str(self.ErrorCode) + "\r\n" + StringResources.TextDescription() + ":" + self.Message
	@staticmethod
	def CreateFailedResult(msg):
		'''创建一个失败的结果对象'''
		failed = OperateResult()
		failed.Message = msg
		return failed
	@staticmethod
	def CreateSuccessResult(Content1=None,Content2=None,Content3=None,Content4=None,Content5=None,Content6=None,Content7=None,Content8=None,Content9=None,Content10=None):
		'''创建一个成功的对象'''
		success = OperateResult()
		success.IsSuccess = True
		success.Message = StringResources.SuccessText()
		if(Content2 == None & Content3 == None & Content4 == None & Content5 == None & Content6 == None & Content7 == None & Content8 == None & Content9 == None & Content10 == None) :
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


class INetMessage:
	'''数据消息的基本基类'''
	def ProtocolHeadBytesLength(self):
		'''协议头数据长度，也即是第一次接收的数据长度'''
		return 0
	def GetContentLengthByHeadBytes(self):
		'''二次接收的数据长度'''
		return 0
	def CheckHeadBytesLegal(self,token):
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
		if super().HeadBytes != None:
			return super().HeadBytes[2]*256 + super().HeadBytes[3]-4
		else:
			return 0
	def CheckHeadBytesLegal(self,token):
		'''令牌检查是否成功'''
		if super().HeadBytes != None:
			if( super().HeadBytes[0] == 0x03 & super().HeadBytes[1] == 0x00 ):
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
		if super().HeadBytes != None:
			return super().HeadBytes[4] * 256 + super().HeadBytes[5]
		else:
			return 0
	def CheckHeadBytesLegal(self,token):
		'''令牌检查是否成功'''
		return True
	def GetHeadBytesIdentity(self):
		'''获取头子节里的消息标识'''
		return super().HeadBytes[0] * 256 + super().HeadBytes[1]


class ByteTransform:
	'''数据转换类的基础，提供了一些基础的方法实现.'''
	def TransBool(self, buffer, index ):
		'''将buffer数组转化成bool对象'''
		return buffer[index] != 0x00
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
		data = self.TransByteArray(buffer,index,4)
		return struct.unpack('<i',data)[0]
	def TransInt32Array(self, buffer, index, length ):
		'''从缓存中提取int数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransInt32( buffer, index + 4 * i ))
		return tmp

	def TransUInt32(self, buffer, index ):
		'''从缓存中提取uint结果'''
		data = self.TransByteArray(buffer,index,4)
		return struct.unpack('<I',data)[0]
	def TransUInt32Array(self, buffer, index, length ):
		'''从缓存中提取uint数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransUInt32( buffer, index + 4 * i ))
		return tmp
	
	def TransInt64(self, buffer, index ):
		'''从缓存中提取long结果'''
		data = self.TransByteArray(buffer,index,8)
		return struct.unpack('<q',data)[0]
	def TransInt64Array(self, buffer, index, length):
		'''从缓存中提取long数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransInt64( buffer, index + 8 * i ))
		return tmp
	
	def TransUInt64(self, buffer, index ):
		'''从缓存中提取ulong结果'''
		data = self.TransByteArray(buffer,index,8)
		return struct.unpack('<Q',data)[0]
	def TransUInt64Array(self, buffer, index, length):
		'''从缓存中提取ulong数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransUInt64( buffer, index + 8 * i ))
		return tmp
	
	def TransSingle(self, buffer, index ):
		'''从缓存中提取float结果'''
		data = self.TransByteArray(buffer,index,4)
		return struct.unpack('<f',data)[0]
	def TransSingleArray(self, buffer, index, length):
		'''从缓存中提取float数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransSingle( buffer, index + 4 * i ))
		return tmp
	
	def TransDouble(self, buffer, index ):
		'''从缓存中提取double结果'''
		data = self.TransByteArray(buffer,index,8)
		return struct.unpack('<d',data)[0]
	def TransDoubleArray(self, buffer, index, length):
		'''从缓存中提取double数组结果'''
		tmp = []
		for i in range(length):
			tmp.append( self.TransSingle( buffer, index + 8 * i ))
		return tmp

	def TransString( self, buffer, index, length, encoding ):
		'''从缓存中提取string结果，使用指定的编码'''
		data = self.TransByteArray(buffer,index,length)
		return data.decode(encoding)

	def BoolArrayTransByte(self, values):
		if (values == None): return None
		return SoftBasic.BoolArrayToByte( values )
	def BoolTransByte(self, value):
		return self.BoolArrayTransByte([value])

	def ByteTransByte(self, value ):
		buffer = bytearray(1)
		buffer[0] = value
		return buffer

	def Int16ArrayTransByte(self, values ):
		if (values == None) : return None
		buffer = bytearray(len(values) * 2)
		for i in range(len(values)):
			buffer[(i*2): (i*2+2)] = struct.pack('<h',values[i])
		return buffer
	def Int16TransByte(self, value ):
		return self.Int16ArrayTransByte([value])

	def UInt16ArrayTransByte(self, values ):
		if (values == None) : return None
		buffer = bytearray(len(values) * 2)
		for i in range(len(values)):
			buffer[(i*2): (i*2+2)] = struct.pack('<H',values[i])
		return buffer
	def UInt16TransByte(self, value ):
		return self.UInt16ArrayTransByte([value])

	def Int32ArrayTransByte(self, values ):
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = struct.pack('<i',values[i])
		return buffer
	def Int32TransByte(self, value ):
		return self.Int32ArrayTransByte([value])

	def UInt32ArrayTransByte(self, values ):
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = struct.pack('<I',values[i])
		return buffer
	def UInt32TransByte(self, value ):
		return self.UInt32ArrayTransByte([value])

	def Int64ArrayTransByte(self, values ):
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = struct.pack('<q',values[i])
		return buffer
	def Int64TransByte(self, value ):
		return self.Int64ArrayTransByte([value])

	def UInt64ArrayTransByte(self, values ):
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = struct.pack('<Q',values[i])
		return buffer
	def UInt64TransByte(self, value ):
		return self.UInt64ArrayTransByte([value])

	def FloatArrayTransByte(self, values ):
		if (values == None) : return None
		buffer = bytearray(len(values) * 4)
		for i in range(len(values)):
			buffer[(i*4): (i*4+4)] = struct.pack('<f',values[i])
		return buffer
	def FloatTransByte(self, value ):
		return self.FloatArrayTransByte([value])

	def DoubleArrayTransByte(self, values ):
		if (values == None) : return None
		buffer = bytearray(len(values) * 8)
		for i in range(len(values)):
			buffer[(i*8): (i*8+8)] = struct.pack('<d',values[i])
		return buffer
	def DoubleTransByte(self, value ):
		return self.DoubleArrayTransByte([value])

	def StringTransByte(self, value, encoding ):
		return value.encode(encoding)

class RegularByteTransform(ByteTransform):
	def __init__(self):
		return

class ReverseBytesTransform(ByteTransform):
	def TransInt16(self, buffer, index ):
		data = self.TransByteArray(buffer,index,2)
		return struct.unpack('>h',data)[0]




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
		return segment.join(str_list)
	@staticmethod
	def ByteToBoolArray( InBytes, length ):
		'''从字节数组中提取bool数组变量信息'''
		if InBytes == None:
			return None
		if length > len(InBytes) * 8:
			length = len(InBytes) * 8
		buffer = []
		for  i in range(length):
			index = int(i / 8)
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
		if (array == None) : return None

		length = 0
		if len(array) % 8 == 0:
			length = int(len(array) / 8)
		else:
			length = int(len(array) / 8) + 1
		buffer = bytearray(length)

		for i in range(len(array)):
			index = int(i / 8)
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

class NetworkBase:
	'''网络基础类的核心'''
	Token = uuid.UUID('{00000000-0000-0000-0000-000000000000}')
	CoreSocket = None
	def Receive(self,socket,length):
		totle = 0
		data = ""
		try:
			while totle < length:
				data += socket.Receive(length-totle)
				totle += data.length
			return OperateResult.CreateSuccessResult(data)
		except Exception as e:
			result = OperateResult()
			result.Message = str(e)
			return result
	def Send(self,socket,data):
		try:
			socket.send(data)
			return OperateResult.CreateSuccessResult()
		except Exception as e:
			return OperateResult.CreateFailedResult(str(e))





#modbus = socket.socket()
#ip_port = ('127.0.0.1',502)
#modbus.connect(ip_port)

#send = b'\x00\x00\x00\x00\x00\x06\x01\x03\x00\x00\x00\x01'
#modbus.send(send)
#recive = modbus.recv(1024)
#print(recive)

#modbus.close()

data = b'\xA2'
print(SoftBasic.ByteToBoolArray(data,8))

ii = 100
data = b'\x64\x00'
# print(SoftBasic.ByteToHexString(struct.pack('<h',ii)))
print(struct.unpack('<h',data)[0])
print(SoftBasic.ByteToHexString(SoftBasic.BoolArrayToByte([True,False,False,True,False,False,False,False,True])))

bytesMy = bytearray(4)
bytesMy[0] = 1
bytesMy[1] = 2
bytesMy[2] = 3
bytesMy[3] = 4
print(SoftBasic.ByteToHexString(bytesMy[2:4]))
bytesMy[0:2] = struct.pack('<h',123)
print(SoftBasic.ByteToHexString(bytesMy))