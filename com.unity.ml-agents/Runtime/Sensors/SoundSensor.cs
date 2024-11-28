using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Unity.MLAgents.Sensors;
using System.IO;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// A sensor implementation for detecting sound spheres.
    /// </summary>
    public class SoundSensor : ISensor, IBuiltInSensor
    {
        private Transform m_AgentTransform;
        private string m_Name;
        private float m_DetectionRadius;
        private List<Vector3> m_DetectedSounds;

        private static string m_LogFilePath = Application.dataPath + "/SoundSensorLogs.txt";

        /// <summary>
        /// Initializes the sound sensor.
        /// </summary>
        /// <param name="agentTransform">The transform of the agent using the sensor.</param>
        /// <param name="name">Name of the sensor.</param>
        /// <param name="detectionRadius">The detection radius for sound spheres.</param>
        public SoundSensor(Transform agentTransform, string name, float detectionRadius)
        {
            m_AgentTransform = agentTransform;
            m_Name = name;
            m_DetectionRadius = detectionRadius;
            m_DetectedSounds = new List<Vector3>();

            // Create or clear the log file at the start
            File.WriteAllText(m_LogFilePath, "Sound Detection Log\n");
        }

        /// <inheritdoc/>
        public string GetName()
        {
            return m_Name;
        }

        /// <inheritdoc/>
        public ObservationSpec GetObservationSpec()
        {
            return ObservationSpec.Vector(1, ObservationType.Default);
        }

        /// <inheritdoc/>
        public CompressionSpec GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        /// <inheritdoc/>
        public BuiltInSensorType GetBuiltInSensorType()
        {
            return BuiltInSensorType.Unknown;
        }

        /// <inheritdoc/>
        public int Write(ObservationWriter writer)
        {
            m_DetectedSounds.Clear();
            var soundSpheres = SoundManager.Instance.GetSoundSpheres();

            foreach (var soundSphere in soundSpheres)
            {
                float distance = Vector3.Distance(m_AgentTransform.position, soundSphere.transform.position);
                if (distance <= m_DetectionRadius)
                {
                    m_DetectedSounds.Add(soundSphere.transform.position);

                    // Log to console and file
                    string logMessage = $"[{Time.time:F2}] {m_Name}: Detected sound at {soundSphere.transform.position}";
                    Debug.Log(logMessage);
                    File.AppendAllText(m_LogFilePath, logMessage + "\n");
                }
            }

            // Write the count of detected sounds to the observation
            // writer.Add(m_DetectedSounds.Count);
            return m_DetectedSounds.Count;
        }

        /// <inheritdoc/>
        public byte[] GetCompressedObservation()
        {
            return null;
        }

        /// <inheritdoc/>
        public void Update()
        {
            m_DetectedSounds.Clear();
        }

        /// <inheritdoc/>
        public void Reset()
        {
            m_DetectedSounds.Clear();
        }

        public List<Vector3> GetDetectedSounds()
{
    return m_DetectedSounds;
}
    }
}
