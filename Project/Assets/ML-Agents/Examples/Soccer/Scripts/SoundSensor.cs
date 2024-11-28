using Unity.MLAgents.Sensors;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.MLAgents.Sensors
{
    public class SoundSensor : Unity.MLAgents.Sensors.ISensor
    {
        private List<float> soundObservations; // List to store sound data
        private ObservationSpec observationSpec; // Observation specification
        private string sensorName; // Name of the sensor

        public SoundSensor(int observationSize, string name = "SoundSensor")
        {
            // Initialize the observation list and specification
            soundObservations = new List<float>(observationSize);
            sensorName = name;
            observationSpec = ObservationSpec.Vector(observationSize, ObservationType.Default);
        }

        /// <summary>
        /// Define the shape and type of the observations.
        /// </summary>
        public ObservationSpec GetObservationSpec()
        {
            return observationSpec;
        }
        /// <summary>
        /// Returns the current list of sound observations as an array.
        /// </summary>
        public float[] GetCurrentObservations()
        {
            return soundObservations.ToArray();
        }

        /// <summary>
        /// Collect the observation data into the ObservationWriter.
        /// </summary>
        public int Write(ObservationWriter writer)
        {
            // Ensure the number of observations matches the spec
            int expectedObservations = observationSpec.Shape[0];

            // Pad or trim the observation list as necessary
            if (soundObservations.Count < expectedObservations)
            {
                for (int i = soundObservations.Count; i < expectedObservations; i++)
                {
                    soundObservations.Add(0f); // Pad with zeros
                }
            }
            else if (soundObservations.Count > expectedObservations)
            {
                soundObservations.RemoveRange(expectedObservations, soundObservations.Count - expectedObservations);
            }

            // Write the observations to the writer
            writer.AddList(soundObservations);
            return expectedObservations; // Return the number of observations written
        }

        /// <summary>
        /// Add a single sound intensity value to the observations.
        /// </summary>
        public void AddSoundObservation(float soundIntensity)
        {
            soundObservations.Add(Mathf.Clamp(soundIntensity, 0f, 1f)); // Clamp the value between 0 and 1
        }

        /// <summary>
        /// Clear the observation list before collecting new data.
        /// </summary>
        public void Update()
        {
            soundObservations.Clear();
        }

        /// <summary>
        /// Reset the sensor, typically at the start of a new episode.
        /// </summary>
        public void Reset()
        {
            soundObservations.Clear();
        }

        /// <summary>
        /// Return the name of the sensor.
        /// </summary>
        public string GetName()
        {
            return sensorName;
        }

        /// <summary>
        /// Return compressed observations (if not using compression, return null).
        /// </summary>
        public byte[] GetCompressedObservation()
        {
            return null; // No compression applied
        }

        /// <summary>
        /// Return the compression specification (default if no compression).
        /// </summary>
        public CompressionSpec GetCompressionSpec()
        {
            return CompressionSpec.Default(); // No compression
        }
    }
}
