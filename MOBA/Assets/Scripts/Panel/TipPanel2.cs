using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel2 : BasePanel {
	//提示文本
	private Text text;
	private Coroutine autoClose; // 协程引用

	//初始化
	public override void OnInit() {
		skinPath = "Perfabs/Panel/TipPanel2";
		layer = PanelManager.Layer.Tip;
	}
	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		text = skin.transform.Find("Text").GetComponent<Text>();
		//提示语
		if(args.Length == 1){
			text.text = (string)args[0];
		}

		// 启动自动关闭协程
		if(autoClose != null) {
			StopCoroutine(autoClose);
		}
		autoClose = StartCoroutine(AutoClose());
	}

	// 自动关闭协程
	private IEnumerator AutoClose() {
		yield return new WaitForSeconds(1f); // 等待1秒
		// 停止正在运行的协程
		if(autoClose != null) {
			StopCoroutine(autoClose);
			autoClose = null;
		}

		// 关闭面板（基类方法）
		Close();
	}

}
