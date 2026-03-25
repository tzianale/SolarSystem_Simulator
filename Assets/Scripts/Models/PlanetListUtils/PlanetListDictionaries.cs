using UnityEngine;
using Utils;

namespace Models.PlanetListUtils
{
    using System;
    using UnityEngine.Events;
    using System.Collections.Generic;
    
    /// <summary>
    /// Contains the various Dictionaries that will be used in the Planet Info tab
    /// </summary>
    public static class PlanetListDictionaries
    {
        private const string FloatStringFormat = "N2";
        private const float NoMass = 0f;
        
        /// <summary>
        /// Creates a dictionary of observable fields that allow the user to modify the simulation parameters of a given planet
        /// </summary>
        /// 
        /// <param name="currentPlanetModel">The planet object to be affected by the user's modifications</param>
        /// 
        /// <returns>
        /// A dictionary where the keys are property names and the values are containers holding:
        /// <list type="bullet">
        /// <item>
        /// <description>A function to get the current value of the property as a string</description>
        /// </item>
        /// <item>
        /// <description>An action to update the property based on user input</description>
        /// </item>
        /// </list>
        /// </returns>
        /// 
        /// <remarks>
        /// The dictionary includes:
        /// <list type="bullet">
        /// <item>
        /// <description><c>Planet Mass</c>: Allows viewing and updating the planet's mass, displayed in Earth masses</description>
        /// </item>
        /// <item>
        /// <description><c>Planet X-Position</c>: Allows viewing and updating the planet's X-coordinate</description>
        /// </item>
        /// <item>
        /// <description><c>Planet Y-Position</c>: Allows viewing and updating the planet's Y-coordinate</description>
        /// </item>
        /// <item>
        /// <description><c>Planet Z-Position</c>: Allows viewing and updating the planet's Z-coordinate</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static Dictionary<string, TwoObjectContainer<Func<string>, UnityAction<string>>> 
            GetVariablePropertiesDictionary(GameObject currentPlanetModel)
        {
            return new Dictionary<string, TwoObjectContainer<Func<string>, UnityAction<string>>>
            {
                {
                    "Planet Mass",
                    new TwoObjectContainer<Func<string>, UnityAction<string>>(
                        () => currentPlanetModel.GetComponent<CelestialBody>().GetMass()
                            .ToString(FloatStringFormat) + "_Earth masses",
                            
                        updatedData =>
                        {
                            var updatedMass = float.Parse(updatedData);

                            if (updatedMass > NoMass)
                            {
                                currentPlanetModel.GetComponent<CelestialBody>().SetMass(updatedMass);
                            }
                        })
                },
                {
                    "Planet X-Position", 
                    new TwoObjectContainer<Func<string>, UnityAction<string>>(
                        () => currentPlanetModel.transform.position.x
                            .ToString(FloatStringFormat),
                        updatedData =>
                        { 
                            var updatedX = float.Parse(updatedData);

                            var currentPlanetPosition = currentPlanetModel.transform.position;
                            var currentPlanetRotation = currentPlanetModel.transform.rotation;

                            currentPlanetPosition.x = updatedX;

                            currentPlanetModel.transform.SetPositionAndRotation(currentPlanetPosition, currentPlanetRotation);
                        })
                },
                {
                    "Planet Y-Position", 
                    new TwoObjectContainer<Func<string>, UnityAction<string>>(
                        () => currentPlanetModel.transform.position.y
                            .ToString(FloatStringFormat),
                        updatedData =>
                        { 
                            var updatedY = float.Parse(updatedData);

                            var currentPlanetPosition = currentPlanetModel.transform.position;
                            var currentPlanetRotation = currentPlanetModel.transform.rotation;

                            currentPlanetPosition.y = updatedY;

                            currentPlanetModel.transform.SetPositionAndRotation(currentPlanetPosition, currentPlanetRotation);
                        })
                },
                {
                    "Planet Z-Position", 
                    new TwoObjectContainer<Func<string>, UnityAction<string>>(
                        () => currentPlanetModel.transform.position.z
                            .ToString(FloatStringFormat),
                        updatedData =>
                        { 
                            var updatedZ = float.Parse(updatedData);

                            var currentPlanetPosition = currentPlanetModel.transform.position;
                            var currentPlanetRotation = currentPlanetModel.transform.rotation;

                            currentPlanetPosition.z = updatedZ;

                            currentPlanetModel.transform.SetPositionAndRotation(currentPlanetPosition, currentPlanetRotation);
                        })
                }
            };
        }

        
        /// <summary>
        /// Creates a dictionary of fields that will display specific properties of a planet in real-time, 
        /// continuously updating the values
        /// </summary>
        /// 
        /// <param name="currentPlanetModel">The planet object whose properties are to be displayed.</param>
        /// <param name="star">The star object around which the current planet is orbiting</param>
        /// 
        /// <returns>
        /// A dictionary where the keys are property names and the values are functions returning the 
        /// current state of those properties as strings
        /// </returns>
        /// 
        /// <remarks>
        /// The dictionary includes:
        /// <list type="bullet">
        /// <item>
        /// <description><c>Current Speed</c>: The current speed of the planet, retrieved from its <c>velocity.magnitude</c></description>
        /// </item>
        /// <item>
        /// <description><c>Distance to Sun</c>: The current distance from the planet to its star, calculated from the positions of both</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static Dictionary<string, Func<string>> GetLiveStatsDictionary(GameObject currentPlanetModel, GameObject star)
        {
            return new Dictionary<string, Func<string>>
            {
                {"Current Speed", () => currentPlanetModel.GetComponent<CelestialBody>().GetVelocity().magnitude.ToString(FloatStringFormat)},
                {"Distance to " + star.name , () => (currentPlanetModel.transform.position - star.transform.position).magnitude.ToString(FloatStringFormat)}
            };
        }
    }
}