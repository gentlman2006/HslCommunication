import string
import uuid
import socket

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
	def TransBool(self, buffer, index, length=1 ):
		return buffer[index] != 0x00

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
	def ByteToHexString(inBytes,segment):
		'''将字节数组转换成十六进制的表示形式，需要传入2个参数，数据和分隔符，该方法还存在一点问题'''
		str_list = []
		for byte in inBytes:
			str_list.append(hex(byte))
		return segment.join(str_list)

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





modbus = socket.socket()
ip_port = ('127.0.0.1',502)
modbus.connect(ip_port)

send = b'\x00\x00\x00\x00\x00\x06\x01\x03\x00\x00\x00\x01'
modbus.send(send)
recive = modbus.recv(1024)
print(recive)


print(send[5])
modbus.close()