using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    public static void GenVFX(GameObject VFX, Vector3 pos)
    {
        GameObject newEx = Instantiate(VFX, pos, Quaternion.identity);
        Destroy(newEx, 1f);
    }

    public static void GenExplodeRocks(List<GameObject> obPrefabs, GameObject parent, Vector3 position,int rocksNum)
    {
        int seed;
        GameObject prefab, newOb;
        for (int i = 0;i < rocksNum; i++)
        {
            seed = Random.Range(0, obPrefabs.Count - 1);
            prefab = obPrefabs[seed];
            newOb = Instantiate(prefab, position, Quaternion.identity);
            newOb.transform.parent = parent.transform;

            newOb.transform.localScale = new Vector3(Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f), Random.Range(0.1f, 0.5f));
            //newOb.transform.position = position;
            Destroy(newOb, 3f);
        }
    }

    public static void GenScore(GameObject scorePrefab,Vector3 objPos,string score,GameObject parent)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(objPos);
        GameObject newScore = Instantiate(scorePrefab, objPos, Quaternion.identity);
        newScore.GetComponent<Text>().text = score;
        newScore.transform.SetParent(parent.transform);
        newScore.transform.position = new Vector3(pos.x,pos.y+130,pos.z);
        Destroy(newScore, 0.5f);
    }

}
