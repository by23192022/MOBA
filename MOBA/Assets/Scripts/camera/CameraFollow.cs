using UnityEngine;
using System.Collections;
using System; // 添加System命名空间以使用Action

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 拖拽角色对象到此
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 8f, -10f); // 相机位置偏移
    public KeyCode viewSwitchKey = KeyCode.V; // 视角切换快捷键
    public bool isTP = true; // 当前是否为第三人称视角

    public Vector3 offset2 = new Vector3(0f, 1.6f, 0f); // 相机位置偏移

    public float sensitivity = 100f;  // 鼠标灵敏度
    private float xRotation = 0f;      // 垂直旋转角度

    [SerializeField] private WeaponDB weaponDB; // 拖入武器数据库
    public Weapon CurrentWeapon;
    public event Action OnWeaponChanged; // 声明静态事件（仅一个player）

    [Header("角色模型")]
    public GameObject TPModel;  	// 第三人称全身模型
    [Header("第一人称武器")]
    //public GameObject FPModel;   	// 第一人称枪械模型
    [SerializeField] private GameObject _fpModel; // 私有字段
    public GameObject FPModel 	//  属性封装
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
            if (_fpModel == value) return; // 避免重复更新
            //Debug.Log($"武器从 {_fpModel?.name} 切换为 {value?.name}");

            _fpModel = value;  // 赋值操作
            UpdateWeapon(); // 触发更新逻辑
        }
    }

    // 更新当前武器
    private void UpdateWeapon()
    {
        string weaponName = _fpModel ? _fpModel.name : "Default";
        CurrentWeapon = weaponDB.GetWeapon(weaponName);
        Debug.Log($"1)武器已更新: {CurrentWeapon.name}"); 

        OnWeaponChanged?.Invoke();
    }

    // 兼容：在Inspector面板值变更时触发（编辑器模式）
    private void OnValidate()
    {
        if (!Application.isPlaying) 
            UpdateWeapon();
    }

    IEnumerator  Start()
    {
        // 延迟一帧确保所有实例化完成
        yield return null; 
        // 初始化值target、TPModel、FPModel
        setPerfabs();

        // 设置第一人称视角
        setFPView();

        // 设置第三人称视角，持枪
        setTPView();
   
        // 初始化TP\FPModel可见状态
        ModelView();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 视角切换按键被按下
        if (Input.GetKeyDown(viewSwitchKey))
        {
            isTP = !isTP;
            ModelView();        //TP、FP模型可见性
        }

        //第一人称视角需要锁定鼠标到屏幕中心
        Cursor.lockState = isTP ? CursorLockMode.None : CursorLockMode.Locked;

        if (isTP)
        {
            // 第三人称视角
            // 计算目标位置并平滑移动
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // 保持相机朝向角色
            transform.LookAt(target);
        }
        else
        {

            // 第一人称视角
            //transform.position = target.position + offset2;
            //transform.rotation = target.rotation;

 
            // 获取鼠标输入
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            // 垂直视角控制（上下看）
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            // 水平旋转控制（左右转）
            target.Rotate(Vector3.up * mouseX);

            // 同步摄像机位置到Player
            transform.position = target.position + offset2;
            // 同步水平旋转到摄像机
            transform.rotation = Quaternion.Euler(
                xRotation,
                target.eulerAngles.y,  // 使用Player的Y轴旋转
                0
            );
        }
    }

    void setPerfabs()
    {
        // player标签：Target和TPModel；FPModel标签：FPModel

        // 查找玩家模型
        GameObject playerObj = GameObject.FindWithTag("Player");
        if(playerObj != null)
        {
            TPModel = playerObj;
            target = playerObj.transform;
        }
        else
        {
            Debug.LogError("未找到玩家对象，请确保场景中存在'Player'标签的对象");
        }

        // 查找第一人称武器
        FPModel = GameObject.FindWithTag("FPModel"); // 属性赋值触发执行set{}
        if(FPModel == null)
        {
            Debug.LogError("未找到武器对象，请确保场景中存在'FPModel'标签的对象");
        }
    }

    void setFPView()
    {
        if(CurrentWeapon.name == "AAssaultRifle_01")
        {
            // 把FPModel设置为当前相机（脚本挂载的物体）的子物体
            FPModel.transform.SetParent(transform, false);
            // 设置FPModel相对于相机的坐标、旋转、缩放
            FPModel.transform.localPosition = new Vector3(0.054f, -0.08f, -0.034f);
            FPModel.transform.localEulerAngles = new Vector3(0f, 83f, 0f);
            FPModel.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        else if(CurrentWeapon.name  == "SSword")
        {
                Debug.Log("未设置Sword的第一人称视角");
        }

        Camera cam = GetComponent<Camera>();  // 直接获取当前物体上的Camera组件
        cam.nearClipPlane = 0.05f; 	// 设置近裁剪平面

        // 设置准星

    }

    void setTPView()
    {
        // 设置TPModel的坐标、旋转、缩放
        TPModel.transform.position = new Vector3(4, 0, -7);
        TPModel.transform.rotation = Quaternion.Euler(0, -90, 0);
        TPModel.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        // 删掉盾牌
        Transform shield = TPModel.transform.Find("root/pelvis/Shield/Shield");
        if(shield) Destroy(shield.gameObject);

        // 替换武器
        Transform weaponParent = TPModel.transform.Find("root/pelvis/Weapon");
        // 删除原武器        
        Transform sword = weaponParent.Find("Sword");
        if(sword) Destroy(sword.gameObject);
        // 加载并实例化新武器，并设置坐标、旋转、缩放
        GameObject weaponPrefab = Resources.Load<GameObject>("Perfabs/FPModel/AAssaultRifle_01");
        if(weaponPrefab)
        {
            GameObject newWeapon = Instantiate(weaponPrefab, weaponParent);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.Euler(-45, 120, 0);
            newWeapon.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("未能加载预制体，请检查路径");
        }

    }

    void ModelView()
    {
        // 第一人称
        SetObjectVisibility(FPModel, !isTP);
        // 第三人称
        SetObjectVisibility(TPModel, isTP);
    }

    // 通用方法：设置对象及其子物体的所有渲染器状态
    void SetObjectVisibility(GameObject target, bool isVisible)
    {
        if (target == null) return;

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isVisible;
        }
    }

}