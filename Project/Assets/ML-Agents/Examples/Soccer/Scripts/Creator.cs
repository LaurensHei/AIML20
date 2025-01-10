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

  public void CreateSound(Vector3 position, string fromName, string toName, string soundType)
{
    if (soundCreater != null)
    {
        // Instantiate a new sound object at the given position
        GameObject newSound = Instantiate(soundCreater, position, Quaternion.identity);
        newSound.transform.localScale = Vector3.one;
        newSound.name = $"sound_{fromName}_{toName}";

        SoundObject soundObj = newSound.GetComponent<SoundObject>();
        if (soundObj == null) 
        {
            soundObj = newSound.AddComponent<SoundObject>();
        }

        soundObj.originName = fromName;
        soundObj.endName = toName;
        soundObj.soundType = soundType;
        

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

        // Modify transparency of the sound object
        Renderer renderer = newSound.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material; 
            Color color = material.color;
            color.a = 0.2f; 
            material.color = color;

            
            material.SetFloat("_Mode", 3); 
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
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
        Debug.LogWarning("Collision detected");
        GameObject firstObj = collision.gameObject;
        GameObject secObj = gameObject;

        bool isBallCollision = secObj.CompareTag("Ball") || firstObj.CompareTag("Ball");
        string collisionType = isBallCollision ? "BallCollision" : "GenericCollision";

        // Call the method to create a sound at the collision point
        Vector3 soundPosition = collision.contacts[0].point;
        CreateSound(soundPosition, firstObj.name, secObj.name, collisionType);
    }



}
