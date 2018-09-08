from HslCommunication import ModbusTcpNet

if __name__ == "__main__":
    modbusTcpNet = ModbusTcpNet("127.0.0.1",502)
    read = modbusTcpNet.ReadInt64("x=4;10")
    if read.IsSuccess:
    	print(read.Content)
    else:
    	print(read.Message)