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
        printReadResult(siemens.ReadInt16("M100"))