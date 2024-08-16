using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HPの増減の描画
public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    
    public void SetHP(float HP)
    {
        health.transform.localScale = new Vector3(HP, 1, 1);
    }
}
