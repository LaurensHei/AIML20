using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSensor : MonoBehaviour
{
 void OnTriggerEnter(Collider other) {
        // Check if the trigger is caused by a sound sphere
        if (other.gameObject.name.Contains("sound"))
        {
            Debug.Log("Player entered the sound area!");
            HandleSoundDetection(other.gameObject);
        }
    }

    void HandleSoundDetection(GameObject soundSource)
    {
        // Custom logic when a sound sphere is detected
        Debug.Log("Sound detected from: " + soundSource.name);

        // Example: Trigger a sound or an alert
    }
}
