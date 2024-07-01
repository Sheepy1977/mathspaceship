using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SideControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject spaceShipCamera;
    public GameObject flame;
    public float scale = 1f;
    private AudioSource engineSound;
    private bool isPressed;
    private Vector3 addForce = Vector3.zero;
    void Start()
    {
        
        engineSound = flame.GetComponent<AudioSource>();
    }

    void Update()
    {
        switch (gameObject.name)
        {
            case "ÉÏ":
                addForce = spaceShipCamera.transform.up * scale;
                break;
            case "ÏÂ":
                addForce = -spaceShipCamera.transform.up * scale;
                break;
            case "×ó":
                addForce = -spaceShipCamera.transform.right * scale;
                break;
            case "ÓÒ":
                addForce = spaceShipCamera.transform.right * scale;
                break;
            case "ºóÍË":
                addForce = -spaceShipCamera.transform.forward * scale;
                break;
        }


        if (isPressed && gameControl.speed <= 80)
        {
            if (gameControl.speed <= 80)
            {
                spaceShipCamera.GetComponent<Rigidbody>().AddForce(addForce, ForceMode.Force);
                flame.SetActive(true);
                if (!engineSound.isPlaying) engineSound.Play();
            }
        }
        else
        {
            flame.SetActive(false);
            engineSound.Stop();
        }
    }                  

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    
}
