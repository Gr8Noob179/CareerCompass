using UnityEngine;

namespace CareerCompass
{
    public class FallbackAdvisorService : IAdvisorService
    {
        private IAdvisorService _primary;
        private IAdvisorService _fallback;

        public FallbackAdvisorService(IAdvisorService primary, IAdvisorService fallback)
        {
            _primary = primary;
            _fallback = fallback;
        }

        public async Awaitable<AdviceResult> RequestAdviceAsync(AdviceRequest request)
        {
            AdviceResult result = await _primary.RequestAdviceAsync(request);

            if (result.Succeeded)
            {
                return result;
            }

            AdviceResult fallbackResult = await _fallback.RequestAdviceAsync(request);
            fallbackResult.Source = AdviceSource.Fallback;
            fallbackResult.Error = result.Error;
            return fallbackResult;
        }
    }
}
