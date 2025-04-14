using System;


public partial class MsgHandler
{
    //游戏大厅
    //保存个性签名
    public static void MsgSavePerson(ClientState c, MsgBase msgBase)
    {
        MsgSavePerson msg = (MsgSavePerson)msgBase;
        Player player = c.player;
        if (player == null) return;

        /* EventHandler.OnDisconnect玩家下线时会调用DbManager.UpdatePlayerData保存c.player.data数据。*/
        player.data.sign = msg.sign;
        if (msg.sign.Length > 18)
            msg.result = 1;
        //msg.result默认为0;

        player.Send(msg);
    }

    //查询个人数据
    public static void MsgGetPerson(ClientState c, MsgBase msgBase)
    {
        MsgGetPerson msg = (MsgGetPerson)msgBase;
        Player player = c.player;
        if (player == null) return;

        msg.grade = player.data.grade;
        msg.coin = player.data.coin;
        msg.sign = player.data.sign;

        player.Send(msg);
    }
}
