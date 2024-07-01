using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    //public GameObject gameControl;
    public GameObject GameControl;
    public GameObject snakeHead;
    public GameObject mainMenuScreen; 
    public GameObject settingScreen;
    public GameObject difficultScreen;
    public GameObject helpScreen;
    public GameObject restartQuestionScreen;
    public GameObject guideScreen;
    public GameObject cover;
    public GameObject warningMsg;

    public GameObject speedButton;
    public GameObject directionButton;
    public GameObject fireButton;
    public GameObject reloadButton;
    public Button returnButton;
    public AudioSource clickSound;

    public Toggle touchMode;
    public Toggle gyroMode;
    public Toggle leftHand;
    public Toggle rightHand;
    public Toggle upControl;
    public Toggle downControl;
    public Toggle vibrateOn;
    public Toggle vibrateOff;
    public Slider slide;
    private gameControl comp;
    private string buttonSideSetting;
    private int gameStatus;
    
    private void Start()
    {
        restartQuestionScreen.SetActive(false);
        ReadSettings();
        //comp = gameControl.GetComponent<gameControl>();
    }

    void ReadSettings()
    {
        bool vibrateSetting = gameControl.vibrateOn;
        buttonSideSetting = gameControl.buttonSide;
        string controlModeSettng = gameControl.controlMode;
        float sensetivitySetting = gameControl.sensetivity;
        bool isUpControl = gameControl.isUpControl;

        if (vibrateSetting)
        {
            vibrateOn.isOn = true;
        }
        else
        {
            vibrateOff.isOn = true;
        }

        if (buttonSideSetting == "Left")
        {
            leftHand.isOn = true;
            SwitchButtons("left");
        }
        else
        {
            rightHand.isOn = true;
            SwitchButtons("right");
        }
        if (isUpControl)
        {
            upControl.isOn = true;
        }
        else
        {
            downControl.isOn = true;
        }
        
        if (controlModeSettng == "touch")
        {
            touchMode.isOn = true;
        }
        else
        {
            gyroMode.isOn = true;
        }
        slide.value = sensetivitySetting;
    }

  
    private void Update()
    {
        gameStatus = gameControl.gameStatus;
        //Debug.Log("gs" + gameStatus.ToString());
        if (gameControl.isQuestionGen) reloadButton.GetComponent<Button>().interactable = false;
        else reloadButton.GetComponent<Button>().interactable = true;
    }

    public void Guide()
    {
        GameControl.SendMessage("MsgHandle", "Guide");
    }

    public void NewGame()
    {
        GameControl.SendMessage("MsgHandle", "New Game");
        clickSound.Play();
        warningMsg.SetActive(true);
    }


    public void PracticeMode() //自由模式
    {
        GameControl.SendMessage("MsgHandle", "Practice Game");
        clickSound.Play();
        warningMsg.SetActive(true);
    }

    public void LevelMode()  //闯关模式
    {
        return;//Demo
        /*GameControl.SendMessage("MsgHandle", "Level Game");
        clickSound.Play();
        warningMsg.SetActive(true);*/
    }

    public void TryReturnToMainMenu()
    {
        restartQuestionScreen.SetActive(true);
        warningMsg.SetActive(false);
        GameControl.SendMessage("MsgHandle", "Pause");
        clickSound.Play();
    }

    public void CancelRestart()
    {
        clickSound.Play();
        restartQuestionScreen.SetActive(false);
        warningMsg.SetActive(true);
        GameControl.SendMessage("MsgHandle", "Resume");
    }

    public void Restart()
    {
        restartQuestionScreen.SetActive(false);
        warningMsg.SetActive(true);
        GameControl.SendMessage("MsgHandle", "Restart");
        clickSound.Play();
    }
    public void Help()
    {
        mainMenuScreen.SetActive(false);
        helpScreen.SetActive(true);
        clickSound.Play();
        GameControl.SendMessage("SetSettings", "NotFirstPlay");//点击过help按钮就认为不是第一次玩了。
        
    }
    public void Difficult()
    {
        mainMenuScreen.SetActive(false);
        settingScreen.SetActive(false);
        difficultScreen.SetActive(true);
        clickSound.Play();
    }

    public void InGameSetting()
    {
        GameControl.SendMessage("MsgHandle", "Pause");
        mainMenuScreen.SetActive(false);
        difficultScreen.SetActive(false);
        settingScreen.SetActive(true);
        ReadSettings();
        AddListener();
        clickSound.Play();
    }

    public void TopMenuSetting()
    {
        mainMenuScreen.SetActive(false);
        difficultScreen.SetActive(false);
        settingScreen.SetActive(true);
        ReadSettings();
        AddListener();
        clickSound.Play();
    }


    public void ReturnToStartScreen()
    {
        clickSound.Play();
        if (gameStatus != 4)
        {
            mainMenuScreen.SetActive(true);
            helpScreen.SetActive(false);
            difficultScreen.SetActive(false);
            guideScreen.SetActive(false);
            cover.SetActive(false);
        }
        else
        {
            GameControl.SendMessage("MsgHandle", "Resume");
        }
        settingScreen.SetActive(false);
    }
    public void Link()
    {
        Application.OpenURL("https://www.patreon.com/SimpleGamesforMobilePhone/shop/math-space-ship-simple-game-for-android-225491?utm_medium=clipboard_copy&utm_source=copyLink&utm_campaign=productshare_creator&utm_content=join_link");
    }
    public void RequestAmmo()
    {
        GameControl.SendMessage("MsgHandle", "Request Ammo");
        clickSound.Play();
    }
    void AddListener()
    {
        touchMode.onValueChanged.AddListener(SetPlayMode);
        leftHand.onValueChanged.AddListener(SetLeftHand);
        vibrateOn.onValueChanged.AddListener(SetVibrate);
        upControl.onValueChanged.AddListener(SetUpDown);
    }
    void SetPlayMode(bool isOn)
    {
        if (isOn)
        {
            GameControl.SendMessage("SetSettings", "Set Touch Mode");
        }
        else
        {
            GameControl.SendMessage("SetSettings", "Set Gyro Mode");
        }
    }



    void SetLeftHand(bool isOn)
    {
        clickSound.Play();
        if (isOn)
        {
            GameControl.SendMessage("SetSettings", "Set Left Hand");
            SwitchButtons("left");
        }
        else
        {
            GameControl.SendMessage("SetSettings", "Set Right Hand");
            SwitchButtons("right");
        }
        
    }

    void SetUpDown(bool isOn)
    {
        clickSound.Play();
        if (isOn)
        {
            GameControl.SendMessage("SetSettings", "Set Up");
        }
        else
        {
            GameControl.SendMessage("SetSettings", "Set Down");
        }
    }

    void SetVibrate(bool isOn)
    {
        clickSound.Play();
        if (isOn == true)
        {
            GameControl.SendMessage("SetSettings", "Set Vibrate On");
        }
        else
        {
            GameControl.SendMessage("SetSettings", "Set Vibrate Off");
        }
    }

    public void SensetivitySlider()
    {
        GameControl.SendMessage("SetSensetivity", slide.value.ToString());
        
    }


    public void Share()
    {
        //ShareAppMessageOption option = new();
        //WX.ShareAppMessage(option);
    }
    void SwitchButtons(string mode)
    {
        clickSound.Play();
        if (mode == "left") //左手控制加速
        {
            directionButton.transform.localPosition = new Vector2(850f, 480f); //滚转控制
            speedButton.transform.localPosition = new Vector2(200f, 480f);//加速及侧向控制 在左边
            fireButton.transform.localPosition = new Vector2(200f,220f);

            returnButton.transform.localPosition = new Vector2(1400f, 210f);
            reloadButton.transform.localPosition = new Vector2(1250f, 480f);
        }
        else
        {
            directionButton.transform.localPosition = new Vector2(590f, 480f);
            speedButton.transform.localPosition = new Vector2(1300f, 480f);//加速及侧向控制 在右边
            fireButton.transform.localPosition = new Vector2(1300f, 220f);

            returnButton.transform.localPosition = new Vector2(220f, 210f);
            reloadButton.transform.localPosition = new Vector2(100f, 480f);
        }
    }
}
