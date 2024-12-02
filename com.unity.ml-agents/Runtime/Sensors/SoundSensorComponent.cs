using UnityEngine;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

namespace Unity.MLAgents.Sensors
{
    [AddComponentMenu("ML Agents/Sound Sensor Component", (int)MenuGroup.Sensors)]
    public class SoundSensorComponent : SensorComponent
    {
        public Transform AgentTransform;
        public string SensorName = "SoundSensor";
        [Range(1f, 100f)] public float DetectionRadius = 10f;

        [Header("Visualization")]
        public bool ShowGizmo = true;
        public Color GizmoColor = Color.green;
        public bool ShowInEditMode = false;

        private SoundSensor m_Sensor;
        private SoundManager soundManager;
        public override ISensor[] CreateSensors()
        {
            if (AgentTransform == null)
            {
                Debug.LogError("Agent Transform is not assigned to SoundSensorComponent.");
                return null;
            }
            soundManager = GetComponentInParent<SoundManager>();

            if (soundManager == null)
            {
                Debug.Log("manager is null here in Component");
            }

            m_Sensor = new SoundSensor(AgentTransform, SensorName, DetectionRadius, soundManager);
            return new ISensor[] { m_Sensor };
        }

        public SoundSensor GetSensor() => m_Sensor;

        public List<Vector4> GetSensorData() => m_Sensor?.GetDetectedSounds();

        // Automatically draw the detection radius
        void OnDrawGizmos()
        {
            if (ShowGizmo && AgentTransform != null)
            {
                Gizmos.color = GizmoColor;
                Gizmos.DrawWireSphere(AgentTransform.position, DetectionRadius);
            }
        }

        // Ensure a default AgentTransform is set if missing
        void Reset()
        {
            if (AgentTransform == null)
            {
                AgentTransform = transform; // Default to the component's GameObject transform
            }
        }

        public bool IsHearingSound()
        {
            // Assume GetSensorData returns a list of sounds with tags or identifiers
            List<Vector4> sensorData = GetSensorData();
            foreach (var sound in sensorData)
            {
                if (sound.z < 0) 
                {
                    return true;
                }
            }
            return false;
        }
    }
}
