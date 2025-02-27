using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;     // 恒定移动速度
    public float rotationSpeed = 10f; // 转向速度
    private bool isMoving = false;

    private CameraFollow cameraController;

    private Animator animator;
    private float lastH;        //用于优化
    private float lastV;


    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFollow>();
        animator = GetComponent<Animator>();        // 获取Animator组件

    }

    void Update()
    {
        MoveControl();
        AniControl();

    }

    void AniControl()
    {
        float currentSpeed = GetComponent<PlayerController>().moveSpeed; // 获取实际速度

        //Debug.Log($"当前Speed参数值: {currentSpeed}");     // 调试输出确认数值

        if (isMoving && currentSpeed > 0)
        {
            //同步参数到动画系统
            animator.SetBool("isIdle", false);      //需要
            animator.SetBool("isMove", true);       //Move
        }
        else
        {
            animator.SetBool("isMove", false);      //需要
            animator.SetBool("isIdle", true);       //Idle
        }
        

    }


    void MoveControl()
    {
        if (cameraController.isTP)
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

            Vector3 moveDir = Camera.main.transform.right * horizontal
                            + Camera.main.transform.forward * vertical;
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
                );
            }
        }
    }

    void FP()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //记录isMoving的值，用于AniControl()
        float currentH = Input.GetAxisRaw("Horizontal");
        float currentV = Input.GetAxisRaw("Vertical");
        // 检测任意方向键按下
        if ((currentH != 0 || currentV != 0) && (lastH == 0 && lastV == 0))
        {
            isMoving = true;
            Debug.Log("任意方向键按下");
        }
        // 检测所有方向键松开
        if (currentH == 0 && currentV == 0 && (lastH != 0 || lastV != 0))
        {
            isMoving = false;
            Debug.Log("所有方向键松开");
        }
        // 记录当前帧值
        lastH = currentH;
        lastV = currentV;

        Vector3 moveDir = transform.forward * vertical + transform.right * horizontal;
        moveDir *= moveSpeed * Time.deltaTime;
        transform.Translate(moveDir, Space.World);
    }

}
