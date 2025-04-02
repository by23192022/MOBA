using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePanel : BasePanel {
	//账号文本
	private Text idText;
	//账号按钮（含头像）
	private Button idButton;
	//等级文本
	private Text gradeText;
	//金币文本
	private Text coinText;
	//开始按钮
	private Button playButton;
	//道具按钮
	private Button gameButton;
	//商城按钮
	private Button shopButton;
	//好友按钮
	private Button friendButton;
	//介绍按钮
	private Button infoButton;

	//初始化
	public override void OnInit() {
		skinPath = "Perfabs/Panel/HomePanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		idText = skin.transform.Find("MyPanel/IdText").GetComponent<Text>();
		idButton = skin.transform.Find("MyPanel/IdBtn").GetComponent<Button>();
		gradeText = skin.transform.Find("MyPanel/GradeText").GetComponent<Text>();
		coinText = skin.transform.Find("MyPanel/CoinText").GetComponent<Text>();
		playButton = skin.transform.Find("OptionPanel/PlayBtn").GetComponent<Button>();
		gameButton = skin.transform.Find("OptionPanel/GameBtn").GetComponent<Button>();
		shopButton = skin.transform.Find("OptionPanel/ShopBtn").GetComponent<Button>();
		friendButton = skin.transform.Find("OptionPanel/FriendBtn").GetComponent<Button>();
		infoButton = skin.transform.Find("OptionPanel/InfoBtn").GetComponent<Button>();

		//显示id
		idText.text = GameMain.id;
		//按钮事件
		idButton.onClick.AddListener(OnIdClick);
		playButton.onClick.AddListener(OnPlayClick);
		gameButton.onClick.AddListener(OnGameClick);
		shopButton.onClick.AddListener(OnShopClick);
		friendButton.onClick.AddListener(OnFriendClick);
		infoButton.onClick.AddListener(OnInfoClick);
		//协议监听
		NetManager.AddMsgListener("MsgGetPerson", OnMsgGetPerson);
		//发送查询
		MsgGetPerson msgGetPerson = new MsgGetPerson();
		NetManager.Send(msgGetPerson);
	}

	//关闭
	public override void OnClose() {
		//Unity事件监听
		idButton.onClick.RemoveListener(OnIdClick);
		playButton.onClick.RemoveListener(OnPlayClick);
		gameButton.onClick.RemoveListener(OnGameClick);
		shopButton.onClick.RemoveListener(OnShopClick);
		friendButton.onClick.RemoveListener(OnFriendClick);
		infoButton.onClick.RemoveListener(OnInfoClick);

		//协议监听
		NetManager.RemoveMsgListener("MsgGetPerson", OnMsgGetPerson);
	}

	//收到个人数据查询协议
	public void OnMsgGetPerson (MsgBase msgBase) {
		MsgGetPerson msg = (MsgGetPerson)msgBase;
		gradeText.text = ""+msg.grade;	//把int型转换成string
		coinText.text =  ""+msg.coin;
	}

	//点击头像按钮
	public void OnIdClick(){
		PanelManager.Open<PersonPanel>(idText.text);
	}

	//点击开始按钮
	public void OnPlayClick(){
		PanelManager.Open<RoomListPanel>();
	}

	//点击道具按钮
	public void OnGameClick(){
		PanelManager.Open<TipPanel>("正在建设中...");
	}

	//点击商城按钮
	public void OnShopClick(){
		PanelManager.Open<TipPanel>("正在建设中...");
	}

	//点击好友按钮
	public void OnFriendClick(){
		PanelManager.Open<TipPanel>("正在建设中...");
	}

	//点击介绍按钮
	public void OnInfoClick(){
		PanelManager.Open<TipPanel>("正在建设中...");
	}

}
