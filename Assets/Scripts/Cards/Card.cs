using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    [Header("Card Data")] public CardInfo CardInfoData;

    [Header("Referencing"), SerializeField] private Image _cardImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _leafCostText;
    [SerializeField] private RoundsAndDefenseManager _roundsAndDefenseManager;
    [ReadOnly] public bool CanBeSelected;


    private float _baseY;
    
    private void Start()
    {
        _baseY = transform.position.y;
        CardInfoData = GameManager.Instance.RoundsAndDefenseManager.GetRandomCard();
        SetupCard(false);
    }
    
    public void SetupCard(bool animate)
    {
        if (animate)
        {
            transform.DOComplete();
            transform.DOMoveY(_baseY - 250, 0.5f).OnComplete(StartAnimationUp);
        }
        else
        {
            ChangeValues();
            MakeCardSelectable();
        }
    }

    private void ChangeValues()
    {
        _cardImage.sprite = CardInfoData.Image;
        _nameText.text = CardInfoData.Name;
        _leafCostText.text = CardInfoData.Cost.ToString();
    }

    private void StartAnimationUp()
    {
        transform.DOComplete();
        transform.DOMoveY(_baseY, 1f).OnComplete(MakeCardSelectable);
        ChangeValues();
    }

    private void MakeCardSelectable()
    {
        CanBeSelected = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanBeSelected && _roundsAndDefenseManager.IsPlacingCard == false)
        {
            AnimateUp(15);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0) && _roundsAndDefenseManager.IsPlacingCard == false && CanBeSelected)
        {
            if (CardInfoData.Cost <= _roundsAndDefenseManager.NumberOfLeafs)
            {
                if (CardInfoData.Type == CardType.Building)
                {
                    _roundsAndDefenseManager.IsPlacingCard = true;
                    _roundsAndDefenseManager.SelectedCard = this;
                    AnimateUp(30);
                }

                if (CardInfoData.Type == CardType.Ant)
                {
                    _roundsAndDefenseManager.NumberOfLeafs -= CardInfoData.Cost;
                    _roundsAndDefenseManager.NumberOfAnts ++;
                    _roundsAndDefenseManager.NumberOfAvailaibleAnts ++;
                    
                    _roundsAndDefenseManager.IsPlacingCard = true;
                    _roundsAndDefenseManager.SelectedCard = this;
                    _roundsAndDefenseManager.RefreshUI();
                    CanBeSelected = false;
                    _roundsAndDefenseManager.UseCard(_roundsAndDefenseManager.SelectedCard, true);
                }
            }
            else
            {
                transform.DOShakePosition(0.2f, new Vector3(10, 0, 0), 20);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_roundsAndDefenseManager.IsPlacingCard && _roundsAndDefenseManager.SelectedCard == true)
        {
            return;
        }

        AnimateDown();
    }

    public void AnimateUp(float value)
    {
        if (CanBeSelected == false)
        {
            return;
        }
        print("up");
        transform.DOComplete();
        transform.DOMoveY(_baseY + value, 0.3f);
    }

    public void AnimateDown()
    {
        if (CanBeSelected == false)
        {
            return;
        }
        print("down");
        transform.DOComplete();
        transform.DOMoveY(_baseY, 0.15f);
    }
}
