using System;


public partial class MsgHandler {

    //保存个性签名
    public static void MsgSavePerson(ClientState c, MsgBase msgBase){
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
    public static void MsgGetPerson(ClientState c, MsgBase msgBase){
        MsgGetPerson msg = (MsgGetPerson)msgBase;
        Player player = c.player;
        if (player == null) return;

        msg.grade = player.data.grade;
        msg.coin = player.data.coin;
        msg.sign = player.data.sign;

        player.Send(msg);
    }
    //查询战绩
    public static void MsgGetAchieve(ClientState c, MsgBase msgBase){
		MsgGetAchieve msg = (MsgGetAchieve)msgBase;
		Player player = c.player;
		if(player == null) return;

		msg.win = player.data.win;
		msg.lost = player.data.lost;

		player.Send(msg);
	}


	//请求房间列表
	public static void MsgGetRoomList(ClientState c, MsgBase msgBase){
		MsgGetRoomList msg = (MsgGetRoomList)msgBase;
		Player player = c.player;
		if(player == null) return;

		player.Send(RoomManager.ToMsg());
	}

	//创建房间
	public static void MsgCreateRoom(ClientState c, MsgBase msgBase){
		MsgCreateRoom msg = (MsgCreateRoom)msgBase;
		Player player = c.player;
		if(player == null) return;
		//已经在房间里
		if(player.roomId >=0 ){
            Console.WriteLine("MsgCreateRoom.玩家已经在房间" + player.roomId + "内");
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//创建
		Room room = RoomManager.AddRoom();
		room.AddPlayer(player.id);

		msg.result = 0;
		player.Send(msg);
	}

	//进入房间
	public static void MsgEnterRoom(ClientState c, MsgBase msgBase){
		MsgEnterRoom msg = (MsgEnterRoom)msgBase;
		Player player = c.player;
		if(player == null) return;
		//已经在房间里
		if(player.roomId >=0 ){
            Console.WriteLine("MsgEnterRoom.玩家已经在房间" + player.roomId + "内");
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//获取房间
		Room room = RoomManager.GetRoom(msg.id);
		if(room == null){
            Console.WriteLine("MsgEnterRoom.房间" + player.roomId + "为空");
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//房间添加玩家失败
		if(!room.AddPlayer(player.id)){
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//返回协议	
		msg.result = 0;
		player.Send(msg);
	}


	//获取房间信息
	public static void MsgGetRoomInfo(ClientState c, MsgBase msgBase){
		MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
		Player player = c.player;
		if(player == null) return;

		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null){
            Console.WriteLine("正在发送MsgGetRoomInfo，但协议内容为空");
            player.Send(msg);
			return;
		}

        Console.WriteLine("正在发送MsgGetRoomInfo");
        player.Send(room.ToMsg());
	}

	//离开房间
	public static void MsgLeaveRoom(ClientState c, MsgBase msgBase){
		MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
		Player player = c.player;
		if(player == null) return;

		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null){
			msg.result = 1;
			player.Send(msg);
			return;
		}
        Console.WriteLine("正在移除玩家" + player.id);
		room.RemovePlayer(player.id);
		//返回协议
		msg.result = 0;
		player.Send(msg);
	}

	/*

	//请求开始战斗
	public static void MsgStartBattle(ClientState c, MsgBase msgBase){
		MsgStartBattle msg = (MsgStartBattle)msgBase;
		Player player = c.player;
		if(player == null) return;
		//room
		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null){
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//是否是房主
		if(!room.isOwner(player)){
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//开战
		if(!room.StartBattle()){
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//成功
		msg.result = 0;
		player.Send(msg);
	}
	*/


}


