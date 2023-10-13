using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Debug.Log(SceneLoadData.sceneToLoad);
        var asyncLoad = SceneManager.LoadSceneAsync(SceneLoadData.sceneToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
