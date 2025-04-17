using System;


public partial class MsgHandler {

	//同步位置协议
	public static void MsgSyncHuman(ClientState c, MsgBase msgBase){
        MsgSyncHuman msg = (MsgSyncHuman)msgBase;
		Player player = c.player;
		if(player == null) return;
		//room
		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null){
			return;
		}
		//status
		if(room.status != Room.Status.FIGHT)
        {
			return;
		}
        //是否作弊(两次同步信息的位置相差太⼤)
        if (NetManager.GetTimeStamp() - room.lastjudgeTime >= 5 && (
			Math.Abs(player.x - msg.x) > 10 ||
			Math.Abs(player.y - msg.y) > 10 ||
			Math.Abs(player.z - msg.z) > 10)){
			Console.WriteLine("疑似作弊 " + player.id);
		}
        //NetManager.GetTimeStamp() - room.lastjudgeTime >= 5
        //游戏开始5s内跳过检测，避免因初始位置跳变误报

        //更新信息
        player.x = msg.x;
		player.y = msg.y;
		player.z = msg.z;
		player.ex = msg.ex;
		player.ey = msg.ey;
		player.ez = msg.ez;
		//广播
		msg.id = player.id;
		room.Broadcast(msg);
	}

	//开火协议
	public static void MsgFire(ClientState c, MsgBase msgBase){
		MsgFire msg = (MsgFire)msgBase;
		Player player = c.player;
		if(player == null) return;
		//room
		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null){
			return;
		}
		//status
		if(room.status != Room.Status.FIGHT){
			return;
		}
		//广播
		msg.id = player.id;
		room.Broadcast(msg);
	}

	//击中协议
	public static void MsgHit(ClientState c, MsgBase msgBase){
		MsgHit msg = (MsgHit)msgBase;
		Player player = c.player;
		if(player == null) return;
		//targetPlayer
		Player targetPlayer = PlayerManager.GetPlayer(msg.targetId);
		if(targetPlayer == null){
			return;
		}
		//room
		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null){
			return;
		}
		//status
		if(room.status != Room.Status.FIGHT){
			return;
		}

        // 获取武器
        Weapon weapon = WeaponDB.Instance.GetWeapon(msg.weaponName);
        //状态
		targetPlayer.hp -= weapon.damage;
		//广播
		msg.id = player.id;
        msg.damage = weapon.damage;
		room.Broadcast(msg);
	}

}


