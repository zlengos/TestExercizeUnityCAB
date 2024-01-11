using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Configs
{
    [CreateAssetMenu(order = 51, fileName = "SO_Skins", menuName = "Artefact/SkinsTable")]
    public class SkillsConfig : ScriptableObject
    {
        #region Fields

        [Header("Enemies")] 
        [SerializeField] private Skill[] allSkills;

        #region Properties

        public Skill[] AllSkills => allSkills;

        #endregion

        #endregion
    }

    [Serializable]
    public class Skill
    {
        #region Fields

        [Header("Skill Properties")] 
        [SerializeField] private string name;
        [SerializeField] private Sprite sprite;
        
        [SerializeField] private float cost;
        
        [SerializeField] private bool isBought;
        
        [SerializeField] private bool isEquipped;

        #region Properties

        public string Name => name;
        
        public Sprite Sprite => sprite;

        public float Cost => cost;
        
        public bool IsBought => isBought;

        public bool IsEquipped => isEquipped;
        
        #endregion

        public void SetBought() => isBought = true;

        public void SetEquipped(SkillsConfig skins)
        {
            foreach (var skin in skins.AllSkills)
            {
                skin.isEquipped = false;
            }
            this.isEquipped = true;
        }
        
        #endregion
    }
}