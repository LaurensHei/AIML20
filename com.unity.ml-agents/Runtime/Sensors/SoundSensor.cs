using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using System.IO;

namespace Unity.MLAgents.Sensors
{
    public class SoundSensor : ISensor, IBuiltInSensor
    {
        private Transform m_AgentTransform;
        private string m_Name;
        private float m_DetectionRadius;
        private List<Vector3> m_DetectedSounds;

        private static string m_LogFilePath = Application.dataPath + "/SoundSensorLogs.txt";

        public SoundSensor(Transform agentTransform, string name, float detectionRadius)
        {
            m_AgentTransform = agentTransform;
            m_Name = name;
            m_DetectionRadius = detectionRadius;
            m_DetectedSounds = new List<Vector3>();
            File.WriteAllText(m_LogFilePath, "Sound Detection Log\n");
        }

        public string GetName()
        {
            return m_Name;
        }

        public ObservationSpec GetObservationSpec()
        {
            // Returning space for 3-dimensional data (Vector3)
            return ObservationSpec.Vector(3, ObservationType.Default);
        }

        public CompressionSpec GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        public BuiltInSensorType GetBuiltInSensorType()
        {
            return BuiltInSensorType.Unknown;
        }

        public int Write(ObservationWriter writer)
        {
            m_DetectedSounds.Clear();
            var soundSpheres = SoundManager.Instance.GetSoundSpheres();

            // Ensure the agent's transform is valid
            if (m_AgentTransform == null)
            {
                Debug.LogError("AgentTransform is null. The SoundSensor is not properly configured or the agent was destroyed.");
                return 0;
            }
            if (soundSpheres == null)
            {
                Debug.LogWarning("SoundManager or SoundSpheres is null.");
                return 0;
            }

            float counter = 0;
            foreach (var soundSphere in soundSpheres)
            {
                float distance = Vector3.Distance(m_AgentTransform.position, soundSphere.transform.position);
                if (distance <= m_DetectionRadius)
                {

                    m_DetectedSounds.Add(soundSphere.transform.position);
                    writer.Add(soundSphere.transform.position);
                    counter++;
                    string logMessage = $"[{Time.time:F2}] {m_Name}: Detected sound at {soundSphere.transform.position}";
                    Debug.Log(logMessage);
                    File.AppendAllText(m_LogFilePath, logMessage + "\n");

                }
            }

            // Return the total number of elements written
            // writer.Add(counter);
            return m_DetectedSounds.Count * 4;
        }


        public byte[] GetCompressedObservation()
        {
            return null;
        }

        public void Update()
        {
            m_DetectedSounds.Clear();
        }

        public void Reset()
        {
            m_DetectedSounds.Clear();
        }

        /// <summary>
        /// Retrieves the detected sound positions as a list of Vector3.
        /// </summary>
        /// <returns>A list of detected sound positions.</returns>
        public List<Vector3> GetDetectedSounds()
        {
            return new List<Vector3>(m_DetectedSounds);
        }
    }
}
