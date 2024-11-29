using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    private SoundManager soundManager;
    string target;

    public bool IsTarget = false;
    void Start()
    {
        soundManager = SoundManager.Instance; // Get the SoundManager instance
        if(IsTarget)
            gameObject.name += " target";
    }

    void OnCollisionEnter(Collision collision)
    {
        // When collision occurs, create a new sound at the collision point
        if (soundManager != null)
        {
            Vector3 collisionPoint = collision.contacts[0].point; // Collision point
        
            soundManager.CreateSound(collisionPoint, collision.gameObject.name,gameObject.name); 
        }
    }
}
