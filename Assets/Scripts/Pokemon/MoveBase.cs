using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu]
public class MoveBase : ScriptableObject
{
    //技のマスターデータ

    //名前
    [SerializeField] new string name;
    //詳細
    [TextArea]
    [SerializeField] string description;
    //威力
    [SerializeField] int power;
    //命中率
    [SerializeField] int accuracy;
    //PP
    [SerializeField] int pp;
    //タイプ
    [SerializeField] PokemonType type;

    [SerializeField] MoveTarget target;

    [SerializeField] MoveCategory category;

    //どのステータスをどの程度変化させるかのリスト
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;


    public List<SecondaryEffects> SecondaryEffects {get => secondaryEffects;}
    public MoveEffects Effects { get => effects; }
    public MoveTarget Target { get => target; }
    public MoveCategory Category { get => category; }
    //他のファイルから参照するためにプロパティを使う
    public string Name { get => name; }
    public string Description { get => description; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public int PP { get => pp; }
    public PokemonType Type { get => type; }


}

public enum MoveCategory
{
    Physical,
    Special,
    Stat,
}
public enum MoveTarget
{
    Foe,
    Self,
}


[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    public List<StatBoost> Boosts { get => boosts; }
    [SerializeField] ConditionID status;
    public ConditionID Status { get => status; }//戦闘終了時に回復しない
    [SerializeField] ConditionID volatileStatus;
    public ConditionID VolatileStatus { get => volatileStatus; }//戦闘終了時に回復する
}

//追加効果の実装
[System.Serializable]
public class SecondaryEffects: MoveEffects
{
    [SerializeField] int chance;//追加効果の命中率
    [SerializeField] MoveTarget target;//追加効果の対象

    public int Chance { get => chance; }
    public MoveTarget MoveTarget{ get => target; }

}
public class StatBoost
{
    public Stat stat; //ポケモンクラスで定義した６つのステータスを格納するクラス
    public int boost; //どの程度変化するか
}