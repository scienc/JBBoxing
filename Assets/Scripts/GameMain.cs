using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour {
    public JBBoxingMain boxingNet;
    public enum BOXINGSTATE {
        IDEL,
        READY,
        GAME,
        LOSE,
        WIN,
    }

    public Text timeTex;
    public Text scoreTex;
    public Text waterLineTex;

    public Animation waterLineAnim;

    public Transform waterImage;
    public Transform waterArrow;

    public GameObject IdelPage;
    public Animation IdelAnim;

    public GameObject ReadyPage;
    public Animation readyAnim;

    public GameObject WinPage;
    public GameObject LosePage;

    private int currentPowerNum = 0;
    private int currentWaterLine = 0;
    private int currentTime = 0;
    private int gameTime = 30;
    private int subWaterLine = 20;
    private int gameWaterLine = 600;
    private BOXINGSTATE boxState = BOXINGSTATE.IDEL;

    private int waterMinY = -440;
    private int waterMaxY = -198;
    private float waterOffsetY = 0.4f;
    private float waterArrowMinY = -400;

    private Queue<int> receivePower = new Queue<int> ();

    private DOTween waterDT;
    private DOTween waterArrowDT;

    private Sequence mWaterScoreSeq;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake () {
        boxingNet.delegatePower = ReceiveBoxing;
        InitPage ();
        mWaterScoreSeq = DOTween.Sequence ();
        mWaterScoreSeq.SetAutoKill (false);
    }

    void Update () {
        if (receivePower.Count > 0) {
            ExecturePower (receivePower.Dequeue ());
        }
    }

    public void InitPage () {
        currentPowerNum = 0;
        currentTime = 0;
        currentWaterLine = 0;
        timeTex.text = "00:30";
        scoreTex.text = currentPowerNum.ToString ();
        boxState = BOXINGSTATE.IDEL;
        ChangePage (BOXINGSTATE.IDEL);
        waterLineTex.text = currentWaterLine.ToString ();
        DOTween.KillAll (true);
        waterImage.localPosition = new Vector3 (0, waterMinY, 0);
        waterArrow.localPosition = new Vector3 (776, waterArrowMinY, 0);
        waterLineAnim.Stop ();
        waterLineAnim.transform.localPosition = Vector3.zero;
        IdelAnim.Play ("IdleLoop");
    }

    private void ChangePage (BOXINGSTATE state) {
        IdelPage.SetActive (state == BOXINGSTATE.IDEL);
        ReadyPage.SetActive (state == BOXINGSTATE.READY);
        LosePage.SetActive (state == BOXINGSTATE.LOSE);
        WinPage.SetActive (state == BOXINGSTATE.WIN);
    }

    public void ReceiveBoxing (int power) {
        receivePower.Enqueue (power);
    }

    private void ExecturePower (int power) {
        if (boxState == BOXINGSTATE.IDEL) {
            ReadyGame ();
        } else if (boxState == BOXINGSTATE.GAME) {
            currentPowerNum += 1;
            scoreTex.text = currentPowerNum.ToString ();
            int oldVlaue = currentWaterLine;
            currentWaterLine += (power * 2);
            RefreshWaterLine (oldVlaue, currentWaterLine);

        }
    }

    public void ReadyGame () {
        boxState = BOXINGSTATE.READY;
        StartCoroutine (PlayReadyEffect ());
    }
    IEnumerator PlayReadyEffect () {
        IdelAnim.Play ("IdleBegin");
        yield return new WaitForSeconds (1);
        ChangePage (boxState);
        readyAnim.Play ("readyAnim");
        yield return new WaitForSeconds (3.5f);
        BeginGame ();
    }

    public void BeginGame () {
        InvokeRepeating ("RefreshTime", 0, 1.0f);
        InvokeRepeating ("SubWaterLine", 1, 30.0f);
        boxState = BOXINGSTATE.GAME;
        ChangePage (boxState);
        waterLineAnim.Play ("waterLineAnim1");
    }
    public void SubWaterLine () {
        int oldVlaue = currentWaterLine;
        if (currentWaterLine - subWaterLine < 0) {
            currentWaterLine = 0;
        } else {
            currentWaterLine -= subWaterLine;
        }
        if (oldVlaue != currentWaterLine)
            RefreshWaterLine (oldVlaue, currentWaterLine);
    }
    public void RefreshTime () {
        if (currentTime >= gameTime) {
            GameEnd (false);
            return;
        }
        currentTime += 1;
        TimeSpan ts = TimeSpan.FromSeconds ((gameTime - currentTime));
        timeTex.text = ToStringMinute (ts);
    }
    public void RefreshWaterLine (int oldvalue, int newvalue) {
        if (currentWaterLine >= gameWaterLine) {
            currentWaterLine = gameWaterLine;
            waterLineTex.text = currentWaterLine.ToString ();
            GameEnd (true);
            return;
        }
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~");
        Debug.Log("oldvalue " + oldvalue + " newValue " + newvalue);
        mWaterScoreSeq.Append (DOTween.To (delegate (float value) {            
            waterLineTex.text = Mathf.CeilToInt(value).ToString();
        }, oldvalue, newvalue, 0.4f));
        //waterLineTex.text = currentWaterLine.ToString ();
        waterImage.DOLocalMoveY (waterMinY + currentWaterLine * waterOffsetY, 0.2f);
        waterArrow.DOLocalMoveY (waterArrowMinY + currentWaterLine * waterOffsetY, 0.2f);
    }
    public void GameEnd (bool win) {
        CancelInvoke ("RefreshTime");
        CancelInvoke ("SubWaterLine");
        boxState = win?BOXINGSTATE.WIN : BOXINGSTATE.LOSE;
        ChangePage (boxState);
        waterLineAnim.Stop ();
        StartCoroutine (GameEndEffect ());
    }

    IEnumerator GameEndEffect () {
        yield return new WaitForSeconds (3.0f);
        InitPage ();
    }

    public string ToStringMinute (TimeSpan span) {
        TimeSpan t = span;
        if (t.TotalSeconds < 0) {
            return "00:00";
        } else {
            var format = "{0:D2}:{1:D2}";
            return string.Format (format, (int) t.TotalMinutes, t.Seconds);
        }
    }
}