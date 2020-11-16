package HslCommunication.Core.IMessage;

import HslCommunication.Utilities;

public class FinsMessage implements INetMessage
{

    /**
     * 消息头的指令长度
     */
    public int ProtocolHeadBytesLength(){
        return 8;
    }


    /**
     * 从当前的头子节文件中提取出接下来需要接收的数据长度
     * @return 返回接下来的数据内容长度
     */
    public int GetContentLengthByHeadBytes(){
        if(HeadBytes == null) return 0;

        byte[] buffer = new byte[4];
        buffer[0] = HeadBytes[7];
        buffer[1] = HeadBytes[6];
        buffer[2] = HeadBytes[5];
        buffer[3] = HeadBytes[4];
        return Utilities.getInt(buffer,0);
    }


    /**
     * 检查头子节的合法性
     * @param token 特殊的令牌，有些特殊消息的验证
     * @return 是否合法的验证
     */
    public boolean CheckHeadBytesLegal(byte[] token)
    {
        if(HeadBytes == null) return false;
        if(HeadBytes[0] == 0x46 && HeadBytes[1] == 0x49 && HeadBytes[2] == 0x4E && HeadBytes[3] == 0x53){
            return true;
        }
        else {
            return false;
        }
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
