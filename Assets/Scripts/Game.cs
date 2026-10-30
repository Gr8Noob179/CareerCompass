using System;
using UnityEngine;
using UnityEngine.UI;

namespace CareerCompass
{
    public enum AdvisorMode
    {
        Mock,
        Live,
        Outage
    }

    public class Game : MonoBehaviour
    {
        [SerializeField] private string _liveEndpoint = "http://localhost:8787/advice";
        [SerializeField] private float _timeoutSeconds = 5f;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Text _statusText;
        [SerializeField] private CreationScreen _creationScreen;
        [SerializeField] private HubScreen _hubScreen;
        [SerializeField] private MissionScreen _missionScreen;

        public CareerCatalog Catalog { get; private set; }
        public PlayerProfile Profile { get; set; }
        public AdvisorMode Mode { get; set; }

        private async void Start()
        {
            ShowOnly(_statusText.gameObject);
            _statusText.text = "Loading careers...";
            string path = Application.streamingAssetsPath + "/careers.json";

            try
            {
                Catalog = await CareerCatalog.LoadAsync(path.Contains("://") ? path : new Uri(path).AbsoluteUri);
            }
            catch (Exception exception)
            {
                _statusText.text = exception.Message;
                return;
            }

            Profile = PlayerProfile.Load();

            if (Profile == null)
            {
                ShowCreation();
                return;
            }

            ShowHub();
        }

        public IAdvisorService CreateAdvisor()
        {
            MockAdvisorService mock = new MockAdvisorService(Catalog);

            if (Mode == AdvisorMode.Mock)
            {
                return mock;
            }

            string endpoint = Mode == AdvisorMode.Live ? _liveEndpoint : "https://advisor.invalid/advice";
            return new FallbackAdvisorService(new HttpAdvisorService(endpoint, _timeoutSeconds, Catalog), mock);
        }

        public void ShowCreation()
        {
            ShowOnly(_creationScreen.gameObject);
            _creationScreen.Show(this);
        }

        public void ShowHub()
        {
            ShowOnly(_hubScreen.gameObject);
            _hubScreen.Show(this);
        }

        public void ShowMission(CareerDefinition career)
        {
            ShowOnly(_missionScreen.gameObject);
            _missionScreen.Show(this, career);
        }

        public void ScrollToTop()
        {
            _scrollRect.verticalNormalizedPosition = 1f;
        }

        public static void ClearChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }

        private void ShowOnly(GameObject screen)
        {
            _statusText.gameObject.SetActive(screen == _statusText.gameObject);
            _creationScreen.gameObject.SetActive(screen == _creationScreen.gameObject);
            _hubScreen.gameObject.SetActive(screen == _hubScreen.gameObject);
            _missionScreen.gameObject.SetActive(screen == _missionScreen.gameObject);
            ScrollToTop();
        }
    }
}
