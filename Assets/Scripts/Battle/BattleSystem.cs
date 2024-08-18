using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public enum BattleState
{
    START,
    PLAYERACTION,//行動選択
    PLAYERMOVE,//技選択
    ENEMYMOVE,
    BUSY,
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;


    int currentAction;// 0:Fight, 1:Run
    int currentMove;// 0:左上, 1:右上, 2:左下, 3:右下
    BattleState state;
    public UnityAction BattleOver;


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
        //Hudの描画
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog
        ($"やせいの{enemyUnit.Pokemon.Base.Name} があらわれた！！！");

        PlayerAction();
    }

    private void PlayerAction()
    {
        state = BattleState.PLAYERACTION;
        dialogBox.EnableActionSelector(true);

        StartCoroutine(dialogBox.TypeDialog("どうする？"));
    }

    private void PlayerMove()
    {
        Debug.Log("playermove");
        state = BattleState.PLAYERMOVE;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);

    }

    //プレイヤーの技発動
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.BUSY;
        //技を決定
        Move move = playerUnit.Pokemon.Moves[currentMove];
        move.PP--;

        yield return dialogBox.TypeDialog
        ($"{playerUnit.Pokemon.Base.Name} の{move.Base.Name}!!");
        //攻撃アニメーション
        playerUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.4f);
        enemyUnit.PlayerHitAnimation();
        yield return new WaitForSeconds(0.4f);
        //ダメージ計算
        DamageDetails damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        //HPの描画
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        //戦闘不能ならメッセージ
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog
        ($"{enemyUnit.Pokemon.Base.Name}はたおれた!!");
            enemyUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.5f);
            BattleOver();

        }
        //戦闘可能ならEnemyMove
        else
        {
            //それ以外ならenemyMove
            StartCoroutine(EnemyMove());
        }

    }

    IEnumerator EnemyMove()
    {
        state = BattleState.ENEMYMOVE;
        //技を決定 =>ランダム
        Move move = enemyUnit.Pokemon.GetRandomMove();
        move.PP--;
        yield return dialogBox.TypeDialog
        ($"{enemyUnit.Pokemon.Base.Name} の{move.Base.Name}!!");

        enemyUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.4f);
        playerUnit.PlayerHitAnimation();
        yield return new WaitForSeconds(0.4f);
        //ダメージ計算
        DamageDetails damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        //HPの描画
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        //戦闘不能ならメッセージ
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog
        ($"{playerUnit.Pokemon.Base.Name}はたおれた!!");
            playerUnit.PlayerFaintAnimation();
            yield return new WaitForSeconds(0.5f);
            //戦えるポケモンがいるなら、次のポケモンをセットして自分のターンにする
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon == null)
            {

                BattleOver();
            }
            else
            {
                //nextをセット
                //モンスターの生成と描画
                playerUnit.SetUp(nextPokemon);//playerの戦闘可能なpokemonをセット
                //Hudの描画
                playerHud.SetData(playerUnit.Pokemon);
                dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

                yield return dialogBox.TypeDialog
                ($"ゆけっ！！{enemyUnit.Pokemon.Base.Name}！！！");

                PlayerAction();
            }
        }
        else
        {
            //それ以外ならenemyMove
            PlayerAction();
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
        if (state == BattleState.PLAYERACTION)
        {
            HundleActionSelection();
        }
        else if (state == BattleState.PLAYERMOVE)
        {
            HundleMoveSelection();
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

        currentAction = Mathf.Clamp(currentAction,0,3);
        

        //色をつけてどちらを選んでいるか管理する

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                PlayerMove();
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
        currentMove = Mathf.Clamp(currentMove,0,playerUnit.Pokemon.Moves.Count-1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // 技決定
            //・技選択のUIは非表示
            dialogBox.EnableMoveSelector(false);
            //・ダイアログ復活
            dialogBox.EnableDialogText(true);
            //技決定の処理
            StartCoroutine(PerformPlayerMove());
        }
    }
}
