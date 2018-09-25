from HslCommunication import MelsecMcAsciiNet
from HslCommunication import SoftBasic

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
    print(SoftBasic.GetUniqueStringByGuidAndRandom())
    melsecNet = MelsecMcAsciiNet("192.168.8.12",6002)
    if melsecNet.ConnectServer().IsSuccess == False:
        print("connect falied  ")
    else:
        # 读写bool数值
        melsecNet.WriteBool("M200",True)
        printReadResult(melsecNet.ReadBool("M200"))

        # 读写bool数组
        melsecNet.WriteBool("M300",[True,False,True,True,False])
        printReadResult(melsecNet.ReadBool("M300",5))

        # 读写int16数值
        melsecNet.WriteInt16("D200", 12358)
        printReadResult(melsecNet.ReadInt16("D200"))

        # 读写int16数值
        melsecNet.WriteInt16("D201", -12358)
        printReadResult(melsecNet.ReadInt16("D201"))

        # 读写uint16数值
        melsecNet.WriteUInt16("D202", 52358)
        printReadResult(melsecNet.ReadUInt16("D202"))

        # 读写int32数值
        melsecNet.WriteInt32("D210", 12345678)
        printReadResult(melsecNet.ReadInt32("D210"))

        # 读写int32数值
        melsecNet.WriteInt32("D212", -12345678)
        printReadResult(melsecNet.ReadInt32("D212"))

        # 读写uint32数值
        melsecNet.WriteUInt32("D214", 123456789)
        printReadResult(melsecNet.ReadInt32("D214"))

        # 读写int64数值
        melsecNet.WriteInt64("D220", 12345678901234)
        printReadResult(melsecNet.ReadInt64("D220"))

        # 读写float数值
        melsecNet.WriteFloat("D230", 123.456)
        printReadResult(melsecNet.ReadFloat("D230"))

        # 读写double数值
        melsecNet.WriteDouble("D240", 123.456789)
        printReadResult(melsecNet.ReadDouble("D240"))

        # 读写字符串
        melsecNet.WriteString("D250", '123456')
        printReadResult(melsecNet.ReadString("D250",3))

        # 读写int数组
        melsecNet.WriteInt16("D260", [123,456,789,-1234])
        printReadResult(melsecNet.ReadInt16("D260",4))

        melsecNet.ConnectClose()