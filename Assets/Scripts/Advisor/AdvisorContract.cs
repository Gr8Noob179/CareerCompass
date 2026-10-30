using System;
using System.Collections.Generic;
using UnityEngine;

namespace CareerCompass
{
    public interface IAdvisorService
    {
        public Awaitable<AdviceResult> RequestAdviceAsync(AdviceRequest request);
    }

    public enum AdviceSource
    {
        Mock,
        Live,
        Fallback
    }

    public class AdviceResult
    {
        public AdviceResponse Response;
        public AdviceSource Source;
        public string Error = "";

        public bool Succeeded
        {
            get
            {
                return Response != null;
            }
        }
    }

    [Serializable]
    public class AdviceRequest
    {
        public const string ContractVersion = "1.0";

        public string contractVersion = ContractVersion;
        public string playerName;
        public int level;
        public List<string> interests;
        public List<SkillAmount> skills;
        public List<string> completedCareerIds;

        public AdviceRequest(PlayerProfile profile)
        {
            playerName = profile.displayName;
            level = Progression.GetLevel(profile.experience);
            interests = profile.interests;
            skills = profile.skills;
            completedCareerIds = profile.completedCareerIds;
        }
    }

    [Serializable]
    public class AdviceResponse
    {
        public string contractVersion;
        public string message;
        public List<CareerRecommendation> recommendations = new List<CareerRecommendation>();
    }

    [Serializable]
    public class CareerRecommendation
    {
        public string careerId;
        public string reason;
    }
}
