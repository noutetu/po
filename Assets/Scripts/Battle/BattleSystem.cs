using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;

public enum BattleState
{
    START,
    ACTIONSELECTION,//行動選択
    MOVESELECTION,//技選択
    RUNNINGTURN,
    BUSY,
    PARTYSCREEN,//ポケモン選択状態
    BATTLEOVER,//バトル終了
}

public enum BattleAction
{
    MOVE,
    SWITCHPOKEMON,
    ITEM,
    RUN,
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    int currentAction;// 0:Fight, 1:Run
    int currentMove;// 0:左上, 1:右上, 2:左下, 3:右下
    int currentMember;
    BattleState state;
    BattleState? preState; // ?はnullを含む
    public UnityAction OnBattleOver;


    //これらの変数をどこから取得する
    PokemonParty playerParty;
    Pokemon wildPokemon;



    public void StartBattle(PokemonParty pokemonParty, Pokemon wildPokemon)
    {
        this.playerParty = pokemonParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        state = BattleState.START;
        //モンスターの生成と描画
        playerUnit.SetUp(playerParty.GetHealthyPokemon());//playerの戦闘可能なpokemonをセット
        enemyUnit.SetUp(wildPokemon);//野生ポケモンをセット
        partyScreen.Init();
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog
        ($"やせいの{enemyUnit.Pokemon.Base.Name} があらわれた！！！");

        ActionSelection();
    }

    private void ActionSelection()
    {
        state = BattleState.ACTIONSELECTION;
        dialogBox.EnableActionSelector(true);

        StartCoroutine(dialogBox.TypeDialog("どうする？"));
    }

    private void Moveselection()
    {
        Debug.Log("playermove");
        state = BattleState.MOVESELECTION;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);

    }

    void OpenPartyAction()
    {
        state = BattleState.PARTYSCREEN;
        partyScreen.gameObject.SetActive(true);
        partyScreen.SetPartyData(playerParty.Pokemons);
    }

    IEnumerator RunTurns(BattleAction battleAction)
    {
        state = BattleState.RUNNINGTURN;
        //技コマンドの選択-----------------------------------------------------------
        if (battleAction == BattleAction.MOVE)
        {
            //それぞれの技の決定
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            //先行の決定
            bool playerGoesFirst = true;

            //スピードが高ければ先行
            BattleUnit firstUnit = playerUnit;
            BattleUnit secondUnit = enemyUnit;

            //それぞれの技の優先度を比較
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            if (playerMovePriority < enemyMovePriority)
            {
                //敵が先行
                playerGoesFirst = false;
            }
            else if (playerMovePriority == enemyMovePriority)
            {
                //モンスターのスピードが速い方が先行
                if (playerUnit.Pokemon.Speed < enemyUnit.Pokemon.Speed)
                {
                    playerGoesFirst = false;
                }
            }
            if(playerGoesFirst ==false)
            {
                firstUnit = enemyUnit;
                secondUnit = playerUnit;
            }
            //先行の処理
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BATTLEOVER)
            {
                yield break;
            }

            if (secondUnit.Pokemon.HP > 0)
            {
                //後攻の処理
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BATTLEOVER)
                {
                    yield break;
                }
            }
        }
        //------------------------------------------------------------------------------
        else
        {
            //手持ちコマンドの選択-----------------------------------------------------------
            if (battleAction == BattleAction.SWITCHPOKEMON)
            {
                //入れ替えるモンスターの決定
                Pokemon selectedMember = playerParty.Pokemons[currentMember];

                //入れ替え開始
                state = BattleState.BUSY;
                yield return SwitchPokemon(selectedMember);
            }
            //アイテムコマンドの選択-----------------------------------------------------------
            else if (battleAction == BattleAction.ITEM)
            {

            }
            //　敵の行動----------------------------------------------------------------------
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BATTLEOVER)
            {
                yield break;
            }
        }
        if (state != BattleState.BATTLEOVER)
        {
            ActionSelection();
        }
    }


    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        //やられたポケモンが
        //playerUnitなら
        if (faintedUnit.IsPlayerUnit)
        {
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon == null)
            { BattleOver(); }
            else
            {
                //他にモンスターがいるなら選択画面
                OpenPartyAction();
            }
        }
        else
        {
            BattleOver();
        }
    }

    void BattleOver()
    {
        //enemyunitならバトル終了
        state = BattleState.BATTLEOVER;
        playerParty.Pokemons.ForEach((p) => p.OnBattleOver());
        OnBattleOver();

    }


    //技の実行(実行する側、喰らう側、わざ)
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //まひなら技を出せない
        bool canRunMove = sourceUnit.Pokemon.OnBeforeTurn();//技を使うポケモン(sourceUnit)が動けるかどうか
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        if (!canRunMove)
        {
            //技が使えない場合でダメージを受ける場合がある
            yield return sourceUnit.Hud.UpdateHP();

            yield break;
        }

        move.PP--;
        yield return dialogBox.TypeDialog
        ($"{sourceUnit.Pokemon.Base.Name} の{move.Base.Name}!!");
        //技が命中したかどうかチェックする
        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayerAttackAnimation();
            yield return new WaitForSeconds(0.4f);
            targetUnit.PlayerHitAnimation();
            yield return new WaitForSeconds(0.4f);
            //変化技なら
            if (move.Base.Category == MoveCategory.Stat)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                //ダメージ計算
                DamageDetails damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                //HPの描画 
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            //追加効果をチェック
            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                //それぞれの追加効果を反映
                foreach (SecondaryEffects secondary in move.Base.SecondaryEffects)
                {
                    //一定確率で状態異常
                    if (UnityEngine.Random.Range(1, 101) <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.MoveTarget);
                    }
                }
            }

            //戦闘不能ならメッセージ
            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return dialogBox.TypeDialog
            ($"{targetUnit.Pokemon.Base.Name}はたおれた!!");
                targetUnit.PlayerFaintAnimation();
                yield return new WaitForSeconds(0.5f);
                CheckForBattleOver(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog
            ($"{sourceUnit.Pokemon.Base.Name}の攻撃は当たらなかった");
        }
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BATTLEOVER)
        {
            yield break;
        }
        //RunningTurnまで待機
        yield return new WaitUntil(() => state == BattleState.RUNNINGTURN);

        //ターン終了時
        //状態異常ダメージを受ける
        sourceUnit.Pokemon.OnAfterTurn();
        yield return sourceUnit.Hud.UpdateHP();
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        //戦闘不能ならメッセージ
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog
        ($"{sourceUnit.Pokemon.Base.Name}はたおれた!!");
            sourceUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.5f);
            CheckForBattleOver(sourceUnit);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {

        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                //自身に対してステータス変化
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                //相手に対してステータス変化
                target.ApplyBoosts(effects.Boosts);
            }
        }
        //何かしらの状態異常があれば
        if (effects.Status != ConditionID.None)
        {
            target.SetStatus(effects.Status);
        }
        if (effects.VolatileStatus != ConditionID.None)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }
        yield return ShowStatusChanges((source));
        yield return ShowStatusChanges((target));
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if(move.Base.AnyHit)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuracy;
        //技を出す側のboostされた命中率
        int accuracy = source.StatBoosts[Stat.Accuracy];
        //技を受ける側のブーストされた回避率
        int evasion = target.StatBoosts[Stat.Evasion];

        float[] boostValue = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        //命中率アップ
        if (accuracy > 0)
        {
            moveAccuracy *= boostValue[accuracy];
        }
        else
        {
            moveAccuracy /= boostValue[-accuracy];
        }


        //回避率アップ
        if (evasion > 0)
        {
            moveAccuracy /= boostValue[evasion];
        }
        else
        {
            moveAccuracy *= boostValue[-evasion];
        }
        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    //ステータス変化のログを表示する関数
    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        //Queの中身がなくなるまで繰り返す
        while (pokemon.StatusChanges.Count > 0)
        {
            string message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1)
        {
            yield return dialogBox.TypeDialog("急所にあたった");
        }
        if (damageDetails.TypeEffectivenss > 1)
        {
            yield return dialogBox.TypeDialog("こうかはバツグンだ！！");
        }
        if (damageDetails.TypeEffectivenss < 1)
        {
            yield return dialogBox.TypeDialog("こうかはいまひとつだ、、、");
        }
    }

    public void HundleUpdate()
    {
        if (state == BattleState.ACTIONSELECTION)
        {
            HundleActionSelection();
        }
        else if (state == BattleState.MOVESELECTION)
        {
            HundleMoveSelection();
        }
        else if (state == BattleState.PARTYSCREEN)
        {
            HundlePartySelection();
        }
    }

    public void HundleActionSelection()
    {

        //下を入力するとRun,上を入力するとFightになる
        // 0:Fight,  1:bag
        // 2:Pokemon 3:run
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAction--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);


        //色をつけてどちらを選んでいるか管理する

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                Moveselection();
            }
            if (currentAction == 2)
            {
                preState = state;
                OpenPartyAction();
            }
        }
    }

    public void HundleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Move move = playerUnit.Pokemon.Moves[currentMove];
            // 技決定
            if(move.PP == 0)
            {
                return;
            }
            //・技選択のUIは非表示
            dialogBox.EnableMoveSelector(false);
            //・ダイアログ復活
            dialogBox.EnableDialogText(true);
            //技決定の処理
            //StartCoroutine(PlayerMove());
            StartCoroutine(RunTurns(BattleAction.MOVE));
        }
        //キャンセル
        if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    private void HundlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        //選択中のモンスター名に色をつける
        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //モンスター決定
            Pokemon selectedMember = playerParty.Pokemons[currentMember];

            //入れ替える'現在のキャラと戦闘不能は入れ替えることはできない

            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessage("そのポケモンは戦えない");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessage("すでに場に出ています");
                return;
            }

            //ポケモン選択画面を消す
            partyScreen.gameObject.SetActive(false);


            //入れ替えの処理をする
            //戦闘不能時は敵の攻撃がない
            //Playerが入れ替えアクションをした場合は、敵の攻撃がある
            // ActionSelectionによって入れ替えが行われるのかどうか
            if (preState == BattleState.ACTIONSELECTION)
            {
                //ポケモンを交代した場合
                preState = null;
                StartCoroutine(RunTurns(BattleAction.SWITCHPOKEMON));
            }
            else
            {
                //死に出しした場合
                state = BattleState.BUSY;
                StartCoroutine(SwitchPokemon(selectedMember));
            }

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            //ポケモン選択画面を消したい
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }

    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            //元のポケモンを下げる
            yield return dialogBox.TypeDialog($"戻れ！{playerUnit.Pokemon.Base.Name}");
            //下げるアニメーション
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(1.5f);

        }

        //新しいのを出す
        yield return dialogBox.TypeDialog
        ($"ゆけっ！！{newPokemon.Base.Name}!!!");
        yield return new WaitForSeconds(0.3f);
        //nextをセット
        //モンスターの生成と描画
        playerUnit.SetUp(newPokemon);//playerの戦闘可能なpokemonをセット
        yield return new WaitForSeconds(0.6f);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        state = BattleState.RUNNINGTURN;
    }
}