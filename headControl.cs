using System;
using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
//using UnityEngine.Windows;
using Image = UnityEngine.UI.Image;
using Input = UnityEngine.Input;

using UnityEngine.Windows;

public class headControl : MonoBehaviour
{
    //public List<GameObject> bodies;
    //public GameObject gameControl;//此处的gameControl是GameObject
    public GameObject GameControl;//此处的gameControl是GameObject
    public GameObject textOutput;
    public GameObject spaceShipHead; //太空船首节
    
    public GameObject directionButton;
    public GameObject flame, boundBox, outBoundWarning ,gameOverFlame;


    public AudioSource engineSound;//加速声音
    public Material engineLight;
    static public float maxSpeedUpTime = 3f;//最大连续加速时间
    public float forceScale = 2f;
    public float boundWarningDistance = 80f;
    public float boundReturnDistance = 150f;
    public float speedMax = 100f;

    public Text DebugMsgText;

    private float speedUpTimestamp;//连续加速时间戳
    private float speed = 0;
    private bool gyroStart;
    
    private int gameStatus,speedUp;
    //private gameControl comp;//此处的gameControl是一个Class
    private string controlMode;
    private float sensetivity;
    //Quaternion cameraBase = Quaternion.Euler(90, 0, 0);//将欧拉角转换为四元数
    private float newVolume = 1.0f;
    private AudioSource engineAmbientSound;
    private GameObject sceneCenter;
    private Vector3 oriCameraPos;
    private Quaternion oriCameraRotation;


    void Start()
    {
        /*if (!gyroStart)
        {
            StartGyroscopeOption option = new();
            option.interval = "ui";
            WX.StartGyroscope(option);
            gyroStart = true;
        }*/
        flame.SetActive(false);
        outBoundWarning.SetActive(false);
        //GameObject debugObj = GameObject.Find("DEBUG MSG");
        //debugMsg = debugObj.GetComponent<Text>();
        sceneCenter = GameObject.Find("场景中心");
        oriCameraPos = gameObject.transform.position;//不可用transform = xxx.transform的模式来简单存储原始transform，因为会形成直接关联，原始值无法存储
        oriCameraRotation = gameObject.transform.rotation;
        engineAmbientSound = spaceShipHead.transform.GetChild(0).GetComponent<AudioSource>();
        engineLight.DisableKeyword("_EMISSION");//关掉自发光
    }


    void GetDirButtonArea(out Vector2 dirButtonMinBounds,out Vector2 dirButtonMaxBounds)
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

    private void FixedUpdate()
    {
        if (CheckBound()) return;
        //transform.rotation = GetRotationByPointer(); //用手指触摸屏幕控制
        controlMode = gameControl.controlMode;
        RotateCamera();
        /*if (controlMode == "touch")
        {
            
            RotateCamera();
        }
        else
        {
            WX.OnGyroscopeChange(HandleGyroChange);
        }*/
        //WxTouch();
        if (gameStatus == 2)
        {
            spaceShipHead.transform.forward = Vector3.Lerp(spaceShipHead.transform.forward, transform.forward, Time.deltaTime * 5);
            spaceShipHead.transform.position = transform.position;
        }
    }




    void Update()
    {
        gameStatus = gameControl.gameStatus;
        controlMode = gameControl.controlMode;
        sensetivity = gameControl.sensetivity * 0.1f;
        speed = gameControl.speed;
        float step = 1f / Time.deltaTime; //1秒内加到极速
        float speedAdd = speedMax / step;
        float speedReduce = speedAdd / 4; //自然减速比加速慢一半
        
        if (gameStatus == 2)//游戏处于运行中
        {
            /*if (controlMode == "gyro")
            {
                this.transform.rotation = cameraBase * ConvertRotation(gyro.attitude); //用手机陀螺仪控制
            }
           */
            if (!engineAmbientSound.isPlaying) engineAmbientSound.Play();
            if (speedUp == 1)//加速
            {
                if (speedUpTimestamp == 0) speedUpTimestamp = Time.time;
                var speedUpDurationTime = Time.time - speedUpTimestamp;
                //Debug.Log("dur:" + speedUpDurationTime.ToString());
                //Debug.Log("stamp:" + speedUpTimestamp.ToString());
                if (speedUpDurationTime <= maxSpeedUpTime && speed < speedMax)//解决会粘住加速按钮的问题
                {
                    flame.SetActive(true);
                    engineLight.EnableKeyword("_EMISSION");//自发光
                    if (!engineSound.isPlaying) engineSound.Play();
                    var addForce = transform.forward * forceScale;
                    gameObject.GetComponent<Rigidbody>().AddForce(addForce, ForceMode.Force);
                }
                else
                {
                    SpeedNoraml(speedReduce);
                    flame.SetActive(false);
                    engineLight.DisableKeyword("_EMISSION");//关掉自发光
                }
            }else if (speedUp == -1) //减速
            {
                flame.SetActive(false);
                engineLight.DisableKeyword("_EMISSION");//关掉自发光
                //speed -= speedBrake;
            }else if (speedUp == 0)
            {
                SpeedNoraml(speedReduce);
                engineLight.DisableKeyword("_EMISSION");//关掉自发光
            }
           
            //MoveBody();
            //targetPosition = MoveSnakeHead(cameraTransform.position, cameraForward);
            //this.transform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime);// 平滑地移动相机到目标位置

        }
        else
        {
            flame.SetActive(false);
            engineAmbientSound.Stop();
        }
        if (gameStatus == 4) //暂停游戏结束
        {
            speed = 0f;
            engineSound.Stop();
            engineAmbientSound.Stop();
            flame.SetActive(false);
        }

        if (gameStatus == 0) //游戏结束
        {
            Die();
            float randomAngle = UnityEngine.Random.Range(0f, 360f);
            Quaternion targetRotation = Quaternion.Euler(randomAngle, 0f, 0f);
            spaceShipHead.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3);
        }
    }

    void Die()
    {
        speed = 0f;
        engineSound.Stop();
        engineAmbientSound.Stop();
        gameOverFlame.SetActive(true);
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        engineLight.DisableKeyword("_EMISSION");//关掉自发光
    }

    void SpeedNoraml(float speedReduce)
    {
        flame.SetActive(false);
        speed -= speedReduce; ; //不按加速键会自动减速
        if (engineSound.isPlaying)
        {
            newVolume -= speedReduce;//不按的话0.5秒内声音消失
            engineSound.volume = newVolume;
            if (newVolume <= 0)
            {
                engineSound.Stop();
                engineSound.volume = 1.0f;
                newVolume = 1.0f;
            }
        }
    }

    bool CheckBound()
    {
        float dis = Vector3.Magnitude(transform.position - sceneCenter.transform.position);
        //Debug.Log(dis);

        if (dis >= boundWarningDistance && dis < boundReturnDistance)
        {
            textOutput.SendMessage("MsgShow", "警告:飞船接近边界");
            return false;
        }
        else if (dis >= boundReturnDistance)
        {
            StartCoroutine(OutBoundShow());
            return true;
        }
        else
        {
            return false;
        }
    }


    IEnumerator OutBoundShow()
    {
        textOutput.SendMessage("MsgShow", "飞船超过边界,自动返回");
        var backgroundImage = outBoundWarning.GetComponent<Image>();
        StartCoroutine(FadeInOut(backgroundImage, "in", 0.5f));
        yield return new WaitForSeconds(0.5f);
        gameObject.transform.SetPositionAndRotation(oriCameraPos, oriCameraRotation);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        speed = 0.5f;
        StartCoroutine(FadeInOut(backgroundImage, "out", 0.5f));
        yield return new WaitForSeconds(0.5f);
        outBoundWarning.SetActive(false);
    }


    IEnumerator FadeInOut(Image image, string mode,float duration)
    {
        float t = 0;
        float targetAlpha = 0;
        if (mode == "in") targetAlpha = 1;
        float currentAlpha = image.color.a;
        while (t < duration)
        {
            t += Time.deltaTime / duration;
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, t);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
    }


    void MsgHandle(string msg)
    { 
        switch (msg)
        {
            case "Speed Up":
                speedUp = 1;//加速中
                break;
            case "Speed Normal":
                speedUp = 0;
                speedUpTimestamp = 0f;
                break;
            case "Speed Slow":
                speedUp = -1; //刹车中
                speedUpTimestamp = 0f;
                break;
            case "Left":
                Sideway("Left");
                break;
            case "Right":
                Sideway("Right");
                break;
            case "Up":
                Sideway("Up");
                break;
            case "Down":
                Sideway("Down");
                break;
            case "Die":
                Die();
                break;
            case "init":
                transform.position = gameControl.oriTransform.position;
                transform.forward = gameControl.oriTransform.forward;
                gameOverFlame.SetActive(false);
                spaceShipHead.transform.position = gameControl.oriTransform.position;
                spaceShipHead.transform.forward = gameControl.oriTransform.forward;
                Debug.Log("reinit:" + transform.position.ToString());
                break;
        }
    }

    void Sideway(string mode)
    {
        Vector3 sideWayDir = Vector3.zero;
        Vector3 addForce = Vector3.zero;
        if (mode == "Left")  addForce = new Vector3(-1,0,0) * 1f;
        if(mode == "Right")  addForce = new Vector3(1, 0, 0) * 1f;
        if(mode == "Up")  addForce = new Vector3(0, 1, 0) * 1f;
        if(mode == "Down")  addForce = new Vector3(0,-1, 0) * 1f;
        gameObject.GetComponent<Rigidbody>().AddForce(addForce, ForceMode.Force);
    }

    void RotateCamera()
    {
        GetDirButtonArea(out Vector2 dirButtonMinBounds, out Vector2 dirButtonMaxBounds);
        bool isUpControl = gameControl.isUpControl;
        Vector2 input = Vector2.zero;
        if (Input.touchCount > 0)
        {
            foreach (UnityEngine.Touch t in Input.touches)
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

        CheckRotateCamera(roX, roY);

        /*if (MathF.Abs(roY) > 0.3 / Time.deltaTime || MathF.Abs(roX) > 0.3 / Time.deltaTime) return;
        if ((x >= 0 && x <= 80) ||(x >= 280 && x <= 360)) transform.Rotate(Vector3.left, roY, Space.Self);
        transform.Rotate(Vector3.up, roX, Space.Self);
        transform.Rotate(Vector3.forward, -transform.rotation.eulerAngles.z, Space.Self);*/
    }

    void CheckRotateCamera(float roX,float roY)
    {
        float x = transform.rotation.eulerAngles.x;
        if (MathF.Abs(roY) > 0.3 / Time.deltaTime || MathF.Abs(roX) > 0.3 / Time.deltaTime) return;
        if ((x >= 0 && x <= 80) || (x >= 280 && x <= 360)) transform.Rotate(Vector3.left, roY, Space.Self);
        transform.Rotate(Vector3.up, roX, Space.Self);
        transform.Rotate(Vector3.forward, -transform.rotation.eulerAngles.z, Space.Self);
    }


}
