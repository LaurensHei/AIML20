using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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


    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    /// <summary>
    /// The area bounds.
    /// </summary>

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>

    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 m_BallStartingPos;

    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SoccerSettings m_SoccerSettings;


    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    private int m_ResetTimer;

    public int blueTeamScore = 0; // performance count blue team
    public int purpleTeamScore = 0; // performance count purple team
    public string lastTeamBall;

    public TMP_Text scoreText;
    private float timeLastWrite = 0f;
    public float writeInterval = 10f;

    private string directoryPath;
    private string filePath = "Assets/ML-Agents/Examples/Soccer/Prefabs/scores.txt";

    void Start()
    {


        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_PurpleAgentGroup.RegisterAgent(item.Agent);
            }
        }
        scoreText.text = "Blue team: 0 - Purple team: 0";

        string directoryPath = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log($"Directory created at {directoryPath}");
        }

        try
        {
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to clear the scores file: {ex.Message}");
        }
        ResetScene();
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        timeLastWrite += Time.fixedDeltaTime; 

        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_PurpleAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
        // Check if it's time ot write the scores to the file.
        if (timeLastWrite >= writeInterval)
        {
            WriteScoresTofile();
            timeLastWrite = 0f; // Reset timer
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

    public void GoalTouched(Team scoredTeam)
    {
        if (scoredTeam == Team.Blue)
        {
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_PurpleAgentGroup.AddGroupReward(-1);
            blueTeamScore += 3;
        }
        if (scoredTeam == Team.Purple)
        {
            m_PurpleAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
            purpleTeamScore += 3;
        }
        m_PurpleAgentGroup.EndGroupEpisode();
        m_BlueAgentGroup.EndGroupEpisode();
        ResetScene();

    }

    public void purplePlayerTouched(Team teamAtBall)
    {
        if (lastTeamBall == "purple")
        {
            purpleTeamScore += 1;
        }
        if (lastTeamBall == "blue") 
        {
            blueTeamScore -=0;
            lastTeamBall = "purple";
        }
        else {
            Debug.LogWarning("no team at ball");
            lastTeamBall = "purple";
        }
    }

    public void bluePlayerTouched(Team teamAtBall)
    {
        if (lastTeamBall == "blue")
        {
            blueTeamScore += 1;
        }
        if (lastTeamBall == "purple")
        {
            purpleTeamScore -= 0;
            lastTeamBall = "blue";
        }
        else {
            Debug.LogWarning("no team at ball");
            lastTeamBall = "blue";
        }
    }


    public void ResetScene()
    {
        m_ResetTimer = 0;

        //Reset Agents
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

        //Reset Ball
        ResetBall();
        DisplayScores();
    }


    public void DisplayScores() 
    {
        if (scoreText != null)
        {
            scoreText.text = $"Blue Team: {blueTeamScore} \nPurple Team: {purpleTeamScore}";
        }
        else
        {
            Debug.LogWarning("scoreText is null");
        }
        
    }

    private void WriteScoresTofile()
    {
        string content = $"Time: {Time.time:F2}, Blue Team Score: {blueTeamScore}, Purple Team Score {purpleTeamScore}\n";

        try
        {   
            File.AppendAllText(filePath, content);
            Debug.Log($"Scores written to {filePath}");
        } catch (System.Exception ex) 
        {
            Debug.LogError($"Failed to write scores to the file: {ex.Message}");
        }
    }

}
