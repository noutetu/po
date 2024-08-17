using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

//レベルに応じたステータスの違うモンスターを生成するクラス
//ただしデータのみ
public class Pokemon
{
    //ベースとなるデータ
    public PokemonBase Base { get; set; }
    public int Level { get; set; }
    //使える技のリスト
    public List<Move> Moves { get; set; }

    //
    public int HP { get; set; }

    //コンストラクタ
    public Pokemon(PokemonBase pBase, int pLevel)
    {
        if (pBase == null)
        {
            return;
        }
        Base = pBase;
        Level = pLevel;
        HP = MaxHP;
        Moves = new List<Move>();


        //使える技の設定:覚える技のレベル以上なら、リストに追加 
        foreach (LearnableMove learnableMove in pBase.LearnableMoves)
        {


            if (Level >= learnableMove.Level)
            {
                Moves.Add(new Move(learnableMove.Base));
            }

            //4つ以上の技は使えない
            if (Moves.Count >= 4)
            {
                break;
            }
        }

    }

    //レベルに応じたステータスを返すもの

    /*関数版
    public int Attack()
    {
        return Mathf.FloorToInt((_base.Attack * level)/100f) + 5;
    }*/

    //プロパティ版

    //攻撃
    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }
    //防御
    public int Defence
    {
        get { return Mathf.FloorToInt((Base.Defence * Level) / 100f) + 5; }
    }
    //特攻
    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }
    //特防
    public int SpDefence
    {
        get { return Mathf.FloorToInt((Base.SpDefence * Level) / 100f) + 5; }
    }
    //素早さ
    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }
    //HP
    public int MaxHP
    {
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 5; }
    }

    public bool TakeDamage(Move move,Pokemon attacker)
    {
        float modifiers = Random.Range(0.85f,1f);
        float a = (2* attacker.Level + 10)/250f;
        float d = a* move.Base.Power * ((float)attacker.Attack/Defence) + 2;
        int damage = Mathf.FloorToInt(d*modifiers);

        HP -= damage;

        if(HP <= 0)
        {
            HP = 0;
            return true; 
        }
        return false;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0,Moves.Count);
        return Moves[r];
    }
}
