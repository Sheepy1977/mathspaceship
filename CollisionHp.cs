using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHp : MonoBehaviour
{
    public GameObject gameControl;
    private void Start()
    {
        gameControl = GameObject.Find("��Ϸ����");
    }

    private void OnCollisionEnter(Collision collision)
    {
        var cl = collision.gameObject; //ײ��������
        if (cl.name.Contains("�ɴ�") || cl.name.Contains("MyBullet"))
        {
            gameControl.SendMessage("MsgHandle", "HP+");
            Destroy(gameObject);
        }
    }
}
 