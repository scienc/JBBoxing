﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Image))]
public class UGUISpriteAnimation : MonoBehaviour {
    private Image ImageSource;
    private int mCurFrame = 0;
    private float mDelta = 0;

    public float FPS = 5;
    public List<Sprite> SpriteFrames;
    public bool IsPlaying = false;
    public bool Foward = true;
    public bool AutoPlay = false;
    public bool Loop = false;

    public int FrameCount {
        get {
            return SpriteFrames.Count;
        }
    }

    void Awake () {
        ImageSource = GetComponent<Image> ();
    }

    void Start () {
        if (AutoPlay) {
            Play ();
        } else {
            IsPlaying = false;
        }
    }

    private void SetSprite (int idx) {
        ImageSource.sprite = SpriteFrames[idx];
        //该部分为设置成原始图片大小，如果只需要显示Image设定好的图片大小，注释掉该行即可。
        ImageSource.SetNativeSize ();
    }

    public void Play () {
        IsPlaying = true;
        Foward = true;
        ImageSource.enabled = true;
    }

    public void PlayReverse () {
        IsPlaying = true;
        Foward = false;
    }

    void Update () {
        if (!IsPlaying || 0 == FrameCount) {
            return;
        }

        mDelta += Time.deltaTime;
        if (mDelta > 1 / FPS) {
            mDelta = 0;
            if (Foward) {
                mCurFrame++;
            } else {
                mCurFrame--;
            }

            if (mCurFrame >= FrameCount) {
                if (Loop) {
                    mCurFrame = 0;
                } else {
                    Stop ();
                    return;
                }
            } else if (mCurFrame < 0) {
                if (Loop) {
                    mCurFrame = FrameCount - 1;
                } else {
                    Stop ();
                    return;
                }
            }
            SetSprite (mCurFrame);
        }
    }

    public void Pause () {
        IsPlaying = false;
    }

    public void Resume () {
        if (!IsPlaying) {
            IsPlaying = true;
        }
    }

    public void Stop () {
        mCurFrame = 0;
        SetSprite (mCurFrame);
        IsPlaying = false;
        ImageSource.enabled = false;
    }

    public void Rewind () {
        mCurFrame = 0;
        SetSprite (mCurFrame);
        Play ();
    }
}