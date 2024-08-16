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
    Psycic,
    Bug,
    Rock,
    Ghost,
    Dragon,

}
