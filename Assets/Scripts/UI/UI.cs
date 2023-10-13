using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField]
        protected string nextSceneName;
    
        // Start is called before the first frame update
        // void Start()
        // {
        //     
        // }

        protected void LoadScene()
        {
            SceneLoadData.SceneToLoad = nextSceneName;
            SceneManager.LoadScene("Loading");
        }
    }
}
