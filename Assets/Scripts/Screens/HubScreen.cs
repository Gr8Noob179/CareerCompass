using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CareerCompass
{
    public class HubScreen : MonoBehaviour
    {
        [SerializeField] private Text _profileText;
        [SerializeField] private TextButtonView _mockButton;
        [SerializeField] private TextButtonView _liveButton;
        [SerializeField] private TextButtonView _outageButton;
        [SerializeField] private TextButtonView _askButton;
        [SerializeField] private Text _adviceText;
        [SerializeField] private Transform _recommendationContainer;
        [SerializeField] private Transform _careerContainer;
        [SerializeField] private TextButtonView _buttonPrefab;
        [SerializeField] private TextButtonView _resetButton;

        private Game _game;
        private int _requestNumber;

        public void Show(Game game)
        {
            _game = game;
            _requestNumber++;
            PlayerProfile profile = game.Profile;
            List<string> skillLines = profile.skills.ConvertAll(skill => skill.skill + " " + skill.amount);
            _profileText.text = profile.displayName + "\nLevel " + Progression.GetLevel(profile.experience) + " · " + profile.experience + " XP\nSkills: " + (skillLines.Count > 0 ? string.Join(", ", skillLines) : "none yet");
            _mockButton.Setup("Mock", () => SetMode(AdvisorMode.Mock));
            _liveButton.Setup("Live API", () => SetMode(AdvisorMode.Live));
            _outageButton.Setup("Outage test", () => SetMode(AdvisorMode.Outage));
            _askButton.Setup("Ask the advisor", OnAskClicked);
            _askButton.SetInteractable(true);
            _resetButton.Setup("Reset progress", OnResetClicked);
            _adviceText.text = "";
            Game.ClearChildren(_recommendationContainer);
            Game.ClearChildren(_careerContainer);
            SetMode(game.Mode);

            foreach (CareerDefinition career in game.Catalog.careers)
            {
                bool isUnlocked = Progression.IsUnlocked(career, profile.experience);
                string action = profile.completedCareerIds.Contains(career.id) ? "Replay: " : "Start: ";
                string label = career.title + "\n" + (isUnlocked ? action + career.mission.title : "Locked until level " + career.requiredLevel);
                AddCareerButton(_careerContainer, career, label).SetInteractable(isUnlocked);
            }
        }

        private void SetMode(AdvisorMode mode)
        {
            _game.Mode = mode;
            _mockButton.SetHighlighted(mode == AdvisorMode.Mock);
            _liveButton.SetHighlighted(mode == AdvisorMode.Live);
            _outageButton.SetHighlighted(mode == AdvisorMode.Outage);
        }

        private async void OnAskClicked()
        {
            _requestNumber++;
            int requestNumber = _requestNumber;
            _askButton.SetInteractable(false);
            _adviceText.text = "Thinking...";
            Game.ClearChildren(_recommendationContainer);
            AdviceResult result = await _game.CreateAdvisor().RequestAdviceAsync(new AdviceRequest(_game.Profile));

            if (requestNumber != _requestNumber || _game.Profile == null)
            {
                return;
            }

            _askButton.SetInteractable(true);

            if (!result.Succeeded)
            {
                _adviceText.text = "The advisor failed: " + result.Error;
                return;
            }

            string fallbackNote = result.Source == AdviceSource.Fallback ? " (live API failed: " + result.Error + ")" : "";
            _adviceText.text = "Source: " + result.Source + fallbackNote + "\n" + result.Response.message;

            foreach (CareerRecommendation recommendation in result.Response.recommendations)
            {
                CareerDefinition career = _game.Catalog.FindCareer(recommendation.careerId);
                AddCareerButton(_recommendationContainer, career, career.title + "\n" + recommendation.reason).SetInteractable(Progression.IsUnlocked(career, _game.Profile.experience));
            }
        }

        private TextButtonView AddCareerButton(Transform container, CareerDefinition career, string label)
        {
            TextButtonView button = Instantiate(_buttonPrefab, container);
            button.Setup(label, () => _game.ShowMission(career));
            return button;
        }

        private void OnResetClicked()
        {
            _requestNumber++;
            PlayerProfile.Delete();
            _game.Profile = null;
            _game.ShowCreation();
        }
    }
}
