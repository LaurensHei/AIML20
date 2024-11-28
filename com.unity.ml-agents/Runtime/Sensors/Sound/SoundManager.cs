using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public GameObject soundSpherePrefab; // Prefab for the sound sphere
    private List<GameObject> soundSpheres = new List<GameObject>(); // List to store active sound spheres

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure there's only one instance
            return;
        }
        Instance = this;
    }

    // Method to create a new sound at a given position
    public void CreateSound(Vector3 position, string obj1, string obj2)
    {
        // Instantiate a new sound sphere at the given position
        GameObject newSound = Instantiate(soundSpherePrefab, position, Quaternion.identity);

        // Add it to the list of sound spheres
        soundSpheres.Add(newSound);

        // Optionally, set additional properties on the sound sphere (e.g., a unique name, etc.)
        newSound.name = "SoundSphere_" + obj1 + "_" + obj2; // Unique name based on objects
    }

    // Update all sound spheres in the scene
    void Update()
    {
        // Iterate through the list of sound spheres and update their behavior
        for (int i = soundSpheres.Count - 1; i >= 0; i--)
        {
            GameObject soundSphere = soundSpheres[i];

            // If sound sphere is destroyed, remove it from the list
            if (soundSphere == null)
            {
                soundSpheres.RemoveAt(i);
                continue;
            }
        }
    }

    public List<GameObject> GetSoundSpheres()
    {
        return soundSpheres;
    }
}
