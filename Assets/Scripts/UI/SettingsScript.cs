using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Manages the settings for the game, including resolution and quality settings.
    /// </summary>
    public class SettingsScript : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        private readonly List<Resolution> filteredResolutions = new List<Resolution>(); // List to keep unique resolutions

        private void Start()
        {
            PopulateResolutions();
            resolutionDropdown.value = GetCurrentResolutionIndex();
            resolutionDropdown.RefreshShownValue();
        }

        private void PopulateResolutions()
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            filteredResolutions.Clear();

            foreach (Resolution res in Screen.resolutions)
            {
                string option = res.width + "x" + res.height;
                if (!options.Contains(option))
                {
                    options.Add(option);
                    filteredResolutions.Add(res);
                }
            }

            resolutionDropdown.AddOptions(options);
        }

        private int GetCurrentResolutionIndex()
        {
            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                if (filteredResolutions[i].width == Screen.currentResolution.width &&
                    filteredResolutions[i].height == Screen.currentResolution.height)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Sets the screen resolution based on the selected index.
        /// </summary>
        /// <param name="resolutionIndex">The index of the resolution to set.</param>
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        /// <summary>
        /// Sets the quality level of the game graphics.
        /// </summary>
        /// <param name="qualityIndex">The index of the quality level to set.</param>
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }
    }

}

