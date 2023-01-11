using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Editor Variables

    [Header("Referencing"), SerializeField]
    private GameObject _selection;

    [Space(10), ReadOnly] public bool IsOccupied;
    [ReadOnly] public bool IsSelected;

    #endregion

    #region Private Variables

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        _selection.SetActive(false);
    }

    #endregion

    #region Methods

    public void Select()
    {
        _selection.SetActive(true);
        IsSelected = true;
    }

    public void Unselect()
    {
        _selection.SetActive(false);
        IsSelected = false;
    }

    #endregion
}