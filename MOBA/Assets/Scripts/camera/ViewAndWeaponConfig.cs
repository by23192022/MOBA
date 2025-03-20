using UnityEngine;
using System.Collections;
using System;

public class ViewAndWeaponConfig : MonoBehaviour
{
    [SerializeField] private WeaponDB weaponDB; // 拖入武器数据库
    public Weapon CurrentWeapon;
    public event Action OnWeaponChanged;        // 声明静态事件（仅一个player）

    [Header("角色模型")]
    public GameObject TPModel;                      // 第三人称全身模型
    [Header("第一人称武器模型")]
    //public GameObject FPModel;                    // 第一人称枪械模型
    [SerializeField] private GameObject _fpModel;   // 私有字段
    public GameObject FPModel                       // 公共属性
    {
        get => _fpModel;
        set
        {
            //防御式编程
            if (value != null && !value.CompareTag("FPModel"))
            {
                Debug.LogError("只能设置FPModel类型的模型！");
                return;
            }
            if (_fpModel == value) return;          // 避免重复更新
            //Debug.Log($"武器从 {_fpModel?.name} 切换为 {value?.name}");

            _fpModel = value;           // 赋值操作
            UpdateWeapon();             // 触发更新逻辑
        }
    }
    /* 属性封装
    当通过代码为FPModel赋值时：
    cameraFollow.FPModel = xxx 
    --> 触发属性setter 
    --> 更新_fpModel 
    --> 执行UpdateWeaponName()
    */


    private CameraFollow cameraFollow;
    private const string shieldParentPath = "root/pelvis/Shield";
    private const string weaponParentPath = "root/pelvis/Weapon";


    /* 使用协程版 Start:
       （1）操作场景中动态生成的对象（如：查找场景中带有 "Player" 标签的对象）
       （2）加载资源
       （3）等待异步
     */
    IEnumerator Start()
    //void Start()
    {
        // 延迟一帧确保所有实例化完成
        yield return null;
        cameraFollow = GetComponent<CameraFollow>();

        // 初始化值target、TPModel、FPModel
        setPerfabs();

        // 设置第一人称视角
        setFPView();

        // 设置第三人称视角，持枪
        setTPView();

        // 初始化TP\FPModel可见状态
        UpdateModelVisibility(cameraFollow.isTP);
    }

    // 更新当前武器
    private void UpdateWeapon()
    {
        CurrentWeapon = weaponDB.GetWeapon(_fpModel ? _fpModel.name : "Default");
        Debug.Log($"1)武器已更新: {CurrentWeapon.name}");

        OnWeaponChanged?.Invoke();
    }

    // 兼容：在Inspector面板值变更时触发（编辑器模式）
    private void OnValidate()
    {
        if (!Application.isPlaying)
            UpdateWeapon();
    }


    void setPerfabs()
    {
        TPModel = GameObject.FindWithTag("Player");
        FPModel = GameObject.FindWithTag("FPModel");    // 属性赋值触发执行set{}

        if (TPModel != null)
        {
            cameraFollow.target = TPModel.transform;
        }

        if (TPModel == null || FPModel == null)
            Debug.LogError("请确保场景中存在'Player'和'FPModel'标签的对象");
    }

    void setFPView()
    {
        //if (CurrentWeapon.name == "AAssaultRifle_01")
        if (CurrentWeapon.category == WeaponCategory.Ranged)
        {
            // 把FPModel设置为当前相机（脚本挂载的物体）的子物体
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
    }


    void setTPView()
    {
        if (TPModel == null) return;

        // 配置模型基础参数（坐标、旋转、缩放）
        TPModel.transform.position = new Vector3(4, 0, -7);
        TPModel.transform.rotation = Quaternion.Euler(0, -90, 0);
        TPModel.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        // 清理盾牌和旧武器
        CleanChildObject(shieldParentPath);
        CleanChildObject(weaponParentPath);

        // 加载并装备当前武器
        EquipCurrentWeapon();
    }
 
    private void CleanChildObject(string ParentPath)
    {
        Transform Parent = TPModel.transform.Find(ParentPath);

        if (Parent != null)
        {
            // 删除所有子物体
            foreach (Transform child in Parent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void EquipCurrentWeapon()
    {
        if (CurrentWeapon == null || string.IsNullOrEmpty(CurrentWeapon.name))
        {
            Debug.LogWarning("当前没有有效武器配置");
            return;
        }

        // 根据武器名称加载预制体
        GameObject weaponPrefab = Resources.Load<GameObject>($"Perfabs/FPModel/{CurrentWeapon.name}");
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