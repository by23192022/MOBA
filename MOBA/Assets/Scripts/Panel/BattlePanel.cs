using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel {
	//ctrl血条
	private Slider healthSlider;
	//FP准星
	private Image crosshairImage;
	private CameraFollow cameraFollow;
	private Weapon CurrentWeapon;


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
		crosshairImage = skin.transform.Find("CrosshairImage").GetComponent<Image>();

		cameraFollow = Camera.main.GetComponent<CameraFollow>();
		CurrentWeapon = (Weapon)args[0];
	}

	//关闭
	public override void OnClose() {

	}

	public void Update(){
		if ( cameraFollow == null || CurrentWeapon == null ) return;

		if ( !cameraFollow.isTP && CurrentWeapon.category == WeaponCategory.Ranged )
			crosshairImage.gameObject.SetActive(true);
		else
			crosshairImage.gameObject.SetActive(false);
	}

}