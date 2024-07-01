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
        gcontrol = GameObject.Find("��Ϸ����");
        gameControl comp = gcontrol.GetComponent<gameControl>();
        obPrefabs = comp.obPrefabs;
        tempObParent = comp.tempObParent;
        hitCount = 0;
        rockHitTimes = gameControl.rockHitTimes;
        scoreCanvans = GameObject.Find("�÷ֻ���");
    }

    private void Update()
    {
        if (Vector3.Magnitude(transform.position - new Vector3(0,0,0)) > 20f) Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        var cl = collision.gameObject; //ײ��������
        if (lastCl != cl && !cl.name.Contains("Rock"))//�����������ײ������ײ��Ч
        {
            MyFunctions.GenVFX(explodeVFX, collision.transform.position);
            lastCl = cl;
        }
        if (cl.name.Contains("Bullet"))
        {
            if (hitCount >=rockHitTimes && !hasBeenHit)
            {
                MyFunctions.GenExplodeRocks(obPrefabs, tempObParent, transform.position, 2);
                if (gameObject.name.Contains("�ϰ���")) gcontrol.SendMessage("MsgHandle", "Hit Rock"); //ը���������ʯ���Ʒ֣����ǿ��Լ������顣
                gameObject.name = "������";
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
