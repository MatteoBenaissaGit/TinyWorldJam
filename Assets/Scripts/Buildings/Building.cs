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
    [Space(10), Header("Ant & usability")] public bool CanBeUsed = false;
    [ReadOnly] public bool IsUsed = false;
    public int MaxAntsInBuilding = 1;
    [ReadOnly] public int CurrentAntsInBuilding = 0;

    public virtual void Start()
    {
        if (_antsUI != null)
        {
            _antsUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (CanBeUsed && GameManager.Instance.SelectedTile != TileOccupied && _antsUI.activeInHierarchy)
        {
            HideAntUI();
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
        if ((IsUsed && MaxAntsInBuilding <= 1) || CurrentAntsInBuilding+1 > MaxAntsInBuilding)
        {
            return;
        }

        CurrentAntsInBuilding++;
        IsUsed = true;
    }

    public virtual void Unuse()
    {
        if (IsUsed == false || CurrentAntsInBuilding <= 0)
        {
            return;
        }

        CurrentAntsInBuilding--;
        if (CurrentAntsInBuilding <= 0)
        {
            IsUsed = false;
        }
    }

    private bool _use = true;
    public bool HaveToUse(int currentNumberOfAnts)
    {
        if (_use &&(
            CurrentAntsInBuilding >= MaxAntsInBuilding ||
            currentNumberOfAnts <= 0))
        {
            _use = false;
        }

        if (_use == false &&
            CurrentAntsInBuilding == 0)
        {
            _use = true;
        }
        
        return _use;
    }

    public void ShowAntUI()
    {
        if (_antsUI == null || _antsUI.activeInHierarchy || GameManager.Instance.CurrentGameState != GameState.ManagingDefense)
        {
            return;
        }
        
        _antsUI.SetActive(true);
        _antsUI.transform.DOComplete();
        _antsUI.transform.localScale = Vector3.zero;
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
