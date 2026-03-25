using UnityEngine;
using UnityEngine.SceneManagement;

namespace Models
{
    /// <summary>
    /// Provides functionality for loading scenes in the game.
    /// </summary>
    public class SettingScript : MonoBehaviour
    {
        /// <summary>
        /// Provides functionality for loading scenes in the game.
        /// </summary>
        public void loadScene()
        {
            SceneManager.LoadSceneAsync(0);
        }
    }
}