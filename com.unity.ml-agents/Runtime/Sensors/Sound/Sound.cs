using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f); // Increment for scaling
    private float soundRange = 20f; // Max size of the sound sphere
    private float soundSpeed = 400f; // Speed of scaling

    SoundManager soundManager;
    private void Start()
    {




    }

    public void Add()
    {
        if (soundManager != null)
        {
            soundManager.RegisterSoundSphere(gameObject);
        }
        else
        {
            Debug.LogError("SoundManager not found in the environment.");
        }
    }
    void Update()
    {

        // Scale the sound sphere over time
        transform.localScale += scaleChange * soundSpeed;
        AdjustColorIntensityBasedOnSize(gameObject, 0.01f);
        // If the sound sphere exceeds the defined range, destroy it
        if (transform.localScale.x > soundRange)
        {
            Destroy(gameObject); // Destroy the sound sphere when it reaches max size
            soundManager.UnregisterSoundSphere(gameObject);
        }
    }

    private void AdjustColorIntensityBasedOnSize(GameObject sphere, float scaleFactor)
    {
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Get the current color
            Color originalColor = renderer.material.color;

            // Calculate intensity factor: smaller size => brighter, larger size => darker
            float intensityFactor = Mathf.Clamp(1.0f / scaleFactor, 0.2f, 1.0f); // Limit to prevent overly bright/dark colors

            // Adjust color intensity
            Color adjustedColor = originalColor * intensityFactor;

            // Keep the original alpha (transparency)
            adjustedColor.a = originalColor.a;

            // Apply the adjusted color
            renderer.material.color = adjustedColor;
        }
    }

    public void Initialize(SoundManager manager)
    {
        soundManager = manager;
    }
}
