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
    //�����ǳ�ʼ��Ϸ��������
    [Header("===�����ǳ�ʼ��Ϸ��������====")]
    public GameObject mainGameObj;//���ض��󣬼��ɴ����
    public GameObject spaceShip;//�ɴ�
    public GameObject gameOverScreen; //��������
    public GameObject gameRunScreen;//��Ϸ�л���
    public GameObject gameStartScreen;//��Ϸǰ����
    public GameObject guideScreen;
    public GameObject newFeature;
    public GameObject cover; //�ڸ�
    public GameObject tips;

    public GameObject helpScreen;
    public GameObject helpButton;
    public GameObject hitScreen;//������Ļ
    public GameObject hpScreen;//��Ѫ��Ļ
    public GameObject difficultScreen;//�Ѷ���Ļ
    public GameObject settingScreen;//������Ļ
    public GameObject levelUpScreen;//������Ļ
    public List<GameObject> obPrefabs;//�ϰ���Ԥ����
    public List<GameObject> enemyPrefabs;
    public GameObject enemyParent;
    public GameObject obParent;//�ϰ���Ԥ���常����
    public GameObject tempObParent;//��ը�����ʯ��ŵ�
    public GameObject tipParent;//�򵼸�����
    public GameObject numTipPrefab;
    public GameObject enemyTipPrefab;

    public List<GameObject> hpPrefabs;//Ѫ��Ԥ����
    public GameObject hpParent;//Ѫ��Ԥ���常����
    public List<GameObject> numberPrefabs;//����Ԥ����
    public GameObject numParent;//���ָ�����
    public GameObject questionCountDownParent;
    public GameObject questionCountDownBar;//���⵹��ʱ����
    public GameObject reloadButtonBoard; //������ť��Ե����˸�ã�

    public GameObject spawnVFX;
    public GameObject textOutput;
    public UnityEngine.UI.Text questionCountDownText;
    public UnityEngine.UI.Text difficultStatusText;//�Ѷ���ʾ����
    public UnityEngine.UI.Image difficultStatusBg;//
    //public UnityEngine.UI.Text difficultButtonText; //�Ѷ����ð�ť����
    //public UnityEngine.UI.Image difficultButtonColor;//�ѶȰ�ť����ɫ
    public UnityEngine.UI.Image questionBgColor;//���ⱳ����ɫ
    //public AudioSource startBgm;
    //public AudioSource runningBgm;
    public AudioSource reloadSound;
    public AudioSource hpAddSound;
    public GameObject explodeVFX;

    //������Ĭ�ϳ�ʼ��������
    [Header("====������Ĭ�ϳ�ʼ��������====")]
    public int sceneWidth = 25;
    public int sceneHeight = 25;
    public int sceneDeep = 25;
    public int obCount = 10;//�ϰ���Ĭ������
    public int maxObCount = 50;//����ϰ�������
    private int addObCount;
    public int hpCount = 15;//Ѫ��Ĭ������
    public int addBodyInter = 99999999;//5���һ������
    static public float distance;
    
    static public float sensetivity = 1;//����������
    public int maxAnswer = 20;//20���ڼӼ���
    public int minAnswer;
    static public int health = 10;

    static public int maxBodyCount = 40;//���������
    static public int maxAnswerCount = 5;//��ѡ�𰸸���
    public int questionRefreshTime = 10;//�����Ѷ���Ŀˢ���ٶ�

    static public string gameMode = "free";//Ĭ��������ģʽ
    private int currentLevel = 0; //��ʼ�ؿ�
    private int currentObCount;
    //������Щ������Ҫ�������������
    [Header("====������Щ������Ҫ�������������====")]
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
    static public string controlMode = "touch";//touch����ָ�϶�  gyro��������
    static public string difficult = "��";
    static public bool vibrateOn = true;
    static public string buttonSide = "right"; //������
    static public bool isUpControl = true;//���·������
    static public bool isFirstPlay = true;//�ǲ��ǵ�һ���棿
    static public string gameLevel;//��ǰ�ؿ�
    static public int levelUpScore;//�����������
    static public bool isTooSlow;
    static public int ammoMount;
    static public int rockHitTimes;//������ʯ���β��ܴ�
    static public Transform oriTransform;
    static public bool isQuestionGen = false; //��������������
    static public int enemyCount;
    static public int enemyMaxSpeed;
    static public float enemyShootWaitMinTime;//���ʱ����С���
    static public float shotMaxBias;//������ƫ��
    static public int requestAmmoAva = 0;//�������벹������

    private Vector3 lastPosition = new Vector3(0, 0, 0);
    private bool hpSpawn;
    private float lastSpawnTime = 0f;//hp���������ʼʱ���
    private float lastEnemyDieTimeStamp; //��������ʱ���
    private float enemySpawnTime;
    private float questionTimer;//��Ŀ��ʾʱ���ʱ������Ե����Ѷ�
    private bool alreadyShowNewFeature; //�Ѿ���ʾ��������ҳ�棿

    private float pauseTimeStamp;
    private List<int> randomAnswers = new();
    
    
    private int addAmmo;
    private int scoreAdd; 

    private int rockHitCount = 0; //������ʯ����
    private int enemyHitCount = 0; //���зɴ�����
    private int ammoFiredCount= 0; //�������
    private int beenHitCount = 0; //�����д���
    private int requestAmmoCount = 0; //���벹������
    
    private float travelDisCount = 0; //���о���

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
    void GetSettings() //��ȡ�趨
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
        health = 10;//��ʼѪ��
        score = 0;
        gameStatus = 1; // 0 ���� gameover 1 �������������� 2���������� 4������ͣ
        gameLevel = "��1��";
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
        if (gameStatus == 2)//������
        {
            if (health <= 0 )
            {
                gameOverReason = "�����ľ�";
                GameOver();
                Debug.Log("GAME OVER");
            }
            
            if (Time.time - questionTimer > questionRefreshTime) //�����Ѷ�ÿ10�����³���һ��
            {
                DestroyAnswer();
                PropSpawn();//���³���
               if (currentObCount < maxObCount)
                    StartCoroutine(SpawnOb(addObCount, obPrefabs, obParent, "�ϰ���_", true,maxRange:40f));
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
                CheckGameLevel();//����ģʽ
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
            HpSpawn();//����Ѫ��
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
                if (t.name == "��_0") return;
            }
            DestroyAnswer(); //����������ɱ�־���ڣ����Ǵ�0��ʧ�ˣ���ô������������ʹ𰸡�
            PropSpawn();
        }
    }


    void EnemySpawn(int enemyCount)
    {
        StartCoroutine(SpawnOb(enemyCount, enemyPrefabs, enemyParent, "����_", false,minRange:20f,maxRange:30f,tipPrefab:enemyTipPrefab));
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
        //BodyHandle("removeAllBody"); //������ȥ�����е��ӷɴ�
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

    void PropSpawn() //��������
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
        int num1 = Random.Range(1, answer - 1); //�������+0 ������� 
        int num2;
        if (choice == 0) //����Ϊ��
        {
            num2 = answer - num1; 
        }
        else //����Ϊ��
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
        StartCoroutine(SpawnOb(maxAnswerCount, numberPrefabs, numParent, "��_", true,maxRange:30f,tipPrefab:numTipPrefab));
        //ChangeAnswerText(randomAnswers);//�˴���Э�̽����ӳٲ������������滹û����������Ͳ�������
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
        if (health <= 5 && !hpSpawn && Time.time - lastSpawnTime > 10) //��������һ��ɾ������Ѫ����10����������µ�Ѫ��
        {
            StartCoroutine(SpawnOb(hpCount, hpPrefabs,hpParent, "Ѫ��_", false));
            hpSpawn = true;
            lastSpawnTime = Time.time;
            //hpSpawnFx.Play();
            textOutput.SendMessage("MsgShow", "Hp packs are out");
        }
        if (health > 10 )
        {
            //��Ѫ������10ʱ����30�����ɾ������Ѫ��
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

    void BodyHandle(string mode) //֪ͨ��ͷ����������
    {
        mainGameObj.SendMessage("MsgHandle", mode);
    }

    void NewGame() //��ʾ��Ϸģʽѡ��ҳ��
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
        gameLevel = "��1��";
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
        
        //GameObject.Find("��").GetComponent<Guide>().enabled = true; //����GUIDE
        difficultScreen.SetActive(false);
        gameStatus = 2;
        //PropSpawn();//���ɵ���
        questionTimer = Time.time;
        lastEnemyDieTimeStamp = Time.time;
        if (gameMode =="practice")
            StartCoroutine(SpawnOb(obCount,obPrefabs,obParent, "�ϰ���_",true));
    }

    IEnumerator SpawnOb(int obCount,List<GameObject> prefabs,GameObject parent, string pre ,bool randomColor,float maxRange = 20f,float minRange = 5f,GameObject tipPrefab =null)
    {
        for (int i = 0;i < obCount;i++)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            Vector3 position = new Vector3(0,0,0) + randomDirection * Random.Range(minRange,maxRange); //ʼ�ջ���ԭ��

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
        //Rigidbody r = mainGameObj.GetComponent<Rigidbody>(); //ֹͣ��ͷ�Ҷ�
        spaceShip.transform.position = oriTransform.position;
        spaceShip.transform.forward = new Vector3(0,0,1);
        mainGameObj.transform.position = oriTransform.position;
        mainGameObj.transform.forward = new Vector3(0, 0, 1);
        rockHitCount = 0; //������ʯ����
        enemyHitCount = 0; //���зɴ�����
        ammoFiredCount = 0; //�������
        beenHitCount = 0; //�����д���
        requestAmmoCount = 0; //���벹������
        travelDisCount = 0; //���о���
        requestAmmoAva = 0;
}

    void MsgHandle(string msg) //ͨ����Ϣ������
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
                    if (!isQuestionGen) PropSpawn(); //��ҩ�ľ�����
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
                if (!isQuestionGen) PropSpawn(); //��ҩ�ľ�����
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
        score += scoreAdd * 2; //�ɴ���2��
        enemyHitCount++;
        requestAmmoAva += 2;
        string msg = "You have shot down " + enemyHitCount.ToString() + " enemy ship(s)";
        int count = enemyParent.transform.childCount - 1;
        //if (count > 0) msg = msg + ",there are " + count.ToString() + "��"; else msg = msg + "\nAll clear for now.";
        textOutput.SendMessage("MsgShow", msg);
    }


    void CheckRock()
    {
        int currentObCount = obParent.transform.childCount;
        if (currentObCount < 2)
        {
            int count = Random.Range(3, addObCount);
            StartCoroutine(SpawnOb(count, obPrefabs, obParent, "�ϰ���_", true));
            //textOutput.SendMessage("MsgShow", "��ʯ������");
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
        gameLevel = "��1��";
        if (gameMode == "level") difficultText = gameLevel; else difficultText = difficult;
        //SetDifficultButtonText(difficultText);
        //SetDifficultdColor(easyColor);
        questionRefreshTime = 999999;

        maxAnswer = 10;
        minAnswer = 1;
        maxAnswerCount = 3;//��ѡ��3��
        addObCount = 3;//һ�����2���ϰ���
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
        gameLevel = "��1��";
        if (gameMode == "level") difficultText = gameLevel; else difficultText = difficult;
        //SetDifficultButtonText(difficultText);
        //SetDifficultdColor(easyColor);
        questionRefreshTime = 999999;
        
        maxAnswer = 10;
        minAnswer = 1;
        addBodyInter = 9999999;
        maxAnswerCount = 3;//��ѡ��3��
        addObCount = 3;//һ�����2���ϰ���
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
        gameLevel = "��2��";
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
        enemySpawnTime = 15;//��5������һ������
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
        gameLevel = "��3��";
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
        enemySpawnTime = 10;//��5������һ������
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
        gameLevel = "��4��";
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
        enemySpawnTime = 5;//��5������һ������
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
        //StartCoroutine(FlashScreen(hpScreen)); //��˸��Ļ
        DestroyItemsByParent(numParent, needVFX:true);
        isQuestionGen = false;
    }

    void WrongAnswer()
    {
        health--;
        //wrongAnswerFx.Play();
        Debug.Log("�����");
        textOutput.SendMessage("MsgShow", "Wrong box");
        //BodyHandle("addbody");
        StartCoroutine(FlashScreen(hitScreen)); //��˸��Ļ
    }

    void HpAdd()
    {
        health++;
        Debug.Log("��Ѫ HP+1");
        textOutput.SendMessage("MsgShow", "HP +1");
        hpAddSound.Play();
        StartCoroutine(FlashScreen(hpScreen)); //��˸��Ļ
    }

    void Collide()
    {
        if (health >0) health -= 1;
        beenHitCount++;
        HitShock("long");
        StartCoroutine(FlashScreen(hitScreen)); //��˸��Ļ
        textOutput.SendMessage("MsgShow", "Ship damaged");
    }

    void DestroyItemsByParent(GameObject parent ,bool needVFX=false) //���ĳ��������������������
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
        PlayerPrefs.SetString("Sensetivity", sensetivity.ToString()); //��string��Ϊ�˷���ͳһ��ȡ
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
