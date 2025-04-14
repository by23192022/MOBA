//挂载在Player、Sync身上
//血条生成与血量控制

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;

//需要使用角色的camp参数，不能设为BaseHuman的分部类（与它同一时间执行）
//public partial class BaseHuman : MonoBehaviour
public class HealthController : MonoBehaviour
{
    // 同步角色的World Space血条
    public string HealthBarPath_T = "Perfabs/Health/CanvasTeammate";		//队友
    public string HealthBarPath_E = "Perfabs/Health/CanvasEnemy";		//敌人

    public GameObject healthBar;				//当前生成的血条
    public Vector3 offset = new Vector3(0, 1.2f, 0);			//同步角色血条位置

    private Slider healthSlider;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth;

    private Camera mainCamera;
    private BattlePanel battlePanel;
    private BaseHuman baseHuman;

    public event System.Action OnHit; // 实例事件
    public event System.Action OnDeath; // 实例事件
    public event System.Action OnRecover; // 实例事件

    ////外部调用Init()，先执行Init()再执行Start()
    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // 更新同步角色血条位置和方向
        if (gameObject.CompareTag("SyncHuman") && healthBar != null)
        {
	// 更新血条位置
       	healthBar.transform.position = transform.position + offset;
       	// 血条面向摄像机
       	healthBar.transform.rotation = mainCamera.transform.rotation;
        }
    }


    ////需要在角色的camp设置好后调用，BattleManager.cs
    //初始化
    public void Init() 
    {
        baseHuman = gameObject.GetComponent<BaseHuman>();

        currentHealth = maxHealth;
        SetHealthSlider();	//根据角色类型设置血条
    }

    private void SetHealthSlider()
    {
        if (gameObject.CompareTag("Player"))
        {
	battlePanel = PanelManager.GetPanel<BattlePanel>();
	healthSlider = battlePanel.GetHealthSlider();
        }
        else if (gameObject.CompareTag("SyncHuman"))
        {
	GameObject healthBarPrefab = null;
	if(baseHuman.camp == GameMain.camp)				//队友
	{
		healthBarPrefab = ResManager.LoadPrefab(HealthBarPath_T);
	}
	else if (baseHuman.camp != GameMain.camp && baseHuman.camp != 0)	//敌人
	{
		healthBarPrefab = ResManager.LoadPrefab(HealthBarPath_E);
	}
	else
	{
		Debug.Log("无法加载sync血条");
	}

	healthBar = Instantiate(healthBarPrefab, transform.position + offset, Quaternion.identity);
	// 获取 Canvas 组件并设置 Event Camera            
	healthBar.GetComponent<Canvas>().worldCamera = mainCamera;
	healthSlider = healthBar.GetComponentInChildren<Slider>();
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        Debug.Log($"【{baseHuman.id}】血条初始值: {healthSlider.value}");
    }


    public void GetHit(float damage)
    {
        //OnHit?.Invoke(); 	// 触发事件，播放受击动画

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"【{baseHuman.id}】当前生命值: {currentHealth}"); 

        healthSlider.value = currentHealth;
        if(currentHealth <= 0) 
            Death();
        else 
            OnHit?.Invoke(); 	// 触发事件，播放受击动画
/* 
 限制死亡后不可以播放GetHit动画
 最后一次伤害不再播放GetHit，而是播放Death
*/
    }

    public void Death()
    {
        OnDeath?.Invoke(); // 触发事件，播放死亡动画

/*
        Destroy(gameObject);			// 销毁角色
        if(gameObject.CompareTag("Enemy"))	// 同步销毁血条（仅敌人需要）
        {
            Destroy(healthBar);
        }
*/
    }

    public void Recover(float health)
    {
        currentHealth = Mathf.Min(maxHealth, health);  
        Debug.Log($"【{baseHuman.id}】当前生命值: {currentHealth}"); 

        if (currentHealth > 0)
        {
            OnRecover?.Invoke(); // 触发事件，播放复活动画
            healthSlider.value = currentHealth;
        }
    }

}
