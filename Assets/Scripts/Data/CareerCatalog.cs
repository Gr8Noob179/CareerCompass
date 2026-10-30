using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CareerCompass
{
    [Serializable]
    public class CareerCatalog
    {
        public List<string> interests = new List<string>();
        public List<CareerDefinition> careers = new List<CareerDefinition>();

        public CareerDefinition FindCareer(string careerId)
        {
            return careers.Find(career => career.id == careerId);
        }

        public static async Awaitable<CareerCatalog> LoadAsync(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                await webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception("Could not load the careers: " + webRequest.error);
                }

                return JsonUtility.FromJson<CareerCatalog>(webRequest.downloadHandler.text);
            }
        }
    }

    [Serializable]
    public class CareerDefinition
    {
        public string id;
        public string title;
        public int requiredLevel = 1;
        public List<string> interests = new List<string>();
        public List<string> skills = new List<string>();
        public MissionDefinition mission = new MissionDefinition();
    }

    [Serializable]
    public class MissionDefinition
    {
        public string title;
        public string introduction;
        public int experienceReward;
        public List<MissionStep> steps = new List<MissionStep>();
    }

    [Serializable]
    public class MissionStep
    {
        public string prompt;
        public List<MissionChoice> choices = new List<MissionChoice>();
    }

    [Serializable]
    public class MissionChoice
    {
        public string text;
        public string feedback;
        public List<SkillAmount> rewards = new List<SkillAmount>();
    }

    [Serializable]
    public class SkillAmount
    {
        public string skill;
        public int amount;

        public SkillAmount() { }

        public SkillAmount(string skill, int amount)
        {
            this.skill = skill;
            this.amount = amount;
        }
    }
}
