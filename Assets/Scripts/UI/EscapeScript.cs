using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI
{
    /// <summary>
    /// Offers useful public methods to buttons.
    /// When the buttons are clicked, a previously given GameObject will be hidden or activated.
    /// There is also a third case, where, if the specific method is called, the game will switch to the Main Menu
    /// </summary>
    public class EscapeScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        public GameObject Panel { private get; set; }

        private void Start()
        {
            Panel = panel;
        }

        /// <summary>
        /// Sets the panel to active, allowing for it to be seen in the scene
        /// </summary>
        public void OpenPanel()
        {
            if (Panel != null)
            {
                Panel.SetActive(true);
            }
        }

        /// <summary>
        /// Sets the panel to inactive, effectively hiding it
        /// </summary>
        public void ClosePanel()
        {
            if (Panel != null)
            {
                Panel.SetActive(false);
            }
        }

        /// <summary>
        /// Loads the Main Menu scene
        /// </summary>
        public void EscapeGame()
        {
            SceneManager.LoadSceneAsync((int)ScenesIndexes.MainMenu);
        }
    }
}
