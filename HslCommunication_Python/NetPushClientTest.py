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
说明：这个类可以用来接收C#的服务器推送的数据，java的类也可以接收推送的数据。
Description: This class can be used to receive push data from a server in C #, and Java classes can also receive push Data.
'''
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