using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : BasePanel {
	//等级变化
	private Text gradeText;
	private Text gradeNum;
	//胜利提示图片
	private Image winImage;
	private Text winText;
	//失败提示图片
	private Image lostImage;
	private Text lostText;
	//确定按钮
	private Button okBtn;

	//初始化
	public override void OnInit() {
		skinPath = "Perfabs/Panel/ResultPanel";
		layer = PanelManager.Layer.Tip;
	}
	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		gradeText = skin.transform.Find("GradeText").GetComponent<Text>();
		gradeNum = skin.transform.Find("GradeNum").GetComponent<Text>();
		winImage = skin.transform.Find("WinImage").GetComponent<Image>();
		winText = skin.transform.Find("WinText").GetComponent<Text>();
		lostImage = skin.transform.Find("LostImage").GetComponent<Image>();
		lostText = skin.transform.Find("LostText").GetComponent<Text>();
		okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
		//监听
		okBtn.onClick.AddListener(OnOkClick);
		//协议监听
		NetManager.AddMsgListener("MsgGetPerson", OnMsgGetPerson);

		//显示哪个图片
		if(args.Length == 1){
			bool isWIn = (bool)args[0];
			if(isWIn){
				winImage.gameObject.SetActive(true);
				winText.gameObject.SetActive(true);
				lostImage.gameObject.SetActive(false);
				lostText.gameObject.SetActive(false);
				gradeText.text = "+1";
			}else{
				winImage.gameObject.SetActive(false);
				winText.gameObject.SetActive(false);
				lostImage.gameObject.SetActive(true);
				lostText.gameObject.SetActive(true);
				gradeText.text = "-1";
			}
		}

		//发送查询
		MsgGetPerson msgGetPerson = new MsgGetPerson();
		NetManager.Send(msgGetPerson);
	}
		
	//关闭
	public override void OnClose() {
		//Unity事件监听
		okBtn.onClick.RemoveListener(OnOkClick);
		//协议监听
		NetManager.RemoveMsgListener("MsgGetPerson", OnMsgGetPerson);
	}

	//当按下确定按钮
	public void OnOkClick(){
		PanelManager.Open<HomePanel>();
		PanelManager.Open<RoomPanel>();
		Close();
	}

	//收到个人数据查询协议
	public void OnMsgGetPerson (MsgBase msgBase) {
		MsgGetPerson msg = (MsgGetPerson)msgBase;
		gradeNum.text = ""+msg.grade;	//把int型转换成string
	}
}
