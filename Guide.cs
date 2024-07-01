using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    public Canvas canvas;
    public List<GameObject> uiObjects;
    public List<GameObject> guideTexts;

    public GameObject guideButton;
    public GameObject guideBoard;
    public GameObject cover;
    public GameObject tips; //目标指示器
    
    public Image ImageUp;
    public Image ImageDown;
    public Image ImageLeft;
    public Image ImageRight;

    private int counter = 0;
    private bool isFinished = false;
    void Start()
    {
        Vector2 leftUp, leftDown, rightUp, rightDown;
        GetScreenBound(out leftUp,out leftDown, out rightUp,out rightDown);
        cover.SetActive(true);
        tips.SetActive(false);
       
    }

    public void Skip()
    {
        isFinished = true;
    }

    private void Update()
    {
        OffTexts();
        if (!isFinished)
        {
            guideTexts[counter].SetActive(true);
            uiObjects[counter].SetActive(true);
            
            DoGuide(uiObjects[counter]);
        }
        else
        {
            OffTexts();
            guideButton.SetActive(false);
            cover.SetActive(false);
            guideBoard.SetActive(false);
            tips.SetActive(true);
        }
        
    }

    void OffTexts()
    {
        foreach (GameObject obj in guideTexts)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in uiObjects)
        {
            obj.SetActive(false);
        }
    }

    public void NextGuide() //按钮操作
    {
        if (counter < uiObjects.Count - 1)
        {
            guideTexts[counter].SetActive(false);
            uiObjects[counter].SetActive(false);
            counter++;
        }
        else
        {
            isFinished = true;
        }
    }

    void GetScreenBound(out Vector2 leftUp,out Vector2 leftDown,out Vector2 rightUp,out Vector2 rightDown)
    {
        leftUp = new Vector2(0, 0);
        rightUp = new Vector2(canvas.pixelRect.width, 0);
        leftDown = new Vector2(0, canvas.pixelRect.height);
        rightDown = new Vector2(canvas.pixelRect.width, canvas.pixelRect.height);
    }

    void DoGuide(GameObject uiObj)
    {
        RectTransform rectTransform = uiObj.GetComponent<RectTransform>();
        Vector2 min = rectTransform.rect.min;
        Vector2 max = rectTransform.rect.max;
        Vector2 sizeDelta = rectTransform.sizeDelta;
        Vector3 scale = rectTransform.localScale;
        // 将边界转换为画布坐标
        Vector2 canvasMin = uiObj.transform.TransformPoint(min); 
        Vector2 canvasMax = uiObj.transform.TransformPoint(max);
        float width = canvasMax.x - canvasMin.x;
        float height = canvasMax.y - canvasMin.y;
        Vector2 up = new(canvasMin.x + width/2, canvasMax.y);
        Vector2 down = new(canvasMin.x + width / 2, canvasMin.y);
        Vector2 right = new(canvasMax.x , canvasMin.y + height / 2);
        Vector2 left = new(canvasMin.x, canvasMin.y + height / 2);

        SetCoverPosAndSize(ImageUp, sizeDelta, up, scale, "up");
        SetCoverPosAndSize(ImageDown, sizeDelta, down, scale ,"down");
        SetCoverPosAndSize(ImageLeft, sizeDelta, left, scale, "left");
        SetCoverPosAndSize(ImageRight, sizeDelta, right, scale,"right");
    }

    void SetCoverPosAndSize(Image image, Vector2 sizeDelta ,Vector2 position, Vector3 scale,string mode)
    {
        image.GetComponent<RectTransform>().localScale = scale;
        image.transform.position = position;
        if (mode == "up" || mode=="down") image.GetComponent<RectTransform>().sizeDelta = new (4000,4000);
        if (mode == "left" || mode == "right") image.GetComponent<RectTransform>().sizeDelta = new(4000, sizeDelta.y);
    }
}
