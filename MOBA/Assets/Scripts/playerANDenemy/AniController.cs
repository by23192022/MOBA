//挂载在Player、Enemy身上
//空闲、移动、攻击、受击、阵亡、复活

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AniController : MonoBehaviour
{
    private Animator animator;    
    private NavMeshAgent navAgent; // 导航代理引用
    private HealthController healthController; // 添加本地HealthController组件引用
    private Camera mainCamera;
    private CameraFollow cameraFollow;
    private PlayerController playerController; 

    private void OnEnable()
    {
        // 添加空检查
        if (healthController == null) {
            Debug.LogError("HealthController 引用未初始化！");
            return;
        }

        // 合并所有事件订阅到一个OnEnable方法
        healthController.OnHit += PlayGetHitAnimation;
        healthController.OnDeath += PlayDeathAnimation;
        healthController.OnRecover += PlayOnRecoverAnimation;
     
        // 如果是玩家才订阅攻击事件（根据需求调整）
        if (gameObject.CompareTag("Player") && cameraFollow!= null)
        {
                cameraFollow.OnWeaponChanged += HandleWeaponChange;
        }

    }


    private void HandleWeaponChange()
    {
        // 每次武器变更时强制清理旧订阅（避免事件内存泄漏问题）
        Attacking.OnAttack01 -= PlayAttack01Animation;

        // 判断武器名称
        if (cameraFollow.CurrentWeapon.name == "SSword")
        {
                Attacking.OnAttack01 += PlayAttack01Animation;
        }
        else if (cameraFollow.CurrentWeapon.name == "AAssaultRifle_01")
        {
                Debug.Log("暂未配置射击动画");
        }
    }



    private void OnDisable()
    {
        // 合并所有事件取消订阅到一个OnDisable方法
        if (healthController != null)
        {
            healthController.OnHit -= PlayGetHitAnimation;
            healthController.OnDeath -= PlayDeathAnimation;
            healthController.OnRecover -= PlayOnRecoverAnimation;
        }

        if (cameraFollow!= null)
        {
            cameraFollow.OnWeaponChanged -= HandleWeaponChange;
        }

        // 对象禁用时双重保险取消订阅
        Attacking.OnAttack01 -= PlayAttack01Animation;

    }

//Awake → OnEnable → Start
//healthController初始化语句↓要先于对healthController的调用语句（OnEnable）
//不可以使用Start()
    void Awake()
    {
        animator = GetComponent<Animator>();        // 获取Animator组件

        //healthController初始化
        healthController = GetComponent<HealthController>(); 

        // 获取CameraFollow组件
        mainCamera = Camera.main;
        cameraFollow = mainCamera.GetComponent<CameraFollow>();

        playerController = GetComponent<PlayerController>();
        navAgent = GetComponent<NavMeshAgent>(); // 获取导航组件
    }

    void Update()
    {
        // 控制移动动画
        float speed = 0;
        if(gameObject.CompareTag("Player"))
        {
        	speed = playerController.currentSpeed; //手动计算速度
        	//Debug.Log($"Player当前速度={speed}");
        }
        else if(gameObject.CompareTag("Enemy"))
        {
        	speed = navAgent.velocity.magnitude; //使用NavAgent组件
        	//Debug.Log($"Enemy当前速度={speed}");
        }
        animator.SetFloat("Speed", speed);

    }

    void PlayAttack01Animation()
    {
        animator.SetTrigger("Attack01");
    }

    void PlayGetHitAnimation()
    {
        animator.SetTrigger("GetHit");
    }

    void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }

    void PlayOnRecoverAnimation()
    {
        animator.SetTrigger("Recover");
    }

}
