using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    private SoundManager soundManager;
    string target;

    public bool IsTarget = false;
    void Start()
    {
        soundManager = GetComponentInParent<SoundManager>();// Get the SoundManager instance
        if (soundManager == null)
        {
            Debug.LogError("manager is null here in trigger");
        }
        if (IsTarget){
            gameObject.name += " target";
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("Collision detected");
        // When collision occurs, create a new sound at the collision point
        if (soundManager != null)
        {
            GameObject firstObj = collision.gameObject;
            GameObject secObj = gameObject;
            
            bool isBallCollision = secObj.CompareTag("ball") || firstObj.CompareTag("ball");
            string collisionType = isBallCollision ? "BallCollision" : "GenericCollision";

            Vector3 collisionPoint = collision.contacts[0].point; // Collision point

            soundManager.CreateSound(collisionPoint, collision.gameObject.name, gameObject.name, collisionType);
        }

    }
}
