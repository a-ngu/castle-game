using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Attribute
{
    public class AttributeModifier
    {
        public readonly float Multiplier;
        public readonly object Source;

        public AttributeModifier(float multiplier, object source)
        {
            Multiplier = multiplier;
            Source = source;
        }
    }
}
