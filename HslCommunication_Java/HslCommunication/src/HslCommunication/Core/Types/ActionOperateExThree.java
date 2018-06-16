package HslCommunication.Core.Types;

/**
 * 带三个参数的类型对象
 * @param <T1> 类型一
 * @param <T2> 类型二
 * @param <T3> 类型三
 */
public class ActionOperateExThree<T1,T2,T3>
{
    /**
     * 实际的操作方法，需要继承重写
     * @param content1 类型一对象
     * @param content2 类型二对象
     * @param content3 类型三对象
     */
    public void Action(T1 content1,T2 content2,T3 content3){

    }
}
