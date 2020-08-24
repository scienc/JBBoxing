using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPage : MonoBehaviour {
    public Image LeftCloud;
    public Image rightCloud;
    //public float speed = 1.0f;

    public System.Action delegateEnterFinsih;
    public System.Action delegateLeaveFinsih;

    public GameObject cloudOpen;//
    public GameObject cloudClose;//
    public System.Action delegateOpenFinsih;//
    public System.Action delegateCloseFinsih;//

    public IEnumerator CloudOpen(float duration)//
    {
        yield return new WaitForSeconds(duration);//
        //duration = duration / 2;//
        cloudOpen.SetActive(true);
        yield return new WaitForSeconds(2);//
        cloudOpen.SetActive(false);
        delegateOpenFinsih();//
        //StopAllCoroutines();
    }
    public IEnumerator CloudClose(float duration)//
    {
        yield return new WaitForSeconds(duration);//
        //duration = duration / 2;//
        cloudClose.SetActive(true);
        yield return new WaitForSeconds(2);//
        cloudClose.SetActive(false);
        delegateCloseFinsih();//
        //StopAllCoroutines();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
           
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
           
        }
    }



    public void ChangePage (float duration) {
        StartCoroutine (ChangePageWait (duration));
    }

    IEnumerator ChangePageWait (float duration) {
        duration = duration / 2;
        yield return new WaitForSeconds (Enter (duration));
        delegateEnterFinsih ();
        yield return new WaitForSeconds (Leave (duration));
        delegateLeaveFinsih ();
    }

    private float Enter (float duration) {
        //float duration = 1.0f * speed;
        LeftCloud.transform.localPosition = new Vector3 (-1700.0f, 0, 0);
        rightCloud.transform.localPosition = new Vector3 (1600, 0, 0);
        LeftCloud.transform.DOLocalMoveX (-230, duration);
        rightCloud.transform.DOLocalMoveX (400, duration);
        return duration;
    }

    private float Leave (float duration) {
        //float duration = 1.0f * speed;
        LeftCloud.transform.localPosition = new Vector3 (-230.0f, 0, 0);
        rightCloud.transform.localPosition = new Vector3 (400, 0, 0);
        LeftCloud.transform.DOLocalMoveX (-1700, duration);
        rightCloud.transform.DOLocalMoveX (1600, duration);
        return duration;
    }
}