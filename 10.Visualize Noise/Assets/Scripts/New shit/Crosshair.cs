using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float crosshairSize = 10f;
    [SerializeField] private float crosshairThickness = 2f;
    [SerializeField] private float crosshairGap = 5f;

    private Texture2D crosshairTexture;

    void Start()
    {
        CreateCrosshairTexture();
        Cursor.visible = false; // Hide the default cursor
    }

    void OnGUI()
    {
        if (crosshairTexture != null)
        {
            float screenCenterX = Screen.width / 2f;
            float screenCenterY = Screen.height / 2f;

            // Draw top line
            GUI.DrawTexture(new Rect(
                screenCenterX - crosshairThickness / 2f,
                screenCenterY - crosshairGap - crosshairSize,
                crosshairThickness,
                crosshairSize
            ), crosshairTexture);

            // Draw bottom line
            GUI.DrawTexture(new Rect(
                screenCenterX - crosshairThickness / 2f,
                screenCenterY + crosshairGap,
                crosshairThickness,
                crosshairSize
            ), crosshairTexture);

            // Draw left line
            GUI.DrawTexture(new Rect(
                screenCenterX - crosshairGap - crosshairSize,
                screenCenterY - crosshairThickness / 2f,
                crosshairSize,
                crosshairThickness
            ), crosshairTexture);

            // Draw right line
            GUI.DrawTexture(new Rect(
                screenCenterX + crosshairGap,
                screenCenterY - crosshairThickness / 2f,
                crosshairSize,
                crosshairThickness
            ), crosshairTexture);
        }
    }

    void CreateCrosshairTexture()
    {
        crosshairTexture = new Texture2D(1, 1);
        crosshairTexture.SetPixel(0, 0, crosshairColor);
        crosshairTexture.Apply();
    }

    void OnDestroy()
    {
        Cursor.visible = true; // Show cursor when script is destroyed
    }
}