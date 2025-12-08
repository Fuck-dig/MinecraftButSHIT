using UnityEngine;

public class BlockOutlineRaycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxRayDistance = 5f;
    [SerializeField] private LayerMask blockLayer;
    
    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.black;
    [SerializeField] private float outlineThickness = 0.02f;
    
    private Camera playerCamera;
    private GameObject currentHighlightedBlock;
    private GameObject outlineObject;
    private LineRenderer[] lineRenderers;
    
    void Start()
    {
        playerCamera = Camera.main;
        CreateWireframeOutline();
    }
    
    void CreateWireframeOutline()
    {
        // Create parent object for all outline lines
        outlineObject = new GameObject("BlockOutline");
        outlineObject.SetActive(false);
        
        // Create 12 line renderers for the 12 edges of a cube
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
        PerformRaycast();
    }
    
    void PerformRaycast()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxRayDistance, blockLayer))
        {
            GameObject hitBlock = hit.collider.gameObject;
            
            if (hitBlock.CompareTag("Block"))
            {
                // Position outline at block
                outlineObject.transform.position = hitBlock.transform.position;
                outlineObject.transform.rotation = hitBlock.transform.rotation;
                outlineObject.SetActive(true);
                
                // Update outline to match block size
                Vector3 size = hitBlock.transform.localScale;
                UpdateOutlineEdges(size);
                
                currentHighlightedBlock = hitBlock;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            }
        }
        else
        {
            outlineObject.SetActive(false);
            currentHighlightedBlock = null;
            Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.red);
        }
    }
    
    void UpdateOutlineEdges(Vector3 size)
    {
        float x = size.x / 2f;
        float y = size.y / 2f;
        float z = size.z / 2f;
        
        // Define 8 corners of the cube
        Vector3[] corners = new Vector3[8]
        {
            new Vector3(-x, -y, -z), // 0: bottom-left-back
            new Vector3( x, -y, -z), // 1: bottom-right-back
            new Vector3( x, -y,  z), // 2: bottom-right-front
            new Vector3(-x, -y,  z), // 3: bottom-left-front
            new Vector3(-x,  y, -z), // 4: top-left-back
            new Vector3( x,  y, -z), // 5: top-right-back
            new Vector3( x,  y,  z), // 6: top-right-front
            new Vector3(-x,  y,  z)  // 7: top-left-front
        };
        
        // Define 12 edges connecting the corners
        int[][] edges = new int[12][]
        {
            // Bottom face
            new int[] {0, 1}, new int[] {1, 2}, new int[] {2, 3}, new int[] {3, 0},
            // Top face
            new int[] {4, 5}, new int[] {5, 6}, new int[] {6, 7}, new int[] {7, 4},
            // Vertical edges
            new int[] {0, 4}, new int[] {1, 5}, new int[] {2, 6}, new int[] {3, 7}
        };
        
        // Set positions for each line renderer
        for (int i = 0; i < 12; i++)
        {
            lineRenderers[i].SetPosition(0, corners[edges[i][0]]);
            lineRenderers[i].SetPosition(1, corners[edges[i][1]]);
        }
    }
    
    public GameObject GetTargetedBlock()
    {
        return currentHighlightedBlock;
    }
    
    public bool GetTargetedBlockInfo(out RaycastHit hitInfo)
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        return Physics.Raycast(ray, out hitInfo, maxRayDistance, blockLayer);
    }
    
    void OnDestroy()
    {
        if (outlineObject != null)
        {
            Destroy(outlineObject);
        }
    }
}