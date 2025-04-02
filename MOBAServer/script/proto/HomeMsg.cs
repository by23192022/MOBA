//查询个人数据
public class MsgGetPerson : MsgBase
{
    public MsgGetPerson() { protoName = "MsgGetPerson"; }
    //服务端回
    public int grade = 0;
    public int coin = 0;
    public string sign = "";
}

//保存个性签名
public class MsgSavePerson : MsgBase
{
    public MsgSavePerson() { protoName = "MsgSavePerson"; }
    //客户端发
    public string sign = "";
    //服务端回（0-成功 1-文字太长）
    public int result = 0;
}

