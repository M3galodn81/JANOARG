using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains all titles
[CreateAssetMenu(fileName = "New Player Title List", menuName = "JANOARG/Title List", order = 200)]
public class PlayerTitleList : ScriptableObject
{
    public PlayerTitle[] titles;
}

public enum TitleRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class PlayerTitle
{
    //TODO: Titles from clearing songs
    //TODO: Special Titles 

    // Title Class
    public int id;
    public string title;
    public string description;
    public TitleRarity rarity;
    public bool isUnlocked;


    [HideInInspector]
    public Color textColor;

    // TODO Functions
    public Color GetColor(TitleRarity rarity) => rarity switch
    {
        TitleRarity.Common      => new Color(1f, 1f, 1f, 1f),               //white
        TitleRarity.Uncommon    => new Color(0.749f, 0.949f, 1f, 1f),       //light blue
        TitleRarity.Rare        => new Color(1f, 1f, 0.79f, 1f),            //light gold
        TitleRarity.Epic        => new Color(1f, 0.769f, 0.769f, 1f),       //light red
        TitleRarity.Legendary   => new Color(1f, 1f, 0.79f, 1f)             // rainbow it ig
        _                       => new Color(1f, 1f, 1f, 1f),               //
    };
        
    

    //
    public void Unlock()
    {
        isUnlocked = true;
    }

    public void CheckRequirements()
    {
        if (isUnlocked)
        {
            return;
        }
    }

    public void GetProgress()
    {

    }


}

