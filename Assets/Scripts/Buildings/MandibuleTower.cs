using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MandibuleTower : Building
{
    [Header("Referencing"), SerializeField] private List<Image> _fillImagesList;
    [Header("Parameters"), Range(0, 100)] public float AttackDamagePerAnt = 1;

    private void Start()
    {
        UpdateUI();
        ScaleAnimation();
    }
    
    private void UpdateUI()
    {
        for (int i = 0; i < MaxAntsInBuilding; i++)
        {
            _fillImagesList[i].gameObject.SetActive(i < CurrentAntsInBuilding);
        }
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
