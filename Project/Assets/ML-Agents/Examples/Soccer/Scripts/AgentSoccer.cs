using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public enum Team
{
    Blue = 0,
    Purple = 1
}

public class AgentSoccer : Agent
{
    public enum Position
    {
        Striker,
        Goalie,
        Generic
    }

    [HideInInspector]
    public Team team;
    float m_KickPower;
    float m_BallTouch;
    public Position position;

    const float k_Power = 2000f;
    float m_Existential;
    float m_LateralSpeed;
    float m_ForwardSpeed;

    [HideInInspector]
    public Rigidbody agentRb;
    SoccerSettings m_SoccerSettings;
    BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;

    EnvironmentParameters m_ResetParams;
    private SoundSensorComponent soundSensorComponent;

    Transform ballTransform;

    private float timeSinceLastBallTouch = 0f; // Tracks time since last ball interaction
    private Vector3 lastPosition; // Tracks meaningful movement
    private int actionStepCounter = 0; // Counter for periodic checks

    private bool isRespondingToBallSound = false; 
    private float responseTimer = 0f;
    [SerializeField] private float responseDur = 1f;

    public override void Initialize()
    {
        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>();
        m_Existential = envController != null ? 1f / envController.MaxEnvironmentSteps : 1f / MaxStep;

        m_BehaviorParameters = GetComponent<BehaviorParameters>();
        team = m_BehaviorParameters.TeamId == (int)Team.Blue ? Team.Blue : Team.Purple;
        initialPos = transform.position + (team == Team.Blue ? new Vector3(-5f, 0.5f, 0) : new Vector3(5f, 0.5f, 0));
        rotSign = team == Team.Blue ? 1f : -1f;

        position = Position.Generic; // Default position, adjust as necessary
        m_LateralSpeed = position == Position.Goalie ? 1.0f : 0.3f;
        m_ForwardSpeed = position == Position.Striker ? 1.3f : 1.0f;

        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        m_ResetParams = Academy.Instance.EnvironmentParameters;

        soundSensorComponent = GetComponent<SoundSensorComponent>() ?? gameObject.AddComponent<SoundSensorComponent>();

        ballTransform = GameObject.FindWithTag("ball").transform;
        if (ballTransform == null)
        {
            Debug.LogError("Ball object not found in the scene!");
        }

        lastPosition = transform.position;
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        if (forwardAxis == 1) dirToGo = transform.forward * m_ForwardSpeed;
        else if (forwardAxis == 2) dirToGo = transform.forward * -m_ForwardSpeed;

        if (rightAxis == 1) dirToGo += transform.right * m_LateralSpeed;
        else if (rightAxis == 2) dirToGo += transform.right * -m_LateralSpeed;

        if (rotateAxis == 1) rotateDir = transform.up * -1f;
        else if (rotateAxis == 2) rotateDir = transform.up * 1f;

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
    }



   public override void OnActionReceived(ActionBuffers actionBuffers)
{
    actionStepCounter++;


    // Encourage meaningful movement every 5 steps
    if (actionStepCounter % 5 == 0)
    {
        float distanceMoved = (transform.position - lastPosition).magnitude;

        if (distanceMoved < 0.2f) // Barely moved since the last check
        {
            AddReward(-0.1f); // Reduced penalty for ineffective movement
            Debug.Log($"{name} penalized for lack of meaningful movement.");
        }

        lastPosition = transform.position;
    }

    // Gradual penalty for inactivity
    timeSinceLastBallTouch += Time.fixedDeltaTime;
    if (timeSinceLastBallTouch > 5.0f) // No interaction for 5 seconds
    {
        AddReward(-0.05f); // Smaller penalty for not touching the ball
        Debug.Log($"{name} penalized for not interacting with the ball.");
    }

    // Encourage moving toward the ball
    if (soundSensorComponent != null && soundSensorComponent.IsHearingSound())
    {
        if (IsMovingToward(ballTransform.position))
        {
            AddReward(0.2f); // Small reward for progress toward the ball
            Debug.Log($"{name} rewarded for moving toward the ball.");
        }

    }

    if (isRespondingToBallSound)
    {
        // Chase current position of the ball
        Vector3 dirToBall = (ballTransform.position - transform.position).normalized;
        float chaseSpeed = 1.0f;
        agentRb.AddForce(dirToBall * chaseSpeed, ForceMode.VelocityChange);

        // Adjust timer
        responseTimer -= Time.fixedDeltaTime;
        if (responseTimer <= 0f)
        {
            isRespondingToBallSound = false;
        }
    }
    else
    {
        MoveAgent(actionBuffers.DiscreteActions);
    }

    MoveAgent(actionBuffers.DiscreteActions);
}
   void OnCollisionEnter(Collision c)
{
    if (c.gameObject.CompareTag("ball"))
    {
        var force = k_Power * m_KickPower;
        var dir = c.contacts[0].point - transform.position;
        dir = dir.normalized;

        if (position == Position.Goalie) force = k_Power;

        c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);

        AddReward(1.5f); // Increased reward for touching the ball
        Debug.Log($"{name} rewarded for touching the ball.");

        timeSinceLastBallTouch = 0f; // Reset the no-ball-touch timer
    }
    else if (c.gameObject.CompareTag("blueAgent") || c.gameObject.CompareTag("purpleAgent"))
    {
        AgentSoccer otherAgent = c.gameObject.GetComponent<AgentSoccer>();
        if (otherAgent != null && otherAgent.team == this.team)
        {
            AddReward(0.5f); // Reward for teamwork
        }
        else if (otherAgent != null && otherAgent.team != this.team)
        {
            AddReward(-1.0f); // Increased penalty for passing to opponents
        }
    }
    else if (c.gameObject.CompareTag("wall"))
    {
        AddReward(-0.1f); // Reduced penalty for wall collisions to avoid excessive discouragement
        Debug.Log($"{name} penalized for colliding with the wall.");
    }
}


    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);
        soundSensorComponent?.GetSensor()?.Reset();

        // Reset response to sound at the start of an episode
        isRespondingToBallSound = false;
        responseTimer = 0f;
    }

    bool IsMovingToward(Vector3 targetPosition)
    {
        Vector3 toTarget = (targetPosition - transform.position).normalized;
        Vector3 agentDirection = agentRb.velocity.normalized;

        float dotProduct = Vector3.Dot(agentDirection, toTarget);
        return dotProduct > 0.5f; // Slightly stricter alignment requirement
    }
 

    void OnTriggerEnter(Collider other)
    {
        SoundObject soundObj = other.gameObject.GetComponent<SoundObject>();
        GameObject soundGameObject = soundObj != null ? soundObj.gameObject : null;
        if (soundObj != null)
        {
            if (soundObj.soundType == "BallCollision")
            {
                Debug.LogWarning($"{gameObject.name} heard a ball collision from {soundObj.originName}");

                isRespondingToBallSound = true;
                responseTimer = responseDur;
            } 
            else
            {
                Debug.LogWarning($"{gameObject.name} heard a regular collision");
            }
        }
    }


}
