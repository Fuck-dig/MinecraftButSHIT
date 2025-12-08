using UnityEngine;

public class HidePlayerBody : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The player model GameObject to hide (NOT the torch!)")]
    public GameObject playerModel;
    
    [Tooltip("The camera that shouldn't see the player model")]
    public Camera playerCamera;
    
    [Header("Or Auto-Find by Name/Tag")]
    [Tooltip("If playerModel is not assigned, search for child with this name")]
    public string playerModelName = "PlayerModel";
    
    [Tooltip("Objects with these names will NOT be hidden (like torch)")]
    public string[] excludeNames = new string[] { "Torch", "TorchLight", "Weapon", "Hand" };
    
    [Header("Method")]
    [Tooltip("Choose which method to use")]
    public HideMethod method = HideMethod.Layer;
    
    public enum HideMethod
    {
        Layer,          // Best for performance - uses layers
        DisableRenderer // Simple - just disables the renderer
    }
    
    void Start()
    {
        // Auto-find components if not assigned
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        // Try to find player model if not assigned
        if (playerModel == null)
        {
            Transform found = transform.Find(playerModelName);
            if (found != null)
            {
                playerModel = found.gameObject;
                Debug.Log("Found player model: " + playerModel.name);
            }
            else
            {
                Debug.LogError("Player model not found! Assign it manually or check the name.");
                return;
            }
        }
        
        if (method == HideMethod.Layer)
        {
            SetupLayerMethod();
        }
        else if (method == HideMethod.DisableRenderer)
        {
            SetupDisableRenderer();
        }
    }
    
    void SetupLayerMethod()
    {
        // Method 1: Using Layers (recommended - best performance)
        
        int playerLayer = LayerMask.NameToLayer("PlayerBody");
        
        // If "PlayerBody" layer doesn't exist, use layer 8
        if (playerLayer == -1)
        {
            playerLayer = 8; // You'll need to create this layer manually
            Debug.LogWarning("Create a layer called 'PlayerBody' in Edit > Project Settings > Tags and Layers (or use layer 8)");
        }
        
        // Set ONLY player model to the player layer (excludes torch and other items)
        SetLayerSelectively(playerModel, playerLayer);
        
        // Make camera ignore the player body layer
        playerCamera.cullingMask &= ~(1 << playerLayer);
        
        Debug.Log("Player model hidden using Layer method. Torch and items remain visible.");
    }
    
    void SetupDisableRenderer()
    {
        // Method 2: Disable only the player model renderer
        Renderer renderer = playerModel.GetComponent<Renderer>();
        
        if (renderer != null)
        {
            renderer.enabled = false;
            Debug.Log("Player model renderer disabled. Children (torch) remain visible.");
        }
        else
        {
            Debug.LogWarning("No renderer found on player model!");
        }
    }
    
    void SetLayerSelectively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        
        // Check if this object should be excluded
        foreach (string excludeName in excludeNames)
        {
            if (obj.name.Contains(excludeName))
            {
                Debug.Log("Skipping: " + obj.name);
                return; // Don't change layer for excluded objects
            }
        }
        
        // Set this object's layer
        obj.layer = newLayer;
        
        // Recursively set children's layers (but respect exclusions)
        foreach (Transform child in obj.transform)
        {
            SetLayerSelectively(child.gameObject, newLayer);
        }
    }
}

// Alternative: Put this on your camera if you want more control
public class FirstPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerBody;
    public Vector3 cameraOffset = new Vector3(0, 0.6f, 0); // Eye height
    
    [Header("Hide Player")]
    public bool hidePlayerFromCamera = true;
    
    void Start()
    {
        if (hidePlayerFromCamera && playerBody != null)
        {
            // Hide player using layer method
            int playerLayer = 8; // Make sure this matches your Player layer
            
            // Set player to this layer
            SetLayerRecursively(playerBody.gameObject, playerLayer);
            
            // Camera ignores this layer
            Camera cam = GetComponent<Camera>();
            cam.cullingMask &= ~(1 << playerLayer);
        }
    }
    
    void LateUpdate()
    {
        if (playerBody != null)
        {
            // Keep camera attached to player body with offset
            transform.position = playerBody.position + cameraOffset;
        }
    }
    
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}