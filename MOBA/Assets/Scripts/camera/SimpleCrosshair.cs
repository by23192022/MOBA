using UnityEngine;

public class SimpleCrosshair : MonoBehaviour 
{
    public Texture2D crosshairTexture; // 准星贴图
    public float crosshairSize = 20f;   // 准星大小

    private CameraFollow cameraController;
    private Weapon CurrentWeapon;

    ////在ViewConfig.cs的setFPView里调用
    public void Init(Weapon currentWeapon)
    {
        cameraController = Camera.main.GetComponent<CameraFollow>();
        CurrentWeapon = currentWeapon;
    }

    void OnGUI()
    {
        if (CurrentWeapon ==null) return;

        if (!cameraController.isTP && 
            CurrentWeapon.category == WeaponCategory.Ranged )
        {
            // 计算准星位置（屏幕中心）
            float x = (Screen.width - crosshairSize) * 0.5f;
            float y = (Screen.height - crosshairSize) * 0.5f;
            GUI.DrawTexture(new Rect(x, y, crosshairSize, crosshairSize), crosshairTexture);
        }
    }
}