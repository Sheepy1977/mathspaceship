using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionOb : MonoBehaviour
{
    public GameObject explodeVFX, scorePrefab, scoreCanvans;

    private GameObject gcontrol;
    
    private GameObject tempObParent;
    private List<GameObject> obPrefabs;
    private int hitCount;
    private int rockHitTimes;
    private bool hasBeenHit;
    private GameObject lastCl;
    private void Start()
    {
        gcontrol = GameObject.Find("游戏主控");
        gameControl comp = gcontrol.GetComponent<gameControl>();
        obPrefabs = comp.obPrefabs;
        tempObParent = comp.tempObParent;
        hitCount = 0;
        rockHitTimes = gameControl.rockHitTimes;
        scoreCanvans = GameObject.Find("得分画布");
    }

    private void Update()
    {
        if (Vector3.Magnitude(transform.position - new Vector3(0,0,0)) > 20f) Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        var cl = collision.gameObject; //撞击的物体
        if (lastCl != cl && !cl.name.Contains("Rock"))//自身和自身碰撞不做碰撞特效
        {
            MyFunctions.GenVFX(explodeVFX, collision.transform.position);
            lastCl = cl;
        }
        if (cl.name.Contains("Bullet"))
        {
            if (hitCount >=rockHitTimes && !hasBeenHit)
            {
                MyFunctions.GenExplodeRocks(obPrefabs, tempObParent, transform.position, 2);
                if (gameObject.name.Contains("障碍物")) gcontrol.SendMessage("MsgHandle", "Hit Rock"); //炸碎出来的岩石不计分，但是可以继续打碎。
                gameObject.name = "被击毁";
                gameObject.transform.parent = null;
                MyFunctions.GenScore(scorePrefab, transform.position, "+1",scoreCanvans);
                hasBeenHit = true;
                Destroy(gameObject,0.2f);
            }
            else
            {
                hitCount++;
            }
            Destroy(cl);
        }
    }
}
