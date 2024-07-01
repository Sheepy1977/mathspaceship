using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
//using static System.Net.Mime.MediaTypeNames;
using System;
using Random = UnityEngine.Random;
//using UnityEngine.UIElements;
//using System.Net;
//using UnityEngine.Rendering;

public class gameControl : MonoBehaviour
{
    //以下是初始游戏对象设置
    [Header("===以下是初始游戏对象设置====")]
    public GameObject mainGameObj;//主控对象，即飞船相机
    public GameObject spaceShip;//飞船
    public GameObject gameOverScreen; //结束画面
    public GameObject gameRunScreen;//游戏中画面
    public GameObject gameStartScreen;//游戏前画面
    public GameObject guideScreen;
    public GameObject newFeature;
    public GameObject cover; //遮盖
    public GameObject tips;

    public GameObject helpScreen;
    public GameObject helpButton;
    public GameObject hitScreen;//受伤屏幕
    public GameObject hpScreen;//加血屏幕
    public GameObject difficultScreen;//难度屏幕
    public GameObject settingScreen;//设置屏幕
    public GameObject levelUpScreen;//升级屏幕
    public List<GameObject> obPrefabs;//障碍物预制体
    public List<GameObject> enemyPrefabs;
    public GameObject enemyParent;
    public GameObject obParent;//障碍物预制体父物体
    public GameObject tempObParent;//爆炸后的碎石存放地
    public GameObject tipParent;//向导父物体
    public GameObject numTipPrefab;
    public GameObject enemyTipPrefab;

    public List<GameObject> hpPrefabs;//血包预制体
    public GameObject hpParent;//血包预制体父物体
    public List<GameObject> numberPrefabs;//数字预制体
    public GameObject numParent;//数字父物体
    public GameObject questionCountDownParent;
    public GameObject questionCountDownBar;//问题倒计时按条
    public GameObject reloadButtonBoard; //补给按钮边缘（闪烁用）

    public GameObject spawnVFX;
    public GameObject textOutput;
    public UnityEngine.UI.Text questionCountDownText;
    public UnityEngine.UI.Text difficultStatusText;//难度显示文字
    public UnityEngine.UI.Image difficultStatusBg;//
    //public UnityEngine.UI.Text difficultButtonText; //难度设置按钮文字
    //public UnityEngine.UI.Image difficultButtonColor;//难度按钮背景色
    public UnityEngine.UI.Image questionBgColor;//问题背景颜色
    //public AudioSource startBgm;
    //public AudioSource runningBgm;
    public AudioSource reloadSound;
    public AudioSource hpAddSound;
    public GameObject explodeVFX;

    //以下是默认初始参数设置
    [Header("====以下是默认初始参数设置====")]
    public int sceneWidth = 25;
    public int sceneHeight = 25;
    public int sceneDeep = 25;
    public int obCount = 10;//障碍物默认数量
    public int maxObCount = 50;//最大障碍物数量
    private int addObCount;
    public int hpCount = 15;//血包默认数量
    public int addBodyInter = 99999999;//5秒加一节身体
    static public float distance;
    
    static public float sensetivity = 1;//滑移灵敏度
    public int maxAnswer = 20;//20以内加减法
    public int minAnswer;
    static public int health = 10;

    static public int maxBodyCount = 40;//最大蛇身长度
    static public int maxAnswerCount = 5;//备选答案个数
    public int questionRefreshTime = 10;//地狱难度题目刷新速度

    static public string gameMode = "free";//默认是自由模式
    private int currentLevel = 0; //起始关卡
    private int currentObCount;
    //以下这些变量需要被其他代码访问
    [Header("====以下这些变量需要被其他代码访问====")]
    static public int gameStatus;
    static public int speedUp;
    static public string debugMsg;
    static public string healthStatus;
    static public string speedStatus;
    static public string bodyStatus;
    static public int score = 0;
    static public float oriSpeed = 2.0f;
    static public string questionString;
    static public float speed;
    static public string gameOverReason;
    static public string controlMode = "touch";//touch，手指拖动  gyro，陀螺仪
    static public string difficult = "简单";
    static public bool vibrateOn = true;
    static public string buttonSide = "right"; //左右手
    static public bool isUpControl = true;//上下方向控制
    static public bool isFirstPlay = true;//是不是第一次玩？
    static public string gameLevel;//当前关卡
    static public int levelUpScore;//升级所需分数
    static public bool isTooSlow;
    static public int ammoMount;
    static public int rockHitTimes;//击打岩石几次才能打爆
    static public Transform oriTransform;
    static public bool isQuestionGen = false; //场景中有问题吗？
    static public int enemyCount;
    static public int enemyMaxSpeed;
    static public float enemyShootWaitMinTime;//射击时间最小间隔
    static public float shotMaxBias;//射击最大偏差
    static public int requestAmmoAva = 0;//可用申请补给次数

    private Vector3 lastPosition = new Vector3(0, 0, 0);
    private bool hpSpawn;
    private float lastSpawnTime = 0f;//hp奖励计算初始时间戳
    private float lastEnemyDieTimeStamp; //敌人生成时间戳
    private float enemySpawnTime;
    private float questionTimer;//题目显示时间计时器，针对地狱难度
    private bool alreadyShowNewFeature; //已经显示了新特性页面？

    private float pauseTimeStamp;
    private List<int> randomAnswers = new();
    
    
    private int addAmmo;
    private int scoreAdd; 

    private int rockHitCount = 0; //击碎岩石数量
    private int enemyHitCount = 0; //击中飞船数量
    private int ammoFiredCount= 0; //开火次数
    private int beenHitCount = 0; //被击中次数
    private int requestAmmoCount = 0; //申请补给次数
    
    private float travelDisCount = 0; //飞行距离

    void Start()
    {
        //PlayerPrefs.DeleteKey("V1.5feature");
        GetSettings();
        GameInit();
        oriTransform = spaceShip.transform;
        
    }

    void HitShock(string mode)
    {
        if (vibrateOn)
        {
            //if (mode == "short") WX.VibrateShort(DoNothingShort());
            //if (mode == "long") WX.VibrateLong(DoNothingLong());
        }
    }

    IEnumerable MultiShock(string mode,float gap = 0.2f,int repeat = 2)
    {
        if (vibrateOn)
        {
            if (mode == "short")
            {
                for (int i = 0;i < repeat; i++)
                {
                    //WX.VibrateShort(DoNothingShort());
                    yield return new WaitForSeconds(gap);
                }
            }
            if (mode == "long")
            {
                for (int i = 0; i < repeat; i++)
                {
                    //WX.VibrateLong(DoNothingLong());
                    yield return new WaitForSeconds(gap);
                }
            }
        }
    }

    /*VibrateShortOption DoNothingShort()
    {
        VibrateShortOption option = new VibrateShortOption();
        return option;
    }

    VibrateLongOption DoNothingLong()
    {
        VibrateLongOption option = new VibrateLongOption();
        return option;
    }
    */
    void GetSettings() //读取设定
    {
        string[] settings = new string[7];
        settings[0] = "ControlMode";
        settings[1] = "VibrateOn";
        settings[2] = "ButtonSide";
        settings[3] = "Sensetivity";
        settings[4] = "IsUpControl";
        settings[5] = "IsFirstPlay";
        settings[6] = "V2_0feature";
        string temp;
        for (int i = 0;i < settings.Length; i++)
        {
            temp = CheckPlayerSettings(settings[i]);
            Debug.Log(settings[i] + ":" + temp.ToString());
            if (temp != "wrong")
            {
                switch (settings[i])
                {
                    case "ControlMode":
                        controlMode = temp;
                        break;
                    case "VibrateOn":
                        vibrateOn = bool.Parse(temp);
                        break;
                    case "ButtonSide":
                        buttonSide = temp;
                        break;
                    case "Sensetivity":
                        sensetivity = float.Parse(temp);
                        Debug.Log("redading sensetivity:" + sensetivity);
                        break;
                    case "IsUpControl":
                        isUpControl = bool.Parse(temp);
                        break;
                    case "IsFirstPlay":
                        isFirstPlay = bool.Parse(temp);
                        break;
                    case "V2_0feature":
                        alreadyShowNewFeature = bool.Parse(temp);
                        break;
                }
            }
        }
    }

    string  CheckPlayerSettings(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        else
        {
            return "wrong";
        }
    }

    void GameInit()
    {
        health = 10;//初始血量
        score = 0;
        gameStatus = 1; // 0 代表 gameover 1 代表在启动画面 2代表运行中 4代表暂停
        gameLevel = "第1关";
        currentLevel = 0;
        guideScreen.SetActive(false);
        cover.SetActive(false);
        gameOverScreen.SetActive(false);
        gameRunScreen.SetActive(false);
        helpScreen.SetActive(false);
        hitScreen.SetActive(false);
        hpScreen.SetActive(false);
        settingScreen.SetActive(false);
        difficultScreen.SetActive(false);
        levelUpScreen.SetActive(false);
        gameStartScreen.SetActive(true);
        tips.SetActive(true);
 
        /*if (!alreadyShowNewFeature)
        {
            newFeature.SetActive(true);
        }
        else
        {
            newFeature.SetActive(false);
            PlayerPrefs.SetString("V2_0feature", true.ToString());
            PlayerPrefs.Save();
        }
        */
        //WX.ReportGameStart();
    }

    private void FixedUpdate()
    {
        var dis = Vector3.Magnitude(mainGameObj.transform.position - lastPosition);
        speed = Mathf.Round((dis * 10) / (Time.deltaTime));
        travelDisCount += dis;
        lastPosition = mainGameObj.transform.position;
    }

    void Update()
    {
        currentObCount = obParent.transform.childCount;
        if (gameStatus == 2)//运行中
        {
            if (health <= 0 )
            {
                gameOverReason = "能量耗尽";
                GameOver();
                Debug.Log("GAME OVER");
            }
            
            if (Time.time - questionTimer > questionRefreshTime) //地狱难度每10秒重新出题一次
            {
                DestroyAnswer();
                PropSpawn();//重新出题
               if (currentObCount < maxObCount)
                    StartCoroutine(SpawnOb(addObCount, obPrefabs, obParent, "障碍物_", true,maxRange:40f));
                questionTimer = Time.time;
            }
            if (questionRefreshTime < 100 && isQuestionGen)
            {
                SetQuestionCountDownBar(questionRefreshTime, questionTimer);
            }
            else
            {
                questionCountDownParent.SetActive(false);
            }

            if (gameMode == "level")
            {
                CheckGameLevel();//闯关模式
                //Debug.Log(gameLevel);
            }

            GenQuestionString();
        }
    }

    void GenQuestionString()
    {
        if (!isQuestionGen)
        {
            if (ammoMount <= 6)
            {
                questionString = "Ammo low";
                reloadButtonBoard.SetActive(true);
            }
            else if (enemyParent.transform.childCount == 0 && !isQuestionGen)
            {
                if (difficult != "Practice") questionString = "Watch out enemy"; else questionString = "Practice";
                reloadButtonBoard.SetActive(false);
            }
            else
            {
                questionString = "Shoot down Enemy";
            }
        }
    }



    void LateUpdate()
    {
        if (gameStatus == 2)
        {
            CheckRock();
            HpSpawn();//生成血包
            if (enemyCount > 0 && enemyParent.transform.childCount == 0 && Time.time - lastEnemyDieTimeStamp > enemySpawnTime)
            {
                EnemySpawn(enemyCount);
            }
            if (randomAnswers.Count > 0) ChangeAnswerText(randomAnswers);
            CheckRightAnswer();
        }

    }

    void CheckRightAnswer()
    {
        if (isQuestionGen)
        {
            foreach (Transform t in numParent.transform)
            {
                if (t.name == "答案_0") return;
            }
            DestroyAnswer(); //如果问题生成标志还在，但是答案0消失了，那么重新生成问题和答案。
            PropSpawn();
        }
    }


    void EnemySpawn(int enemyCount)
    {
        StartCoroutine(SpawnOb(enemyCount, enemyPrefabs, enemyParent, "敌人_", false,minRange:20f,maxRange:30f,tipPrefab:enemyTipPrefab));
        textOutput.SendMessage("MsgShow", "Enemy coming");
    }

    void CheckGameLevel()
    {
        switch (currentLevel)
        {
            case 0:
                levelUpScore = 15;
                if (score >= levelUpScore)
                {
                    currentLevel = 1;
                    //levelUpScreen.SetActive(true);
                    //gameStatus = 4;
                    textOutput.SendMessage("MsgShow", "More enemies");
                    Medium();
                    return;
                }
                break;
            case 1:
                levelUpScore = 30;
                if (score >= levelUpScore)
                {
                    currentLevel = 2;
                    //levelUpScreen.SetActive(true);
                    //gameStatus = 4;
                    textOutput.SendMessage("MsgShow", "More enemies");
                    Hard();
                    return;
                }
                break;
            case 2:
                levelUpScore = 60;
                if (score >= levelUpScore)
                {
                    currentLevel = 3;
                    //levelUpScreen.SetActive(true);
                    //gameStatus = 4;
                    textOutput.SendMessage("MsgShow", "More enemies");
                    Hell();
                    return;
                }
                break;
            case 3:
                levelUpScore = 100;
                if (score > levelUpScore)
                {
                    currentLevel = 3;
                    textOutput.SendMessage("MsgShow", "Mission Accomplished!!");
                    GameOver("Finished");
                    return;
                }
                break;
        }
    }
    
    public void ContinuePlay()
    {
        levelUpScreen.SetActive(false);
        gameStatus = 2;
        hpCount = 15;
        DestroyAnswer();
        //BodyHandle("removeAllBody"); //升级后去掉所有的子飞船
        if (currentLevel == 1) Medium();
        if (currentLevel == 2) Hard();
        if (currentLevel == 3) Hell();
        PropSpawn();
    }


    void SetQuestionCountDownBar(int questionRefreshTime, float questionTimer)
    {
        questionCountDownParent.SetActive(true);
        float barFullLength = 1000f;
        float remain = questionRefreshTime - (Time.time - questionTimer);
        float currentLength = barFullLength *  (remain / questionRefreshTime);
        questionCountDownBar.GetComponent<RectTransform>().sizeDelta = new Vector2(currentLength, 20f);
        int t = (int)remain;
        questionCountDownText.text = t.ToString();
    }

    void PropSpawn() //道具生成
    {
        randomAnswers = new();
        while (randomAnswers.Count < maxAnswerCount)
        {
            int randomNumber = UnityEngine.Random.Range(minAnswer, maxAnswer + 1);
            if (!randomAnswers.Contains(randomNumber))
            {
                randomAnswers.Add(randomNumber);
            }
        }
        int answer = randomAnswers[0];
        int choice = Random.Range(0, 2);
        int num1 = Random.Range(1, answer - 1); //避免出现+0 这种情况 
        int num2;
        if (choice == 0) //问题为加
        {
            num2 = answer - num1; 
        }
        else //问题为减
        {
            num1 = Random.Range(answer + 1,maxAnswer + 1);
            num2 = num1 - answer;
            if (num2 > num1)
            {
                (num1, num2) = (num2, num1);
            }
        }
        string[] ops = { " + ", " - " };
        questionString = num1.ToString() + ops[choice] + num2.ToString() + " = ?";
        StartCoroutine(SpawnOb(maxAnswerCount, numberPrefabs, numParent, "答案_", true,maxRange:30f,tipPrefab:numTipPrefab));
        //ChangeAnswerText(randomAnswers);//此处用协程进行延迟操作，避免引擎还没生成完物体就操作属性
        questionTimer = Time.time;
        isQuestionGen = true;
        textOutput.SendMessage("MsgShow", "Supply box is out");
    }

    void SpawnTips(GameObject prefab,string name)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        GameObject newTip = Instantiate(prefab, pos, Quaternion.identity);
        newTip.transform.SetParent(tipParent.transform);
        newTip.name = name;
    }


    void ChangeAnswerText(List<int> randomAnswers)
    {
        if (numParent.transform.childCount == 0) return;
        foreach (Transform child0 in numParent.transform)
        {
            string name = child0.name;
            int i = Convert.ToInt32(name.Substring(name.IndexOf("_") + 1));
            foreach (Transform child1 in child0.GetChild(0))
            {
                if (child1.name.Contains("Text"))
                {
                    child1.GetComponent<TextMeshPro>().text = randomAnswers[i].ToString();
                }
            }
        }

    }
  

    void HpSpawn()
    {
        //Debug.Log("time:" +Time.time + "  lastSpawnTime" + lastSpawnTime);
        if (health <= 5 && !hpSpawn && Time.time - lastSpawnTime > 10) //设置在上一次删掉所有血包后10秒才能生成新的血包
        {
            StartCoroutine(SpawnOb(hpCount, hpPrefabs,hpParent, "血包_", false));
            hpSpawn = true;
            lastSpawnTime = Time.time;
            //hpSpawnFx.Play();
            textOutput.SendMessage("MsgShow", "Hp packs are out");
        }
        if (health > 10 )
        {
            //当血量大于10时或者30秒过后，删掉所有血包
            DestroyItemsByParent(hpParent);
            hpSpawn = false;
            lastSpawnTime = Time.time + 10;
        }
    }

    IEnumerator FlashScreen(GameObject screen,float duration = 0.2f)
    {
        screen.SetActive(true);
        yield return new WaitForSeconds(duration);
        screen.SetActive(false);
    }

    void BodyHandle(string mode) //通知蛇头增减蛇身体
    {
        mainGameObj.SendMessage("MsgHandle", mode);
    }

    void NewGame() //显示游戏模式选择页面
    {
        difficultScreen.SetActive(true);
        gameStartScreen.SetActive(false);
    }


    void NewPracticeGame()
    {
        difficultScreen.SetActive(false);
        gameStartScreen.SetActive(false);
        gameMode = "practice";
        Practice();
        GameOn();
    }
    
    void NewLevelGame()
    {
        difficultScreen.SetActive(false);
        gameStartScreen.SetActive(false);
        gameMode = "level";
        gameLevel = "第1关";
        Easy();
        GameOn();
    }

    void GameOn()
    {
        gameRunScreen.SetActive(true);

        if (isFirstPlay)
        {
            guideScreen.SetActive(true);
            cover.SetActive(true);
            isFirstPlay = false;
            PlayerPrefs.SetString("IsFirstPlay", false.ToString());
            PlayerPrefs.Save();
        }
        else
        {
            cover.SetActive(false);
            guideScreen.SetActive(false);
        }
        
        //GameObject.Find("向导").GetComponent<Guide>().enabled = true; //启动GUIDE
        difficultScreen.SetActive(false);
        gameStatus = 2;
        //PropSpawn();//生成道具
        questionTimer = Time.time;
        lastEnemyDieTimeStamp = Time.time;
        if (gameMode =="practice")
            StartCoroutine(SpawnOb(obCount,obPrefabs,obParent, "障碍物_",true));
    }

    IEnumerator SpawnOb(int obCount,List<GameObject> prefabs,GameObject parent, string pre ,bool randomColor,float maxRange = 20f,float minRange = 5f,GameObject tipPrefab =null)
    {
        for (int i = 0;i < obCount;i++)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            Vector3 position = new Vector3(0,0,0) + randomDirection * Random.Range(minRange,maxRange); //始终基于原点

            int c = prefabs.Count;
            GameObject prefab;
            if (c > 1)
            {
                int seed = Random.Range(0, c - 1);
                prefab = prefabs[seed];
            }
            else
            {
                prefab = prefabs[0];
            }
            MyFunctions.GenVFX(spawnVFX, position);
            GameObject newOb = Instantiate(prefab, position, Quaternion.identity);
            newOb.name = pre + i.ToString();
            newOb.transform.parent = parent.transform;
            if (randomColor)
            {
                float r = Random.Range(0f, 1f);
                float g = Random.Range(0f, 1f);
                float b = Random.Range(0f, 1f);
                float a = 0.5f;
                Color rc = new Color(r, g, b, a);
                //StartCoroutine(AlphaAnimator(newOb,rc));
                newOb.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = rc;
            }

            if (tipPrefab && tipParent)
            {
                SpawnTips(tipPrefab, "Tip" + newOb.name);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void GameOver(string mode="not finished")
    {
        //startBgm.Stop();
        //runningBgm.Stop();
        gameStatus = 0;
        BodyHandle("Die");
        gameRunScreen.SetActive(false);
        string headMsg;
        if (mode == "not finished")
        {
            headMsg = "Sorry,Pilot, you didn't make it\n\n" ;
        }
        else
        {
            headMsg = "Great,Pilot, well done!\n\n";
        }

        string msg = headMsg + "You have shot down " + enemyHitCount + " enemy ship(s),and been hit " + beenHitCount + " times\nYou fired " + ammoFiredCount + " rounds,request " + requestAmmoCount + " times supply\nYou have travled " + (int)travelDisCount + " meters in this mission\n\nGood luck next time!";
        textOutput.SendMessage("GameOverMsg", msg);
        StartCoroutine(GameOverLateAction());
    }

    IEnumerator GameOverLateAction()
    {
        yield return new WaitForSeconds(5f);
        tipParent.SetActive(false);
        gameOverScreen.SetActive(true);
        gameRunScreen.SetActive(false);
        gameStartScreen.SetActive(false);
        GetSettings();
        Reinit();
    }


    void Reinit()
    {
        
        DestroyItemsByParent(obParent);
        DestroyItemsByParent(hpParent);
        DestroyItemsByParent(enemyParent);
        DestroyAnswer();

        lastEnemyDieTimeStamp = Time.time;
        lastSpawnTime = Time.time;
        BodyHandle("init");
        isQuestionGen = false;
        //Rigidbody r = mainGameObj.GetComponent<Rigidbody>(); //停止蛇头乱动
        spaceShip.transform.position = oriTransform.position;
        spaceShip.transform.forward = new Vector3(0,0,1);
        mainGameObj.transform.position = oriTransform.position;
        mainGameObj.transform.forward = new Vector3(0, 0, 1);
        rockHitCount = 0; //击碎岩石数量
        enemyHitCount = 0; //击中飞船数量
        ammoFiredCount = 0; //开火次数
        beenHitCount = 0; //被击中次数
        requestAmmoCount = 0; //申请补给次数
        travelDisCount = 0; //飞行距离
        requestAmmoAva = 0;
}

    void MsgHandle(string msg) //通用消息处理方法
    {
        switch (msg)
        {
            case "New Game":
                NewGame();
                break;
            case "Practice Game":
                NewPracticeGame();
                break;
            case "Level Game":
                NewLevelGame();
                break;
            case "Restart":
                Reinit();
                SceneManager.LoadSceneAsync(0);
                break;
            case "Collide":
                Collide();
                break;
            case "HP+":
                HpAdd();
                break;
            case "Right Answer":
                RightAnswer();
                break;
            case "Wrong Answer":
                WrongAnswer();
                break;
            case "Hit Rock":
                HitRock();
                break;
            case "Help":
                gameStartScreen.SetActive(false);
                helpScreen.SetActive(true);
                break;
            case "Pause":
                Pause();
                break;
            case "Resume":
                Resume();
                break;
            case "Guide":
                isFirstPlay = true;
                helpScreen.SetActive(false);
                NewPracticeGame();
                break;
            case "Fire":
                if (ammoMount <= 0)
                {
                    if (!isQuestionGen) PropSpawn(); //弹药耗尽出题
                    textOutput.SendMessage("MsgShow", "ammo out");
                    break;
                }
                ammoMount--;
                ammoFiredCount++;
                HitShock("short");
                break;
            case "Request Ammo":
                if (requestAmmoAva <=0)
                {
                    textOutput.SendMessage("MsgShow", "Can't request");
                    break;
                }
                else
                {
                    requestAmmoAva--;
                    requestAmmoCount++;
                }
                if (!isQuestionGen) PropSpawn(); //弹药耗尽出题
                break;
            case "Shoot Down Enemy":
                ShootEnemy();
                break;
            case "Enemy Collide":
                EnemyCollide();
                break;
            default:
                break;
        }
    }

    void EnemyCollide()
    {
        string msg = "Enemy ship crashed";
        int count = enemyParent.transform.childCount - 1;
        //if (count > 0) msg = msg + ",there are " + count.ToString() + " enemy ship(s)"; else msg = msg + "\nAll clear for now.";
        textOutput.SendMessage("MsgShow", msg);
        lastEnemyDieTimeStamp = Time.time;
    }

    void HitRock()
    {
        score += scoreAdd;
        requestAmmoAva++;
        rockHitCount++;
        MultiShock("short",gap:0.2f);
        textOutput.SendMessage("MsgShow", "");
    }

    void ShootEnemy()
    {
        lastEnemyDieTimeStamp = Time.time;
        score += scoreAdd * 2; //飞船算2分
        enemyHitCount++;
        requestAmmoAva += 2;
        string msg = "You have shot down " + enemyHitCount.ToString() + " enemy ship(s)";
        int count = enemyParent.transform.childCount - 1;
        //if (count > 0) msg = msg + ",there are " + count.ToString() + "艘"; else msg = msg + "\nAll clear for now.";
        textOutput.SendMessage("MsgShow", msg);
    }


    void CheckRock()
    {
        int currentObCount = obParent.transform.childCount;
        if (currentObCount < 2)
        {
            int count = Random.Range(3, addObCount);
            StartCoroutine(SpawnOb(count, obPrefabs, obParent, "障碍物_", true));
            //textOutput.SendMessage("MsgShow", "岩石出现了");
        }
    }

    void Pause()
    {
        gameStatus = 4;
        pauseTimeStamp = Time.time;
    }

    void Resume()
    {
        gameStatus = 2;
        float pauseDuration = Time.time - pauseTimeStamp;
        questionTimer += pauseDuration;
    }

    void SetDifficultButtonText(string text )
    {
        //difficultButtonText.text = text ;
        return;
    }

    void SetDifficultdColor(Color color)
    {
        //difficultButtonColor.color = color;
        //questionBgColor.color = new Color(color.r, color.g, color.b, 0.1f);
        //difficultStatusBg.color = color;
        return;
    }

    void Practice()
    {
        difficult = "Practice";
        string difficultText;
        gameLevel = "第1关";
        if (gameMode == "level") difficultText = gameLevel; else difficultText = difficult;
        //SetDifficultButtonText(difficultText);
        //SetDifficultdColor(easyColor);
        questionRefreshTime = 999999;

        maxAnswer = 10;
        minAnswer = 1;
        maxAnswerCount = 3;//备选答案3个
        addObCount = 3;//一道题加2个障碍物
        ammoMount = 50;
        addAmmo = 50;

        scoreAdd = 0;


        rockHitTimes = 1;
        enemyCount = 0;
        enemySpawnTime = 30;
        enemyMaxSpeed = 20;
        enemyShootWaitMinTime = 4;
        shotMaxBias = 4f;

        questionCountDownParent.SetActive(false);
        
    }


    void Easy()
    {
        difficult = "Simple";
        string difficultText;
        gameLevel = "第1关";
        if (gameMode == "level") difficultText = gameLevel; else difficultText = difficult;
        //SetDifficultButtonText(difficultText);
        //SetDifficultdColor(easyColor);
        questionRefreshTime = 999999;
        
        maxAnswer = 10;
        minAnswer = 1;
        addBodyInter = 9999999;
        maxAnswerCount = 3;//备选答案3个
        addObCount = 3;//一道题加2个障碍物
        ammoMount = 30;
        addAmmo = 20;

        scoreAdd = 1;

        rockHitTimes = 2;
        enemyCount = 1;
        enemySpawnTime = 10;
        enemyMaxSpeed = 20;
        enemyShootWaitMinTime = 4;
        shotMaxBias = 4f;

        questionCountDownParent.SetActive(false);
        PlayerPrefs.SetString("Difficult", difficult);
        PlayerPrefs.Save();
    }

    void Medium()
    {
        difficult = "Medium";
        string difficultText;
        gameLevel = "第2关";
        if (gameMode == "level") difficultText = gameLevel; else difficultText = difficult;
        //SetDifficultButtonText(difficultText);
        //SetDifficultdColor(medColor);
        questionRefreshTime = 999999;
        
        maxAnswer = 50;
        minAnswer = 10;
        addBodyInter = 10;
        addObCount = 5;
        //ammoMount = 20;
        addAmmo = 20;

        scoreAdd = 1;

        rockHitTimes = 4;
        enemyCount = 2;
        enemySpawnTime = 15;//隔5秒生成一个敌人
        enemyMaxSpeed = 30;
        maxAnswerCount = 5;
        enemyShootWaitMinTime = 3;
        shotMaxBias = 3f;

        questionCountDownParent.SetActive(false);
        PlayerPrefs.SetString("Difficult", difficult);
        PlayerPrefs.Save();
    }

    void Hard()
    {
        difficult = "Hard";
        string difficultText;
        gameLevel = "第3关";
        if (gameMode == "level") difficultText = gameLevel; else difficultText = difficult;
        //SetDifficultButtonText(difficultText);
        //SetDifficultdColor(hardColor);
        questionRefreshTime = 20;
        
        maxAnswer = 80;
        minAnswer = 40;
        addBodyInter = 8;
        addObCount = 4;
        maxAnswerCount = 8;
        //ammoMount = 10;
        addAmmo = 10;

        scoreAdd = 1;

        enemyCount = 4;
        enemySpawnTime = 10;//隔5秒生成一个敌人
        enemyMaxSpeed = 40;
        rockHitTimes = 6;
        enemyShootWaitMinTime = 2;
        shotMaxBias = 2f;

        PlayerPrefs.SetString("Difficult", difficult);
        PlayerPrefs.Save();
    }

    void Hell()
    {
        difficult = "Hell";
        string difficultText;
        gameLevel = "第4关";
        if (gameMode == "level") difficultText = gameLevel; else difficultText = difficult;
        //SetDifficultButtonText(difficultText);
        //SetDifficultdColor(hellColor);
        questionRefreshTime = 10;
        
        maxAnswer = 99;
        minAnswer = 60;
        addBodyInter = 3;
        addObCount = 5;
        maxAnswerCount = 10;
        //ammoMount = 10;
        addAmmo = 10;

        scoreAdd = 1;

        enemyCount = 8;
        enemySpawnTime = 5;//隔5秒生成一个敌人
        enemyMaxSpeed = 50;
        enemyShootWaitMinTime = 0;
        shotMaxBias = 1f;

        rockHitTimes = 8;
        PlayerPrefs.SetString("Difficult", difficult);
        PlayerPrefs.Save();
    } 

    void RightAnswer()
    {
        reloadSound.Play();
        if (ammoMount < 150) ammoMount += addAmmo; else return;
        textOutput.SendMessage("MsgShow", "Reload compeleted,added " + addAmmo.ToString() + " rounds");
        //StartCoroutine(FlashScreen(hpScreen)); //闪烁屏幕
        DestroyItemsByParent(numParent, needVFX:true);
        isQuestionGen = false;
    }

    void WrongAnswer()
    {
        health--;
        //wrongAnswerFx.Play();
        Debug.Log("错误答案");
        textOutput.SendMessage("MsgShow", "Wrong box");
        //BodyHandle("addbody");
        StartCoroutine(FlashScreen(hitScreen)); //闪烁屏幕
    }

    void HpAdd()
    {
        health++;
        Debug.Log("吃血 HP+1");
        textOutput.SendMessage("MsgShow", "HP +1");
        hpAddSound.Play();
        StartCoroutine(FlashScreen(hpScreen)); //闪烁屏幕
    }

    void Collide()
    {
        if (health >0) health -= 1;
        beenHitCount++;
        HitShock("long");
        StartCoroutine(FlashScreen(hitScreen)); //闪烁屏幕
        textOutput.SendMessage("MsgShow", "Ship damaged");
    }

    void DestroyItemsByParent(GameObject parent ,bool needVFX=false) //清除某个根物体下所有子物体
    {
        foreach (Transform tr in parent.transform)
        {
            if (needVFX) MyFunctions.GenVFX(explodeVFX, tr.position);
            Destroy(tr.gameObject);
        }
    }

    void DestroyAnswer()
    {
        DestroyItemsByParent(numParent);
    }

    void SetSensetivity(string msg) 
    {
        float recSen = float.Parse(msg);
        sensetivity = recSen;
        PlayerPrefs.SetString("Sensetivity", sensetivity.ToString()); //用string是为了方便统一读取
        PlayerPrefs.Save();
        Debug.Log("rcv sen:" + recSen.ToString());
    }

    void SetSettings(string msg)
    {
        switch (msg)
        {
            case "Set Touch Mode":
                controlMode = "touch";
                PlayerPrefs.SetString("ControlMode", controlMode);
                PlayerPrefs.Save();
                break;
            case "Set Gyro Mode":
                controlMode = "gyro";
                PlayerPrefs.SetString("ControlMode", controlMode);
                PlayerPrefs.Save();
                break;
            case "Set Left Hand":
                buttonSide = "Left";
                PlayerPrefs.SetString("ButtonSide", buttonSide);
                PlayerPrefs.Save();
                break;
            case "Set Right Hand":
                buttonSide = "Right";
                PlayerPrefs.SetString("ButtonSide", buttonSide);
                PlayerPrefs.Save();
                break;
            case "Set Vibrate On":
                vibrateOn = true;
                PlayerPrefs.SetString("VibrateOn", vibrateOn.ToString());
                PlayerPrefs.Save();
                break;
            case "Set Vibrate Off":
                vibrateOn = false;
                PlayerPrefs.SetString("VibrateOn", vibrateOn.ToString());
                PlayerPrefs.Save();
                break;
            case "Set Up":
                isUpControl = true;
                PlayerPrefs.SetString("IsUpControl", isUpControl.ToString());
                PlayerPrefs.Save();
                break;
            case "Set Down":
                isUpControl = false;
                PlayerPrefs.SetString("IsUpControl", isUpControl.ToString());
                PlayerPrefs.Save();
                break;
            case "NotFirstPlay":
                isFirstPlay = false;
                PlayerPrefs.SetString("IsFirstPlay", isFirstPlay.ToString());
                PlayerPrefs.Save();
                break;
        }
        Debug.Log("gamecontrol rcv:" + msg);
        PlayerPrefs.Save();
    }

}
