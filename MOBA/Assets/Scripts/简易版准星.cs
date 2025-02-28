using UnityEngine;

public class SimpleCrosshair : MonoBehaviour 
{
    public Texture2D crosshairTexture; // 准星贴图
    public float crosshairSize = 20f;   // 准星大小

    private CameraFollow cameraController;
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFollow>();
    }

    void OnGUI()
    {
        if (!cameraController.isTP)
        {
            // 计算准星位置（屏幕中心）
            float x = (Screen.width - crosshairSize) * 0.5f;
            float y = (Screen.height - crosshairSize) * 0.5f;
            GUI.DrawTexture(new Rect(x, y, crosshairSize, crosshairSize), crosshairTexture);
        }
    }
}