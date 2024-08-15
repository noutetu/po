using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;    
    [SerializeField] BattleUnit enemyUnit;    
    [SerializeField] BattleHud playerHud;    
    [SerializeField] BattleHud enemyHud;    


    public void Start()
    {
        //モンスターの生成と描画
        playerUnit.SetUp();
        enemyUnit.SetUp();
        //Hudの描画
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
        
    }
}
