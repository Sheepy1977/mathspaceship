using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class speedUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject spaceShip;
    public GameObject speedButton;
    //public GameObject GameControl;
    public GameObject speedButtonBoard;

    private float maxSpeedUpTime;
    private float progress = 0f;
    private int speedUpStatus;
    private bool visi;
    void Start()
    {
        maxSpeedUpTime = headControl.maxSpeedUpTime;
        visi = true;
    }
    void Update()
    {
        float step = 1 / (maxSpeedUpTime / Time.deltaTime);
        var image = speedButtonBoard.GetComponent<Image>();
        if (Input.GetMouseButton(0) && speedUpStatus == 1)
        {
            if (gameControl.buttonSide == "Left")
            {
                image.fillClockwise = false;
            }
            else
            {
                image.fillClockwise = true;
            }
            progress += step; // 每帧增加0.2的进度
        }
        else
        {
            progress = 0f; 
        }
        progress = Mathf.Clamp(progress, 0f, 1f); // 限制进度在0到1之间
        // 更新Image的填充量
        image.fillAmount = progress;
        
    }

    void FixedUpdate()
    {
        if (progress == 1)
        {
            if (visi)
            {
                speedButtonBoard.SetActive(false);
                visi = false;
            }
            else
            {
                speedButtonBoard.SetActive(true);
                visi = true;
            }
        }
        else
        {
            speedButtonBoard.SetActive(true);
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 position = eventData.position;
        spaceShip.SendMessage("MsgHandle", "Speed Up");
        speedUpStatus = 1;
    }
     
    public void OnPointerUp(PointerEventData eventData)
    {
        spaceShip.SendMessage("MsgHandle", "Speed Normal");
        speedUpStatus = 0;
    }
  
}
