using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SyncHuman : BaseHuman {
    //预测信息，哪个时间到达哪个位置
    private Vector3 forecastPos;	//预测的信息
    private Vector3 forecastRot;
    private float forecastTime;		//最近⼀次收到的位置同步协议的时间

    private Transform muzzlePoint_TP; 	//枪口位置


    void Start()
    {
        //初始化预测信息
        forecastPos = transform.position;
        forecastRot = transform.eulerAngles;
        forecastTime = Time.time;

        muzzlePoint_TP = FindChild(transform, "Point");	//没有TP持枪动画，暂时使用Point点
    }

    // 递归查找子物体方法
    private Transform FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var result = FindChild(child, name);
            if (result != null) 
                return result;
        }
        return null;
    }


    protected override void Update()
    {
        base.Update();
        ForecastUpdate();        //更新位置
    }

    //更新位置
    public void ForecastUpdate()
    {
        //时间（同步帧率syncInterval=0.1f ）
        float t = (Time.time - forecastTime)/PlayerController.syncInterval;
        t = Mathf.Clamp(t, 0f, 1f);				//归⼀化的时间差

        //位置
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, forecastPos, t);			//线性插值
        transform.position = pos;
        //旋转
        Quaternion quat = transform.rotation;
        Quaternion forcastQuat = Quaternion.Euler(forecastRot);
        quat = Quaternion.Lerp(quat, forcastQuat, t);			//线性插值
        transform.rotation = quat;
    }


    //移动同步
    public void SyncPos(MsgSyncHuman msg)
    {
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);

        forecastPos = pos;	//跟随不预测
        forecastRot = rot;

        //更新
        lastPos = pos;
        lastRot = rot;
        forecastTime = Time.time;
    }

    //开⽕（客户端收到MsgFire协议，会调用该方法）
    public void SyncFire(MsgFire msg)
    {
        if (weaponDB == null) Debug.Log("SyncHuman：weaponDB为空，请检查代码执行顺序");

        Weapon weapon = weaponDB.GetWeapon(msg.weaponName);	//基类变量weaponDB
        //Vector3 startPoint = new Vector3(msg.x, msg.y, msg.z);	//起点
        Vector3 startPoint = muzzlePoint_TP.position;	        	//起点固定为角色模型的muzzlePoint.position
        Vector3 endPoint = new Vector3(msg.ex, msg.ey, msg.ez);	//终点

        switch (weapon.category)
        {
            case WeaponCategory.Melee:
	//触发动画
                InvokeEvent01();		  //基类方法
                break;
                
            case WeaponCategory.Ranged:
	//同步攻击特效
                CreateBulletTrail(weapon.bulletTrail, startPoint, endPoint);		//基类方法
                //终点是否生成击中特效
                if(msg.isHit) 
                {
                        Vector3 hitNormal = new Vector3(msg.hnx, msg.hny, msg.hnz);	//击中特效方向（法线）
                        CreateHitEffect(weapon.hitEffect, endPoint, hitNormal); 		//基类方法
                }
	//触发动画
                break;
        }
    }

}