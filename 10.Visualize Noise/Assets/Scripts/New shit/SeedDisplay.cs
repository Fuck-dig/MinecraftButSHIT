using UnityEngine;
using UnityEngine.UI;

// Attach this to a GameObject that has a UI Text component assigned.
// It will display the currently saved seed from PlayerPrefs ("LastSeed").
public class SeedDisplay : MonoBehaviour
{
    public Text seedText;

    private int lastSeed = int.MinValue;

    void Start()
    {
        UpdateDisplayedSeed();
    }

    void Update()
    {
        // Poll for changes (lightweight). Update if changed.
        if (PlayerPrefs.HasKey("LastSeed"))
        {
            int seed = PlayerPrefs.GetInt("LastSeed");
            if (seed != lastSeed)
            {
                lastSeed = seed;
                UpdateDisplayedSeed();
            }
        }
        else if (lastSeed != int.MinValue)
        {
            // Seed removed
            lastSeed = int.MinValue;
            UpdateDisplayedSeed();
        }
    }

    private void UpdateDisplayedSeed()
    {
        if (seedText == null) return;

        if (PlayerPrefs.HasKey("LastSeed"))
        {
            int seed = PlayerPrefs.GetInt("LastSeed");
            seedText.text = "Seed: " + seed;
        }
        else
        {
            seedText.text = "Seed: (none)";
        }
    }
}
