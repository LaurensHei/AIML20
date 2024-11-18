using UnityEngine;

public class SoccerBallController : MonoBehaviour
{
    public GameObject area;
    [HideInInspector]
    public SoccerEnvController envController;
    public string purpleGoalTag; //will be used to check if collided with purple goal
    public string blueGoalTag; //will be used to check if collided with blue goal

    public string purpleAgentTag;
    public string blueAgentTag;

    

    void Start()
    {
        envController = area.GetComponent<SoccerEnvController>();
        Debug.LogWarning($"purpleAgentTag: {purpleAgentTag}");
        Debug.LogWarning($"blueAgentTag: {blueAgentTag}");
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(purpleAgentTag)) // ball touched purple agent 
        {
            Debug.LogWarning("Purple player recognized");
            envController.purplePlayerTouched(Team.Purple);
        }
        if (col.gameObject.CompareTag(blueAgentTag)) // ball touched blue agent
        {
            Debug.LogWarning("Blue player recognized");
            envController.bluePlayerTouched(Team.Blue);
        }
        if (col.gameObject.CompareTag(purpleGoalTag)) //ball touched purple goal
        {
            envController.GoalTouched(Team.Blue);
        }
        if (col.gameObject.CompareTag(blueGoalTag)) //ball touched blue goal
        {
            envController.GoalTouched(Team.Purple);
        }
    }
}
