using UnityEngine;
using System.Collections;
using System;

public class ViewConfig : MonoBehaviour
{
    [Header("角色模型")]
    public GameObject TPModel;                    // 第三人称全身模型
    [Header("第一人称武器模型")]
    public GameObject FPModel;                    // 第一人称枪械模型

    private CameraFollow cameraFollow;
    private const string shieldParentPath = "root/pelvis/Shield";
    private const string weaponParentPath = "root/pelvis/Weapon";

    private Camera mainCamera;
    private SimpleCrosshair s;

    //IEnumerator Start()
    void Start()
    {
        // 延迟一帧确保所有实例化完成
        //yield return null;
        cameraFollow = GetComponent<CameraFollow>();
        mainCamera = Camera.main;
        s = mainCamera.GetComponent<SimpleCrosshair>();
    }

    ////在角色模型和ctrl武器模型生成后调用
    //ctrl根据武器生成视角
    public void ctrlRun(Weapon weapon)
    {
        TPModel = GameObject.FindWithTag("Player");	//唯一
        FPModel = GameObject.FindWithTag("FPModel");	//唯一
        if (TPModel == null || FPModel == null)
            Debug.LogError("请确保场景中存在'Player'和'FPModel'标签的对象");

        // 设置第一人称视角
        setFPView(FPModel, weapon);
        // 设置第三人称视角，持枪
        setTPView(TPModel, weapon);

        // 初始化TP\FPModel可见状态
        UpdateModelVisibility(cameraFollow.isTP);		//初始化调用一次，每次变化时再调用
    }


    void setFPView(GameObject FPModel, Weapon CurrentWeapon)
    {
        if (FPModel == null || CurrentWeapon == null) return;

        //if (CurrentWeapon.name == "AAssaultRifle_01")
        if (CurrentWeapon.category == WeaponCategory.Ranged)
        {
            // 把FPModel设置为当前相机（本脚本挂载的物体）的子物体
            FPModel.transform.SetParent(cameraFollow.transform, false);
            // 设置FPModel相对于相机的坐标、旋转、缩放
            FPModel.transform.localPosition = new Vector3(0.33f,-0.32f,0.52f);
            FPModel.transform.localEulerAngles = new Vector3(-0.07f, 80.5f, -6.32f);
            FPModel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
        //else if (CurrentWeapon.name == "SSword")
        else if (CurrentWeapon.category == WeaponCategory.Melee)
        {
            //近战武器SSword的第一人称视角
            FPModel.transform.SetParent(cameraFollow.transform, false);
            FPModel.transform.localPosition = new Vector3(0.45f, -0.34f, 0.54f);
            FPModel.transform.localRotation = Quaternion.Euler(225, -50, 5);
            FPModel.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }

        Camera cam = GetComponent<Camera>();    // 直接获取当前物体上的Camera组件
        cam.nearClipPlane = 0.03f;          	// 设置近裁剪平面

        // 设置准星
        s.Init(CurrentWeapon);
    }


    public void setTPView(GameObject TPModel, Weapon CurrentWeapon)
    {
        if (TPModel == null || CurrentWeapon == null) return;

        // 清理盾牌和旧武器
        CleanChildObject(TPModel, shieldParentPath);
        CleanChildObject(TPModel, weaponParentPath);

        // 加载并装备当前武器
        EquipCurrentWeapon(TPModel, CurrentWeapon);
    }
 
    private void CleanChildObject(GameObject parentObj, string path)
    {
        Transform Parent = parentObj.transform.Find(path);

        if (Parent != null)
        {
            // 删除所有子物体
            foreach (Transform child in Parent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void EquipCurrentWeapon(GameObject TPModel, Weapon CurrentWeapon)
    {
        if (CurrentWeapon == null || string.IsNullOrEmpty(CurrentWeapon.name))
        {
            Debug.LogWarning("当前没有有效武器配置");
            return;
        }

        // 根据武器名称加载预制体
        GameObject weaponPrefab = Resources.Load<GameObject>($"Perfabs/Weapon/{CurrentWeapon.name}");
        if (weaponPrefab == null)
        {
            Debug.LogError($"武器预制体加载失败：{CurrentWeapon.name}");
            return;
        }

        // 查找武器挂载点
        Transform weaponParent = TPModel.transform.Find(weaponParentPath);
        if (weaponParent == null)
        {
            Debug.LogError("找不到武器挂载点");
            return;
        }

        // 实例化武器
        GameObject newWeapon = Instantiate(weaponPrefab, weaponParent);

        // 配置武器模型基础参数（坐标、旋转、缩放）
        ConfigureWeaponTransform(CurrentWeapon, newWeapon.transform);

    }

    private void ConfigureWeaponTransform(Weapon weapon, Transform weaponTransform)
    {
        //switch (weapon.name)
        switch (weapon.category)
        {
            //case "AAssaultRifle_01":
            case WeaponCategory.Ranged:
                weaponTransform.localPosition = Vector3.zero;
                weaponTransform.localRotation = Quaternion.Euler(-45, 120, 0);
                weaponTransform.localScale = Vector3.one;
                break;
            //case "SSword":
            case WeaponCategory.Melee:
                weaponTransform.localPosition = Vector3.zero;
                weaponTransform.localRotation = Quaternion.Euler(-180, 0, 0);
                weaponTransform.localScale = Vector3.one;
                break;
            default:
                // 默认配置
                weaponTransform.localPosition = Vector3.zero;
                weaponTransform.localRotation = Quaternion.identity;
                weaponTransform.localScale = Vector3.one;
                break;
        }
    }



    public void UpdateModelVisibility(bool isTP)
    { 
        SetObjectVisibility(FPModel, !isTP);    // 第一人称
        SetObjectVisibility(TPModel, isTP);     // 第三人称
    }

    // 通用方法：设置对象及其子物体的所有渲染器状态
    private void SetObjectVisibility(GameObject target, bool isVisible)
    {
        if (target == null) return;
        foreach (var renderer in target.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = isVisible;
        }
    }

}