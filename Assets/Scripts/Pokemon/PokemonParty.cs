using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PokemonParty : MonoBehaviour
{
    //トレーナーのポケモンを管理する
    [SerializeField] List<Pokemon> pokemons;
    public List<Pokemon> Pokemons { get { return pokemons; } }

    //戦えるポケモンを渡す（気絶していないポケモン）
    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(monster => monster.HP >0).FirstOrDefault();
    }

    private void Start() {
        foreach(Pokemon pokemon in pokemons)
        {
            pokemon.Init();
        }
    }
}
