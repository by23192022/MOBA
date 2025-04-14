using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameMain : MonoBehaviour {
	public static string id = "";
	public static int camp = 0;
	//地图
	public static GameObject obj1;
	public static GameObject obj2;

	void Start () {
 		//⽹络监听
 		NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
 		NetManager.AddMsgListener("MsgKick", OnMsgKick);
 		//初始化
 		PanelManager.Init();
 		BattleManager.Init();

 		//打开登录⾯板
 		PanelManager.Open<LoginPanel>();

		//地图
		obj1 = GameObject.Find("DemoAllStatic");
		obj2 = GameObject.Find("DemoEnvironment");
		if (obj1 == null || obj2 == null) Debug.Log("找不到obj1、obj2");
		//设为不可见
		GameMain.obj1.SetActive(false);
		GameMain.obj2.SetActive(false);
	}

	void Update () {
 		NetManager.Update();
	}

	//关闭连接
	void OnConnectClose(string err){
		Debug.Log("断开连接");
	} 
	//被踢下线
	void OnMsgKick(MsgBase msgBase){
		PanelManager.Open<TipPanel>("被踢下线");
	}

}

