package HslCommunication.Core.IMessage;

public class MelsecA1EBinaryMessage implements INetMessage {

    /**
     * 消息头的指令长度
     */
    public int ProtocolHeadBytesLength(){
        return 2;
    }


    /**
     * 从当前的头子节文件中提取出接下来需要接收的数据长度
     * @return 返回接下来的数据内容长度
     */
    public int GetContentLengthByHeadBytes(){
        if(HeadBytes == null) return 0;

        int contentLength = 0;

        if (HeadBytes[1] == 0x5B)
        {
            contentLength = 2; //结束代码 + 0x00
            return contentLength;
        }
        else
        {
            int length = (SendBytes[10] % 2 == 0) ? SendBytes[10] : SendBytes[10] + 1;
            switch (HeadBytes[0])
            {
                case (byte) 0x80: //位单位成批读出后，回复副标题
                    contentLength = length / 2;
                    break;
                case (byte) 0x81: //字单位成批读出后，回复副标题
                    contentLength = SendBytes[10] * 2;
                    break;
                case (byte) 0x82: //位单位成批写入后，回复副标题
                    break;
                case (byte) 0x83: //字单位成批写入后，回复副标题
                    break;
                default:
                    break;
            }
            // 在A兼容1E协议中，写入值后，若不发生异常，只返回副标题 + 结束代码(0x00)
            // 这已经在协议头部读取过了，后面要读取的长度为0（contentLength=0）
        }
        return contentLength;
    }


    /**
     * 检查头子节的合法性
     * @param token 特殊的令牌，有些特殊消息的验证
     * @return 是否合法的验证
     */
    public boolean CheckHeadBytesLegal(byte[] token)
    {
        if (HeadBytes != null)
        {
            if ((HeadBytes[0] - SendBytes[0]) == (byte) 0x80) { return true; }
        }
        return false;
    }


    /**
     * 获取头子节里的消息标识
     * @return
     */
    public int GetHeadBytesIdentity(){

        return 0;
    }


    /**
     * 获取消息头字节
     *
     * @return
     */
    @Override
    public byte[] getHeadBytes() {
        return HeadBytes;
    }

    /**
     * 获取消息内容字节
     *
     * @return
     */
    @Override
    public byte[] getContentBytes() {
        return ContentBytes;
    }

    /**
     * 获取发送的消息
     *
     * @return
     */
    @Override
    public byte[] getSendBytes() {
        return SendBytes;
    }

    /**
     * 设置消息头子节
     * @param headBytes 字节数据
     */
    public void setHeadBytes(byte[] headBytes){
        HeadBytes = headBytes;
    }




    /**
     * 设置消息内容字节
     * @param contentBytes 内容字节
     */
    public void setContentBytes(byte[] contentBytes){
        ContentBytes = contentBytes;
    }



    /**
     * 设置发送的字节信息
     * @param sendBytes 发送的字节信息
     */
    public void setSendBytes(byte[] sendBytes){
        SendBytes = sendBytes;
    }


    private byte[] HeadBytes = null;

    private byte[] ContentBytes = null;

    private byte[] SendBytes = null;
}
