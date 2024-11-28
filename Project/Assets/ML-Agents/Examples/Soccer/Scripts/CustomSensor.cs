using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
public class CustomSensor : MonoBehaviour
{
    private SoundSensor soundSensor;
    
    private const int observationSize = 10; // Customize based on your needs

    void Start()
    {
        soundSensor = new SoundSensor(observationSize, "PlayerSoundSensor");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("sound"))
        {
            Debug.Log("Player entered the sound area!");
            HandleSoundDetection(other.gameObject);
        }
    }

    void HandleSoundDetection(GameObject soundSource)
    {
        // Calculate sound intensity based on distance or other parameters
        float soundIntensity = CalculateSoundIntensity(soundSource.transform.position);

        // Add observation to the sound sensor
        soundSensor.AddSoundObservation(soundIntensity);

        Debug.Log("Sound detected from: " + soundSource.name + " with intensity: " + soundIntensity);
    }

    float CalculateSoundIntensity(Vector3 soundSourcePosition)
    {
        float distance = Vector3.Distance(transform.position, soundSourcePosition);
        return Mathf.Clamp01(1.0f / distance); // Inverse relationship with distance
    }

    void Update()
    {
        // Call Update on the sensor to clear observations at the start of a new frame
        soundSensor.Update();
    }
}
