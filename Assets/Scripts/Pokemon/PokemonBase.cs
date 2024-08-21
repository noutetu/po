using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using System;

//ポケモンのマスターデータ
[CreateAssetMenu]
public class PokemonBase :ScriptableObject
{
    //名前
    [SerializeField] new string name;
    public string Name { get => name; }

    //説明
    [TextArea]
    [SerializeField] string discription;
    public string Discription { get =>  discription;}

    //画像
    [SerializeField] Sprite frontSprite;
    public Sprite FrontSprite { get =>  frontSprite;}
    [SerializeField] Sprite backSprite;
    public Sprite BackSprite { get =>  backSprite;}

    //タイプ
    [SerializeField] PokemonType type1;
    public PokemonType Type1 { get => type1;}
    [SerializeField] PokemonType type2;
    public PokemonType Type2 { get =>  type2;}

    //ステータス:h,a,b,c,d,s
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    public int MaxHP     {get => maxHP;}
    public int Attack    {get => attack;}
    public int Defence   {get => defence;}
    public int SpAttack  {get => spAttack;}
    public int SpDefence {get => spDefence;}
    public int Speed     {get => speed;}


    //覚える技リスト
    [SerializeField] List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves{get => learnableMoves;}
}

//どのレベルで何を覚えるか
[Serializable]
public  class LearnableMove
{
    //ヒエラルキーで設定するもの 
    [SerializeField] public MoveBase _base;
    [SerializeField] int level;

    public MoveBase Base {get =>_base;}
    public int Level {get => level;}
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Ghost,
    Dragon,

}

public enum Stats
{
    Attack,
    Defence,
    SpAttack,
    SpDefence,
    Speed,
    MaxHP,
}


public class TypeChart
{
    static float[][]chart =
    {
         // TODO:仮実装
        //攻撃＼防御         NOR  FIR  WAT  ELE  GRS  ICE  FIG  POI
        /*NOR*/ new float[]{1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*FIR*/ new float[]{1f,0.5f,0.5f,  1f,  2f,  2f,  1f,  1f},
        /*WAT*/ new float[]{1f,  2f,0.5f,  1f,0.5f,  1f,  1f,  1f},
        /*ELE*/ new float[]{1f,  1f,  2f,0.5f,0.5f,  1f,  1f,  1f},
        /*GRS*/ new float[]{1f,0.5f,  2f,  1f,0.5f,  1f,  1f,0.5f},
        /*ICE*/ new float[]{1f,0.5f,0.5f,  1f,  2f,0.5f,  1f,  1f},
        /*FIG*/ new float[]{2f,  1f,  1f,  1f,  1f,  2f,  1f,0.5f},
        /*POI*/ new float[]{1f,  1f,  1f,  1f,  2f,  1f,  1f,0.5f},
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenceType)
    {
        if(attackType == PokemonType.None || defenceType == PokemonType.None)
        {
            return 1f;
        }
        int row = (int)attackType -1;
        int col = (int)defenceType -1;
        return chart[row][col];
    }
}