using System.Collections;
using System.Collections.Generic;
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

    //他のファイルから参照するためにプロパティを使う
    public string Name {get => name;}
    public string Description {get => description;}
    public int Power{get => power;}
    public int Accuracy{get => accuracy;}
    public int PP{get => pp;}
    public PokemonType Type{get =>type;}
}
