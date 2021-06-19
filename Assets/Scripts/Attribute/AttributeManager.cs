using System;
using System.Collections.Generic;
using UnityEngine;

namespace CastleGame
{
    public class AttributeManager
    {
        public Attribute MaxHitPoints, MaxManaPoints;
        public Attribute Attack, Defense;

        public AttributeManager(int HP, int MP, int Atk, int Def)
        {
            MaxHitPoints = new Attribute(HP);
            MaxManaPoints = new Attribute(MP);
            Attack = new Attribute(Atk);
            Defense = new Attribute(Def);
        }
    }
}
