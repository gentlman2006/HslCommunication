import string
import uuid
import socket

class StringResources:
	'''系统的资源类'''
	def ConnectedFailed():
		return "连接失败"
	def UnknownError():
		return "未知错误"
	def ErrorCode():
		return "错误代号"
	def TextDescription():
		return "文本描述"
	def ExceptionMessage():
		return "错误信息："
	def ExceptionStackTrace():
		return "错误堆栈："
	def ExceptopnTargetSite():
		return "错误方法："
	def ExceprionCustomer():
		return "用户自定义方法出错："
	def SuccessText():
		return "Success"


class OperateResult:
	'''结果对象类'''
	# 是否成功的标志
	IsSuccess = False
	# 操作返回的错误消息
	Message = StringResources.SuccessText()
	# 错误码
	ErrorCode = 0
	# 返回显示的文本
	def ToMessageShowString(self):
		return StringResources.ErrorCode() + ":" + str(self.ErrorCode) + "\r\n" + StringResources.TextDescription() + ":" + self.Message
	def CreateFailedResult(msg):
		'''创建一个失败的结果对象'''
		failed = OperateResult()
		failed.Message = msg
		return failed
	def CreateSuccessResult():
		'''创建一个成功的对象'''
		success = OperateResult()
		success.IsSuccess = True
		return success
	def CreateSuccessResult(Content):
		'''创建一个成功的结果对象，携带一个结果对象'''
		success = OperateResult()
		success.IsSuccess = True
		success.Content = Content
		return success
	def CreateSuccessResult(Content1,Content2):
		'''创建一个成功的结果对象，携带两个结果对象'''
		success = OperateResult()
		success.IsSuccess = True
		success.Content1 = Content1
		success.Content2 = Content2
		return success



class SoftBasic:
	def GetSizeDescription(size):
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
	def ByteToHexString(inBytes,segment):
		'''将字节数组转换成十六进制的表示形式，需要传入2个参数，数据和分隔符，该方法还存在一点问题'''
		str_list = []
		for byte in inBytes:
			str_list.append(hex(byte))
		return segment.join(str_list)

class NetworkBase:
	Token = UUID('{00000000-0000-0000-0000-000000000000}')
	CoreSocket = null
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
			result.Message = e.message
			return result
	def Send(self,socket,data):
		try:
			socket.send(data)
			return OperateResult.CreateSuccessResult()
		except Exception as e:
			return OperateResult.CreateFailedResult(e.message)

result = OperateResult()
result.Content = 123
print(SoftBasic.GetSizeDescription(12352354))
print(result.ToMessageShowString())
print(SoftBasic.ByteToHexString(b'\x01\x00\x00\x00','-'))
