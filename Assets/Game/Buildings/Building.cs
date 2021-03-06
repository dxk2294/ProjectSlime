using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum ResourceType
{
    // NOTE: If you add or remove to this, do *not* change the values of other
    // enums. This will break all serialized scriptable objects.
    Money = 0,
    Energy = 1,
    CleanEnergy = 2,
    DirtyEnergy = 3,
    LowTechMat = 4,
    HighTechMat = 5,
    Biomass = 6,
    Research = 7,
    Pop = 8,
    Biodiversity = 9,
    BiodiversityPressure = 10,
    SeaLevel = 11,
    SeaLevelPressure = 12,
    TimeToExtiction = 13
}

[System.Serializable]
public class ResourceEffect
{
    [SerializeField]
    public ResourceType AffectedResource;

    [Range(-1000, 1000)]
    public int EffectAmount;

    [SerializeField]
    public bool AffectsStockpile = false;
}

[System.Serializable]
public class SpecialEffect
{
    public enum SpecialEffectType
    {
        // NOTE: If you add or remove to this, do *not* change the values of other
        // enums. This will break all serialized scriptable objects.
        Unknown = 0,
        Tutorial_1 = 1,
        Tutorial_2 = 2,
        Tutorial_3 = 3,
        Tutorial_4 = 4,
    }

    [SerializeField]
    public SpecialEffectType Type;
}

public class ConstructedBuilding
{
    public Building Building;
    public bool Active;
}

[CreateAssetMenu(fileName = "Building", menuName = "Game/Building", order = 1)]
public class Building : ScriptableObject
{
    public string Name;
    public string Description;
    public GameObject VisualsPrefab;
    public Sprite PreviewImage;

    public bool RegionCapital = false;
    public bool Buildable = true;
    public bool Disableable = true;
    public bool Deconstructable = true;

    public int MoneyCost;
    public int ResearchCost;

    public List<ResourceEffect> ResourceEffects;
    public List<SpecialEffect> SpecialEffects;


    public List<ResourceEffect> SortedResourceEffects
    {
        get
        {
            var sorted = from re in ResourceEffects
                         orderby (int)re.AffectedResource
                         select re;
            return sorted.ToList();
        }
    }

    public List<SpecialEffect> SortedSpecialEffects
    {
        get
        {
            return SpecialEffects;
        }
    }

    public int RecurringCostForType(ResourceType type)
    {
        foreach(var effect in ResourceEffects)
        {
            if (effect.AffectedResource == type && effect.EffectAmount < 0)
            {
                return -effect.EffectAmount;
            }
        }
        return 0;
    }

    public int RecurringProductionForType(ResourceType type)
    {
        foreach (var effect in ResourceEffects)
        {
            if (effect.AffectedResource == type && effect.EffectAmount > 0)
            {
                return effect.EffectAmount;
            }
        }
        return 0;
    }
}
