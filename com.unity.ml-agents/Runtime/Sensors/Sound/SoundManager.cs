using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public GameObject soundSpherePrefab; // Prefab for the sound sphere
    private List<GameObject> soundSpheres = new List<GameObject>(); // List to store active sound spheres

    void Awake()
    {

    }

    // Method to create a new sound at a given position
    public void CreateSound(Vector3 position, string obj1, string obj2)
    {
        // Instantiate a new sound sphere at the given position

        GameObject newSound = Instantiate(soundSpherePrefab, position, Quaternion.identity);

        // Optionally, set additional properties on the sound sphere (e.g., a unique name, etc.)
        newSound.name = "SoundSphere_" + obj1 + " " + obj2 + "_"; // Unique name based on objects
        newSound.GetComponent<Sound>().Initialize(this);
        newSound.GetComponent<Sound>().Add();

        // Add it to the list of sound spheres
        RegisterSoundSphere(newSound);
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

    /// <summary>
    /// Returns the list of sound spheres, excluding destroyed or null objects.
    /// </summary>
    /// <returns>List of active sound spheres.</returns>
    public List<GameObject> GetSoundSpheres()
    {

        Clean();

        return soundSpheres;
    }


    /// <summary>
    /// Adds a sound sphere to the manager.
    /// </summary>
    /// <param name="soundSphere">The sound sphere to add.</param>
    public void RegisterSoundSphere(GameObject soundSphere)
    {
        if (soundSphere != null && !soundSpheres.Contains(soundSphere))
        {
            soundSpheres.Add(soundSphere);
        }
    }

    /// <summary>
    /// Removes a sound sphere from the manager.
    /// </summary>
    /// <param name="soundSphere">The sound sphere to remove.</param>
    public void UnregisterSoundSphere(GameObject soundSphere)
    {
        if (soundSpheres.Contains(soundSphere))
        {
            soundSpheres.Remove(soundSphere);
            Clean();
        }
    }

    // Remove any destroyed or null objects from the list
    private void Clean()
    {

        soundSpheres.RemoveAll(sphere => sphere == null);
    }

}
