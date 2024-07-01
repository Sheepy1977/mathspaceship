using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseNewFeature : MonoBehaviour
{
    public void Close()
    {
        Destroy(gameObject);
        PlayerPrefs.SetString("V2_0feature", true.ToString());
        PlayerPrefs.Save();
    }
}
