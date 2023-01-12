using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CardInfo", menuName = "ScriptableObjects/CardInfo", order = 1)]
public class CardInfo : ScriptableObject
{
    public string Name;
    public int Cost;
    public Sprite Image;
    public Building CardBuilding;
    public GameObject PreviewBuilding;
}
