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
        SetupCard();
    }
    
    public void SetupCard()
    {
        _cardImage.sprite = CardInfoData.Image;
        _nameText.text = CardInfoData.Name;
        _leafCostText.text = CardInfoData.Cost.ToString();
        
        transform.DOComplete();
        transform.DOMoveY(_baseY - 250, 0.5f).OnComplete(StartAnimationUp);
    }

    private void StartAnimationUp()
    {
        transform.DOComplete();
        transform.DOMoveY(_baseY, 1f).OnComplete(MakeCardSelectable);
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
                _roundsAndDefenseManager.IsPlacingCard = true;
                _roundsAndDefenseManager.SelectedCard = this;
                AnimateUp(30);
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
        transform.DOComplete();
        transform.DOMoveY(_baseY + value, 0.3f);
    }

    public void AnimateDown()
    {
        transform.DOComplete();
        transform.DOMoveY(_baseY, 0.15f);
    }
}