using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject bullet;
    public GameObject scorePrefab, scoreCanvans;
    public float maxTurnDis = 30f; //最大重对准距离
    public float minFireDis = 20f; //最小开火距离
    public float maxFireDis = 40f; //最大开火距离
    public float minTurnDis = 10f;
    public float maxSpeed = 20f;
    public float forceScale = 5f;
    public int clipAmmo = 6;
    public GameObject explodeVFX;
    private GameObject target;
    private GameObject gControl;
    private GameObject flame;

    private Vector3 lastPosition;
    private float lastShootTimeStamp, lastTurnTimeStamp;
    private bool alreadyTurn;
    private float speed;
    private float shotMaxBias; 
    private string status;
    private float enemyShootWaitMinTime;


    void Start()
    {
        target = GameObject.Find("飞船本体");
        gControl = GameObject.Find("游戏主控");
        flame = gameObject.transform.GetChild(2).gameObject;
        transform.LookAt(target.transform.position); //出生后敌机朝向我方
        lastShootTimeStamp = Time.time;
        lastTurnTimeStamp = Time.time;
        maxSpeed = gameControl.enemyMaxSpeed;
        status = "normal";
        scoreCanvans = GameObject.Find("得分画布");
        enemyShootWaitMinTime = gameControl.enemyShootWaitMinTime;
        shotMaxBias = gameControl.shotMaxBias;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speed = Mathf.Round((Vector3.Magnitude(transform.position - lastPosition) * 10) / (Time.deltaTime));
        lastPosition = transform.position;

    }

    private void Update()
    {
        if (status == "normal")
        {
            if (speed < maxSpeed)
            {
                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * forceScale, ForceMode.Force);
            }
            float dis = Vector3.Magnitude(transform.position - target.transform.position);
            if (dis > maxTurnDis)
            {
                Turn2Position(target.transform.position);
                alreadyTurn = false;
            }
            Vector3 oriPos = new(0, 0, 0);
            if (Vector3.Magnitude(transform.position - oriPos) > 100f) //避免飞太远
            {
                Turn2Position(GetRandomPosition(oriPos));
                alreadyTurn = false;
            }


            if ((dis < minTurnDis && !alreadyTurn) || (Time.time - lastTurnTimeStamp > Random.Range(1, 3)))
            {
                RandomTurn();
                lastTurnTimeStamp = Time.time;
                alreadyTurn = true;
            }

            if (dis > minFireDis && dis < maxFireDis && (Time.time - lastShootTimeStamp) > Random.Range(enemyShootWaitMinTime, 10)) //射击
            {
                lastShootTimeStamp = Time.time;
                StartCoroutine(FireClip(clipAmmo));
            }
        }
    }

    IEnumerator FireClip(int clipAmmo)
    {
        int fireTimes = 0;
        Vector3 targetDirection = (target.transform.position - transform.position).normalized;
        Vector3 randomDirection = GetRandomPosition(target.transform.position);
        while (fireTimes < clipAmmo)
        {
            yield return new WaitForSeconds(0.2f);
            Vector3 firePos = transform.position + targetDirection * 2f;
            GameObject newOb = Instantiate(bullet, firePos, Quaternion.identity);
            newOb.transform.LookAt(randomDirection);
            fireTimes++;
        }
        lastShootTimeStamp = Time.time;
    }

    void Turn2Position(Vector3 position)
    {
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = (position - transform.position).normalized;
        Vector3 newDirection = Vector3.Lerp(currentDirection, targetDirection, Time.deltaTime * 5);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    Vector3 GetRandomPosition(Vector3 position)
    {
        Vector3 randomDirection = Random.onUnitSphere;
        float dis = Random.Range(0f, shotMaxBias);
        Vector3 pos = position + randomDirection * dis;
        return pos;
    }

    void RandomTurn()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        //float angle = Random.Range(0, 90);
        Vector3 newDirection = randomDirection;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newDirection), Time.deltaTime * 5);
        //Debug.Log(gameObject.name + " Random Turn" + newDirection.ToString());
    }

    private void OnCollisionEnter(Collision collision)
    {
        var cl = collision.gameObject; //撞击的物体
        MyFunctions.GenVFX(explodeVFX, cl.transform.position);
        if (cl.name.Contains("MyBullet") && status != "dying")
        {
            MyFunctions.GenVFX(explodeVFX, cl.transform.position);
            gControl.SendMessage("MsgHandle", "Shoot Down Enemy");
            status = "dying";
            StartCoroutine(DyingProcess());
        }
        else if (!cl.name.Contains("EnemyBullet"))
        {
            gControl.SendMessage("MsgHandle", "Enemy Collide");
            Destroy(gameObject, 0.2f);
        }
        if (!cl.name.Contains("飞船")) Destroy(cl);
    }

    IEnumerator DyingProcess()
    {
        flame.SetActive(true);
        yield return new WaitForSeconds(3f);
        MyFunctions.GenVFX(explodeVFX, transform.position);
        MyFunctions.GenScore(scorePrefab, transform.position, "+2",scoreCanvans);
        Destroy(gameObject, 0.2f);
    }
}
