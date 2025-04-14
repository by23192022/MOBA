using System;
using System.Collections.Generic;

public sealed class WeaponDB
{

    // 单例实例（使用Lazy实现延迟加载和线程安全）
    private static readonly Lazy<WeaponDB> _instance = new Lazy<WeaponDB>(() => new WeaponDB());
    // 全局访问点
    public static WeaponDB Instance => _instance.Value;


    // 使用字典存储武器数据（名称作为键）
    private Dictionary<string, Weapon> weapons = new Dictionary<string, Weapon>();


    // 私有构造函数（防止外部实例化）
    private WeaponDB()
    {
        LoadDefaultWeapons(); // 自动加载默认武器
    }

    // 初始化默认武器数据
    private void LoadDefaultWeapons()
    {
        // 添加突击步枪
        AddWeapon(new Weapon
        {
            name = "AAssaultRifle_01",
            category = WeaponCategory.Ranged,
            damage = 10,
            range = 25,

        });

        // 添加剑
        AddWeapon(new Weapon
        {
            name = "SSword",
            category = WeaponCategory.Melee,
            damage = 20,
            range = 1,

        });
    }


    // 添加武器到数据库
    public void AddWeapon(Weapon weapon)
    {
        if (weapon == null) return;

        // 避免重复添加同名武器
        if (!weapons.ContainsKey(weapon.name))
        {
            weapons.Add(weapon.name, weapon);
        }
        else
        {
            // 可记录警告日志
            Console.WriteLine($"武器 {weapon.name} 已存在，添加失败");
        }
    }

    // 通过名称获取武器
    public Weapon GetWeapon(string weaponName)
    {
        if (weapons.TryGetValue(weaponName, out Weapon weapon))
        {
            return weapon;
        }
        return null; // 未找到返回null
    }


}
