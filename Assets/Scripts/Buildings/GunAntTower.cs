using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GunAntTower : Building
{
    [Header("Referencing"), SerializeField] private Image _fillImages;
    [SerializeField] private GameObject _ant;

    private void Start()
    {
        UpdateUI();
        ScaleAnimation();
    }
    
    private void UpdateUI()
    {
        _fillImages.gameObject.SetActive(CurrentAntsInBuilding > 0);
        _ant.SetActive(CurrentAntsInBuilding >= 1);
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
