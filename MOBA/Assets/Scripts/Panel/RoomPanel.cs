using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomPanel : BasePanel {
	//退出按钮
	private Button closeButton;
	//开战按钮
	private Button startButton;
	//列表容器
	private Transform content;

	//玩家信息物体
	private GameObject playerObj;

	//初始化
	public override void OnInit() {
		skinPath = "Perfabs/Panel/RoomPanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		startButton = skin.transform.Find("StartBtn").GetComponent<Button>();
		closeButton = skin.transform.Find("CloseBtn").GetComponent<Button>();
		content = skin.transform.Find("Scroll View/Viewport/Content");
		playerObj =  skin.transform.Find("Player").gameObject;
		//不激活玩家信息
		playerObj.SetActive(false);
		//按钮事件
		startButton.onClick.AddListener(OnStartClick);
		closeButton.onClick.AddListener(OnCloseClick);
		//协议监听
		NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
		NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
		NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);
		//发送查询
		MsgGetRoomInfo msg = new MsgGetRoomInfo();
		NetManager.Send(msg);
        		Debug.Log("执行发送MsgGetRoomInfo");
	}

	//关闭
	public override void OnClose() {
		//Unity事件监听
		startButton.onClick.RemoveListener(OnStartClick);
		closeButton.onClick.RemoveListener(OnCloseClick);

		//协议监听
		NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
		NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
		NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
	}

	//收到玩家列表协议
	public void OnMsgGetRoomInfo (MsgBase msgBase) {
		MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
		Debug.Log("收到OnMsgGetRoomInfo");

		//清除玩家列表
		for(int i = content.childCount-1; i >= 0 ; i--){
			GameObject o = content.GetChild(i).gameObject;
			Destroy(o);
		}

		//重新生成列表
		if(msg.players == null){
			return;
		}
		for(int i = 0; i < msg.players.Length; i++){
			GeneratePlayerInfo(msg.players[i]);
		}
	}

	//创建一个玩家信息单元
	public void GeneratePlayerInfo(PlayerInfo playerInfo){
		//创建物体
		GameObject o = Instantiate(playerObj);
		o.SetActive(true);
		o.transform.SetParent(content);

		//获取组件
		Transform trans = o.transform;
		Text idText = trans.Find("IdText").GetComponent<Text>();
		Image idImage = trans.Find("IdImage").GetComponent<Image>();
		Text ownerText = trans.Find("OwnerText").GetComponent<Text>();
		Text achieveText = trans.Find("AchieveText").GetComponent<Text>();
		Text campText = trans.Find("CampText").GetComponent<Text>();

		//填充信息
		idText.text = playerInfo.id;
		//idImage.image = ;
		//房主标志
		if(playerInfo.isOwner == 1){
			ownerText.text = "房主";
		}
		else{
			ownerText.text = "";
		}

		achieveText.text = "胜" + playerInfo.win + " 负" + playerInfo.lost;

		//设置阵营 1-A 2-B
		if(playerInfo.camp == 1){
			campText.text = "A";
		}
		else {
			campText.text = "B";
		}
	}

	//点击退出按钮
	public void OnCloseClick(){
		MsgLeaveRoom msg = new MsgLeaveRoom();
		NetManager.Send(msg);
	}

	//收到退出房间协议
	public void OnMsgLeaveRoom (MsgBase msgBase) {
		MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
		//成功退出房间
		if(msg.result == 0){
			PanelManager.Open<TipPanel2>("已退出房间");
			PanelManager.Open<RoomListPanel>();
			Close();
		}
		//退出房间失败
		else{
			PanelManager.Open<TipPanel>("退出房间失败");
		}
	}

	//点击开战按钮
	public void OnStartClick(){
		MsgStartBattle msg = new MsgStartBattle();
		NetManager.Send(msg);
	}

	//收到开战返回
	public void OnMsgStartBattle (MsgBase msgBase) {
		MsgStartBattle msg = (MsgStartBattle)msgBase;
		//开战
		if(msg.result == 0){
			//关闭界面
			Close();
		}
		//开战失败
		else{
			PanelManager.Open<TipPanel>("开战失败！两队至少都需要一名玩家，只有队长可以开始战斗！");
		}
	}

}
