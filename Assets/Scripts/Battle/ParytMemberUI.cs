using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParytMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpbar;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        Debug.Log("SetData is Acted");
        nameText.text =pokemon.Base.Name;
        levelText.text ="LV:" + pokemon.Level;
        hpbar.SetHP((float)pokemon.HP/pokemon.MaxHP);
    }

}
