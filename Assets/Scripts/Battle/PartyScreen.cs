using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    //ポケモン選択画面の管理

    ParytMemberUI[] memberSlots;
    [SerializeField] Text messageText;

    //parytMemberUIの取得
    public void Init()
    {
        memberSlots = GetComponentsInChildren<ParytMemberUI>();
    }

    //battleSystemから手持ちのポケモンデータをもらって、それぞれにデータをセットする
    public void SetPartyData(List<Pokemon> pokemons)
    {
        for(int i =0; i < memberSlots.Length; i++)
        {
            if(i < pokemons.Count)
            {
                memberSlots[i].SetData(pokemons[i]);
            }
            else{
                memberSlots[i].gameObject.SetActive(false);
            }
        }
        messageText.text = "ポケモンを選択してください";
    }
}
