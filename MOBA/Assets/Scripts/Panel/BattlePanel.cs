using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel {
	//ctrl血条
	private Slider healthSlider;

	public Slider GetHealthSlider() {
		return healthSlider; // 虽然变量是private，但通过公共方法返回
	}


	//初始化
	public override void OnInit() {
		skinPath = "Perfabs/Panel/BattlePanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		healthSlider = skin.transform.Find("HealthSlider").GetComponent<Slider>();
	}

	//关闭
	public override void OnClose() {

	}

	public void Update(){

	}

}