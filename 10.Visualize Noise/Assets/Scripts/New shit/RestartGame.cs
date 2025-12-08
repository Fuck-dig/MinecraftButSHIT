using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    [Header("Restart Settings")]
    [Tooltip("Leave empty to restart current scene")]
    public string sceneToLoad = "";
    
    [Header("Optional: Restart Key")]
    public KeyCode restartKey = KeyCode.R;
    public bool enableKeyRestart = true;

    void Update()
    {
        // Optional: Press R key to restart
        if (enableKeyRestart && Input.GetKeyDown(restartKey))
        {
            Restart();
        }
    }

    /// <summary>
    /// Restarts the game by reloading the scene
    /// </summary>
    public void Restart()
    {
        // Stop time scale in case it was modified
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Load specified scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    /// <summary>
    /// Restarts the game with a delay
    /// </summary>
    public void RestartWithDelay(float delay)
    {
        StartCoroutine(RestartCoroutine(delay));
    }

    private System.Collections.IEnumerator RestartCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Restart();
    }

    /// <summary>
    /// Restart to a specific scene by name
    /// </summary>
    public void RestartToScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Restart to first scene (index 0)
    /// </summary>
    public void RestartToFirstScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}