
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Attribute
{
    [Serializable]
    public class Attribute
    {
        [SerializeField] private int _baseValue;
        private List<AttributeModifier> _modifiers;

        private bool _isDirty;
        private int _value;
        public int Value
        {
            get
            {
                if (_isDirty)
                {
                    UpdateValue();
                }
                return _value;
            }
            private set => _value = value;
        }

        public Attribute(int baseValue)
        {
            _baseValue = baseValue;
            _modifiers = new List<AttributeModifier>();
            _isDirty = false;
        }

        public void AddModifier(AttributeModifier modifier)
        {
            _modifiers.Add(modifier);
            _isDirty = true;
        }

        public void RemoveModifier(AttributeModifier modifier)
        {
            _modifiers.Remove(modifier);
            _isDirty = true;
        }

        public void RemoveSource(object source)
        {
            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                if (_modifiers[i].Source.Equals(source))
                {
                    _modifiers.RemoveAt(i);
                    _isDirty = true;
                }
            }
        }

        private void UpdateValue()
        {
            float tempVal = _baseValue;
            foreach (AttributeModifier modifier in _modifiers)
            {
                tempVal *= (modifier.Multiplier + 1);
            }
            _value = Mathf.RoundToInt(tempVal);
            _isDirty = false;
        }
    }
}
