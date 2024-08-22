using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    //キーとバリュー
    public static Dictionary<ConditionID, Condition> conditions { get; set; } = new Dictionary<ConditionID, Condition>()
   {
        {
            ConditionID.Poison,new Condition()
            {
                Name = "どく",
                StartMessage = "はどくになった",
                //OnAfterTurn = Poison,
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    //毒ダメージ
                    Debug.Log($"{pokemon.Base.Name}はどくをくらった");
                    //毒ダメージを与える
                    pokemon.UpdateHP(pokemon.MaxHP/8);
                    //メッセージを表示
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はどくのダメージを受けた");
                }
            }
        },
        {
            ConditionID.Burn,new Condition()
            {
                Name = "やけど",
                StartMessage = "はやけどをおった",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    //やけどダメージを与える
                    pokemon.UpdateHP(pokemon.MaxHP / 16);
                    //メッセージを表示する
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はやけどのダメージを受けた");
                }
            }
        },
        {
            ConditionID.Paralysis,new Condition()
            {
                Name = "まひ",
                StartMessage = "はまひした",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    //一定確率で技が出せずに自分のターンが終わる
                    //1,2,3,4が出る中で１が出たら(25%の確率で)
                    if(Random.Range(1,5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はしびれて動けない");
                        return false;
                    }
                    return true;
                }
            }
        },

        {
            ConditionID.Freeze,new Condition()
            {
                Name = "こおり",
                StartMessage = "はこおってしまった",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    //一定確率で技が出せずに自分のターンが終わる
                    //1,2,3,4が出る中で１が出たら(25%の確率で)
                    if(Random.Range(1,5) == 1)
                    {

                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}のこおりがとけた");
                        pokemon.CureStatus();
                        return true;

                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はこおってしまって動けない");
                    return false;
                }
            }
        },

   };

}

public enum ConditionID
{
    None,        //なし
    Poison,      // 毒
    Burn,        //やけど
    Sleep,       //眠り
    Paralysis,   //まひ
    Freeze,      //こおり
}
