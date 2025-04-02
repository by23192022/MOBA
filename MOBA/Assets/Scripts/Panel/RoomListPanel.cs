using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomListPanel : BasePanel {
	//创建关闭按钮
	private Button closeButton;
	//创建房间按钮
	private Button createButton;
	//刷新列表按钮
	private Button reflashButton;
	//列表容器
	private Transform content;
	//房间物体
	private GameObject roomObj;

	//初始化
	public override void OnInit() {
		skinPath = "Perfabs/Panel/RoomListPanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		closeButton = skin.transform.Find("CloseBtn").GetComponent<Button>();
		createButton = skin.transform.Find("CreateBtn").GetComponent<Button>();
		reflashButton = skin.transform.Find("ReflashBtn").GetComponent<Button>();
		content = skin.transform.Find("Scroll View/Viewport/Content");
		roomObj = skin.transform.Find("Room").gameObject;

		//不激活房间
		roomObj.SetActive(false);
		//按钮事件
		closeButton.onClick.AddListener(OnCloseClick);
		createButton.onClick.AddListener(OnCreateClick);
		reflashButton.onClick.AddListener(OnReflashClick);
		//协议监听
		NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
		NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
		NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
		//发送查询
		MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
		NetManager.Send(msgGetRoomList);
	}

	//关闭
	public override void OnClose() {
		//Unity事件监听
		closeButton.onClick.RemoveListener(OnCloseClick);
		createButton.onClick.RemoveListener(OnCreateClick);
		reflashButton.onClick.RemoveListener(OnReflashClick);

		//协议监听
		NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
		NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
		NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
	}


	//点击关闭按钮
	public void OnCloseClick(){
		Close();		
	}

	//点击刷新按钮
	public void OnReflashClick(){
		MsgGetRoomList msg = new MsgGetRoomList();
		NetManager.Send(msg);
	}

	//收到房间列表协议
	public void OnMsgGetRoomList (MsgBase msgBase) {
		MsgGetRoomList msg = (MsgGetRoomList)msgBase;
		//清除房间列表
		for(int i = content.childCount-1; i >= 0 ; i--){
			GameObject o = content.GetChild(i).gameObject;
			Destroy(o);
		}
		//重新生成列表
		if(msg.rooms == null){
			return;
		}
		for(int i = 0; i < msg.rooms.Length; i++){
			GenerateRoom(msg.rooms[i]);
		}
	}

	//创建一个房间单元
	public void GenerateRoom(RoomInfo roomInfo){
		//创建物体
		GameObject o = Instantiate(roomObj);
		o.transform.SetParent(content);
		o.SetActive(true);
		//o.transform.localScale = Vector3.one;
		//获取组件
		Transform trans = o.transform;
		//Image idImage= trans.Find("IdImage").GetComponent<Image>();
		Text idText = trans.Find("IdText").GetComponent<Text>();
		Text countText = trans.Find("CountText").GetComponent<Text>();
		Text statusText = trans.Find("StatusText").GetComponent<Text>();
		Button btn = trans.Find("JoinButton").GetComponent<Button>();
		//填充信息
		//idImage.image = ;
		idText.text = roomInfo.id.ToString();
		countText.text = roomInfo.count.ToString();
		if(roomInfo.status == 0){
			statusText.text = "准备中";
		}
		else{
			statusText.text = "战斗中";
		}
		//按钮事件
		btn.name = idText.text;
/*通过匿名委托/lambda 捕获当前btn.name的值*/
		btn.onClick.AddListener(delegate(){
			OnJoinClick(btn.name);
		});
		//btn.onClick.AddListener(() => OnJoinClick(btn.name));

	}

	//点击加入房间按钮
	public void OnJoinClick(string idString) {
		MsgEnterRoom msg = new MsgEnterRoom();
		msg.id = int.Parse(idString);
		NetManager.Send(msg);
	}

	//收到进入房间协议
	public void OnMsgEnterRoom (MsgBase msgBase) {
		MsgEnterRoom msg = (MsgEnterRoom)msgBase;
		//成功进入房间
		if(msg.result == 0){
			PanelManager.Open<TipPanel2>("加入成功");
			PanelManager.Open<RoomPanel>();
			Close();
		}
		//进入房间失败
		else{
			PanelManager.Open<TipPanel>("进入房间失败");
		}
	}

	//点击新建房间按钮
	public void OnCreateClick() {
		MsgCreateRoom msg = new MsgCreateRoom();
		NetManager.Send(msg);
	}

	//收到新建房间协议
	public void OnMsgCreateRoom (MsgBase msgBase) {
		MsgCreateRoom msg = (MsgCreateRoom)msgBase;
		//成功创建房间
		if(msg.result == 0){
			PanelManager.Open<TipPanel2>("创建成功");
			PanelManager.Open<RoomPanel>();
			Close();
		}
		//创建房间失败
		else{
			PanelManager.Open<TipPanel2>("创建房间失败");
		}
	}

}
