using System;
using System.Threading;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Photoresistor_Lab
{
    public class Program
    {
        public static void Main()
        {
            var photoresistor = new AnalogInput(Pins.GPIO_PIN_A3);
            int ambientLight = 0;
            int averageAmbientLight = 0;
            float sensorVoltage = 0;

            float lightThresholdVoltage = 0.85f;
            float darkThresholdVoltage = 2.1f;

            // setup an array to hold our samples
            int numberOfSamplesToAverage = 3;
            int[] previousSamples = new int[numberOfSamplesToAverage];
            for (int i = 0; i < numberOfSamplesToAverage; i++) {
                previousSamples[i] = 0;
            }

            while (true)
            {
                // read the analog input
                ambientLight = photoresistor.Read();

                // average (oversample) the last two readings
                averageAmbientLight = AverageAndStore(ref previousSamples, ambientLight);

                // convert the digital value back to voltage
                // sensorVoltage = AnalogValueToVoltage(ambientLight);
                sensorVoltage = AnalogValueToVoltage(averageAmbientLight);

                // output
                Debug.Print("Light Level = Raw: " + ambientLight.ToString() + 
                            ", Average: " + averageAmbientLight.ToString() + 
                            ", Voltage: " + AnalogValueToVoltage(averageAmbientLight).ToString());

                if (sensorVoltage < lightThresholdVoltage) {
                    Debug.Print("Very bright.");
                } else if (sensorVoltage > darkThresholdVoltage ) {
                    Debug.Print("Dark.");
                } else {
                    Debug.Print("Moderately Bright.");
                }

                // wait 1/4 second
                Thread.Sleep(250);
            }
        }

        /// <summary>
        /// Converts an analog input value voltage.
        /// </summary>
        public static float AnalogValueToVoltage (int analogValue)
        {
            return ((float)analogValue / 1023f) * 3.3f;
        }

        /// <summary>
        /// Averages the new value in with an existing sample set of any size. Adds the new value
        /// to the sample set and returns the average.
        /// </summary>
        /// <returns>The and store.</returns>
        /// <param name="sampleSet">existing sample set.</param>
        /// <param name="newValue">New value.</param>
        public static int AverageAndStore(ref int[] sampleSet, int newValue)
        {
            int sum = 0;
            int average = 0;

            // sum up all the values
            for (int i = 0; i < sampleSet.Length; i++) {
                sum += sampleSet[i];
            }
            sum += newValue;

            // calculate the average
            average = sum / (sampleSet.Length + 1);

            // swap the values a slot
            for (int i = 0; i < sampleSet.Length - 1; i++) {
                sampleSet[i] = sampleSet[(i + 1)];
            }
            sampleSet[sampleSet.Length - 1] = newValue;

            return average;
        }
    }
}