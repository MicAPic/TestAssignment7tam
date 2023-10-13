using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField]
        private string nextSceneName;
    
        // Start is called before the first frame update
        // void Start()
        // {
        //     
        // }

        // Update is called once per frame
        void Update()
        {
            // if (Keyboard.current.anyKey.isPressed)
            // {
            //     LoadScene(nextSceneName);
            // }
        }

        public void LoadScene(string sceneName)
        {
            SceneLoadData.sceneToLoad = sceneName;
            SceneManager.LoadScene("Loading");
        }
    }
}
