using UnityEngine;
using System.Collections.Generic;

public class TorchBuilder : MonoBehaviour
{
    // Drag your torch textures here in the Inspector
    public Texture2D torchTexture;
    public bool createOnStart = true;
    
    void Start()
    {
        if (createOnStart)
        {
            CreateTorch();
        }
    }
    
    public GameObject CreateTorch()
    {
        // Create the torch mesh
        GameObject torch = new GameObject("Torch");
        torch.transform.position = transform.position;
        MeshFilter meshFilter = torch.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = torch.AddComponent<MeshRenderer>();
        
        Mesh mesh = new Mesh();
        mesh.name = "TorchMesh";
        
        float p = 1f / 16f; // One pixel
        
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        
        // STICK PART (2x2x10 pixels, centered at bottom)
        // Bottom: 7-9 pixels from left, 6-8 from front
        float stickWidth = 2 * p;
        float stickHeight = 10 * p;
        float halfStick = stickWidth / 2f;
        
        // Stick - Front face
        AddQuad(vertices, uvs, triangles,
            new Vector3(-halfStick, 0, halfStick),
            new Vector3(halfStick, 0, halfStick),
            new Vector3(halfStick, stickHeight, halfStick),
            new Vector3(-halfStick, stickHeight, halfStick),
            new Vector2(7*p, 6*p), new Vector2(9*p, 6*p),
            new Vector2(9*p, 16*p), new Vector2(7*p, 16*p));
        
        // Stick - Back face
        AddQuad(vertices, uvs, triangles,
            new Vector3(halfStick, 0, -halfStick),
            new Vector3(-halfStick, 0, -halfStick),
            new Vector3(-halfStick, stickHeight, -halfStick),
            new Vector3(halfStick, stickHeight, -halfStick),
            new Vector2(7*p, 6*p), new Vector2(9*p, 6*p),
            new Vector2(9*p, 16*p), new Vector2(7*p, 16*p));
        
        // Stick - Right face
        AddQuad(vertices, uvs, triangles,
            new Vector3(halfStick, 0, halfStick),
            new Vector3(halfStick, 0, -halfStick),
            new Vector3(halfStick, stickHeight, -halfStick),
            new Vector3(halfStick, stickHeight, halfStick),
            new Vector2(7*p, 6*p), new Vector2(9*p, 6*p),
            new Vector2(9*p, 16*p), new Vector2(7*p, 16*p));
        
        // Stick - Left face
        AddQuad(vertices, uvs, triangles,
            new Vector3(-halfStick, 0, -halfStick),
            new Vector3(-halfStick, 0, halfStick),
            new Vector3(-halfStick, stickHeight, halfStick),
            new Vector3(-halfStick, stickHeight, -halfStick),
            new Vector2(7*p, 6*p), new Vector2(9*p, 6*p),
            new Vector2(9*p, 16*p), new Vector2(7*p, 16*p));
        
        // FLAME PART (4x4x4 pixels on top, slightly larger)
        float flameSize = 4 * p;
        float halfFlame = flameSize / 2f;
        float flameBottom = stickHeight;
        float flameTop = stickHeight + flameSize;
        
        // Flame - Front face
        AddQuad(vertices, uvs, triangles,
            new Vector3(-halfFlame, flameBottom, halfFlame),
            new Vector3(halfFlame, flameBottom, halfFlame),
            new Vector3(halfFlame, flameTop, halfFlame),
            new Vector3(-halfFlame, flameTop, halfFlame),
            new Vector2(0, 0), new Vector2(4*p, 0),
            new Vector2(4*p, 4*p), new Vector2(0, 4*p));
        
        // Flame - Back face
        AddQuad(vertices, uvs, triangles,
            new Vector3(halfFlame, flameBottom, -halfFlame),
            new Vector3(-halfFlame, flameBottom, -halfFlame),
            new Vector3(-halfFlame, flameTop, -halfFlame),
            new Vector3(halfFlame, flameTop, -halfFlame),
            new Vector2(0, 0), new Vector2(4*p, 0),
            new Vector2(4*p, 4*p), new Vector2(0, 4*p));
        
        // Flame - Right face
        AddQuad(vertices, uvs, triangles,
            new Vector3(halfFlame, flameBottom, halfFlame),
            new Vector3(halfFlame, flameBottom, -halfFlame),
            new Vector3(halfFlame, flameTop, -halfFlame),
            new Vector3(halfFlame, flameTop, halfFlame),
            new Vector2(0, 0), new Vector2(4*p, 0),
            new Vector2(4*p, 4*p), new Vector2(0, 4*p));
        
        // Flame - Left face
        AddQuad(vertices, uvs, triangles,
            new Vector3(-halfFlame, flameBottom, -halfFlame),
            new Vector3(-halfFlame, flameBottom, halfFlame),
            new Vector3(-halfFlame, flameTop, halfFlame),
            new Vector3(-halfFlame, flameTop, -halfFlame),
            new Vector2(0, 0), new Vector2(4*p, 0),
            new Vector2(4*p, 4*p), new Vector2(0, 4*p));
        
        // Flame - Top face
        AddQuad(vertices, uvs, triangles,
            new Vector3(-halfFlame, flameTop, halfFlame),
            new Vector3(halfFlame, flameTop, halfFlame),
            new Vector3(halfFlame, flameTop, -halfFlame),
            new Vector3(-halfFlame, flameTop, -halfFlame),
            new Vector2(0, 0), new Vector2(4*p, 0),
            new Vector2(4*p, 4*p), new Vector2(0, 4*p));
        
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
        
        // Create and apply material
        Material torchMat = new Material(Shader.Find("Standard"));
        torchMat.mainTexture = torchTexture;
        torchMat.SetFloat("_Glossiness", 0f);
        torchMat.SetFloat("_Metallic", 0f);
        
        // Make flame emit light
        torchMat.EnableKeyword("_EMISSION");
        torchMat.SetColor("_EmissionColor", new Color(1f, 0.7f, 0.3f) * 2f);
        
        meshRenderer.material = torchMat;
        
        // Add point light for illumination
        GameObject lightObj = new GameObject("TorchLight");
        lightObj.transform.parent = torch.transform;
        lightObj.transform.localPosition = new Vector3(0, stickHeight + flameSize/2f, 0);
        
        Light torchLight = lightObj.AddComponent<Light>();
        torchLight.type = LightType.Point;
        torchLight.color = new Color(1f, 0.6f, 0.2f);
        torchLight.intensity = 1.5f;
        torchLight.range = 10f;
        
        // Add flame flicker
        torch.AddComponent<TorchFlicker>();
        
        return torch;
    }
    
    void AddQuad(List<Vector3> verts, List<Vector2> uvs, List<int> tris,
                 Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3,
                 Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        int startIndex = verts.Count;
        
        verts.Add(v0);
        verts.Add(v1);
        verts.Add(v2);
        verts.Add(v3);
        
        uvs.Add(uv0);
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
        
        tris.Add(startIndex);
        tris.Add(startIndex + 2);
        tris.Add(startIndex + 1);
        
        tris.Add(startIndex);
        tris.Add(startIndex + 3);
        tris.Add(startIndex + 2);
    }
}

// Optional flame flicker script
public class TorchFlicker : MonoBehaviour
{
    private Light torchLight;
    private float baseIntensity = 2f;
    
    void Start()
    {
        torchLight = GetComponent<Light>();
    }
    
    void Update()
    {
        // Flicker the light intensity
        float flicker = Random.Range(-0.2f, 0.2f);
        torchLight.intensity = baseIntensity + flicker;
    }
}