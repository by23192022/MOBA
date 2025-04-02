using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonPanel : BasePanel {
	//账号文本
	private Text idText;
	//账号头像
	//private Image idImage;
	//个性签名文本
	private InputField signInput;
	//等级文本
	private Text gradeText;
	//战绩文本
	private Text achieveText;
	//关注按钮
	private Button followButton;
	//关闭按钮
	private Button closeButton;

	//初始化
	public override void OnInit() {
		skinPath = "Perfabs/Panel/PersonPanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		idText = skin.transform.Find("IdText").GetComponent<Text>();
		//idImage = skin.transform.Find("IdImage").GetComponent<Button>();
		signInput = skin.transform.Find("SignInput").GetComponent<InputField>();
		gradeText = skin.transform.Find("GradeText").GetComponent<Text>();
		achieveText = skin.transform.Find("AchieveText").GetComponent<Text>();
		followButton = skin.transform.Find("FollowBtn").GetComponent<Button>();
		closeButton = skin.transform.Find("CloseBtn").GetComponent<Button>();

		//显示id
		if (args?[0] is string id && !string.IsNullOrEmpty(id)) 
		{
        			idText.text = id;
		}
		//显示头像
		//idButton.image = ;
		//按钮事件
		followButton.onClick.AddListener(OnFollowClick);
		closeButton.onClick.AddListener(OnCloseClick);
		//结束编辑事件
		signInput.onEndEdit.AddListener(OnSignSave);
		//协议监听
		NetManager.AddMsgListener("MsgGetPerson", OnMsgGetPerson);
		NetManager.AddMsgListener("MsgSavePerson", OnMsgSavePerson);
		NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
		//发送查询
		MsgGetPerson msgGetPerson = new MsgGetPerson();
		NetManager.Send(msgGetPerson);
		MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
		NetManager.Send(msgGetAchieve);
	}

	//关闭
	public override void OnClose() {
		//Unity事件监听
		followButton.onClick.RemoveListener(OnFollowClick);
		closeButton.onClick.RemoveListener(OnCloseClick);
		signInput.onEndEdit.RemoveListener(OnSignSave);
		//协议监听
		NetManager.RemoveMsgListener("MsgGetPerson", OnMsgGetPerson);
		NetManager.RemoveMsgListener("MsgSavePerson", OnMsgSavePerson);
		NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
	}

	//点击关注按钮
	public void OnFollowClick(){
		PanelManager.Open<TipPanel>("正在建设中...");
	}

	//点击关闭按钮
	public void OnCloseClick(){
		Close();		
	}

	//收到个人数据查询协议
	public void OnMsgGetPerson (MsgBase msgBase) {
		MsgGetPerson msg = (MsgGetPerson)msgBase;
		gradeText.text = ""+msg.grade;	//把int型转换成string
		if(msg.sign != null ) {
			signInput.text = msg.sign;
		}
	}

/*
InputField.onEndEdit 事件需要接收一个接受string参数的委托（UnityAction<string>）
*/
	//个性签名输入框结束编辑后
	public void OnSignSave(string text)
	{
		MsgSavePerson msgSavePerson = new MsgSavePerson();
		msgSavePerson.sign = text;
		NetManager.Send(msgSavePerson);
	}

	//收到个人数据保存协议
	public void OnMsgSavePerson (MsgBase msgBase) {
		MsgSavePerson msg = (MsgSavePerson)msgBase;
		 if(msg.result == 0){
			 PanelManager.Open<TipPanel2>("修改成功");
		 }
		 else{
			 PanelManager.Open<TipPanel2>("修改失败，字数太长");
		 }
	}

	//收到成绩查询协议
	public void OnMsgGetAchieve (MsgBase msgBase) {
		MsgGetAchieve msg = (MsgGetAchieve)msgBase;
		achieveText.text = "胜 " + msg.win + "负"+ msg.lost ;
	}

}
