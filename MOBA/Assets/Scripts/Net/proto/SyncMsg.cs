//同步角色信息
public class MsgSyncHuman:MsgBase {
	public MsgSyncHuman() {protoName = "MsgSyncHuman";}
	//位置、旋转、炮塔旋转
	public float x = 0f;		
	public float y = 0f;
	public float z = 0f;
	public float ex = 0f;		
	public float ey = 0f;
	public float ez = 0f;	
	//服务端补充
	public string id = "";		//哪个角色
}

//开火
public class MsgFire:MsgBase {
	public MsgFire() {protoName = "MsgFire";}
	//使用的武器（决定特效预制体）
	public string weaponName = "";
	//射线特效初始位置、终点位置
	public float x = 0f;		
	public float y = 0f;
	public float z = 0f;
	public float ex = 0f;
	public float ey = 0f;
	public float ez = 0f;
	public bool isHit = false;		//终点是否生成击中特效
	public float hnx = 0f;		//击中特效方向（法线）
	public float hny = 0f;
	public float hnz = 0f;
	//服务端补充
	public string id = "";		//哪个角色
}

//击中
public class MsgHit:MsgBase {
	public MsgHit() {protoName = "MsgHit";}
	//使用的武器（决定伤害值）
	public string weaponName = "";
	//击中谁
	public string targetId = "";
	//服务端补充
	public string id = "";		//哪个角色
	public int damage = 0;		//受到的伤害
}