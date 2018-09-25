from HslCommunication import SiemensS7Net
from HslCommunication import SiemensPLCS

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
    siemens = SiemensS7Net(SiemensPLCS.S1200, "192.168.8.12")
    if siemens.ConnectServer().IsSuccess == False:
        print("connect falied")
    else:
        siemens.WriteBool("M80.6",True)
        printReadResult(siemens.ReadBool("M80.6"))

        siemens.WriteByte("M100", 58)
        printReadResult(siemens.ReadByte("M100"))

        siemens.WriteInt16("M102", 12358)
        printReadResult(siemens.ReadInt16("M102"))

        siemens.WriteInt16("M104", -12358)
        printReadResult(siemens.ReadInt16("M104"))

        siemens.WriteUInt16("M106", 52358)
        printReadResult(siemens.ReadUInt16("M106"))

        siemens.WriteInt32("M108", 12345678)
        printReadResult(siemens.ReadInt32("M108"))

        siemens.WriteInt32("M112", -12345678)
        printReadResult(siemens.ReadInt32("M112"))

        siemens.WriteUInt32("M116", 123456789)
        printReadResult(siemens.ReadInt32("M116"))

        siemens.WriteInt64("M120", 12345678901234)
        printReadResult(siemens.ReadInt64("M120"))

        siemens.WriteFloat("M130", 123.456)
        printReadResult(siemens.ReadFloat("M130"))

        siemens.WriteDouble("M140", 123.456789)
        printReadResult(siemens.ReadDouble("M140"))

        siemens.WriteString("M150", '123456')
        printReadResult(siemens.ReadString("M150",6))

        siemens.WriteInt16("M160", [123,456,789,-1234])
        printReadResult(siemens.ReadInt16("M160",4))


        siemens.ConnectClose()