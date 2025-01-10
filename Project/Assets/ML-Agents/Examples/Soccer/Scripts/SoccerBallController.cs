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
<<<<<<< HEAD
=======
        if (envController == null)
        {
            Debug.LogError("SoccerEnvController not found on the area object.");
        }
>>>>>>> rewardSystemChrys
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(purpleAgentTag))
        {
<<<<<<< HEAD
=======
            Debug.Log("Ball touched by Purple Agent");
>>>>>>> rewardSystemChrys
            envController.purplePlayerTouched(Team.Purple);
        }
        else if (col.gameObject.CompareTag(blueAgentTag))
        {
<<<<<<< HEAD
=======
            Debug.Log("Ball touched by Blue Agent");
>>>>>>> rewardSystemChrys
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
