using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Ingredient : Resource
{
    //A resource represents the "mathematical" change that can get slotted into 

    [ExportGroup("Always")] //these are always used
    [Export] public Dictionary<PotionStats, float> AlwaysStatChange = new Dictionary<PotionStats, float>();

    [ExportGroup("As Base")] //these are only used when you have a base
    [Export] public Dictionary<PotionStats, float> BaseStateChange = new Dictionary<PotionStats, float>();
    //I don't think we need anything else here...

    [ExportGroup("As Modifier")] //these are only used when you use it as a modifier - not gonna deal with something that modifies in multiple ways rn (-S)
    [Export] public ModifierType ModType;
    [Export] public float ModStrength;
    [Export] public int ModStepsModifier = -1; //-1 applies to all previous steps, other values only apply to the X previous valid steps

    [ExportGroup("As MetaMod")]
    [Export] public ModifierType MetaModType;
    [Export] public float MetaModStrength;
    [Export] public int MetaStepsModifier = -1; //-1 aapplies to all previous steps, other values only apply to the X previous valid steps

}


/// <summary>
/// The stats that an ingredient can modify - people will want certain ranges of these effects in their potions
/// </summary>
public enum PotionStats
{
    Flavor,
    Vision,
    Vitality,
    Energy,
    Charm
}

/// <summary>
/// Modifier types are how a modifier or metamod can change what it is applied to.
/// </summary>
public enum ModifierType
{
    NONE, //ingredient does nothing as this type of mod
    Clarity, //changes the clarity of the bases that it effects
    Linear, //changes the value directly
    Scalar, //multiplies the value by this amount
    Percentile, //changes the value by a % increase or decrease - equates to multiplying by (1 + X/100)
}
