using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RessourceBuilding : Building
{
    [Header("Referencing"), SerializeField] private Image _fillImage;
    [Header("Parameters"), Range(0, 10)] public int RessourcePerRound = 1;

    private void Start()
    {
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        _fillImage.gameObject.SetActive(IsUsed);
    }

    public override void Use()
    {
        base.Use();
        UpdateUI();
    }

    public override void Unuse()
    {
        base.Unuse();
        UpdateUI();
    }
}
