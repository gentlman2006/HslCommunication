from HslCommunication import NetPushClient


def PushCallBack(keyword,value):
    print('key:'+keyword+' value:'+value)

if __name__ == "__main__":
    pushNet = NetPushClient("127.0.0.1",12345,"E")
    create = pushNet.CreatePush(PushCallBack)
    if create.IsSuccess:
        input('按键退出')
        pushNet.ClosePush()
    else:
        print(create.Message)