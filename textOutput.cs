using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class textOutput : MonoBehaviour
{
    public GameObject healthObj;
    public GameObject speedObj;
    public GameObject ammoObj;
    public GameObject scoreObj;
    public GameObject questionObj;
    public Text gameOverScoreText;
    public Text scoreText;
    public Text gameOverText;
    public Text questionText;
    public Text msgShowText;
    public Text ammoRequestAvaCountText;
    public float msgLastTime = 2f;

    //public TextMeshProUGUI questionText;
    public GameObject snakeHead;
    private string lastQuestion;

    private int lastScore;
    private int counter;
    private int lastAmmo = 0;
    private int lastHealth = 0;
    private int lastSpeed;
    private float targetScale = 1.5f;
    private float currentScale = 1.0f;
    private string lastMsg;
    private Queue<string> msgQueue =new Queue<string>();
    private float msgTimer;

    void Start()
    {
        //comp = gameControl.GetComponent<gameControl>();
        lastScore = 0;
        counter = 0;
        currentScale = targetScale;
        //questionPos = questionObj.transform.position;
        SetBar(speedObj, 0, maxNum: 80, minNum: 5, needAnimate: false);
        gameOverText.text = "";
    }

    
    
    void Update()
    {
        int speed = (int)gameControl.speed;
        int health = gameControl.health;
        int ammo = gameControl.ammoMount;
        int score = gameControl.score;
        string question = gameControl.questionString;
        string difficult = gameControl.difficult;
        //Debug.Log(questionObj.transform.localPosition.ToString());
        msgTimer += Time.deltaTime;
        gameOverScoreText.text = "Your score is " + score;


        float step = 0.3f/Time.deltaTime; //0.3秒内完成
        float delta = (targetScale - 1.0f) / step;
        ammoRequestAvaCountText.text = "Reload\n" + gameControl.requestAmmoAva.ToString();
        scoreObj.transform.GetChild(0).GetComponent<Text>().text = score.ToString();
        if (score != lastScore)
        {
            scoreObj.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
            if (counter > step)
            {
                scoreObj.transform.localScale = new Vector3(1f,1f,1f);
                counter = 0;
                lastScore = score;
                currentScale = targetScale;
            }
            else
            {
                counter++;
                currentScale -= delta;
                scoreObj.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            }
        }
        if (lastSpeed != speed)
        {
            SetBar(speedObj, speed, maxNum: 80, minNum: 5,needAnimate:false);
            lastSpeed = speed;
        }
        if (lastHealth != health)
        {
            SetBar(healthObj, health, 20, 5,true);
            lastHealth = health;
        }
        if (lastAmmo != ammo)
        {
            SetBar(ammoObj, ammo, 100, 10,true);
            lastAmmo = ammo;
        }

        if (question != lastQuestion)
        {
            //questionText.color = Color.red;
            StartCoroutine(ShowQuestion(question));
            lastQuestion = question;
        }
        //questionText.text = question;//普通text方法
        MsgCheck();
    }

    IEnumerator ShowQuestion(string question)
    {
        string output = "";
        for (int i = 0; i < question.Length; i++)
        {
            string word = question.Substring(i, 1);
            output += word;
            questionText.text = output;
            yield return new WaitForSeconds(0.1f); ; // 等待0.5秒
        }
    }

        
    void SetBar(GameObject obj,int text,int maxNum,int minNum,bool needAnimate)
    {
        Image bar = obj.transform.GetChild(0).GetComponent<Image>();
        float progress = (float)text / (float)maxNum; //int 除 int 还是int...
        progress = Mathf.Clamp(progress, 0f, 1f); // 限制进度在0到1之间
        if (needAnimate) StartCoroutine(AnimatBar(bar, progress)); else bar.fillAmount = progress;

        Text textObj = obj.transform.GetChild(1).GetComponent<Text>();
        textObj.text = text.ToString();
        if (text > maxNum) text = maxNum;
        if (text <= minNum)
        {
            textObj.color = Color.red;
        }
        else
        {
            textObj.color = Color.white;
        }
    }

    public void GameOverMsg(string msg)
    {
        gameOverText.text = msg;
    }


    void MsgCheck()
    {
        if (msgTimer > 2f && msgQueue.Count >0) 
        {
            msgQueue.Dequeue();
            msgTimer = 0;
        }
        string allMsg = "";
        if (msgQueue.Count > 0)
        {
            foreach (var item in msgQueue)
            {
                allMsg += item + "\n";
            }
            
        }
        msgShowText.text = allMsg;
    }

    public  void MsgShow(string msg)
    {
        if (msg == lastMsg) return;
        if (msgQueue.Count >= 3) msgQueue.Dequeue();
        msgQueue.Enqueue(msg);
        msgTimer = 0;
        lastMsg = msg;
    }



    IEnumerator ShowMsg(string msg)
    {
        msgShowText.text = msg;
        yield return new WaitForSeconds(msgLastTime);
        msgShowText.text = "";
    }


    IEnumerator AnimatBar(Image obj,float targetProgress)
    {
        float step = 0.01f;
        float progress = obj.fillAmount;
        while (progress  < targetProgress)
        {
            yield return new WaitForSeconds(0.1f);
            obj.fillAmount = progress;
            progress += step;
        }
        obj.fillAmount = targetProgress;
    }
}
