using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WaterAnimator : MonoBehaviour
{
    public int frameCount = 32; // number of frames in your water_still.png
    public float frameDuration = 0.05f; // time per frame (20 fps)
    private Renderer rend;
    private Material mat;
    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame = (currentFrame + 1) % frameCount;
            float frameOffset = (float)currentFrame / frameCount;
            mat.mainTextureOffset = new Vector2(0, frameOffset);
        }
    }
}