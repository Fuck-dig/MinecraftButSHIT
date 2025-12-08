using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0, 24)]
    public float timeOfDay = 12f;
    public float dayDurationMinutes = 10f; // Real-time minutes for full day
    
    [Header("Sun/Moon")]
    public Light directionalLight;
    public Gradient lightColor;
    public AnimationCurve lightIntensity;
    
    private float timeScale;
    
    void Start()
    {
        // Calculate how fast time should move
        timeScale = 24f / (dayDurationMinutes * 60f);
        
        // Setup default gradients if not set
        if (lightColor.colorKeys.Length == 0)
        {
            SetupDefaultGradients();
        }
    }
    
    void Update()
    {
        // Update time
        timeOfDay += Time.deltaTime * timeScale;
        if (timeOfDay >= 24f)
            timeOfDay = 0f;
        
        UpdateLighting();
    }
    
    void UpdateLighting()
    {
        // Rotate the light (sun/moon)
        float rotation = (timeOfDay / 24f) * 360f - 90f;
        directionalLight.transform.rotation = Quaternion.Euler(rotation, 170f, 0f);
        
        // Update light color and intensity based on time
        float normalizedTime = timeOfDay / 24f;
        directionalLight.color = lightColor.Evaluate(normalizedTime);
        directionalLight.intensity = lightIntensity.Evaluate(normalizedTime);
        
        // Update ambient lighting
        RenderSettings.ambientLight = lightColor.Evaluate(normalizedTime) * 0.5f;
    }
    
    void SetupDefaultGradients()
    {
        // Default light color gradient
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        colorKeys[0] = new GradientColorKey(new Color(0.2f, 0.3f, 0.5f), 0f);    // Night
        colorKeys[1] = new GradientColorKey(new Color(1f, 0.6f, 0.3f), 0.25f);   // Sunrise
        colorKeys[2] = new GradientColorKey(new Color(1f, 1f, 1f), 0.5f);        // Day
        colorKeys[3] = new GradientColorKey(new Color(1f, 0.4f, 0.2f), 0.75f);   // Sunset
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);
        
        lightColor.SetKeys(colorKeys, alphaKeys);
        
        // Default intensity curve
        lightIntensity = AnimationCurve.EaseInOut(0f, 0.2f, 1f, 0.2f);
        lightIntensity.AddKey(0.5f, 1.5f); // Bright at noon
    }
    
    // Helper method to set time from inspector or code
    public void SetTime(float hour)
    {
        timeOfDay = Mathf.Clamp(hour, 0f, 24f);
        UpdateLighting();
    }
}