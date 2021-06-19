using System;
using System.Collections.Generic;
using UnityEngine;

namespace CastleGame
{
    public class CharacterAttributes : MonoBehaviour
    {
        [SerializeField]
        private int _baseHP, _baseMP, _baseAtk, _baseDef;

        public int HP { get; private set; }
        public int MP { get; private set; }

        public AttributeManager attributes;

        public event Action OnDeathEvent;

        private void Start()
        {
            attributes = new AttributeManager(_baseHP, _baseMP, _baseAtk, _baseDef);
        }

        public void FillHP(int amount)
        {
            HP = Math.Min(HP + amount, attributes.MaxHitPoints.Value);
        }

        public void DrainHP(int amount)
        {
            HP -= (amount - attributes.Defense.Value);
            if (HP <= 0)
            {
                Die();
            }
        }

        public void FillMP(int amount)
        {
            MP = Math.Min(MP + amount, attributes.MaxManaPoints.Value);
        }

        public void DrainMP(int amount)
        {
            MP = Math.Max(0, MP - amount);
        }

        private void Die()
        {
            if (OnDeathEvent != null)
            {
                OnDeathEvent();
            }
            Destroy(gameObject);
        }
    }
}