using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// A SensorComponent that creates a <see cref="SoundSensor"/>.
    /// </summary>
    [AddComponentMenu("ML Agents/Sound Sensor")]   
    public class SoundSensorComponent : SensorComponent
    {
        /// <summary>
        /// Name of the generated <see cref="SoundSensor"/> object.
        /// Note that changing this at runtime does not affect how the Agent sorts the sensors.
        /// </summary>
        public string SensorName
        {
            get { return m_SensorName; }
            set { m_SensorName = value; }
        }
        [HideInInspector, SerializeField]
        private string m_SensorName = "SoundSensor";

        /// <summary>
        /// The number of float observations in the SoundSensor
        /// </summary>
        public int ObservationSize
        {
            get { return m_ObservationSize; }
            set { m_ObservationSize = value; }
        }

        [HideInInspector, SerializeField]
        private int m_ObservationSize = 10;

        [HideInInspector, SerializeField]
        private ObservationType m_ObservationType = ObservationType.Default;

        private SoundSensor m_Sensor;

        /// <summary>
        /// The type of the observation.
        /// </summary>
        public ObservationType ObservationType
        {
            get { return m_ObservationType; }
            set { m_ObservationType = value; }
        }

        [HideInInspector, SerializeField]
        [Range(1, 50)]
        [Tooltip("Number of frames of sound observations that will be stacked before being fed to the neural network. Using 1 means no stacking.")]
        private int m_ObservationStacks = 1;

        /// <summary>
        /// Whether to stack previous observations. Using 1 means no previous observations.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public int ObservationStacks
        {
            get { return m_ObservationStacks; }
            set { m_ObservationStacks = value; }
        }

        /// <summary>
        /// Creates a SoundSensor.
        /// </summary>
        /// <returns></returns>
        public override ISensor[] CreateSensors()
        {
            // Instantiate the SoundSensor
            m_Sensor = new SoundSensor(m_ObservationSize, m_SensorName);
            if (ObservationStacks != 1)
            {
                // Use a stacking sensor if observation stacking is required
                return new ISensor[] { new StackingSensor(m_Sensor, ObservationStacks) };
            }
            return new ISensor[] { m_Sensor };
        }

        /// <summary>
        /// Returns the underlying SoundSensor
        /// </summary>
        /// <returns></returns>
        public SoundSensor GetSensor()
        {
            return m_Sensor;
        }
    
     /// <summary>
        /// Gets the latest sound observation data.
        /// </summary>
        /// <returns>Array of float values representing the sound observations.</returns>
        public float[] GetSensorData()
        {
            if (m_Sensor == null)
            {
                Debug.LogWarning("SoundSensor is not initialized.");
                return new float[m_ObservationSize];
            }

            // Retrieve the observations from the SoundSensor
            return m_Sensor.GetCurrentObservations();
        }
}
}