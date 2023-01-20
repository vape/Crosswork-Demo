using UnityEngine;
using UnityEngine.SceneManagement;

namespace Crosswork.Demo.UI
{
    public class OpenSceneButton : MonoBehaviour
    {
        [SerializeField]
        private string sceneName;

        public void Open()
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
