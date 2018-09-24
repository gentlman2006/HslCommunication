from HslCommunication import MelsecMcNet
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
    melsecNet = MelsecMcNet("192.168.8.12",6002)
    if melsecNet.ConnectServer().IsSuccess == False:
        print("connect falied  ")
    else:
        printReadResult(melsecNet.ReadInt32("D100"))