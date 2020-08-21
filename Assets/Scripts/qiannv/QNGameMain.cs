using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QNGameMain : MonoBehaviour {
    public JBBoxingMain boxingNet;
    public enum GAMESTATE {
        LOADING,
        BEGIN,
        GAME,
        DEAD,
    }

    public LoadingPage loadingPage;
    public GameObject beginPage;
    public Image beginImage;
    public GameObject gamePage;

    public List<BossControll> bossControll = new List<BossControll> ();
    private GAMESTATE gameState = GAMESTATE.BEGIN;
    private Queue<int> receivePower = new Queue<int> ();

    private bool isReceive = false;

    public int currentBossIndex = 0;
    private BossControll currentbossControll = null;

    void Awake () {
        boxingNet.delegatePower = ReceiveBoxing;
        isReceive = false;
        currentbossControll = bossControll[currentBossIndex];
        currentbossControll.delegateBossDeadEvent = () => {
            gameState = GAMESTATE.DEAD;
            isReceive = false;
        };
        currentbossControll.delegateBossDeadFinish = () => {
            GameIdel ();
        };
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
        GameIdel ();
    }

    public void ReceiveBoxing (int power) {
        if (isReceive)
            receivePower.Enqueue (power);
    }

    private void ExecturePower (int power) {
        if (gameState == GAMESTATE.BEGIN) {
            ReadyGame ();
        } else if (gameState == GAMESTATE.GAME) {
            currentbossControll.Attack (power);
        }
    }

    void Update () {
        if (receivePower.Count > 0) {
            ExecturePower (receivePower.Dequeue ());
        }
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }
        // if (Input.GetKeyDown (KeyCode.Q)) {
        //     ChangeBoss (0);
        // } else if (Input.GetKeyDown (KeyCode.W)) {
        //     ChangeBoss (1);
        // }
        if (Input.GetKeyDown (KeyCode.Space)) {
            float rd = Random.Range (1000, 2000) * 0.001f;
            int power = Mathf.CeilToInt (rd * 30);
            ExecturePower (power);
        }
    }

    public void ChangeBoss (int index) {
        currentBossIndex = 0;
        currentbossControll = bossControll[currentBossIndex];
    }

    public void GameIdel () {
        isReceive = false;
        gameState = GAMESTATE.BEGIN;
        float timeLength = 1.0f;
        loadingPage.gameObject.SetActive (true);
        loadingPage.ChangePage (timeLength);
        loadingPage.delegateEnterFinsih = () => {
            beginPage.gameObject.SetActive (true);
            gamePage.gameObject.SetActive (false);
        };
        loadingPage.delegateLeaveFinsih = () => {
            receivePower.Clear ();
            isReceive = true;
            loadingPage.gameObject.SetActive (false);
        };
    }

    public void ReadyGame () {
        gameState = GAMESTATE.LOADING;
        beginImage.transform.DOShakeScale(0.3f,0.5f);
        isReceive = false;
        float timeLength = 0.0f;
        int random = Random.Range (1, 100);
        if (random < 50) {
            timeLength = AudioManager.PlaySe ("open1");
        } else {
            timeLength = AudioManager.PlaySe ("open2");
        }
        loadingPage.gameObject.SetActive (true);
        loadingPage.ChangePage (timeLength);
        loadingPage.delegateEnterFinsih = () => {
            beginPage.gameObject.SetActive (false);
            gamePage.gameObject.SetActive (true);
            currentbossControll.InitBoss ();
        };
        loadingPage.delegateLeaveFinsih = () => {
            isReceive = true;
            gameState = GAMESTATE.GAME;
        };
    }
}