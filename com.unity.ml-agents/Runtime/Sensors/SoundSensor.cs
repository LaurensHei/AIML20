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
        private List<Vector4> m_DetectedSounds;

        private static string m_LogFilePath = Application.dataPath + "/SoundSensorLogs.txt";

        public SoundSensor(Transform agentTransform, string name, float detectionRadius)
        {
            m_AgentTransform = agentTransform;
            m_Name = name;
            m_DetectionRadius = detectionRadius;
            m_DetectedSounds = new List<Vector4>();
            File.WriteAllText(m_LogFilePath, "Sound Detection Log\n");
        }

        public string GetName()
        {
            return m_Name;
        }

        public ObservationSpec GetObservationSpec()
        {
            // Returning space for 3-dimensional data (Vector3)
            return ObservationSpec.Vector(4, ObservationType.Default);
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


            foreach (var soundSphere in soundSpheres)
            {
                Vector3 posSound = soundSphere.transform.position;
                float distance = Vector3.Distance(m_AgentTransform.position, posSound);
                if (distance <= m_DetectionRadius)
                {

                    float x = posSound.x;
                    float y = posSound.y;
                    float z = posSound.z;
                    float id = GetId(soundSphere.name);
                    Vector4 infoVector = new Vector4(x,y,z,id);
                    m_DetectedSounds.Add(infoVector);
                    writer.Add(infoVector);
                }
            }

            // Return the total number of elements written
            
            return m_DetectedSounds.Count;
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
        public List<Vector4> GetDetectedSounds()
        {
            return new List<Vector4>(m_DetectedSounds);
        }


        Dictionary<string, float> cachedInput = new Dictionary<string, float>();
        private float id = 100;

        float GetId(string input)
        {
            int start = input.IndexOf('_') + 1;
            int end = input.LastIndexOf('_');
            string objectsName = input.Substring(start, end - start);

            if (!cachedInput.ContainsKey(objectsName))
            {
                id++;
                cachedInput.Add(objectsName, id);
                return id;
            }
            return cachedInput[objectsName];

        }






    }
}
