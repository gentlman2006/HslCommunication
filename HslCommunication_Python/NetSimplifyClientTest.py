from HslCommunication import NetSimplifyClient

if __name__ == "__main__":
    # NetSimplifyClient测试
    netSimplifyClient = NetSimplifyClient("127.0.0.1",12345)
    # netSimplifyClient.Token = uuid.UUID('66a469ad-a595-48ed-abe1-912f7085dbcd')
    netSimplifyClient.ConnectServer()

    read = netSimplifyClient.ReadStringFromServer(1,'123')
    if read.IsSuccess:
        print(read.Content)
    else:
        print(read.Message)
    read = netSimplifyClient.ReadStringFromServer(1,'测试信号')
    netSimplifyClient.ConnectClose()