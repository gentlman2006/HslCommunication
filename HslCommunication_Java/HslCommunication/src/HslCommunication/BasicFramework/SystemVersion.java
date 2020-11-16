package HslCommunication.BasicFramework;

/**
 * 系统的版本类
 */
public class SystemVersion {

    /**
     * 根据格式化字符串的版本号初始化
     * @param VersionString 格式化的字符串，例如：1.0.0或1.0.0.0503
     */
    public SystemVersion(String VersionString)
    {
        String[] temp = VersionString.split("\\.");
        if (temp.length >= 3)
        {
            m_MainVersion = Integer.parseInt(temp[0]);
            m_SecondaryVersion = Integer.parseInt(temp[1]);
            m_EditVersion = Integer.parseInt(temp[2]);

            if (temp.length >= 4)
            {
                m_InnerVersion = Integer.parseInt(temp[3]);
            }
        }
    }


    /**
     * 根据指定的数字实例化一个对象
     * @param main 主版本
     * @param sec 次版本
     * @param edit 修订版
     */
    public SystemVersion(int main, int sec, int edit)
    {
        m_MainVersion = main;
        m_SecondaryVersion = sec;
        m_EditVersion = edit;
    }

    /**
     * 根据指定的数字实例化一个对象
     * @param main 主版本
     * @param sec 次版本
     * @param edit 修订版
     * @param inner 内部版本号
     */
    public SystemVersion(int main, int sec, int edit, int inner)
    {
        m_MainVersion = main;
        m_SecondaryVersion = sec;
        m_EditVersion = edit;
        m_InnerVersion = inner;
    }
    private int m_MainVersion = 2;



    /**
     * 主版本
     * @return int数据
     */
    public int MainVersion()
    {
        return m_MainVersion;
    }

    private int m_SecondaryVersion = 0;



    /**
     * 次版本
     * @return int数据
     */
    public int SecondaryVersion() {
        return m_SecondaryVersion;
    }

    private int m_EditVersion = 0;

    /**
     * 修订版
     * @return int数据
     */
    public int EditVersion() {
        return m_EditVersion;
    }
    private int m_InnerVersion = 0;



    /**
     * 内部版本号，或者是版本号表示为年月份+内部版本的表示方式
     * @return int数据
     */
    public int InnerVersion()
    {
        return m_InnerVersion;
    }



    /**
     * 根据格式化为支持返回的不同信息的版本号
     * C返回1.0.0.0
     * N返回1.0.0
     * S返回1.0
     * @param format 格式化信息
     * @return 字符串数据
     */
    public String toString(String format)
    {
        if(format == "C")
        {
            return MainVersion()+"."+SecondaryVersion()+"."+EditVersion()+"."+InnerVersion();
        }

        if(format == "N")
        {
            return MainVersion()+"."+SecondaryVersion()+"."+EditVersion();
        }

        if(format == "S")
        {
            return MainVersion()+"."+SecondaryVersion();
        }

        return toString();
    }


    /**
     * 版本信息
     * @return 字符串数据
     */
    @Override
    public String toString() {
        if(InnerVersion() == 0)
        {
            return MainVersion()+"."+SecondaryVersion()+"."+EditVersion();
        }
        else
        {
            return MainVersion()+"."+SecondaryVersion()+"."+EditVersion()+"."+InnerVersion();
        }
    }


    /**
     * 判断版本是否一致
     * @param sv 对比的版本
     * @return 是否一致
     */
    public boolean IsSameVersion(SystemVersion sv) {
        if (this.m_MainVersion != sv.m_MainVersion) {
            return false;
        }

        if (this.m_SecondaryVersion != sv.m_SecondaryVersion) {
            return false;
        }

        if (this.m_EditVersion != sv.m_EditVersion) {
            return false;
        }

        if (this.m_InnerVersion != sv.m_InnerVersion) {
            return false;
        }

        return true;
    }

    /**
     * 判断是不是小于指定的版本
     * @param sv 对比的版本
     * @return 是否小于
     */
    public boolean IsSmallerThan(SystemVersion sv) {
        if (this.m_MainVersion < sv.m_MainVersion) {
            return true;
        }
        else if(this.m_MainVersion > sv.m_MainVersion) {
            return false;
        }

        if (this.m_SecondaryVersion < sv.m_SecondaryVersion) {
            return true;
        }
        else if (this.m_SecondaryVersion > sv.m_SecondaryVersion) {
            return false;

        }

        if (this.m_EditVersion < sv.m_EditVersion) {
            return true;
        }
        else if (this.m_EditVersion > sv.m_EditVersion) {
            return false;
        }

        if (this.m_InnerVersion < sv.m_InnerVersion) {
            return true;
        }
        else if (this.m_InnerVersion > sv.m_InnerVersion) {
            return false;
        }

        return false;
    }

}
