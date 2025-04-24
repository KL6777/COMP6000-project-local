using System;
using System.Collections.Generic;
using UnityEngine;

public class Personality
{
    public string PersonalityType { get; private set; }
    public float SadHappyMultiplier { get; private set; }
    public float DisgustTrustMultiplier { get; private set; }
    public float AngerFearMultiplier { get; private set; }
    public float AnticipationSurpriseMultiplier { get; private set; }

    public Personality(string personalityType, float sadHappyMultiplier, float disgustTrustMultiplier, float angerFearMultiplier, float anticipationSurpriseMultiplier)
    {
        PersonalityType = personalityType;
        SadHappyMultiplier = sadHappyMultiplier;
        DisgustTrustMultiplier = disgustTrustMultiplier;
        AngerFearMultiplier = angerFearMultiplier;
        AnticipationSurpriseMultiplier = anticipationSurpriseMultiplier;
    }
}