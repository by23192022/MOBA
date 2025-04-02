using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameMain : MonoBehaviour {
	public static string id = "";

/*
	private string ip = "127.0.0.1";
	private int port = 8888;
*/
	void Start () {
 		//⽹络监听
 		NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
 		NetManager.AddMsgListener("MsgKick", OnMsgKick);
 		//初始化
 		PanelManager.Init();

/*
		//网络事件监听
		NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
		NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
		//连接服务器
		NetManager.Connect(ip, port);
*/
 		//打开登录⾯板
 		PanelManager.Open<LoginPanel>();
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

/*
	//连接成功回调
	void OnConnectSucc(string err){
		PanelManager.Open<TipPanel2>("服务器连接成功");
		Debug.Log("OnConnectSucc");
	}
	//连接失败回调
	void OnConnectFail(string err){
		//showConnFail = true;
		PanelManager.Open<TipPanel>(err);
	}
*/
}

