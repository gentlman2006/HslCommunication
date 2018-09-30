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
from HslCommunication import MelsecA1ENet
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
    melsecNet = MelsecA1ENet("192.168.0.100",5000)
    if melsecNet.ConnectServer().IsSuccess == False:
        print("connect falied  ")
    else:
        # bool read write test
        melsecNet.WriteBool("M200",True)
        printReadResult(melsecNet.ReadBool("M200"))

        # bool array read write test
        melsecNet.WriteBool("M300",[True,False,True,True,False])
        printReadResult(melsecNet.ReadBool("M300",5))

        # int16 read write test
        melsecNet.WriteInt16("D200", 12358)
        printReadResult(melsecNet.ReadInt16("D200"))

        # int16 read write test
        melsecNet.WriteInt16("D201", -12358)
        printReadResult(melsecNet.ReadInt16("D201"))

        # uint16 read write test
        melsecNet.WriteUInt16("D202", 52358)
        printReadResult(melsecNet.ReadUInt16("D202"))

        # int32 read write test
        melsecNet.WriteInt32("D210", 12345678)
        printReadResult(melsecNet.ReadInt32("D210"))

        # int32 read write test
        melsecNet.WriteInt32("D212", -12345678)
        printReadResult(melsecNet.ReadInt32("D212"))

        # uint32 read write test
        melsecNet.WriteUInt32("D214", 123456789)
        printReadResult(melsecNet.ReadInt32("D214"))

        # int64 read write test
        melsecNet.WriteInt64("D220", 12345678901234)
        printReadResult(melsecNet.ReadInt64("D220"))

        # float read write test
        melsecNet.WriteFloat("D230", 123.456)
        printReadResult(melsecNet.ReadFloat("D230"))

        # double read write test
        melsecNet.WriteDouble("D240", 123.456789)
        printReadResult(melsecNet.ReadDouble("D240"))

        # string read write test
        melsecNet.WriteString("D250", '123456')
        printReadResult(melsecNet.ReadString("D250",3))

        # int16 array read write test
        melsecNet.WriteInt16("D260", [123,456,789,-1234])
        printReadResult(melsecNet.ReadInt16("D260",4))

        melsecNet.ConnectClose()