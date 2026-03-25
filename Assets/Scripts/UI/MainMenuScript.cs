using Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI
{
    /// <summary>
    /// Manages the main menu interactions, including navigating to settings, quitting the application
    /// and starting different simulation modes
    /// </summary>
    public class MainMenuScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainMenuPanel;

        [SerializeField]
        private GameObject settingsPanel;

        /// <summary>
        /// Quits the application
        /// </summary>
        public void OnQuitButton()
        {
            Application.Quit();
        }

        /// <summary>
        /// Starts the simulation in the explorer mode
        /// </summary>
        public static void OnExplorerButton()
        {
            SimulationModeState.currentSimulationMode = SimulationModeState.SimulationMode.Explorer;
            SceneManager.LoadSceneAsync((int)ScenesIndexes.Explorer);
        }

        /// <summary>
        /// Starts the simulation in the sandbox mode
        /// </summary>
        public static void OnSandboxButton()
        {
            SimulationModeState.currentSimulationMode = SimulationModeState.SimulationMode.Sandbox;
            SceneManager.LoadSceneAsync((int)ScenesIndexes.Sandbox);
        }

        /// <summary>
        /// Activates the settings panel while hiding the main menu panel
        /// </summary>
        public void OpenSettingsPanel()
        {
            settingsPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }

        /// <summary>
        /// Activates the main menu panel while hiding the settings panel
        /// </summary>
        public void CloseSettingsPanel()
        {
            mainMenuPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }
    }
}
