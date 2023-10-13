using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        PlayerNetworkController.OnPlayerLoaded += DeactivateAnimation;
    }

    void OnDisable()
    {
        PlayerNetworkController.OnPlayerLoaded -= DeactivateAnimation;
    }
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        var asyncLoad = SceneManager.LoadSceneAsync(SceneLoadData.SceneToLoad);
        while (!asyncLoad.isDone && !SceneLoadData.CanLoadScene)
        {
            yield return null;
        }
    }

    void DeactivateAnimation()
    {
        Destroy(gameObject);
    }
}
