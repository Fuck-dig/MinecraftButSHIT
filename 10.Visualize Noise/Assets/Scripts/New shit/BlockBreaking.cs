using UnityEngine;
using System.Collections;

public class BlockBreaker : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxRayDistance = 5f;
    [SerializeField] private LayerMask blockLayer;
    
    [Header("Breaking Settings")]
    [SerializeField] private float breakTime = 1f;
    [SerializeField] private KeyCode breakKey = KeyCode.Mouse0;
    
    [Header("Visual Feedback")]
    [SerializeField] private Material[] crackTextures; // Assign crack materials in order
    [SerializeField] private GameObject breakParticles;
    
    [Header("Audio")]
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private AudioClip hitSound;
    
    private Camera playerCamera;
    private GameObject targetBlock;
    private float breakProgress = 0f;
    private bool isBreaking = false;
    private Renderer targetRenderer;
    private Material originalMaterial;
    private AudioSource audioSource;
    
    void Start()
    {
        playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        FindTargetBlock();
        HandleBreaking();
    }
    
    void FindTargetBlock()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxRayDistance, blockLayer))
        {
            GameObject hitBlock = hit.collider.gameObject;
            
            if (hitBlock.CompareTag("Block"))
            {
                // If we're looking at a different block, reset progress
                if (targetBlock != hitBlock)
                {
                    ResetBreaking();
                    targetBlock = hitBlock;
                    targetRenderer = hitBlock.GetComponent<Renderer>();
                    if (targetRenderer != null)
                    {
                        originalMaterial = targetRenderer.material;
                    }
                }
            }
        }
        else
        {
            // Not looking at any block
            if (targetBlock != null)
            {
                ResetBreaking();
            }
        }
    }
    
    void HandleBreaking()
    {
        if (targetBlock == null) return;
        
        // Start breaking when key is held down
        if (Input.GetKey(breakKey))
        {
            if (!isBreaking)
            {
                isBreaking = true;
                if (hitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
            
            breakProgress += Time.deltaTime / breakTime;
            
            // Update crack texture based on progress
            UpdateCrackTexture();
            
            // Break the block when progress reaches 100%
            if (breakProgress >= 1f)
            {
                BreakBlock();
            }
        }
        else
        {
            // Reset if player stops holding the button
            if (isBreaking)
            {
                ResetBreaking();
            }
        }
    }
    
    void UpdateCrackTexture()
    {
        if (crackTextures == null || crackTextures.Length == 0 || targetRenderer == null) return;
        
        // Calculate which crack stage to show
        int crackStage = Mathf.FloorToInt(breakProgress * crackTextures.Length);
        crackStage = Mathf.Clamp(crackStage, 0, crackTextures.Length - 1);
        
        // Apply crack texture (you might want to overlay this instead of replacing)
        if (crackTextures[crackStage] != null)
        {
            targetRenderer.material = crackTextures[crackStage];
        }
    }
    
    void BreakBlock()
    {
        if (targetBlock == null) return;
        
        // Spawn break particles
        if (breakParticles != null)
        {
            Instantiate(breakParticles, targetBlock.transform.position, Quaternion.identity);
        }
        
        // Play break sound
        if (breakSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(breakSound);
        }
        
        // Destroy the block
        Destroy(targetBlock);
        
        // Reset state
        targetBlock = null;
        targetRenderer = null;
        breakProgress = 0f;
        isBreaking = false;
    }
    
    void ResetBreaking()
    {
        // Restore original material
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.material = originalMaterial;
        }
        
        breakProgress = 0f;
        isBreaking = false;
        targetBlock = null;
        targetRenderer = null;
        originalMaterial = null;
    }
    
    // Public method to get break progress (useful for UI)
    public float GetBreakProgress()
    {
        return breakProgress;
    }
    
    public GameObject GetTargetBlock()
    {
        return targetBlock;
    }
}


// SIMPLE VERSION - Instant break with no animations
public class SimpleBlockBreaker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxRayDistance = 5f;
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private KeyCode breakKey = KeyCode.Mouse0;
    
    [Header("Effects")]
    [SerializeField] private GameObject breakParticles;
    [SerializeField] private AudioClip breakSound;
    
    private Camera playerCamera;
    private AudioSource audioSource;
    
    void Start()
    {
        playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(breakKey))
        {
            TryBreakBlock();
        }
    }
    
    void TryBreakBlock()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxRayDistance, blockLayer))
        {
            GameObject hitBlock = hit.collider.gameObject;
            
            if (hitBlock.CompareTag("Block"))
            {
                // Spawn particles
                if (breakParticles != null)
                {
                    Instantiate(breakParticles, hitBlock.transform.position, Quaternion.identity);
                }
                
                // Play sound
                if (breakSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(breakSound);
                }
                
                // Destroy block
                Destroy(hitBlock);
            }
        }
    }
}


// COMBINED VERSION - Outline + Breaking in one script
public class BlockInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxRayDistance = 5f;
    [SerializeField] private LayerMask blockLayer;
    
    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.black;
    [SerializeField] private float outlineThickness = 0.02f;
    
    [Header("Breaking Settings")]
    [SerializeField] private float breakTime = 1f;
    [SerializeField] private KeyCode breakKey = KeyCode.Mouse0;
    [SerializeField] private Material[] crackTextures;
    [SerializeField] private GameObject breakParticles;
    [SerializeField] private AudioClip breakSound;
    
    private Camera playerCamera;
    private GameObject targetBlock;
    private GameObject outlineObject;
    private LineRenderer[] lineRenderers;
    private float breakProgress = 0f;
    private bool isBreaking = false;
    private Renderer targetRenderer;
    private Material originalMaterial;
    private AudioSource audioSource;
    
    void Start()
    {
        playerCamera = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
        CreateWireframeOutline();
    }
    
    void CreateWireframeOutline()
    {
        outlineObject = new GameObject("BlockOutline");
        outlineObject.SetActive(false);
        lineRenderers = new LineRenderer[12];
        
        for (int i = 0; i < 12; i++)
        {
            GameObject lineObj = new GameObject($"OutlineLine_{i}");
            lineObj.transform.SetParent(outlineObject.transform);
            
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = outlineColor;
            lr.endColor = outlineColor;
            lr.startWidth = outlineThickness;
            lr.endWidth = outlineThickness;
            lr.positionCount = 2;
            lr.useWorldSpace = false;
            lr.alignment = LineAlignment.View;
            
            lineRenderers[i] = lr;
        }
    }
    
    void Update()
    {
        FindTargetBlock();
        HandleBreaking();
    }
    
    void FindTargetBlock()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxRayDistance, blockLayer))
        {
            GameObject hitBlock = hit.collider.gameObject;
            
            if (hitBlock.CompareTag("Block"))
            {
                if (targetBlock != hitBlock)
                {
                    ResetBreaking();
                    targetBlock = hitBlock;
                    targetRenderer = hitBlock.GetComponent<Renderer>();
                    if (targetRenderer != null)
                    {
                        originalMaterial = targetRenderer.material;
                    }
                }
                
                // Show outline
                outlineObject.transform.position = hitBlock.transform.position;
                outlineObject.transform.rotation = hitBlock.transform.rotation;
                outlineObject.SetActive(true);
                UpdateOutlineEdges(hitBlock.transform.localScale);
            }
        }
        else
        {
            outlineObject.SetActive(false);
            if (targetBlock != null)
            {
                ResetBreaking();
            }
        }
    }
    
    void UpdateOutlineEdges(Vector3 size)
    {
        float x = size.x / 2f;
        float y = size.y / 2f;
        float z = size.z / 2f;
        
        Vector3[] corners = new Vector3[8]
        {
            new Vector3(-x, -y, -z), new Vector3( x, -y, -z),
            new Vector3( x, -y,  z), new Vector3(-x, -y,  z),
            new Vector3(-x,  y, -z), new Vector3( x,  y, -z),
            new Vector3( x,  y,  z), new Vector3(-x,  y,  z)
        };
        
        int[][] edges = new int[12][]
        {
            new int[] {0, 1}, new int[] {1, 2}, new int[] {2, 3}, new int[] {3, 0},
            new int[] {4, 5}, new int[] {5, 6}, new int[] {6, 7}, new int[] {7, 4},
            new int[] {0, 4}, new int[] {1, 5}, new int[] {2, 6}, new int[] {3, 7}
        };
        
        for (int i = 0; i < 12; i++)
        {
            lineRenderers[i].SetPosition(0, corners[edges[i][0]]);
            lineRenderers[i].SetPosition(1, corners[edges[i][1]]);
        }
    }
    
    void HandleBreaking()
    {
        if (targetBlock == null) return;
        
        if (Input.GetKey(breakKey))
        {
            isBreaking = true;
            breakProgress += Time.deltaTime / breakTime;
            
            UpdateCrackTexture();
            
            if (breakProgress >= 1f)
            {
                BreakBlock();
            }
        }
        else if (isBreaking)
        {
            ResetBreaking();
        }
    }
    
    void UpdateCrackTexture()
    {
        if (crackTextures == null || crackTextures.Length == 0 || targetRenderer == null) return;
        
        int crackStage = Mathf.FloorToInt(breakProgress * crackTextures.Length);
        crackStage = Mathf.Clamp(crackStage, 0, crackTextures.Length - 1);
        
        if (crackTextures[crackStage] != null)
        {
            targetRenderer.material = crackTextures[crackStage];
        }
    }
    
    void BreakBlock()
    {
        if (breakParticles != null)
        {
            Instantiate(breakParticles, targetBlock.transform.position, Quaternion.identity);
        }
        
        if (breakSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(breakSound);
        }
        
        Destroy(targetBlock);
        targetBlock = null;
        targetRenderer = null;
        breakProgress = 0f;
        isBreaking = false;
    }
    
    void ResetBreaking()
    {
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.material = originalMaterial;
        }
        
        breakProgress = 0f;
        isBreaking = false;
        targetBlock = null;
        targetRenderer = null;
        originalMaterial = null;
    }
    
    void OnDestroy()
    {
        if (outlineObject != null)
        {
            Destroy(outlineObject);
        }
    }
}