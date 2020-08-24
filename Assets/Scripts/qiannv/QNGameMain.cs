using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
    private Queue<PowerData> receivePower = new Queue<PowerData> ();

    private bool isReceive = false;

    public int currentBossIndex = 0;
    private BossControll currentbossControll = null;

    public UGUISpriteAnimation koAnim;
    public UGUISpriteAnimation bkgAnim;

    public float difficult = 1.0f;
    public int bossMaxHpBase = 100;
    public int currentbossMaxHpOffset = 0;

    void Awake () {
        boxingNet.delegatePower = ReceiveBoxing;
        isReceive = false;
        ChangeBoss (currentBossIndex);
        //koAnim.Stop();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
        GameIdel ();
    }

    public void ReceiveBoxing (PowerData data) {
        if (isReceive)
            receivePower.Enqueue (data);
    }

    private void ExecturePower (PowerData power) {
        if (gameState == GAMESTATE.BEGIN) {
            ReadyGame ();
        } else if (gameState == GAMESTATE.GAME) {
            power.value = power.value / difficult;
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

        if (Input.GetKeyDown (KeyCode.Space)) {
            float rd = Random.Range (1000, 2000) * 0.001f;
            float power = rd * 30;
            int range = Random.Range (0, 2);
            PowerData data = new PowerData ();
            data.value = power;
            data.direction = range;
            ExecturePower (data);
        }

        if (gameState == GAMESTATE.BEGIN) {
            if (Input.GetKeyDown (KeyCode.Q)) {
                ChangeBoss (0);
            } else if (Input.GetKeyDown (KeyCode.W)) {
                ChangeBoss (1);
            } else if (Input.GetKeyDown (KeyCode.E)) {
                ChangeBoss (2);
            } else if (Input.GetKeyDown (KeyCode.R)) {
                ChangeBoss (3);
            }
        }

        if (Input.GetKeyDown (KeyCode.Alpha1)) {
            currentbossMaxHpOffset = 1;
        } else if (Input.GetKeyDown (KeyCode.Alpha2)) {
            currentbossMaxHpOffset = 2;
        } else if (Input.GetKeyDown (KeyCode.Alpha3)) {
            currentbossMaxHpOffset = 3;
        } else if (Input.GetKeyDown (KeyCode.Alpha4)) {
            currentbossMaxHpOffset = 4;
        } else if (Input.GetKeyDown (KeyCode.Alpha5)) {
            currentbossMaxHpOffset = 5;
        } else if (Input.GetKeyDown (KeyCode.Alpha6)) {
            currentbossMaxHpOffset = 6;
        } else if (Input.GetKeyDown (KeyCode.Alpha7)) {
            currentbossMaxHpOffset = 7;
        } else if (Input.GetKeyDown (KeyCode.Alpha8)) {
            currentbossMaxHpOffset = 8;
        } else if (Input.GetKeyDown (KeyCode.Alpha9)) {
            currentbossMaxHpOffset = 9;
        } else if (Input.GetKeyDown (KeyCode.Alpha0)) {
            currentbossMaxHpOffset = 0;
        }
    }

    public void ChangeBoss (int index) {
        currentBossIndex = index;
        currentbossControll = bossControll[currentBossIndex];
        for (int i = 0; i < bossControll.Count; i++) {
            bossControll[i].gameObject.SetActive (false);
        }
        currentbossControll.gameObject.SetActive (true);
        currentbossControll.delegateBossDeadEvent = () => {
            gameState = GAMESTATE.DEAD;
            isReceive = false;
        };
        currentbossControll.delegateBossDeadFinish = () => {
            GameIdel ();

        };
        currentbossControll.delegateKO = () => {
            koAnim.Play ();
        };
    }

    public void GameIdel () {
        bkgAnim.Pause ();
        isReceive = false;
        gameState = GAMESTATE.BEGIN;
        //float timeLength = 3.0f;

        StartCoroutine (loadingPage.CloudClose (0)); //
        loadingPage.delegateCloseFinsih = () => {
            beginPage.gameObject.SetActive (true);
            gamePage.gameObject.SetActive (false);
        };
        StartCoroutine (loadingPage.CloudOpen (2)); //
        loadingPage.delegateOpenFinsih = () => {
            receivePower.Clear ();
            isReceive = true;
        };

        //loadingPage.gameObject.SetActive(true);
        //loadingPage.ChangePage(timeLength);
        //loadingPage.delegateEnterFinsih = () =>
        //{
        //    beginPage.gameObject.SetActive(true);
        //    gamePage.gameObject.SetActive(false);
        //};
        //loadingPage.delegateLeaveFinsih = () =>
        //{
        //    receivePower.Clear();
        //    isReceive = true;
        //    loadingPage.gameObject.SetActive(false);
        //};

        koAnim.Stop ();
    }

    public void ReadyGame () {
        gameState = GAMESTATE.LOADING;
        beginImage.transform.DOShakeScale (0.3f, 0.5f);
        isReceive = false;
        float timeLength = 0.0f;
        // int random = Random.Range (1, 100);
        // if (random < 50) {
        //     timeLength = AudioManager.PlaySe ("open1");
        // } else {
        //     timeLength = AudioManager.PlaySe ("open2");
        // }
        timeLength = AudioManager.PlaySe ("begin");

        StartCoroutine (loadingPage.CloudClose (0)); //        
        loadingPage.delegateCloseFinsih = () => {
            beginPage.gameObject.SetActive (false);
            gamePage.gameObject.SetActive (true);
            currentbossControll.InitBoss (bossMaxHpBase * (1 + currentbossMaxHpOffset));
            bkgAnim.Play ();
        };
        StartCoroutine (loadingPage.CloudOpen (2)); //
        loadingPage.delegateOpenFinsih = () => {
            isReceive = true;
            gameState = GAMESTATE.GAME;
        };

        //loadingPage.gameObject.SetActive(true);
        //loadingPage.ChangePage(timeLength);
        //loadingPage.delegateEnterFinsih = () =>
        //{
        //    beginPage.gameObject.SetActive(false);
        //    gamePage.gameObject.SetActive(true);
        //    currentbossControll.InitBoss();
        //    bkgAnim.Play();
        //};
        //loadingPage.delegateLeaveFinsih = () =>
        //{
        //    isReceive = true;
        //    gameState = GAMESTATE.GAME;
        //};
    }
}