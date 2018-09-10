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
        printWriteResult(modbusTcpNet.WriteInt16("10", 12345))
        printReadResult(modbusTcpNet.ReadInt16("10"))

        printWriteResult(modbusTcpNet.WriteInt32("11", 123456789))
        printReadResult(modbusTcpNet.ReadInt32("11"))

        printWriteResult(modbusTcpNet.WriteInt64("13", 123456789353423))
        printReadResult(modbusTcpNet.ReadInt64("13"))

        printWriteResult(modbusTcpNet.WriteFloat("17", 123.545))
        printReadResult(modbusTcpNet.ReadFloat("17"))

        printWriteResult(modbusTcpNet.WriteDouble("19", 123.545))
        printReadResult(modbusTcpNet.ReadDouble("19"))

        printWriteResult(modbusTcpNet.WriteString("25", "ashdi6"))
        printReadResult(modbusTcpNet.ReadString("25",3))

        printWriteResult(modbusTcpNet.WriteCoil("1", True))
        printReadResult(modbusTcpNet.ReadCoil("1"))

        printWriteResult(modbusTcpNet.WriteCoil("10", [True,False,False,True]))
        printReadResult(modbusTcpNet.ReadCoil("10",4))

        printWriteResult(modbusTcpNet.WriteUInt16("100", 51234))
        printReadResult(modbusTcpNet.ReadUInt16("100"))

        printReadResult(modbusTcpNet.ReadUInt16("x=4;100"))

        modbusTcpNet.ConnectClose()