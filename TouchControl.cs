using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject shipCamera, gameControlObj;
    public GameObject directionButton;
    private float sensetivity;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sensetivity = gameControl.sensetivity;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        bool isUpControl = gameControl.isUpControl;
        Vector2 input = Vector2.zero;
        GetDirButtonArea(out Vector2 dirButtonMinBounds, out Vector2 dirButtonMaxBounds);
        if (Input.touchCount > 0)
        {
            foreach (Touch t in Input.touches)
            {
                if (t.position.x > dirButtonMinBounds.x && t.position.x < dirButtonMaxBounds.x && t.position.y > dirButtonMinBounds.y && t.position.y < dirButtonMaxBounds.y)
                //Debug.Log(t.position.ToString());
                //if (directionButton.GetComponent<RectTransform>().rect.Contains(t.position)) 
                {
                    input = t.deltaPosition;
                    break;
                }
            }
        }
        if (input == Vector2.zero) return;
        //if (input.x > (dirButtonMaxBounds.x - dirButtonMinBounds.x)/2 || input.y > (dirButtonMaxBounds.y - dirButtonMinBounds.y)/2) return; //试图解决忽然乱转的情况
        float outputY;
        if (isUpControl) outputY = input.y; else outputY = -input.y;

        float x = transform.rotation.eulerAngles.x;
        float roX = input.x * sensetivity;
        float roY = outputY * sensetivity;
        //Debug.Log(roX.ToString() + " " + roY.ToString());
        //if (((x >= 0 && x <= 80) ||(x >= 280 && x <= 360)) && MathF.Abs(roY) < 0.5/Time.deltaTime) transform.Rotate(Vector3.left, roY, Space.Self);
        //if (MathF.Abs(roX) < 0.5/Time.deltaTime) transform.Rotate(Vector3.up, roX, Space.Self);
        transform.Rotate(Vector3.left, roY, Space.Self);
        transform.Rotate(Vector3.up, roX, Space.Self);
        transform.Rotate(Vector3.forward, -transform.rotation.eulerAngles.z, Space.Self);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        return;
    }

    void GetDirButtonArea(out Vector2 dirButtonMinBounds, out Vector2 dirButtonMaxBounds)
    {
        Vector3 center = directionButton.GetComponent<RectTransform>().position;
        float width = directionButton.GetComponent<RectTransform>().rect.width;
        float height = directionButton.GetComponent<RectTransform>().rect.height;

        float leftX = center.x - width / 2;
        float rightX = center.x + width / 2;
        float topY = center.y + height / 2;
        float bottomY = center.y - height / 2;

        dirButtonMinBounds = new Vector2(leftX, bottomY);
        dirButtonMaxBounds = new Vector2(rightX, topY);
    }
}
