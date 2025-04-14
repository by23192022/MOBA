using UnityEngine;
using System;

public class Attacking : MonoBehaviour
{
    public float rangeValue;      // 射程

    [SerializeField] private Transform muzzlePoint; // 当前枪口位置
    [SerializeField] private Transform muzzlePoint_TP; // 枪口位置
    [SerializeField] private Transform muzzlePoint_FP; // 枪口位置

    private Camera mainCamera;
    private CameraFollow cameraFollow;
    private ViewConfig config;
    private WeaponDB weaponDB;
    private BaseHuman baseHuman;

    public Weapon CurrentWeapon { get; private set; }


    ////Start()执行晚于外部调用的UpdateWeapon()
    //void Start()
    void Init()
    {
        mainCamera = Camera.main;
        cameraFollow = mainCamera.GetComponent<CameraFollow>();
        config = mainCamera.GetComponent<ViewConfig>();
        baseHuman = GetComponent<BaseHuman>();
        weaponDB =  baseHuman.weaponDB;        // 从BaseHuman获取武器数据库
    }

    ////需要在角色模型和ctrl武器模型生成后调用
    public void UpdateWeapon(string weaponName)
    {
        if( weaponDB == null )		//使得Init()只被调用一次即可
            Init();

        if (weaponDB == null) Debug.Log("weaponDB为空，请检查代码执行顺序");

        Weapon newWeapon = weaponDB.GetWeapon(weaponName);
        if (newWeapon == null) return;

        CurrentWeapon = newWeapon;
        Debug.Log($"2)武器更新为：{CurrentWeapon.name}");

        rangeValue = CurrentWeapon.range;

        ////（Transform是引用，不是值；初始化一次即可，无需时刻更新）
        muzzlePoint_TP = FindChild(transform, "Point");	//没有TP持枪动画，暂时使用Point点
        GameObject fp = GameObject.FindWithTag("FPModel");
        muzzlePoint_FP = FindChild(fp.transform, "muzzlePoint");

        if (muzzlePoint_TP == null) Debug.LogError("第三人称枪口位置未找到");
        if (muzzlePoint_FP == null) Debug.LogError("第一人称枪口位置未找到");

        ////需要在角色模型和ctrl武器模型生成后调用
        config.ctrlRun(CurrentWeapon);	//根据武器生成视角
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


    void Update()
    {
        if (cameraFollow == null) return;

        // 第一人称视角：攻击-fire1鼠标左键、(移动-WASD、跳跃-jump空格)
        if(!cameraFollow.isTP && Input.GetButtonDown("Fire1"))
        {
                muzzlePoint = muzzlePoint_FP;

                Debug.Log("攻击：FP鼠标左键按下");
                Attack(false);	//非TP视角

        }
        // 第三人称视角：攻击-空格、(移动-鼠标左键、不可跳跃)
        else if(cameraFollow.isTP && Input.GetButtonDown("Jump"))
        {
                muzzlePoint = muzzlePoint_TP;

                Debug.Log("攻击：TP空格按下");
                Attack(true);	//TP视角
        }
    }

    void Attack(bool isTP)
    {
        if (CurrentWeapon == null) return;
        switch (CurrentWeapon.category)
        {
            case WeaponCategory.Melee:
                Wield();
                baseHuman.InvokeEvent01();
                break;
                
            case WeaponCategory.Ranged:
                Shoot(isTP);
                Debug.Log("触发事件：播放远程武器的攻击动画");
                break;
        }
    }

    //处理刀剑挥砍逻辑
    void Wield()
    {
        Debug.Log("挥刀一次");
    }

    //处理枪支射击逻辑
    void Shoot(bool isTP)
    {
        // 根据视角生成射线
        Ray ray;
        if (isTP)
        {
            ////从玩家模型的正前方发射射线transform.forward
            ray = new Ray(muzzlePoint.position, transform.forward);
            rangeValue = 10;		//适当调整
        }
        else
        {
            //从屏幕中心发射射线
            ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            rangeValue = CurrentWeapon.range;
         }

        // 检测射线碰撞
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rangeValue))
        {
            // 是否击中敌人
            bool isEnemy = hit.collider.TryGetComponent<SyncHuman>(out var human) 
                                      && human.camp != GameMain.camp;

            //发送击中信息（至于扣血处理以服务端为准，见BattleManager.cs的OnMsgHit() ）
            if (isEnemy)
            {
                //发送击中信息
                SyncHit(CurrentWeapon.name, GameMain.id, human.id);
                Debug.Log($"【客户端判定】击中敌人：{hit.collider.gameObject.name}", hit.collider.gameObject);
            }

            // 生成射线特效
            baseHuman.CreateBulletTrail(CurrentWeapon.bulletTrail, muzzlePoint.position, hit.point);
            // 在击中点生成特效
            baseHuman.CreateHitEffect(CurrentWeapon.hitEffect, hit.point, hit.normal);

            //同步特效
            SyncShoot(CurrentWeapon.name, muzzlePoint.position, hit.point, true, hit.normal);
        }
        else
        {
                Debug.Log($"{rangeValue}射程内射线未命中任何物体。");

                // 未命中时创建到达最大射程的轨迹
                Vector3 endPoint = ray.origin + ray.direction * rangeValue;
                baseHuman.CreateBulletTrail(CurrentWeapon.bulletTrail, muzzlePoint.position, endPoint);

                //同步特效
                SyncShoot(CurrentWeapon.name, muzzlePoint.position, endPoint, false, Vector3.zero);
        }
    }

    //同步攻击特效信息
    //private void SyncAttack()
    private void SyncShoot(string weaponName, Vector3 startPoint, Vector3 endPoint, bool isHit, Vector3 hitNormal)
    {
        //发送同步协议
        MsgFire msg = new MsgFire();
        msg.weaponName = weaponName;
        msg.x = startPoint.x;
        msg.y = startPoint.y;
        msg.z = startPoint.z;
        msg.ex = endPoint.x;
        msg.ey = endPoint.y;
        msg.ez = endPoint.z;
        msg.isHit = isHit;
        if (isHit)
        {
            //击中点表面的法线向量
            msg.hnx = hitNormal.x;
            msg.hny = hitNormal.y;
            msg.hnz = hitNormal.z;
        }
        NetManager.Send(msg);
    }

    //发送击中信息
    private void SyncHit(string weaponName, string id, string targetId)
    {
        MsgHit msg = new MsgHit();
        msg.weaponName = weaponName;
        msg.targetId = targetId;
        msg.id = id;
        NetManager.Send(msg);
    }

}


