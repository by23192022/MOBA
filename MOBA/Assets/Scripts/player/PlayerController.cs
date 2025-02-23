using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;     // 恒定移动速度
    public float rotationSpeed = 10f; // 转向速度
    private bool isMoving = false;

    private CameraFollow cameraController;
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFollow>();

    }

    void Update()
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

        Vector3 moveDir = transform.forward * vertical + transform.right * horizontal;
        moveDir *= moveSpeed * Time.deltaTime;
        transform.Translate(moveDir, Space.World);
    }

}
