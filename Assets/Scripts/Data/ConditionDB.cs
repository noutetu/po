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
                Id = ConditionID.Poison,
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
                Id = ConditionID.Burn,
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
                Id = ConditionID.Paralysis,
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
                Id = ConditionID.Freeze,
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

        {
            ConditionID.Sleep,new Condition()
            {
                Id = ConditionID.Sleep,
                Name = "ねむり",
                StartMessage = "は眠ってしまった",
                OnStart = (Pokemon pokemon) =>
                {
                    //技を受けた最初の時に何ターン眠るか決める
                    pokemon.StatusTime = Random.Range(1,5);
                },
                OnBeforeMove = (Pokemon pokemon)=>
                {
                    if(pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}目を覚ました");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}は眠っている");
                    pokemon.StatusTime --;
                    return false;
                }
                
            }
        },

        {
            ConditionID.Confusion,new Condition()
            {
                Id = ConditionID.Confusion,
                Name = "こんらん",
                StartMessage = "はこんらんした",
                OnStart = (Pokemon pokemon) =>
                {
                    //技を受けた最初の時に何ターン眠るか決める
                    pokemon.VolatileStatusTime = Random.Range(1,5);
                },
                OnBeforeMove = (Pokemon pokemon)=>
                {
                    if(pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}のこんらんがとけた");
                        return true;
                    }

                    pokemon.VolatileStatusTime --;
                    if(Random.Range(1,3) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はこんらんしている");
                        return true;
                    }
                    pokemon.UpdateHP(pokemon.MaxHP/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}はこんらんしている");
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}は自分を攻撃した");
                    pokemon.StatusTime --;
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
    Confusion,   //混乱
}
