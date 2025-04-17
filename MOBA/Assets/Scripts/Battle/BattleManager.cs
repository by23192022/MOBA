using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager {
	//角色预制体路径
	public static string HumanPath0 = "Perfabs/TPmodel/PlayerDogPBR";
	public static string HumanPath1 = "Perfabs/TPmodel/syncPlayerDogPBR";
	public static string HumanPath2 = "Perfabs/Enemy/EnemyDogPolyart";
	private static WeaponDB weaponDB; // 武器数据
	private static Weapon defaultWeapon; // 默认武器

	//战场中的角色
	public static Dictionary<string, BaseHuman> humans = new Dictionary<string, BaseHuman>();

	//初始化
	public static void Init() {
		//添加监听
		NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
		NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
		NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);

		NetManager.AddMsgListener("MsgSyncHuman", OnMsgSyncHuman);
		NetManager.AddMsgListener("MsgFire", OnMsgFire);
		NetManager.AddMsgListener("MsgHit", OnMsgHit);

		weaponDB = Resources.Load<WeaponDB>("Data/WeaponDatabase"); // 加载资源

		defaultWeapon = weaponDB.GetWeapon("AAssaultRifle_01");	//默认武器
	}

	//添加角色
	public static void AddHuman(string id, BaseHuman human){
		humans[id] = human;
	}

	//删除角色
	public static void RemoveHuman(string id){
		humans.Remove(id);
	}

	//获取角色
	public static BaseHuman GetHuman(string id) {
		if(humans.ContainsKey(id)){
			return humans[id];
		}
		return null;
	}

	//获取玩家控制的角色
	public static BaseHuman GetCtrlHuman() {
		return GetHuman(GameMain.id);
	}

	//重置战场
	public static void Reset() {
		//场景
		foreach(BaseHuman human in humans.Values){
			//角色模型
			MonoBehaviour.Destroy(human.gameObject);
			//sync角色血条
			if(human.TryGetComponent<HealthController>(out var health)){
				MonoBehaviour.Destroy(health.healthBar);
			}
			//ctrl角色武器
			GameObject fp = GameObject.FindWithTag("FPModel");
			MonoBehaviour.Destroy(fp);
			//地图不可见
			GameMain.obj1.SetActive(false);
			GameMain.obj2.SetActive(false);
		}
		//列表
		humans.Clear();
	}

	//开始战斗
	public static void EnterBattle(MsgEnterBattle msg) {
		//重置
		//BattleManager.Reset();

		//激活地图对象
		GameMain.obj1.SetActive(true);
		GameMain.obj2.SetActive(true);

		//关闭界面
		PanelManager.Close("HomePanel");
		PanelManager.Close("RoomPanel");
		PanelManager.Close("ResultPanel");

		//打开界面（用于显示ctrl角色血条）
		PanelManager.Open<BattlePanel>(defaultWeapon);

		//产生角色，分两次生成
		// 第一次生成自己（确保GameMain.camp被赋值）
		foreach (HumanInfo t in msg.humans) {
			if (t.id == GameMain.id) {
				GenerateHuman(t);
				break;
			}
		}
		// 第二次生成其他人（根据GameMain.camp加载队友和敌人预设）
		foreach (HumanInfo t in msg.humans) {
			if (t.id != GameMain.id) {
				GenerateHuman(t);
			}
		}

	}

	//产生角色
	public static void GenerateHuman(HumanInfo humanInfo){
		// 实例化预制体
		GameObject humanPrefab = null;
		// 根据身份选择预制体路径
		if(humanInfo.id == GameMain.id) {
			humanPrefab = ResManager.LoadPrefab(HumanPath0); // 玩家自身预制体（ctrl脚本）
		}
		else {
			humanPrefab = humanInfo.camp == GameMain.camp 
			? ResManager.LoadPrefab(HumanPath1) // 友军预制体（sync脚本）
			: ResManager.LoadPrefab(HumanPath2); // 敌军预制体（sync脚本）
		}

		// 实例化对象
		GameObject humanObj = UnityEngine.Object.Instantiate(humanPrefab) as GameObject;
		humanObj.name = "Human_" + humanInfo.id;
			/* 由于BattleManager是静态工具类，不继承自MonoBehaviour，调用Unity的实例化方法Instantiate需要明确指定命名空间*/

		////预制体包含自定义脚本组件
		BaseHuman human = humanObj.GetComponent<BaseHuman>();
		human.skin = humanObj;

		if(humanInfo.id == GameMain.id) {
			humanObj.tag = "Player";
			GameMain.camp = humanInfo.camp;

			GenerateCamera(humanObj.transform);	    //相机目标
		}
		else {
			humanObj.tag = "SyncHuman";
		}
		//产生武器
		GenerateWeapon(humanObj);

		//属性
		human.camp = humanInfo.camp;
		human.id = humanInfo.id;
		//human.hp = humanInfo.hp;
		//位置，旋转
		Vector3 pos = new Vector3(humanInfo.x, humanInfo.y, humanInfo.z);
		Vector3 rot = new Vector3(humanInfo.ex, humanInfo.ey, humanInfo.ez);
		human.transform.position = pos;
		human.transform.eulerAngles = rot;

		//列表
		AddHuman(humanInfo.id, human);

		////需要在角色模型生成且camp设置好后调用
		//产生角色血条
		HealthController healthController = human.GetComponent<HealthController>(); 
		healthController.Init();

	}

	//设置相机（相机预制体包含自定义脚本组件）
	public static void GenerateCamera(Transform transform){
		Camera mainCamera = Camera.main;
		CameraFollow cf = mainCamera.GetComponent<CameraFollow>(); 
		cf.target = transform;		//设置target变量的值
		cf.isTP = true;			//设置初始视角为第三人称视角
	}

	//产生武器
	public static void GenerateWeapon(GameObject humanObj){
		Camera mainCamera = Camera.main;

		BaseHuman bh = humanObj.GetComponent<BaseHuman>(); 
		bh.InitWeaponDB(weaponDB);	//初始化所有的BaseHuman.weaponDB
		/* BaseHuman.InitWeaponDB() 要先于Attacking.UpdateWeapon、SyncHuman.SyncFire执行 */

		if(humanObj.tag == "Player"){
			// 根据武器名称加载预制体
        			GameObject weaponPrefab = ResManager.LoadPrefab($"Perfabs/Weapon/{defaultWeapon.name}");
			// 实例化对象
			GameObject weaponObj = UnityEngine.Object.Instantiate(weaponPrefab) as GameObject;
			weaponObj.tag = "FPModel";		//ctrl的武器标签改为FPModel
			
			////需要在角色模型和ctrl武器模型生成后调用
			Attacking ac = humanObj.GetComponent<Attacking>(); 
			ac.UpdateWeapon(defaultWeapon.name);
		}
		else if(humanObj.tag == "SyncHuman"){
			////调用了ViewConfig脚本里的setTPView方法，生成武器
			ViewConfig config = mainCamera.GetComponent<ViewConfig>(); 
			config.setTPView(humanObj, defaultWeapon);
		}

	}


	//收到进入战斗协议
	public static void OnMsgEnterBattle(MsgBase msgBase){
		MsgEnterBattle msg = (MsgEnterBattle)msgBase;
		EnterBattle(msg);
	}

	//收到战斗结束协议
	public static void OnMsgBattleResult(MsgBase msgBase){
		MsgBattleResult msg = (MsgBattleResult)msgBase;
		//判断显示胜利还是失败
		bool isWin = false;
		BaseHuman human = GetCtrlHuman();
		if(human!= null && human.camp == msg.winCamp){
			isWin = true;
		}

		//取消FP视角的鼠标锁定到屏幕中心
		Cursor.lockState = CursorLockMode.None;

		//显示界面
		PanelManager.Open<ResultPanel>(isWin);
		//关闭界面
		PanelManager.Close("BattlePanel");
		//重置
		BattleManager.Reset();
		Debug.Log("战斗结束：重置场景");

	}

	//收到玩家退出协议
	public static void OnMsgLeaveBattle(MsgBase msgBase){
		MsgLeaveBattle msg = (MsgLeaveBattle)msgBase;
		//查找角色
		BaseHuman human = GetHuman(msg.id);
		if(human == null){
			return;
		}
		//删除角色
		RemoveHuman(msg.id);
		MonoBehaviour.Destroy(human.gameObject);
		//删除角色血条
		if(human.TryGetComponent<HealthController>(out var health)){
			MonoBehaviour.Destroy(health.healthBar);
		}
	}


	//收到同步协议
	public static void OnMsgSyncHuman(MsgBase msgBase){
		MsgSyncHuman msg = (MsgSyncHuman)msgBase;
		//不同步自己
		if(msg.id == GameMain.id){
			return;
		}
		//查找角色
		SyncHuman human = (SyncHuman)GetHuman(msg.id);
		if(human == null){
			return;
		}
		//移动同步
		human.SyncPos(msg);
	}

	//收到开火协议
	public static void OnMsgFire(MsgBase msgBase){
		MsgFire msg = (MsgFire)msgBase;
		//不同步自己
		if(msg.id == GameMain.id){
			return;
		}
		//查找角色
		SyncHuman human = (SyncHuman)GetHuman(msg.id);
		if(human == null){
			return;
		}
		//开火
		human.SyncFire(msg);
	}

	//收到击中协议
	public static void OnMsgHit(MsgBase msgBase){
		MsgHit msg = (MsgHit)msgBase;
		//查找角色
		BaseHuman human = GetHuman(msg.targetId);
		if(human == null){
			return;
		}

		//被击中
		if(human.TryGetComponent<HealthController>(out var health)){
			health.GetHit(msg.damage);
			Debug.Log($"【服务端判定】击中敌人：{human.name}", human.gameObject);
		}else{
			Debug.Log("找不到HealthController组件");
		}
	}

}
