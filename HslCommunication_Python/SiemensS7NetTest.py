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


        siemens.ConnectClose()