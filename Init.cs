using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//����ͷ������
public class Init : MonoBehaviour
{
    // Start is called before the first frame update
    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        string startSceneName = "Loading";
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name.Equals(startSceneName))
        {
            return;
        }
        SceneManager.LoadScene(startSceneName);
        
    }
}