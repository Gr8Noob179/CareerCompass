using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CareerCompass
{
    public class MockAdvisorService : IAdvisorService
    {
        private CareerCatalog _catalog;

        public MockAdvisorService(CareerCatalog catalog)
        {
            _catalog = catalog;
        }

        public async Awaitable<AdviceResult> RequestAdviceAsync(AdviceRequest request)
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            AdviceResponse response = new AdviceResponse();
            response.contractVersion = AdviceRequest.ContractVersion;

            foreach (CareerDefinition career in Rank(request).Take(3))
            {
                CareerRecommendation recommendation = new CareerRecommendation();
                recommendation.careerId = career.id;
                recommendation.reason = Explain(career, request);
                response.recommendations.Add(recommendation);
            }

            response.message = request.playerName + ", my top pick for you is " + _catalog.FindCareer(response.recommendations[0].careerId).title + ".";
            AdviceResult result = new AdviceResult();
            result.Response = response;
            result.Source = AdviceSource.Mock;
            return result;
        }

        public IEnumerable<CareerDefinition> Rank(AdviceRequest request)
        {
            return _catalog.careers.OrderByDescending(career => Score(career, request));
        }

        private static int Score(CareerDefinition career, AdviceRequest request)
        {
            int score = career.interests.Count(interest => request.interests.Contains(interest)) * 10;
            score += request.skills.Where(skill => career.skills.Contains(skill.skill)).Sum(skill => skill.amount);

            if (request.completedCareerIds.Contains(career.id))
            {
                score -= 15;
            }

            if (request.level < career.requiredLevel)
            {
                score -= 5;
            }

            return score;
        }

        private static string Explain(CareerDefinition career, AdviceRequest request)
        {
            if (request.level < career.requiredLevel)
            {
                return "Unlocks at level " + career.requiredLevel + ".";
            }

            if (request.completedCareerIds.Contains(career.id))
            {
                return "Already done. Replay it to practice.";
            }

            List<string> matches = career.interests.Where(interest => request.interests.Contains(interest)).ToList();
            return matches.Count > 0 ? "Matches your interest in " + string.Join(" and ", matches) + "." : "Something new to explore.";
        }
    }
}
