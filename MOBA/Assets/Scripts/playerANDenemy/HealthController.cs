//挂载在Player、Enemy身上
//血条生成与血量控制

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;

public class HealthController : MonoBehaviour
{
    [Header("血条预制体")]
    public GameObject healthBarPlayerPrefab;	// 玩家的Screen Space血条
    public GameObject healthBarEnemyPrefab;	// 敌人的World Space血条
    private GameObject healthBar;		// 当前生成的血条
    private Slider healthSlider;

    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth;

    public Vector3 offset = new Vector3(0, 1.2f, 0);
    private Camera mainCamera;

    public event System.Action OnHit; // 实例事件
    public event System.Action OnDeath; // 实例事件
    public event System.Action OnRecover; // 实例事件

    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;

        if(healthBarPlayerPrefab != null && healthBarEnemyPrefab != null)
        {
            CreateHealthBar();		// 根据标签生成对应血条
            InitializeHealthSlider();		// 初始化血条数值
        }
        else
        {
            Debug.Log("血条预制体为空！请检查Inspector面板");
        }
    }

    void LateUpdate()
    {

        // 更新敌人血条位置和方向
        if (gameObject.CompareTag("Enemy") && healthBar != null)
        {
	// 更新血条位置
       	healthBar.transform.position = transform.position + offset;
       	// 血条面向摄像机
       	healthBar.transform.rotation = mainCamera.transform.rotation;
        }
    }

    private void CreateHealthBar()
    {
        if (gameObject.CompareTag("Player"))
        {
            // 玩家血条：直接实例化并绑定到场景中的UI Canvas
            healthBar = Instantiate(healthBarPlayerPrefab);
            healthBar.transform.SetParent(GameObject.Find("UI Canvas").transform, false); 
            Debug.Log("这是玩家");
        }
        else if(gameObject.CompareTag("Enemy"))
        {
            healthBar = Instantiate(healthBarEnemyPrefab, transform.position + offset, Quaternion.identity);
            // 获取 Canvas 组件并设置 Event Camera            
            healthBar.GetComponent<Canvas>().worldCamera = mainCamera;
            Debug.Log("这是敌人");
        }
        else
        {
            Debug.LogWarning("未识别的角色标签: " + gameObject.tag);
        }
    }

    private void InitializeHealthSlider()
    {
        healthSlider = healthBar.GetComponentInChildren<Slider>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        Debug.Log($"血条初始值: {healthSlider.value}");
    }


    public void GetHit(float damage)
    {
        //OnHit?.Invoke(); 	// 触发事件，播放受击动画

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log("当前生命值: " + currentHealth); 

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
        Debug.Log("当前生命值: " + currentHealth);         
        if (currentHealth > 0)
        {
            OnRecover?.Invoke(); // 触发事件，播放复活动画
            healthSlider.value = currentHealth;
        }
    }

}
