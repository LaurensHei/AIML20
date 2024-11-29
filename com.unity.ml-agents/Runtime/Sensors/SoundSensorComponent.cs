using UnityEngine;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

namespace Unity.MLAgents.Sensors
{
    [AddComponentMenu("ML Agents/Sound Sensor", (int)MenuGroup.Sensors)]
    public class SoundSensorComponent : SensorComponent
    {
        public Transform AgentTransform;
        public string SensorName = "SoundSensor";
        [Range(1f, 100f)] public float DetectionRadius = 50f;

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

        /// <summary>
        /// Retrieves the detected sound positions as a list of Vector3.
        /// </summary>
        /// <returns>A list of detected sound positions.</returns>
        public List<Vector4> GetSensorData()
        {
            return m_Sensor?.GetDetectedSounds();
        }
    }
}
