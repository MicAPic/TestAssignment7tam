using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField]
        protected string nextSceneName;

        public virtual void LoadScene()
        {
            SceneLoadData.SceneToLoad = nextSceneName;
            SceneManager.LoadScene("Loading");
        }
    }
}
