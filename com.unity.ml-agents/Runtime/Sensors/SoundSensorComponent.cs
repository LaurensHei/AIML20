using UnityEngine;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

namespace Unity.MLAgents.Sensors
{
    [AddComponentMenu("ML Agents/Sound Sensor", (int)MenuGroup.Sensors)]
    public class SoundSensorComponent : SensorComponent
    {
        [Tooltip("The transform of the agent using the sensor.")]
        public Transform AgentTransform;

        [Tooltip("Name of the sensor.")]
        public string SensorName = "SoundSensor";

        [Tooltip("The detection radius for sound spheres.")]
        [Range(1f, 100f)]
        public float DetectionRadius = 50f;

        private SoundSensor m_Sensor;

        public override ISensor[] CreateSensors()
        {
            if (AgentTransform == null)
            {
                Debug.LogError("Agent Transform is not assigned to SoundSensorComponent.");
                return null;
            }

            m_Sensor = new SoundSensor(AgentTransform, SensorName, DetectionRadius);
            return new ISensor[] { m_Sensor };
        }

        public SoundSensor GetSensor()
        {
            return m_Sensor;
        }

        // Add this method to retrieve sound data
        public float[] GetSensorData()
        {
            if (m_Sensor == null)
            {
                Debug.LogError("SoundSensor is not initialized.");
                return new float[0];
            }

            // Retrieve detected sound positions
            List<Vector3> detectedSounds = m_Sensor.GetDetectedSounds();
            float[] soundData = new float[detectedSounds.Count];

            for (int i = 0; i < detectedSounds.Count; i++)
            {
                // Convert detected sound positions into distances from the agent
                soundData[i] = Vector3.Distance(AgentTransform.position, detectedSounds[i]);
            }

            return soundData;
        }
    }
}
