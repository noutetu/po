using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpbar;
    [SerializeField] Text statusText;

    [SerializeField] Color poisonColor;
    [SerializeField] Color burnColor;
    [SerializeField] Color sleepColor;
    [SerializeField] Color paralysisColor;
    [SerializeField] Color freezeColor;

    Pokemon _pokemon;
    //状態異常の色
    Dictionary<ConditionID,Color> statusColors;

    /*public enum ConditionID
    {
        None,        //なし
        Poison,      // 毒
        Burn,        //やけど
        Sleep,       //眠り
        Paralysis,   //まひ
        Freeze,      //こおり
    }*/

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        Debug.Log("SetData is Acted");
        nameText.text = pokemon.Base.Name;
        levelText.text = "LV:" + pokemon.Level;
        hpbar.SetHP((float)pokemon.HP / pokemon.MaxHP);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.Poison,poisonColor},
            {ConditionID.Paralysis,paralysisColor},
            {ConditionID.Burn,burnColor},
            {ConditionID.Freeze,freezeColor},
            {ConditionID.Sleep,sleepColor},
        };
        SetStatusText();
        _pokemon.OnStatusChange += SetStatusText;
    }

    void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Name;
            //色の変更
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_pokemon.isChangedHP)
        {
            yield return hpbar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHP);
            _pokemon.isChangedHP = false;
        }
    }
}
