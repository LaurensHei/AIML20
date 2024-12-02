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
    private Unity.MLAgents.Sensors.SoundSensorComponent soundSensorComponent;


    Transform ballTransform;
    public override void Initialize()
    {
        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>();
        if (envController != null)
        {
            m_Existential = 1f / envController.MaxEnvironmentSteps;
        }
        else
        {
            m_Existential = 1f / MaxStep;
        }

        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        if (m_BehaviorParameters.TeamId == (int)Team.Blue)
        {
            team = Team.Blue;
            initialPos = new Vector3(transform.position.x - 5f, .5f, transform.position.z);
            rotSign = 1f;
        }
        else
        {
            team = Team.Purple;
            initialPos = new Vector3(transform.position.x + 5f, .5f, transform.position.z);
            rotSign = -1f;
        }

        if (position == Position.Goalie)
        {
            m_LateralSpeed = 1.0f;
            m_ForwardSpeed = 1.0f;
        }
        else if (position == Position.Striker)
        {
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.3f;
        }
        else
        {
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.0f;
        }

        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        m_ResetParams = Academy.Instance.EnvironmentParameters;

        // Dynamically add or retrieve SoundSensorComponent
        soundSensorComponent = gameObject.GetComponent<SoundSensorComponent>();
        if (soundSensorComponent == null)
        {
            soundSensorComponent = gameObject.AddComponent<SoundSensorComponent>();
            Debug.Log("___Sensor component initilized____");
        }

        ballTransform = GameObject.FindWithTag("ball").transform;
        if (ballTransform == null)
        {
            Debug.LogError("Ball object not found in the scene!");
        }
    }

    IList<float> Vector4ToIList(Vector4 vector)
    {
        return new List<float> { vector.x, vector.y, vector.z, vector.w }; // Use lowercase 'x', 'y', 'z', 'w'
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                m_KickPower = 1f;
                break;
            case 2:
                dirToGo = transform.forward * -m_ForwardSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * m_LateralSpeed;
                break;
            case 2:
                dirToGo = transform.right * -m_LateralSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (position == Position.Goalie)
        {
            AddReward(m_Existential);
        }
        else if (position == Position.Striker)
        {
            AddReward(-m_Existential);
        }
        MoveAgent(actionBuffers.DiscreteActions);

        if (soundSensorComponent != null && soundSensorComponent.IsHearingSound())
        {
            if (IsMovingToward(ballTransform.position)) // Assuming ballTransform is set up
            {
                AddReward(1f); // Reward for moving toward the ball
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W)) discreteActionsOut[0] = 1;
        if (Input.GetKey(KeyCode.S)) discreteActionsOut[0] = 2;
        if (Input.GetKey(KeyCode.A)) discreteActionsOut[2] = 1;
        if (Input.GetKey(KeyCode.D)) discreteActionsOut[2] = 2;
        if (Input.GetKey(KeyCode.E)) discreteActionsOut[1] = 1;
        if (Input.GetKey(KeyCode.Q)) discreteActionsOut[1] = 2;
    }

    void OnCollisionEnter(Collision c)
    {
        var force = k_Power * m_KickPower;
        if (position == Position.Goalie) force = k_Power;
        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(.2f * m_BallTouch);
            var dir = c.contacts[0].point - transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }



    }

    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);

        if (soundSensorComponent != null)
        {
            var soundSensor = soundSensorComponent.GetSensor();
            soundSensor?.Reset(); // Reset the sensor if it exists
        }
    }


    bool IsMovingToward(Vector3 targetPosition)
    {
        Vector3 toTarget = (targetPosition - transform.position).normalized;
        Vector3 agentDirection = agentRb.velocity.normalized;

        float dotProduct = Vector3.Dot(agentDirection, toTarget);
        return dotProduct > 0.4f; // Adjust threshold for alignment as needed
    }

}
