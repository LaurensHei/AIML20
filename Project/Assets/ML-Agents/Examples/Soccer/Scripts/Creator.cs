using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour
{
    // Prefab for the sound object
    public GameObject soundCreater;

    // List to hold active sound objects
    private List<GameObject> sounds = new List<GameObject>();

    // Scale increment values for sound objects
    private Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);

    // Range and speed settings for scaling
    [Range(0f, 50f)]
    private float soundRange = 20f;
    [Range(0f, 200)]
    private float soundSpeed = 40f;

    // Initializes when the script is loaded
    void Awake()
    {
        if (soundCreater == null)
        {
            Debug.LogError("No soundCreater prefab assigned!");
        }
    }

    // Called once per frame
    void Update()
    {
        // Update the scale of each sound object and manage them
        for (int i = sounds.Count - 1; i >= 0; i--)  // Iterating backwards to safely remove items
        {
            GameObject sound = sounds[i];

            // Scale each sound object smoothly using Time.deltaTime for frame independence
            sound.transform.localScale += scaleChange * soundSpeed;

            // Reverse scaling if object exceeds the max range
            if (sound.transform.localScale.x > soundRange)
            {
                // Reverse the scale increment for the individual object
                Destroy(sound);
                sounds.RemoveAt(i);
            }

           
        }
    }

    // Method to create and store a new sound object at a specific position
    public void CreateSound(Vector3 position, string fromName, string toName)
        {
    // Check if the sound creator prefab is assigned
    if (soundCreater != null)
    {
        // Instantiate a new sound object at the given position
        GameObject newSound = Instantiate(soundCreater, position, Quaternion.identity);

        // Reset its initial scale
        newSound.transform.localScale = Vector3.one;

        // Set the name for debugging or tracking purposes
        newSound.name = "sound " + fromName + " " + toName;

        // Ensure the Collider exists and set it to be a Trigger
        Collider collider = newSound.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("The instantiated sound object does not have a Collider component.");
        }

        // Add the new sound object to the list for tracking
        sounds.Add(newSound);
    }
    else
    {
        Debug.LogWarning("soundCreater prefab is not assigned.");
    }
}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected");

        // Call the method to create a sound at the collision point
        GetComponent<Creator>().CreateSound(collision.contacts[0].point,collision.gameObject.name,gameObject.name);
    }



}
