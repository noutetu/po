using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    //モンスターはBattleSystemから受け取る
   [SerializeField] bool isPlayerUnit;
   [SerializeField] BattleHud hud;

    public bool IsPlayerUnit { get => isPlayerUnit;}
    public BattleHud Hud { get => hud;}
    public Pokemon Pokemon { get; set; }

    Vector3 orginalPos;
    Color originalColor;
    Image image;

    //バトルで使うモンスターを保持
    //モンスターの画像を反映する

    private void Awake() {
        orginalPos = transform.localPosition;
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void SetUp(Pokemon pokemon)
    {
        //_baseからレベルに応じたモンスターを生成する
        // BattleSystemで使うからプロパティに入れる

        Pokemon = pokemon;

        
        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }

        if (!isPlayerUnit)
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }
        hud.SetData(pokemon);
        hud.gameObject.SetActive(true);
        image.color = originalColor;
        PlayerEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }


    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
        {
            transform.localPosition = new Vector3(-750,orginalPos.y);   
        }
        else
        {
            transform.localPosition = new Vector3(750,orginalPos.y);   
        }

        transform.DOLocalMoveX(orginalPos.x,1f);
    }
    //攻撃anim
    public void PlayerAttackAnimation()
    {
        //シーケンス
        //右に動いた後、元の位置に戻る

        Sequence sequence = DOTween.Sequence();

        if(isPlayerUnit)
        {
            sequence.Append(transform.DOLocalMoveX(orginalPos.x+50,0.25f));//後ろに追加
        }
        else
        {
            sequence.Append(transform.DOLocalMoveX(orginalPos.x-50,0.25f));//後ろに追加
        }
        sequence.Append(transform.DOLocalMoveX(orginalPos.x,0.20f));//
    }

    // ダメージAnim
    public void PlayerHitAnimation()
    {
        // 色を一度GLAYにしてから戻す
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayerFaintAnimation()
    {
        //下に下がりながら薄くなる
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(orginalPos.y -150,0.5f));
        sequence.Join(image.DOFade(0,0.5f));
    }
}
