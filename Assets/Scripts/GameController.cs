using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//ゲームの状態を管理
public enum GameState
{
    FreeRoam,//マップ移動
    Battle,//戦闘
}


public class GameController : MonoBehaviour
{
    [SerializeField] Camera worldCamera;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
 GameState state = GameState.FreeRoam;

    void Start()
    {
        playerController.OnEncounted += StartBattle;
        battleSystem.OnBattleOver += endBattle;
    }

    void Update()
    {
        if(state == GameState.FreeRoam)
        {
            //PlayerControllerの処理
            playerController.HundleUpdate();
        }   
        else if(state == GameState.Battle)
        {
            //BattleSystem
            battleSystem.HundleUpdate();
        }
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        // パーティと野生ポケモンの取得
        PokemonParty pokemonParty = playerController.GetComponent<PokemonParty>();
        //シーンないから一致するコンポーネントを一つ取得する
        Pokemon wildPokemon = FindObjectOfType<MapArea>().GetRandomWildPokemon();;
        battleSystem.StartBattle(pokemonParty,wildPokemon);
    }

    public void endBattle()
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
}
