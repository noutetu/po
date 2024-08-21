using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB 
{
    //キーとバリュー
   public static Dictionary<ConditionID,Condition> conditions {get;set;} = new Dictionary<ConditionID, Condition>()
   {
        {
            ConditionID.Poison,new Condition()
            {
                Name = "どく",
                StartMessage = "はどくになった",
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
