using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f); // Increment for scaling
    private float soundRange = 20f; // Max size of the sound sphere
    private float soundSpeed = 400f; // Speed of scaling
    
    
    void Update()
    {
        // Scale the sound sphere over time
        transform.localScale += scaleChange * soundSpeed;
        
        // If the sound sphere exceeds the defined range, destroy it
        if (transform.localScale.x > soundRange)
        {
            Destroy(gameObject); // Destroy the sound sphere when it reaches max size
            SoundManager.Instance.UnregisterSoundSphere(gameObject);

        }
    }
}
