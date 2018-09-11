from HslCommunication import ModbusTcpNet

def printReadResult(result):
    if result.IsSuccess:
    	print(result.Content)
    else:
    	print("failed   "+result.Message)
def printWriteResult(result):
    if result.IsSuccess:
        print("success")
    else:
        print("falied  " + result.Message)

if __name__ == "__main__":
    modbusTcpNet = ModbusTcpNet("127.0.0.1",502)
    if modbusTcpNet.ConnectServer().IsSuccess == False:
        print("connect failed")
    else:
        # 读写寄存器10，数据类型为int16
        printWriteResult(modbusTcpNet.WriteInt16("10", 12345))
        printReadResult(modbusTcpNet.ReadInt16("10"))

        # 读写寄存器11，数据类型为uint16
        printWriteResult(modbusTcpNet.WriteUInt16("11", 51234))
        printReadResult(modbusTcpNet.ReadUInt16("11"))

        # 读写寄存器20，数据类型为int32
        printWriteResult(modbusTcpNet.WriteInt32("20", 123456789))
        printReadResult(modbusTcpNet.ReadInt32("20"))

        # 读写寄存器22，数据类型为uint32
        printWriteResult(modbusTcpNet.WriteUInt32("22", 3223456789))
        printReadResult(modbusTcpNet.ReadInt32("22"))

        # 读写寄存器30，数据类型为int64
        printWriteResult(modbusTcpNet.WriteInt64("30", 123456789353423))
        printReadResult(modbusTcpNet.ReadInt64("30"))

        # 读写寄存器34，数据类型为uint64
        printWriteResult(modbusTcpNet.WriteUInt64("34", 3223456789353423))
        printReadResult(modbusTcpNet.ReadInt64("34"))

        # 读写寄存器40，数据类型为float，该类型存在精度丢失的问题
        printWriteResult(modbusTcpNet.WriteFloat("40", 123.545))
        printReadResult(modbusTcpNet.ReadFloat("40"))

        # 读写寄存器50，数据类型为double，精度相对较高
        printWriteResult(modbusTcpNet.WriteDouble("50", 123.5456))
        printReadResult(modbusTcpNet.ReadDouble("50"))

        # 读写寄存器60，数据类型为string长度为偶数，读取的时候是一个地址占了2个字符，所以为长度的一半
        printWriteResult(modbusTcpNet.WriteString("60", "ashdi6"))
        printReadResult(modbusTcpNet.ReadString("60",3))

        # 读写线圈1，数据类型为bool
        printWriteResult(modbusTcpNet.WriteCoil("1", True))
        printReadResult(modbusTcpNet.ReadCoil("1"))

        # 批量读写线圈10，数据类型为bool[]
        printWriteResult(modbusTcpNet.WriteCoil("10", [True,False,False,True]))
        printReadResult(modbusTcpNet.ReadCoil("10",4))

        # 读取输入寄存器测试
        printReadResult(modbusTcpNet.ReadDiscrete("1"))
        printReadResult(modbusTcpNet.ReadDiscrete("2",5))
        
        printReadResult(modbusTcpNet.ReadUInt16("x=4;100"))

        modbusTcpNet.ConnectClose()