using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHp : MonoBehaviour
{
    public GameObject gameControl;
    private void Start()
    {
        gameControl = GameObject.Find("游戏主控");
    }

    private void OnCollisionEnter(Collision collision)
    {
        var cl = collision.gameObject; //撞击的物体
        if (cl.name.Contains("飞船") || cl.name.Contains("MyBullet"))
        {
            gameControl.SendMessage("MsgHandle", "HP+");
            Destroy(gameObject);
        }
    }
}
 