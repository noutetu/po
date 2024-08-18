using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] GameController gameController;

    int currentAction;// 0:Fight, 1:Run
    int currentMove;// 0:左上, 1:右上, 2:左下, 3:右下
    BattleState state;

    

    public void StartBattle()
    {
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        state = BattleState.START;
        //モンスターの生成と描画
        playerUnit.SetUp();
        enemyUnit.SetUp();
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
            gameController.endBattle();

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
            gameController.endBattle();
        }
        //戦闘可能ならEnemyMove
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
        // 0:Fight, 1:Run
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                currentAction++;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                currentAction--;
            }
        }

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
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
            {
                currentMove++;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                currentMove--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
            {
                currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                currentMove -= 2;
            }
        }
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
