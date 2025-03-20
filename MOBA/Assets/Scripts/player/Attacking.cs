using UnityEngine;
using System; 

public class Attacking : MonoBehaviour
{
    public int damageValue;     // 每次射击造成的伤害
    public float rangeValue;      // 射程
    public ParticleSystem muzzleFlash; // 枪口特效（可选）

    private Camera mainCamera;
    private CameraFollow cameraFollow;
    private ViewAndWeaponConfig config;

    public static event Action OnAttack01; // 声明静态事件（仅一个player）

    private Weapon currentWeapon;


    void Start()
    {
        mainCamera = Camera.main;
        cameraFollow = mainCamera.GetComponent<CameraFollow>();
        config = mainCamera.GetComponent<ViewAndWeaponConfig>();

        // 获取初始武器
        currentWeapon = config.CurrentWeapon; 
        // 订阅武器变更
        config.OnWeaponChanged += UpdateWeapon;
    }

    private void UpdateWeapon()
    {
        currentWeapon = config.CurrentWeapon;
        Debug.Log($"2)武器更新为：{currentWeapon.name}");

        damageValue = currentWeapon.damage;
        rangeValue = currentWeapon.range;
    }

    void OnDisable()
    {
        if (config != null)
        {
            config.OnWeaponChanged -= UpdateWeapon;
        }
    }


    void Update()
    {
        if (cameraFollow == null) return;

        // 第一人称视角：攻击-fire1鼠标左键、(移动-WASD、跳跃-jump空格)
        if(!cameraFollow.isTP && Input.GetButtonDown("Fire1"))
        {
                Debug.Log("攻击：FP鼠标左键按下");
                Attack();

        }
        // 第三人称视角：攻击-空格、(移动-鼠标左键、不可跳跃)
        else if(cameraFollow.isTP && Input.GetButtonDown("Jump"))
        {
                Debug.Log("攻击：TP空格按下");
                Attack();
        }
    }


    //void ChooseAttackMethodByWeapon()
    void Attack()
    {

        if (currentWeapon == null) return;
        switch (currentWeapon.category)
        {
            case WeaponCategory.Melee:
                Wield();
                OnAttack01?.Invoke();
                break;
                
            case WeaponCategory.Ranged:
                Shoot();
                break;
        }
    }


    //处理刀剑挥砍逻辑
    void Wield()
    {
        Debug.Log("挥刀一次");
    }

    //处理枪支射击逻辑
    void Shoot()
    {
        // 播放枪口特效
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // 从屏幕中心发射射线
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 检测射线碰撞
        if (Physics.Raycast(ray, out hit, rangeValue))
        {
            // 获取敌人的Health组件
            if (hit.collider.CompareTag("Enemy") && 
                hit.collider.TryGetComponent<HealthController>(out var health))
            {
                health.GetHit(damageValue);
                Debug.Log($"击中敌人：{hit.collider.gameObject.name}", hit.collider.gameObject);
            }

            // 可选：在击中点生成特效
            // Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

}

