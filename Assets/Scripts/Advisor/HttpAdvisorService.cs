using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CareerCompass
{
    public class HttpAdvisorService : IAdvisorService
    {
        private string _endpoint;
        private float _timeoutSeconds;
        private CareerCatalog _catalog;

        public HttpAdvisorService(string endpoint, float timeoutSeconds, CareerCatalog catalog)
        {
            _endpoint = endpoint;
            _timeoutSeconds = timeoutSeconds;
            _catalog = catalog;
        }

        public async Awaitable<AdviceResult> RequestAdviceAsync(AdviceRequest request)
        {
            AdviceResult result = new AdviceResult();
            result.Source = AdviceSource.Live;

            using (UnityWebRequest webRequest = new UnityWebRequest(_endpoint, UnityWebRequest.kHttpVerbPOST))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(request)));
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                _ = webRequest.SendWebRequest();
                float deadline = Time.realtimeSinceStartup + _timeoutSeconds;

                while (!webRequest.isDone)
                {
                    if (Time.realtimeSinceStartup > deadline)
                    {
                        webRequest.Abort();
                        result.Error = "timed out";
                        return result;
                    }

                    await Awaitable.NextFrameAsync();
                }

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    result.Error = webRequest.error;
                    return result;
                }

                string error;
                result.Response = Validate(webRequest.downloadHandler.text, _catalog, out error);
                result.Error = error;
                return result;
            }
        }

        public static AdviceResponse Validate(string json, CareerCatalog catalog, out string error)
        {
            AdviceResponse response;

            try
            {
                response = JsonUtility.FromJson<AdviceResponse>(json);
            }
            catch (ArgumentException)
            {
                error = "the response is not valid JSON";
                return null;
            }

            if (response == null || response.contractVersion == null || !response.contractVersion.StartsWith("1."))
            {
                error = "unsupported contract version";
                return null;
            }

            response.recommendations.RemoveAll(recommendation => catalog.FindCareer(recommendation.careerId) == null);
            error = "";
            return response;
        }
    }
}
