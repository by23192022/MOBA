using System;

// 武器类型枚举
public enum WeaponCategory
{
    Melee,    // 近战武器
    Ranged    // 远程武器
}

public class Weapon
{
    public string name;		            // 武器名称（如"Sword"）
    public WeaponCategory category; 	// 武器类别
    //public GameObject modelPrefab; 	// 关联的模型预制体
    public int damage; 		            //一次攻击的伤害值
    public int range; 			        //（远程武器）射程

    public float attackSpeed;

    //public ParticleSystem muzzleFlash; 	//（远程武器）枪口特效预制体
    //public LineRenderer bulletTrail;	    //（远程武器）射线特效预制体
    //public ParticleSystem hitEffect;	    //击中特效
}






