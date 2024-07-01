using UnityEngine;
using UnityEngine.UI;
using TMPro;
/* 
* Copyright (c) [2023] [Lizhneghe.Chen https://github.com/Lizhenghe-Chen]
*/
//https://blog.csdn.net/DUYIJIU/article/details/98366918
//https://indienova.com/indie-game-development/unity-off-screen-objective-marker/
public class TargetToScreen : MonoBehaviour
{
    [Header("Assign below variables in the inspector:")]
    [SerializeField] public GameObject ship;
    public Transform TargetTransform; //目标
    [SerializeField] Vector3 TargetPositionOffset = new Vector3(0, -1, 0);
    [SerializeField] float screenBoundOffset = 0.5f;
    [SerializeField] RectTransform TargetImage, TargetImageArrow;
    //[SerializeField] TMPro.TextMeshProUGUI TargetText;
    [Header("Below is for Debug use:")]
    [SerializeField] Vector3 screenPosition, screenBound;
    [SerializeField] Vector2 Arrowdirection;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        var suffix = GetSuffix();
        var targetName = "答案_" + suffix.ToString();
        Transform cube;
        GameObject obj = GameObject.Find(targetName);
        if (!obj)
        {
            OnOffIcon(false);
            return;
        }
        else
        {
            OnOffIcon(true);
            cube = obj.transform.GetChild(0).transform;
            gameObject.transform.GetChild(0).GetComponent<Text>().text = cube.GetChild(0).GetComponent<TextMeshPro>().text;
        }
        TargetTransform = cube;
        TargetToScreenPosition();
    }

    string GetSuffix()
    {
        var suffix = gameObject.name.Substring(8, 1);
        return suffix;
    }

    void OnOffIcon(bool isOn)
    {
        gameObject.GetComponent<Image>().enabled = isOn;
        gameObject.transform.GetChild(0).GetComponent<Text>().enabled = isOn;
        gameObject.transform.GetChild(1).GetComponent<Image>().enabled = isOn;
    }

    /// <summary>
    /// This function is to convert the world position of the target to the screen position, and then update the position of the target image and the arrow image.
    /// </summary>
    public void TargetToScreenPosition()
    {
        screenPosition = Camera.main.WorldToScreenPoint(TargetTransform.position + TargetPositionOffset);// simple way to get the screen position of the target
        (screenBound.x, screenBound.y) = (Screen.width, Screen.height);//get the screen size

        if (screenPosition.z < 0)//如果目标在摄像机后面，翻转屏幕位置。
        {
            screenPosition = -screenPosition;
            if (screenPosition.y > screenBound.y / 2)//这是为了避免小概率情况，即目标位于相机背后并且屏幕位置被翻转，但是y坐标仍然在屏幕内。
            {
                screenPosition.y = screenBound.y;
            }
            else screenPosition.y = 0;
        }
        //clamp the screen position to the screen bound
        TargetImage.transform.position = new Vector2(
                    Mathf.Clamp(screenPosition.x, screenBound.x * screenBoundOffset, screenBound.x - screenBound.x * screenBoundOffset),
                    Mathf.Clamp(screenPosition.y, screenBound.y * screenBoundOffset, screenBound.y - screenBound.y * screenBoundOffset)
                                );

        if (CheckInScreen(TargetTransform.position))
        {
            if (Vector3.Magnitude(TargetTransform.position - ship.transform.position) > 15)  OnOffArrow(false); else OnOffIcon(false);
        }
        else 
        {
            OnOffIcon(true);
            OnOffArrow(true);
        }

  
        //optional, rotate the arrow to point to the target:
        Arrowdirection = screenPosition - TargetImageArrow.transform.position ;//get the direction of the arrow by subtracting the screen position of the target from the screen position of the arrow
        // Debug.Log(Arrowdirection.magnitude);
        TargetImageArrow.transform.up = Arrowdirection;
        //optional, update the distance text
        //TargetText.text = Vector3.Distance(TargetTransform.position, Camera.main.transform.position).ToString("F1") + "m";
    }
    void OnOffArrow(bool isOn)
    {
        transform.GetChild(1).gameObject.SetActive(isOn);
    }

    private bool CheckInScreen(Vector3 worldPos)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        return screenPos.x >= screenBoundOffset && 
            screenPos.x <= Screen.width - Screen.width * screenBoundOffset && 
            screenPos.y >= screenBoundOffset &&
            screenPos.y <= Screen.height - Screen.height * screenBoundOffset;
    }
}