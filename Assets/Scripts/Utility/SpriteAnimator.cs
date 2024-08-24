using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    //Animatorを使わずに画像を切り替え
    //描画するためのRenderer\
    SpriteRenderer spriteRenderer;
    //表示するための画像たち
    List<Sprite> frames;
    public List<Sprite> Frames { get => frames; }
    //フレームレート:どのタイミングで画像を変えるのか
    float frameRate;
    float timer;
    //現在のフレーム
    int currentFrame;

    public SpriteAnimator(SpriteRenderer spriteRenderer, List<Sprite> sprites, float frameRate = 0.16f)
    {
        this.spriteRenderer = spriteRenderer;
        this.frames = sprites;
        this.frameRate = frameRate;
    }

    //アニメーション開始
    public void Start()
    {
        currentFrame = 0;
        timer = 0;
        spriteRenderer.sprite = frames[currentFrame];
    }
    //アニメーションを更新する:timerがframeRateを超えたら次の画像
    public void HundleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count; 
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }

}
