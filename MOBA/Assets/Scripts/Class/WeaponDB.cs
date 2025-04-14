using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Weapons/New Database")]
////数组：支持Inspector的可视化编辑////字典：高效查询，O(1) 时间复杂度
public class WeaponDB : ScriptableObject
{
    // 在 Inspector 中可编辑的数组
    [SerializeField] private Weapon[] _weapons;		//数组

    // 运行时使用的字典存储武器数据（名称作为键）
    private Dictionary<string, Weapon> weapons;

    ////Unity会在 首次加载/编辑器中对资源进行修改后 自动调用此方法：
    private void OnEnable()
    {
        TypeConvert();
    }

    private void TypeConvert()
    {
        weapons?.Clear();
        if (_weapons == null) return;

        weapons = new Dictionary<string, Weapon>();
        foreach (var weapon in _weapons)
        {
            if (weapon != null && !weapons.ContainsKey(weapon.name))
            {
                weapons.Add(weapon.name, weapon);
            }
        }
    }

    // 根据名称获取武器类型
    public Weapon GetWeapon(string weaponName)
    {
        if (weapons.TryGetValue(weaponName, out Weapon weapon))
        {
            return weapon;
        }
        return null; // 未找到返回null
    }
}
