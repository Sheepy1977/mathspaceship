using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Load());

    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("SpaceShip",LoadSceneMode.Single);
    }
}
