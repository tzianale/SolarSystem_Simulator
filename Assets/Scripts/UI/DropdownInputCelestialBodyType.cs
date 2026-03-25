using System;
using System.Collections.Generic;
using Models;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Handles the population of a dropdown menu with celestial body types.
    /// </summary>
    public class DropdownInputCelestialBodyType : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;

        private void Start()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            if (_dropdown) PopulateList();
        }

        private void PopulateList()
        {
            string[] celestialBodiesTypes = Enum.GetNames(typeof(CelestialBodyType));
            _dropdown.AddOptions(new List<String>(celestialBodiesTypes));
        }
    }
}