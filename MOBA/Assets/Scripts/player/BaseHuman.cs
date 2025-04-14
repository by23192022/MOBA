using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BaseHuman : MonoBehaviour {
    //模型
    public GameObject skin;

    public float moveSpeed = 3f;     	// 恒定移动速度
    public float rotationSpeed = 10f; 	// 转向速度

    private Vector3 lastPosition;
    private Vector3 currentVelocity; 	// Vector3 类型
    public float currentSpeed { get; private set; } // 外部只读

    //属于哪⼀名玩家
    public string id = "";
    //阵营
    public int camp = 0;

    //配置武器
    public WeaponDB weaponDB;

    //声明事件：远程武器攻击动画播放
    //public static event Action OnAttack01; 	// 声明静态事件
    public event Action OnAttack01; 		// 声明实例事件（非静态），AniController.cs订阅
    //触发该事件
    public void InvokeEvent01()
    {
        OnAttack01?.Invoke();
    }


    ////在BattleManager.cs中以下方法先于Attacking.UpdateWeapon执行。
    public void InitWeaponDB (WeaponDB _weaponDB) 
    {
        weaponDB = _weaponDB;
        Debug.Log($"BaseHuman【{id}】：已完成weaponDB初始化，weaponDB={weaponDB}");
    }


    void Start () 
    {
        lastPosition = transform.position; // 初始化上一帧位置，避免第一帧计算出错

/*
        if(weaponDB == null)
                weaponDB = Resources.Load<WeaponDB>("Data/WeaponDatabase"); // 加载资源
*/
    }

    protected virtual void Update () 
    {
        CalculateSpeed(); // 计算速度
    }


    void CalculateSpeed()
    {
        // 计算位移差（Vector3）
        Vector3 displacement = transform.position - lastPosition;
        currentVelocity = displacement / Time.deltaTime;

        // 计算水平速度标量值（忽略Y轴高度变化）
        currentSpeed = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;

        // 记录上一帧位置
        lastPosition = transform.position;

        //Debug.Log($"1)当前速度: {currentSpeed}");
    }

    //射击射线特效
    public void CreateBulletTrail(LineRenderer bulletTrail, Vector3 startPoint, Vector3 endPoint)
    {
        LineRenderer trail = Instantiate(bulletTrail, startPoint, Quaternion.identity);
        trail.useWorldSpace = true;        //确保使用世界坐标系！

        if (trail == null)
        {
            Debug.LogError("子弹轨迹实例化失败");
            return;
        }

        trail.SetPosition(0, startPoint);
        trail.SetPosition(1, endPoint);
        Destroy(trail.gameObject, 0.5f);
    }

    //击中特效
    public void CreateHitEffect(ParticleSystem hitEffect, Vector3 hitPoint, Vector3 hitNormal)
    {
        ParticleSystem h = Instantiate(hitEffect, hitPoint, Quaternion.LookRotation(hitNormal));
        Destroy(h.gameObject, 0.7f);
    }

}