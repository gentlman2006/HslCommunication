import string

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



result = OperateResult()
result.Content = 123
print(SoftBasic.GetSizeDescription(12352354))
print(result.ToMessageShowString())
print(SoftBasic.ByteToHexString(b'\x01\x00\x00\x00','-'))
