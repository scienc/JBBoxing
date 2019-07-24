using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour {
    public JBBoxingMain boxingNet;
    public enum BOXINGSTATE {
        IDEL,
        READY,
        GAME,
        END,
    }

    public Text timeTex;
    public Text scoreTex;
    public Text stateTex;
    public Text waterLineTex;
    private int currentPowerNum = 0;
    private int currentWaterLine = 0;
    private int currentTime = 0;
    private int gameTime = 30;
    private int subWaterLine = 20;
    private int gameWaterLine = 600;
    private BOXINGSTATE boxState = BOXINGSTATE.IDEL;

    private Queue<int> receivePower = new Queue<int> ();

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake () {
        boxingNet.delegatePower = ReceiveBoxing;
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
        timeTex.text = "Time : 00:30";
        scoreTex.text = "Num : " + currentPowerNum.ToString ();
        stateTex.text = "State : idel";
        boxState = BOXINGSTATE.IDEL;
        waterLineTex.text = "Water Line : " + currentWaterLine.ToString ();
    }

    public void ReceiveBoxing (int power) {
        receivePower.Enqueue (power);
    }

    private void ExecturePower (int power) {
        if (boxState == BOXINGSTATE.IDEL) {
            ReadyGame ();
        } else if (boxState == BOXINGSTATE.GAME) {
            currentPowerNum += 1;
            scoreTex.text = "Num : " + currentPowerNum;
            currentWaterLine += power;
            RefreshWaterLine ();
        }
    }

    public void ReadyGame () {
        boxState = BOXINGSTATE.READY;
        StartCoroutine (PlayReadyEffect ());
    }
    IEnumerator PlayReadyEffect () {
        stateTex.text = "3";
        yield return new WaitForSeconds (1);
        stateTex.text = "2";
        yield return new WaitForSeconds (1);
        stateTex.text = "1";
        yield return new WaitForSeconds (1);
        BeginGame ();
    }

    public void BeginGame () {
        stateTex.text = "Play Game";
        InvokeRepeating ("RefreshTime", 0, 1.0f);
        InvokeRepeating ("SubWaterLine", 1, 5.0f);
        boxState = BOXINGSTATE.GAME;
    }
    public void SubWaterLine () {
        if (currentWaterLine - subWaterLine < 0) {
            currentWaterLine = 0;
        } else {
            currentWaterLine -= subWaterLine;
        }
        RefreshWaterLine ();
    }
    public void RefreshTime () {
        if (currentTime >= gameTime) {
            GameEnd ();
            return;
        }
        currentTime += 1;
        TimeSpan ts = TimeSpan.FromSeconds ((gameTime - currentTime));
        timeTex.text = "Time : " + ToStringMinute (ts);
    }
    public void RefreshWaterLine () {
        if (currentWaterLine >= gameWaterLine) {
            currentWaterLine = gameWaterLine;
            waterLineTex.text = currentWaterLine.ToString ();
            GameEnd ();
            return;
        }
        waterLineTex.text = "water line : " + currentWaterLine.ToString ();
    }
    public void GameEnd () {
        CancelInvoke ("RefreshTime");
        CancelInvoke ("SubWaterLine");
        boxState = BOXINGSTATE.END;
        stateTex.text = "Game End";
        StartCoroutine (GameEndEffect ());
    }

    IEnumerator GameEndEffect () {
        yield return new WaitForSeconds (1.0f);
        stateTex.text = "Game End 3";
        yield return new WaitForSeconds (1.0f);
        stateTex.text = "Game End 2";
        yield return new WaitForSeconds (1.0f);
        stateTex.text = "Game End 1";
        yield return new WaitForSeconds (1.0f);
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