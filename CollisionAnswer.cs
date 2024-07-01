using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAnswer : MonoBehaviour
{
    public GameObject gameControl;
    public GameObject explodeVFX;
    private int wrongHitCount = 0;
    private void Start()
    {
        gameControl = GameObject.Find("��Ϸ����");
    }

    private void OnCollisionEnter(Collision collision)
    {
        var cl = collision.gameObject; //ײ��������
        Debug.Log("collisionAnswer" + cl.name);
        if (cl.name.Contains("�ɴ�") || cl.name.Contains("MyBullet"))
        {
            if (gameObject.name.Contains("��_0"))
            {
                //MyFunctions.GenVFX(explodeVFX, cl.transform.position);
                gameControl.SendMessage("MsgHandle", "Right Answer");
                Destroy(gameObject, 0.2f);
            }
            else
            {
                if (wrongHitCount == 0) gameControl.SendMessage("MsgHandle", "Wrong Answer");
                wrongHitCount++;
                MyFunctions.GenVFX(explodeVFX, cl.transform.position);
                Destroy(gameObject,0.3f);
            }
            if (cl.name.Contains("MyBullet")) Destroy(cl);
        }
        else
        {
            Destroy(cl);
            if (gameObject.name != "��_0") Destroy(gameObject);
        }
        
    }

    IEnumerator sendMsg()
    {
        yield return new WaitForSeconds(0.1f);
        gameControl.SendMessage("MsgHandle", "Right Answer");
        Destroy(gameObject);
    }
}
 