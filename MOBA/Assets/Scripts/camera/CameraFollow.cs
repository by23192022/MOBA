using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;            // 目标对象
    public float smoothSpeed = 5f;
    public Vector3 tpOffset = new Vector3(0f, 8f, -10f);    // 相机位置偏移
    public Vector3 fpOffset = new Vector3(0f, 1.6f, 0f);
    public KeyCode viewSwitchKey = KeyCode.V;   // 视角切换快捷键
    public bool isTP = true;            // 当前是否为第三人称视角
    public float sensitivity = 100f;    // 鼠标灵敏度
    private float xRotation = 0f;       // 垂直旋转角度

    private ViewAndWeaponConfig config;

    void Start()
    {
        config = GetComponent<ViewAndWeaponConfig>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleViewSwitch();             // 视角切换按键被按下，取反一次

        //第一人称视角需要锁定鼠标到屏幕中心
        Cursor.lockState = isTP ? CursorLockMode.None : CursorLockMode.Locked;

        UpdateCameraPosition();         // 视角切换
    }

    private void HandleViewSwitch()
    {
        if (Input.GetKeyDown(viewSwitchKey))
        {
            isTP = !isTP;
            config?.UpdateModelVisibility(isTP);
        }
    }

    private void UpdateCameraPosition()
    {
        if (isTP)
        {
            TPCamera();
        }
        else
        {
            FPCamera();
        }
    }

    private void TPCamera()
    {
        // 计算目标位置并平滑移动
        Vector3 targetPosition = target.position + tpOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        // 保持相机朝向角色
        transform.LookAt(target); 
    }

    private void FPCamera()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        
        // 垂直视角控制（上下看）
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // player水平旋转控制（左右转）
        target.Rotate(Vector3.up * mouseX);

        // 同步player位置到摄像机
        transform.position = target.position + fpOffset;
        // 同步player的水平旋转到摄像机
        transform.rotation = Quaternion.Euler(
            xRotation,
            target.eulerAngles.y,       // 使用Player的Y轴旋转
            0
        );
    }

}