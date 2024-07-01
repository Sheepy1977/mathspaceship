using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;  //子弹速度
    public float lastTime = 2f;
    private Vector3 oriPos;
    void Start()
    {
        Destroy(gameObject, lastTime);  //2s后销毁自身
        transform.Rotate(90, 0, 0);
        oriPos = transform.position;
    }

    void Update()
    {
        transform.Translate(0, Time.deltaTime * speed,0);  //子弹位移
        /*if (Vector3.Magnitude(transform.position - oriPos) > 2f)
        {
            if (!gameObject.GetComponent<CapsuleCollider>()) gameObject.AddComponent<CapsuleCollider>();
            if (!gameObject.GetComponent<Rigidbody>())
            {
                gameObject.AddComponent<Rigidbody>();
                gameObject.GetComponent<Rigidbody>().useGravity = false;
                gameObject.GetComponent<Rigidbody>().mass = 0.1f;
            }
        }*/
    }
}
 