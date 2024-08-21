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
        }
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
