using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10 + Level; }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        // クリティカル判定 (6.25% の確率)
        float critical = 1f;
        if (Random.value * 100 <= 6.25f)
        {
            critical = 2f;
        }

        // タイプ相性
        float type = TypeChart.GetEffectiveness(move.Base.Type, Base.Type1) *
                     TypeChart.GetEffectiveness(move.Base.Type, Base.Type2);
        
        DamageDetails damageDetails = new DamageDetails
        {
            Fainted =false,
            Critical = critical,
            TypeEffectivenss = type,
        };

        //特殊技かどうか
        float attack = attacker.Attack;
        float defence = attacker.Defence;

        if(move.Base.isSpecial)
        {
            attack = attacker.SpAttack;
            defence = SpDefence;
        }

        // 乱数（0.85 〜 1.0）
        float random = Random.Range(0.85f, 1f);

        // Modifiers の計算
        float modifier = critical * type * random;

        // ダメージの計算 (正しいポケモンのダメージ計算式に基づく)
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defence) + 2;
        int damage = Mathf.FloorToInt(d * modifier);

        // HPの減少
        HP -= damage;

        // ポケモンが倒されたかどうかを返す
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }



    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical{ get; set; }
    public float TypeEffectivenss { get; set; }
}
