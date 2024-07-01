using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionShip : MonoBehaviour
{
    public GameObject gameControl;
    public GameObject explodeVFX;
    public int maxHit;
    private int hitCounter;
    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        var cl = collision.gameObject; //ײ��������
        if (cl.name.Contains("Ѫ��") || cl.name.Contains("��") || cl.name.Contains("MyBullet")) return;
        MyFunctions.GenVFX(explodeVFX, cl.transform.position);
        Destroy(cl);
        if (cl.name.Contains("EnemyBullet")) //
        {
            hitCounter++;
            if (hitCounter > maxHit)
            {
                gameControl.SendMessage("MsgHandle", "Collide");
                hitCounter = 0;
            }
        }
        else
        {
            gameControl.SendMessage("MsgHandle", "Collide");
        }
    }
}
