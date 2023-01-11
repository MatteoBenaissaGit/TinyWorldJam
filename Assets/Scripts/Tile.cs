using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Editor Variables

    [Header("Referencing"), SerializeField] private GameObject _selection;
    
    [Space(10), ReadOnly] public bool IsOccupied;   

    #endregion

    #region Private Variables


    
    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        _selection.SetActive(false);
    }

    #endregion
    
    #region Pointer events

    public void OnPointerEnter(PointerEventData eventData)
    { 
        _selection.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _selection.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
    }
    
    #endregion
}
