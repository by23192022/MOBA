using UnityEngine;

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


    void LateUpdate()
    {
        if (target == null) return;

        // 视角切换按键被按下
        if (Input.GetKeyDown(viewSwitchKey))
        {
            isTP = !isTP;
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
}