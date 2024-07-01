using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class slowDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject snakeHead;
    //public Button thisButton;
    void Start()
    {

    }
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        snakeHead.SendMessage("MsgHandle", "Speed Slow");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        snakeHead.SendMessage("MsgHandle", "Speed Normal");
    }

}
