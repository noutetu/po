using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//レベルに応じたステータスの違うモンスターを生成するクラス
//ただしデータのみ
[System.Serializable]
public class Pokemon
{
    //インスペクターからデータを設定できるようにする
    [SerializeField] int level;
    [SerializeField] PokemonBase pokemonBase;

    //ベースとなるデータ
    public PokemonBase Base { get => pokemonBase; }
    public int Level { get => level; }
    //使える技のリスト
    public List<Move> Moves { get; set; }
    //現在の技を格納
    public Move CurrentMove { get; set; }

    //初期ステータス
    public Dictionary<Stat, int> Stats { get; set; }
    //ステータス変化
    public Dictionary<Stat, int> StatBoosts { get; set; }
    public int HP { get; set; }

    //状態異常を入れる変数
    public Condition Status { get; private set; }
    public bool isChangedHP { get; set; }

    public int StatusTime { get; set; }

    //バトル終了時に解除される
    public Condition VolatileStatus { get; set; }
    public int VolatileStatusTime { get; set; }

    //ログを溜めておく変数を作る：出し入れが簡単なリスト
    public Queue<string> StatusChanges { get; private set; }

    //ステータス変化が起こった時に実行したいことを登録する
    public System.Action OnStatusChange;



    Dictionary<Stat, string> StatDic = new Dictionary<Stat, string>()
    {
        {Stat.Attack, "こうげき"},
        {Stat.Defence, "ぼうぎょ"},
        {Stat.SpAttack, "とくこう"},
        {Stat.SpDefence, "とくぼう"},
        {Stat.Speed, "すばやさ"},

        {Stat.Accuracy, "めいちゅうりつ"},
        {Stat.Evasion, "かいひりつ"},

        
    };


    //コンストラクタ
    public void Init()
    {
        Moves = new List<Move>();
        StatusChanges = new Queue<string>();

        //使える技の設定:覚える技のレベル以上なら、リストに追加 
        foreach (LearnableMove learnableMove in Base.LearnableMoves)
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
        CalculateStats();
        HP = MaxHP;
        ResetStatBoost();

        Status = null;
        VolatileStatus = null;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {

                {Stat.Attack,0},
                {Stat.Defence,0},
                {Stat.SpAttack,0},
                {Stat.SpDefence,0},
                {Stat.Speed,0},


                {Stat.Accuracy,0},
                {Stat.Evasion,0},

        };
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
        VolatileStatus = null;

    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defence, Mathf.FloorToInt((Base.Defence * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefence, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);
        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10 + Level;
    }

    int GetStat(Stat stat)
    {
        int statValue = Stats[stat];

        int boosts = StatBoosts[stat];
        float[] boostValue = new float[] { 1, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boosts >= 0)
        {

            //強化なら
            statValue = Mathf.FloorToInt(statValue * boostValue[boosts]);
        }
        else
        {

            //弱体化なら
            statValue = Mathf.FloorToInt(statValue / boostValue[-boosts]);

        }


        return statValue;
    }
    //状態異常を受けた時に呼び出す
    public void SetStatus(ConditionID conditionID)
    {
        if(Status != null)
        {
            //すでに状態異常なら他の状態異常にならない

            return;
        }
        Status = ConditionDB.conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        //ログに追加
        StatusChanges.Enqueue($"{Base.Name}{Status.StartMessage}");

        OnStatusChange?.Invoke();
        
    }

    public void CureStatus()
    {
        OnStatusChange?.Invoke();
        Status = null;
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if(Status != null)
        {
            //すでに状態異常なら他の状態異常にならない

            return;
        }
        VolatileStatus = ConditionDB.conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        //ログに追加
        StatusChanges.Enqueue($"{Base.Name}{VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }


    //ターン終了時にやりたいこと;状態異常
    public void  OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public bool OnBeforeTurn()
    {
        bool canRunMove = true;
        //状態異常について
        if(Status?.OnBeforeMove != null)
        {
            if(Status.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }
        }
        //混乱系の状態異常について
        if(VolatileStatus?.OnBeforeMove != null)
        {
            if(VolatileStatus.OnBeforeMove(this) == false)
            {
                canRunMove = false;
            }
        }
        return canRunMove;
    }


    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        //ステータス変化を反映
        foreach (StatBoost statBoost in statBoosts)
        {
            //どのステータスを
            Stat stat = statBoost.stat;
            //何段階
            int boost = statBoost.boost;
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}の{StatDic[stat]}が上がった");
            }
            if (boost < 0)
            {
                StatusChanges.Enqueue($"{Base.Name}の{StatDic[stat]}が下がった");
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
        get { return GetStat(Stat.Attack); }
    }
    //防御
    public int Defence
    {
        get { return GetStat(Stat.Defence); }
    }
    //特攻
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    //特防
    public int SpDefence
    {
        get { return GetStat(Stat.SpDefence); }
    }
    //素早さ
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    //HP
    public int MaxHP
    {
        get; private set;
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        // クリティカル判定 (6.25% の確率)
        float critical = 1f;
        if (UnityEngine.Random.value * 100 <= 6.25f)
        {
            critical = 2f;
        }

        // タイプ相性
        float type = TypeChart.GetEffectiveness(move.Base.Type, Base.Type1) *
                     TypeChart.GetEffectiveness(move.Base.Type, Base.Type2);

        DamageDetails damageDetails = new DamageDetails
        {
            Fainted = false,
            Critical = critical,
            TypeEffectivenss = type,
        };

        //特殊技かどうか
        float attack = attacker.Attack;
        float defence = attacker.Defence;

        if (move.Base.Category == MoveCategory.Special)
        {
            attack = attacker.SpAttack;
            defence = SpDefence;
        }

        // 乱数（0.85 〜 1.0）
        float random = UnityEngine.Random.Range(0.85f, 1f);

        // Modifiers の計算
        float modifier = critical * type * random;

        // ダメージの計算 (正しいポケモンのダメージ計算式に基づく)
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defence) + 2;
        int damage = Mathf.FloorToInt(d * modifier);
        UpdateHP(damage);
        
        return damageDetails;
    }
    
    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0 ,MaxHP);
        isChangedHP = true;
    }



    public Move GetRandomMove()
    {
        //PPが0なら排除
        List<Move> movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectivenss { get; set; }
}

