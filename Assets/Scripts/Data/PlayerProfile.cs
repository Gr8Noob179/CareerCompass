using System;
using System.Collections.Generic;
using UnityEngine;

namespace CareerCompass
{
    [Serializable]
    public class PlayerProfile
    {
        private const string StorageKey = "CareerCompass.Profile";

        public string displayName;
        public int experience;
        public List<string> interests = new List<string>();
        public List<SkillAmount> skills = new List<SkillAmount>();
        public List<string> completedCareerIds = new List<string>();

        public void AddSkill(string skill, int amount)
        {
            SkillAmount entry = skills.Find(candidate => candidate.skill == skill);

            if (entry == null)
            {
                skills.Add(new SkillAmount(skill, amount));
                return;
            }

            entry.amount += amount;
        }

        public void Save()
        {
            PlayerPrefs.SetString(StorageKey, JsonUtility.ToJson(this));
            PlayerPrefs.Save();
        }

        public static PlayerProfile Load()
        {
            if (!PlayerPrefs.HasKey(StorageKey))
            {
                return null;
            }

            return JsonUtility.FromJson<PlayerProfile>(PlayerPrefs.GetString(StorageKey));
        }

        public static void Delete()
        {
            PlayerPrefs.DeleteKey(StorageKey);
            PlayerPrefs.Save();
        }
    }
}
