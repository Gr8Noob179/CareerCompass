using System.Collections.Generic;

namespace CareerCompass
{
    public static class Progression
    {
        public static int[] LevelThresholds = new int[] { 0, 200, 450 };

        public static int GetLevel(int experience)
        {
            int level = 1;

            for (int index = 1; index < LevelThresholds.Length; index++)
            {
                if (experience >= LevelThresholds[index])
                {
                    level = index + 1;
                }
            }

            return level;
        }

        public static bool IsUnlocked(CareerDefinition career, int experience)
        {
            return GetLevel(experience) >= career.requiredLevel;
        }

        public static int CompleteMission(PlayerProfile profile, CareerDefinition career, List<SkillAmount> rewards)
        {
            if (profile.completedCareerIds.Contains(career.id))
            {
                return 0;
            }

            foreach (SkillAmount reward in rewards)
            {
                profile.AddSkill(reward.skill, reward.amount);
            }

            profile.completedCareerIds.Add(career.id);
            profile.experience += career.mission.experienceReward;
            return career.mission.experienceReward;
        }
    }
}
