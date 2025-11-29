using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    // defines data for inventory
    [Header("Item")]
    public string itemName;

    [Header("Icon")]
    public Sprite icon;

    [Header("Value")]
    public int value = 1;

    public int maxStackSize = 99;

    public bool isSellable = true;

}
