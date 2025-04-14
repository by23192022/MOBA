//挂载在Player、Sync身上
//空闲、移动、攻击、受击、阵亡、复活

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AniController : MonoBehaviour
{
    private Animator animator;    
    private HealthController healthController; // 添加本地HealthController组件引用
    private BaseHuman baseHuman;


    private void OnEnable()
    {
        if (healthController == null || baseHuman == null) 
        {
            Debug.LogError("HealthController或BaseHuman 引用未初始化！");
            return;
        }

        // 合并所有事件订阅到一个OnEnable方法
        healthController.OnHit += PlayGetHitAnimation;
        healthController.OnDeath += PlayDeathAnimation;
        healthController.OnRecover += PlayOnRecoverAnimation;

        baseHuman.OnAttack01 += PlayAttack01Animation;
    }


    private void OnDisable()
    {
        // 合并所有事件取消订阅到一个OnDisable方法
        if (healthController != null && baseHuman != null)
        {
            healthController.OnHit -= PlayGetHitAnimation;
            healthController.OnDeath -= PlayDeathAnimation;
            healthController.OnRecover -= PlayOnRecoverAnimation;

            baseHuman.OnAttack01 -= PlayAttack01Animation;
        }
    }


//Awake → OnEnable → Start
//healthController初始化语句↓要先于对healthController的调用语句（OnEnable）
//不可以使用Start()
    void Awake()
    {
        animator = GetComponent<Animator>();        // 获取Animator组件
        healthController = GetComponent<HealthController>(); 

        baseHuman = GetComponent<BaseHuman>();
    }

    void Update()
    {
        PlayMoveAnimation();
    }

    void PlayMoveAnimation()
    {
        animator.SetFloat("Speed", baseHuman.currentSpeed);	// 控制移动动画
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
