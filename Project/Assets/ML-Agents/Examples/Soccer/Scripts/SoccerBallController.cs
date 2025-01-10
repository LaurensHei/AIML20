using UnityEngine;

public class SoccerBallController : MonoBehaviour
{
    public GameObject area;
    [HideInInspector]
    public SoccerEnvController envController;

    public string purpleAgentTag = "purpleAgent"; // Tag for purple agents
    public string blueAgentTag = "blueAgent";    // Tag for blue agents
    public string purpleGoalTag = "purpleGoal";  // Tag for the purple goal
    public string blueGoalTag = "blueGoal";      // Tag for the blue goal

    void Start()
    {
        envController = area.GetComponent<SoccerEnvController>();
        if (envController == null)
        {
            Debug.LogError("SoccerEnvController not found on the area object.");
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(purpleAgentTag))
        {

            Debug.Log("Ball touched by Purple Agent");

            envController.purplePlayerTouched(Team.Purple);
        }
        else if (col.gameObject.CompareTag(blueAgentTag))
        {

            Debug.Log("Ball touched by Blue Agent");

            envController.bluePlayerTouched(Team.Blue);
        }
        else if (col.gameObject.CompareTag(blueGoalTag))
        {
            Debug.Log("Ball entered Blue Goal");
            envController.GoalTouched(col.gameObject);
        }
        else if (col.gameObject.CompareTag(purpleGoalTag))
        {
            Debug.Log("Ball entered Purple Goal");
            envController.GoalTouched(col.gameObject);
        }
    }
}
