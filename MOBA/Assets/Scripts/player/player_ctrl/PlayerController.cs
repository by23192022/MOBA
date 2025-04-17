//仅挂载在Player身上，用于Player的移动控制

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : BaseHuman
{
    private bool isMoving = false;

    private Camera mainCamera;
    private CameraFollow cameraFollow;

    //上⼀次发送同步信息的时间
    private float lastSendSyncTime = 0;
    //同步帧率
    public static float syncInterval = 0.1f; 
    //public static float syncInterval = 0.3f; 


    void Start()
    {
        mainCamera = Camera.main;
        cameraFollow = mainCamera.GetComponent<CameraFollow>();
    }

    protected override void Update()
    {
        base.Update();		// 调用基类逻辑
        MoveControl();

        SyncUpdate();        	//同步位置
    }


    //发送同步信息
    public void SyncUpdate(){
        //时间间隔判断
        if(Time.time - lastSendSyncTime < syncInterval){
            return;
        }

        // 计算当前位置和旋转
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        // 计算位置和旋转变化量
        bool posChanged = Vector3.Distance(pos, lastPos) > posThreshold;
        bool rotChanged = Quaternion.Angle(Quaternion.Euler(rot), Quaternion.Euler(lastRot)) > rotThreshold;
        // 无变化则不用发送同步协议
        if (!posChanged && !rotChanged)
        {
            return;
        }
        
        //更新变量
        lastPos = pos; 
        lastRot = rot;
        lastSendSyncTime = Time.time;

        //发送同步协议
        MsgSyncHuman msg = new MsgSyncHuman();
        msg.x = pos.x;
        msg.y = pos.y;
        msg.z = pos.z;
        msg.ex = rot.x;
        msg.ey = rot.y;
        msg.ez = rot.z;
        NetManager.Send(msg);
    }


    void MoveControl()
    {
        if (cameraFollow.isTP)
        {
            TP();
        }
        else
        {
            FP();
        }
    }

    void TP()
    {
        if (Input.GetMouseButtonDown(0)) isMoving = true;
        if (Input.GetMouseButtonUp(0)) isMoving = false;

        if (isMoving)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 mousePos = Input.mousePosition;
            Vector3 delta = mousePos - screenCenter;

            // 转换为标准化方向（保留此步骤）
            float horizontal = delta.x / (Screen.width / 2f);
            float vertical = delta.y / (Screen.height / 2f);

            Vector3 moveDir = mainCamera.transform.right * horizontal
                            + mainCamera.transform.forward * vertical;
            moveDir.y = 0;

            if (moveDir.magnitude > 0.1f)
            {

                // 角色转向
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                // 匀速移动（不再使用 speedFactor）
                transform.Translate(
                    Vector3.forward * moveSpeed * Time.deltaTime,
                    Space.Self // 使用自身坐标系
                    //Space.World // 世界坐标系
                );
            }
        }
    }

    void FP()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = transform.forward * vertical + transform.right * horizontal;
        moveDir *= moveSpeed * Time.deltaTime;
        transform.Translate(moveDir, Space.World);
    }

}
