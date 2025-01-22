using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using TMPro;
using System.IO;

public class SoccerEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentSoccer Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }

    [Tooltip("Max Environment Steps")]
    public int MaxEnvironmentSteps = 25000;

    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 m_BallStartingPos;

    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SoccerSettings m_SoccerSettings;
    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;
    private int m_ResetTimer;

    public float blueTeamScore = 0;
    public float purpleTeamScore = 0;
    public string lastTeamBall;

    public TMP_Text scoreText;
    private float timeLastWrite = 0f;
    public float writeInterval = 10f;

    private string filePath = "Assets/ML-Agents/Examples/Soccer/Prefabs/scores.txt";

    void Start()
    {
        m_SoccerSettings = Object.FindFirstObjectByType<SoccerSettings>();
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = ball.transform.position;

        // Register agents in the appropriate groups
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();

            if (item.Agent.team == Team.Blue)
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            else
                m_PurpleAgentGroup.RegisterAgent(item.Agent);
        }

        scoreText.text = "Blue Team: 0 - Purple Team: 0";

        if (File.Exists(filePath))
            File.WriteAllText(filePath, "");

        ResetScene();
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        timeLastWrite += Time.fixedDeltaTime;

        // Environment reset if max steps are reached
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_PurpleAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }

        // Periodically write scores to file
        if (timeLastWrite >= writeInterval)
        {
            WriteScoresToFile();
            timeLastWrite = 0f;
        }
    }

    public void ResetBall()
    {
        var randomPosX = Random.Range(-2.5f, 2.5f);
        var randomPosZ = Random.Range(-2.5f, 2.5f);

        ball.transform.position = m_BallStartingPos + new Vector3(randomPosX, 0f, randomPosZ);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }

    public void GoalTouched(GameObject goal)
    {
        // Team-level (group) rewards for scoring/conceding
        if (goal.CompareTag("blueGoal"))
        {
            if (lastTeamBall == "purple") // Purple scores in Blue's goal
            {
                m_PurpleAgentGroup.AddGroupReward(2.0f);
                m_BlueAgentGroup.AddGroupReward(-1.5f);
                purpleTeamScore = Mathf.Min(purpleTeamScore + 1f, 20f);
            }
            else if (lastTeamBall == "blue") // Blue scores in own goal
            {
                m_BlueAgentGroup.AddGroupReward(-2.0f);
               
            }
        }
        else if (goal.CompareTag("purpleGoal"))
        {
            if (lastTeamBall == "blue") // Blue scores in Purple's goal
            {
                m_BlueAgentGroup.AddGroupReward(2.0f);
                m_PurpleAgentGroup.AddGroupReward(-1.5f);
                blueTeamScore = Mathf.Min(blueTeamScore + 1f, 20f);
            }
            else if (lastTeamBall == "purple") // Purple scores in own goal
            {
                m_PurpleAgentGroup.AddGroupReward(-2.0f);
                
            }
        }

        // End the group episode and reset
        m_PurpleAgentGroup.EndGroupEpisode();
        m_BlueAgentGroup.EndGroupEpisode();
        ResetScene();
    }

    public void purplePlayerTouched(Team teamAtBall)
    {
        // Team-level logic (steals/passes) => group reward
        if (lastTeamBall == "blue")
        {
            m_PurpleAgentGroup.AddGroupReward(0.5f);
            m_BlueAgentGroup.AddGroupReward(-0.5f);
            //Debug.Log("Purple team stole the ball. Reward given.");
            lastTeamBall = "purple";
        }
        else if (lastTeamBall == "purple")
        {
            m_PurpleAgentGroup.AddGroupReward(0.2f);
            //Debug.Log("Purple team successful pass. Partial reward given.");
        }
        else
        {
            lastTeamBall = "purple";
            //Debug.Log("Purple team first touch.");
        }
    }

    public void bluePlayerTouched(Team teamAtBall)
    {
        // Team-level logic (steals/passes) => group reward
        if (lastTeamBall == "purple")
        {
            m_BlueAgentGroup.AddGroupReward(0.5f);
            m_PurpleAgentGroup.AddGroupReward(-0.5f);
            //Debug.Log("Blue team stole the ball. Reward given.");
            lastTeamBall = "blue";
        }
        else if (lastTeamBall == "blue")
        {
            m_BlueAgentGroup.AddGroupReward(0.2f);
            //Debug.Log("Blue team successful pass. Partial reward given.");
        }
        else
        {
            lastTeamBall = "blue";
            //Debug.Log("Blue team first touch.");
        }
    }

    public void ResetScene()
    {
        m_ResetTimer = 0;

        // Reset each agent
        foreach (var item in AgentsList)
        {
            var randomPosX = Random.Range(-5f, 5f);
            var newStartPos = item.Agent.initialPos + new Vector3(randomPosX, 0f, 0f);
            var rot = item.Agent.rotSign * Random.Range(80.0f, 100.0f);
            var newRot = Quaternion.Euler(0, rot, 0);
            item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);

            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        ResetBall();
        DisplayScores();
    }

    public void DisplayScores()
    {
        // If either team hits 20, reset both scores
        if (Mathf.RoundToInt(blueTeamScore) == 20 || Mathf.RoundToInt(purpleTeamScore) == 20)
        {
            blueTeamScore = 0;
            purpleTeamScore = 0;
        }
        if (scoreText != null)
        {
            scoreText.text = $"Blue Team: {Mathf.RoundToInt(blueTeamScore)} \nPurple Team: {Mathf.RoundToInt(purpleTeamScore)}";
        }
        else
        {
            //Debug.LogWarning("scoreText is null");
        }
    }

    private void WriteScoresToFile()
    {
        string content = $"Time: {Time.time:F2}, Blue Team Score: {Mathf.RoundToInt(blueTeamScore)}, " +
                         $"Purple Team Score: {Mathf.RoundToInt(purpleTeamScore)}\n";

        try
        {
            File.AppendAllText(filePath, content);
            //Debug.Log($"Scores written to {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to write scores to the file: {ex.Message}");
        }
    }
}
