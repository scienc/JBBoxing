using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossControll : MonoBehaviour {
    public RectTransform bkgHPBar;
    public RectTransform fontHpBar;

    public Image bossImage;
    public Image bossDeadImage;

    public UGUISpriteAnimation hitAnim;
    public UGUISpriteAnimation hit2Anim;
    public UGUISpriteAnimation hitBaseAnim;

    public float currentBossHp;
    public float BossMaxHp = 1000;

    private float bar = 663;

    public int bossIndex = 1; //1武海，2守财奴，3：僵尸，4：四眼鱼

    public System.Action delegateBossDeadEvent;
    public System.Action delegateBossDeadFinish;
    public void InitBoss () {
        currentBossHp = BossMaxHp;
        bkgHPBar.sizeDelta = new Vector2 (bar, 45);
        fontHpBar.sizeDelta = new Vector2 (bar, 45);
        hitAnim.Stop ();
        hit2Anim.Stop ();
        hitBaseAnim.Stop ();
        bossImage.enabled = true;
        bossDeadImage.enabled = false;
    }

    public void Attack (float value) {
        currentBossHp -= value;
        if (currentBossHp <= 0) {
            currentBossHp = 0;
            delegateBossDeadEvent ();
        }
        Debug.Log ("current hp " + currentBossHp);
        float offset = (bar * currentBossHp) / BossMaxHp;
        StartCoroutine (waitForHp (offset));
        int random1 = Random.Range (1, 100);
        if (random1 < 50) {
            AudioManager.PlaySe ("normalHit");
        } else {
            AudioManager.PlaySe ("normalHit2");
        }

        int random = Random.Range (1, 100);
        if (random < 50) {
            hitAnim.Play ();
        } else {
            hit2Anim.Play ();
        }
        AudioManager.PlaySe ("hit-" + bossIndex);
        hitBaseAnim.Play ();
        bossImage.transform.localScale = Vector3.one;
        bossImage.transform.DOShakeScale (0.3f);
    }

    IEnumerator waitForHp (float offset) {
        float oldvalue = fontHpBar.sizeDelta.x;
        float newvalue = offset;
        DOTween.To (delegate (float value) { fontHpBar.sizeDelta = new Vector2 (value, 45); }, oldvalue, newvalue, 0.4f);
        yield return new WaitForSeconds (0.2f);
        DOTween.To (delegate (float value) { bkgHPBar.sizeDelta = new Vector2 (value, 45); }, oldvalue, newvalue, 0.4f);
        yield return new WaitForSeconds (0.5f);
        bkgHPBar.sizeDelta = new Vector2 (offset, 45);
        fontHpBar.sizeDelta = new Vector2 (offset, 45);
        if (currentBossHp == 0) {
            StartCoroutine (BossDead ());
        }
    }

    IEnumerator BossDead () {
        bossImage.enabled = false;
        bossDeadImage.enabled = true;
        bossDeadImage.DOFade (1, 0);
        float waitTime = AudioManager.PlaySe ("success-" + bossIndex);
        bossDeadImage.DOFade (0, 1.0f);
        yield return new WaitForSeconds (Mathf.Max (waitTime, 1.0f));
        if (delegateBossDeadFinish != null) {
            delegateBossDeadFinish ();
        }
    }
}