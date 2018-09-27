'''
MIT License

Copyright (c) 2017-2018 Richard.Hu

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
'''
'''
警告：以下代码只能在测试PLC中运行，禁止使用生产现场的PLC来测试，否则，后果自负
Warning: The following code can only be run in the Test plc, prohibit the use of the production site PLC to test, otherwise, the consequences
'''
from HslCommunication import SiemensS7Net
from HslCommunication import SiemensPLCS
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
    siemens = SiemensS7Net(SiemensPLCS.S1200, "192.168.8.12")
    if siemens.ConnectServer().IsSuccess == False:
        print("connect falied")
    else:
        # bool read write test
        siemens.WriteBool("M80.6",True)
        printReadResult(siemens.ReadBool("M80.6"))

        # byte read write test
        siemens.WriteByte("M100", 58)
        printReadResult(siemens.ReadByte("M100"))

        # int16 read write test
        siemens.WriteInt16("M102", 12358)
        printReadResult(siemens.ReadInt16("M102"))

        # int16 read write test
        siemens.WriteInt16("M104", -12358)
        printReadResult(siemens.ReadInt16("M104"))

        # uint16 read write test
        siemens.WriteUInt16("M106", 52358)
        printReadResult(siemens.ReadUInt16("M106"))

        # int32 read write test
        siemens.WriteInt32("M108", 12345678)
        printReadResult(siemens.ReadInt32("M108"))

        # int32 read write test
        siemens.WriteInt32("M112", -12345678)
        printReadResult(siemens.ReadInt32("M112"))

        # uint32 read write test
        siemens.WriteUInt32("M116", 123456789)
        printReadResult(siemens.ReadInt32("M116"))

        # int64 read write test
        siemens.WriteInt64("M120", 12345678901234)
        printReadResult(siemens.ReadInt64("M120"))

        # float read write test
        siemens.WriteFloat("M130", 123.456)
        printReadResult(siemens.ReadFloat("M130"))

        # double read write test
        siemens.WriteDouble("M140", 123.456789)
        printReadResult(siemens.ReadDouble("M140"))

        # string read write test
        siemens.WriteString("M150", '123456')
        printReadResult(siemens.ReadString("M150",6))

        # int16 array read write test
        siemens.WriteInt16("M160", [123,456,789,-1234])
        printReadResult(siemens.ReadInt16("M160",4))

        # read block
        read = siemens.Read("M100",10)
        if read.IsSuccess:
            m100 = read.Content[0]
            m101 = read.Content[1]
            m102 = read.Content[2]
            m103 = read.Content[3]
            m104 = read.Content[4]
            m105 = read.Content[5]
            m106 = read.Content[6]
            m107 = read.Content[7]
            m108 = read.Content[8]
            m109 = read.Content[9]
        else:
            print(read.Message)

        read = siemens.Read("M100",20)
        if read.IsSuccess:
            count = siemens.byteTransform.TransInt32(read.Content,0)
            temp = siemens.byteTransform.TransSingle(read.Content,4)
            name1 = siemens.byteTransform.TransInt16(read.Content,8)
            barcode = read.Content[10:20].decode('ascii')

        read = siemens.ReadFromCoreServer(SoftBasic.HexStringToBytes("03 00 00 24 02 F0 80 32 01 00 00 00 01 00 0E 00 05 05 01 12 0A 10 02 00 01 00 00 83 00 03 20 00 04 00 08 3B"))
        if read.IsSuccess:
            # 显示服务器返回的报文
            print(read.Content)
        else:
            # 读取错误
            print(read.Message)

        siemens.ConnectClose()