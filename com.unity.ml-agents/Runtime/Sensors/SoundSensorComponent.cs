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
        public bool ShowInEditMode = true;

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

        public SoundSensor GetSensor() => m_Sensor;

        public List<Vector4> GetSensorData() => m_Sensor?.GetDetectedSounds();

        // Automatically draw the detection radius
        void OnDrawGizmos()
        {
            // Draw in play mode and edit mode (if ShowInEditMode is true)
            if (ShowGizmo && AgentTransform != null && (Application.isPlaying || ShowInEditMode))
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
    }
}
