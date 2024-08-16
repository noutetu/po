using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;//戦わせるポケモンをセットする
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }

    //バトルで使うモンスターを保持
    //モンスターの画像を反映する

    public void SetUp()
    {
        //_baseからレベルに応じたモンスターを生成する
        // BattleSystemで使うからプロパティに入れる

        Pokemon = new Pokemon(_base,level);
        

        Image image = GetComponent<Image>();
        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }

        if (!isPlayerUnit)
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }
    }
}
