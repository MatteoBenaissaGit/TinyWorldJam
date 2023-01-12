using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Referencing"), SerializeField] private GameObject _antsUI;
    [Header("Gizmos"), SerializeField] private bool _hideGizmos;
    [Space(10), Header("Tile"), ReadOnly] public Tile TileOccupied;
    [Space(10), Header("Ant & usability")] public bool canBeUsed = false;
    [ReadOnly] public bool isUsed = false;
    public int maxAntsInBuilding = 1;
    [ReadOnly] public int currentAntsInBuilding = 0;

    private void Start()
    {
        if (_antsUI != null)
        {
            _antsUI.SetActive(false);
        }
    }

    public void ScaleAnimation()
    {
        transform.DOComplete();
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(scale, 0.4f).SetEase(Ease.OutBounce);
    }
    
    public virtual void Use()
    {
        if (isUsed || currentAntsInBuilding >= maxAntsInBuilding)
        {
            return;
        }

        currentAntsInBuilding++;
        isUsed = true;
    }

    public virtual void Unuse()
    {
        if (isUsed == false || currentAntsInBuilding <= 0)
        {
            return;
        }

        currentAntsInBuilding--;
        if (currentAntsInBuilding <= 0)
        {
            isUsed = false;
        }
    }

    public void ShowAntUI()
    {
        if (_antsUI == null)
        {
            return;
        }
        
        _antsUI.SetActive(true);
        _antsUI.transform.DOComplete();
        _antsUI.transform.DOScale(Vector3.one, 0.25f);
    }

    public void HideAntUI()
    {
        if (_antsUI == null)
        {
            return;
        }
        
        _antsUI.transform.DOComplete();
        _antsUI.transform.DOScale(Vector3.zero, 0.25f).OnComplete(DeactivateAntUI);
    }

    private void DeactivateAntUI()
    {
        _antsUI.SetActive(false);
    }

    #region Gizmos

    #if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (_hideGizmos)
        {
            return;
        }
        
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(transform.position, gameObject.name, style);
    }

#endif
    
    #endregion
}
