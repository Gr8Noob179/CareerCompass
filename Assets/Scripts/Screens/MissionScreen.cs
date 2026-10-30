using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CareerCompass
{
    public class MissionScreen : MonoBehaviour
    {
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _bodyText;
        [SerializeField] private Transform _choiceContainer;
        [SerializeField] private TextButtonView _buttonPrefab;
        [SerializeField] private TextButtonView _continueButton;
        [SerializeField] private TextButtonView _leaveButton;

        private Game _game;
        private CareerDefinition _career;
        private int _stepIndex;
        private List<SkillAmount> _earnedRewards = new List<SkillAmount>();

        public void Show(Game game, CareerDefinition career)
        {
            _game = game;
            _career = career;
            _stepIndex = 0;
            _earnedRewards.Clear();
            _leaveButton.Setup("Leave mission", game.ShowHub);
            ShowStep();
        }

        private void ShowStep()
        {
            MissionStep step = _career.mission.steps[_stepIndex];
            _titleText.text = _career.title + " · " + _career.mission.title + "\nStep " + (_stepIndex + 1) + " of " + _career.mission.steps.Count;
            _bodyText.text = (_stepIndex == 0 ? _career.mission.introduction + "\n\n" : "") + step.prompt;
            _continueButton.gameObject.SetActive(false);
            Game.ClearChildren(_choiceContainer);

            foreach (MissionChoice choice in step.choices)
            {
                TextButtonView button = Instantiate(_buttonPrefab, _choiceContainer);
                button.Setup(choice.text, () => OnChoiceClicked(choice));
            }

            _game.ScrollToTop();
        }

        private void OnChoiceClicked(MissionChoice choice)
        {
            _earnedRewards.AddRange(choice.rewards);
            List<string> rewardLines = choice.rewards.ConvertAll(reward => "+" + reward.amount + " " + reward.skill);
            _bodyText.text = "You chose: " + choice.text + "\n\n" + choice.feedback + "\n\n" + string.Join("\n", rewardLines);
            Game.ClearChildren(_choiceContainer);
            bool isLastStep = _stepIndex == _career.mission.steps.Count - 1;
            _continueButton.gameObject.SetActive(true);
            _continueButton.Setup(isLastStep ? "Finish mission" : "Next step", isLastStep ? Finish : NextStep);
        }

        private void NextStep()
        {
            _stepIndex++;
            ShowStep();
        }

        private void Finish()
        {
            int previousLevel = Progression.GetLevel(_game.Profile.experience);
            int experienceGained = Progression.CompleteMission(_game.Profile, _career, _earnedRewards);
            int newLevel = Progression.GetLevel(_game.Profile.experience);
            _game.Profile.Save();
            _bodyText.text = experienceGained > 0 ? "Mission complete! +" + experienceGained + " XP" : "Practice run. Rewards are only given the first time.";

            if (newLevel > previousLevel)
            {
                _bodyText.text += "\nLevel up! You reached level " + newLevel + ".";
            }

            _continueButton.gameObject.SetActive(false);
            _leaveButton.Setup("Back to careers", _game.ShowHub);
        }
    }
}
