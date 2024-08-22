using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//やりたいこと；毒のダメージを受ける
//　タイミング; 毒を喰らった方のターン終了時
//　何をする  : 毒ダメージを与える
//---
//Conditionクラスにターン終了時に実行したい関数を用意する
//ConditionDBで具体的な内容を書く
//BattleSystemの方でターン終了時に実行する

public class Condition 
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    //状態異常時のメッセージ
    public string StartMessage{ get; set;}

    public Action<Pokemon> OnAfterTurn;
    public Func<Pokemon,bool> OnBeforeMove;
    public Action<Pokemon> OnStart;

}
