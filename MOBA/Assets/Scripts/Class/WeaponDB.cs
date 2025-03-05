using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Weapons/New Database")]
public class WeaponDB : ScriptableObject
{
    public Weapon[] weapons;

    // 根据名称获取武器类型
    public Weapon GetWeapon(string weaponName)
    {
        foreach (var weapon in weapons)
        {
            // 检查当前武器的名称是否与查询名称匹配
            if (weapon.name == weaponName)
                return weapon; // 找到匹配项，返回该武器配置
        }
        return null; // 遍历完成未找到匹配项，返回空
    }
}