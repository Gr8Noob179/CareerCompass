using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CareerCompass
{
    public class CreationScreen : MonoBehaviour
    {
        private const int InterestCount = 2;

        [SerializeField] private InputField _nameField;
        [SerializeField] private Transform _interestContainer;
        [SerializeField] private TextButtonView _buttonPrefab;
        [SerializeField] private TextButtonView _startButton;

        private Game _game;
        private List<string> _selectedInterests = new List<string>();
        private Dictionary<string, TextButtonView> _interestButtons = new Dictionary<string, TextButtonView>();

        public void Show(Game game)
        {
            _game = game;
            _selectedInterests.Clear();
            _interestButtons.Clear();
            Game.ClearChildren(_interestContainer);

            foreach (string interest in game.Catalog.interests)
            {
                TextButtonView button = Instantiate(_buttonPrefab, _interestContainer);
                button.Setup(interest, () => ToggleInterest(interest));
                _interestButtons[interest] = button;
            }

            _nameField.onValueChanged.RemoveAllListeners();
            _nameField.onValueChanged.AddListener(changedName => Refresh());
            _startButton.Setup("Start exploring", OnStartClicked);
            Refresh();
        }

        private void ToggleInterest(string interest)
        {
            if (!_selectedInterests.Remove(interest))
            {
                _selectedInterests.Add(interest);
            }

            if (_selectedInterests.Count > InterestCount)
            {
                _selectedInterests.RemoveAt(0);
            }

            Refresh();
        }

        private void Refresh()
        {
            foreach (KeyValuePair<string, TextButtonView> entry in _interestButtons)
            {
                entry.Value.SetHighlighted(_selectedInterests.Contains(entry.Key));
            }

            _startButton.SetInteractable(_nameField.text.Trim().Length > 0 && _selectedInterests.Count == InterestCount);
        }

        private void OnStartClicked()
        {
            PlayerProfile profile = new PlayerProfile();
            profile.displayName = _nameField.text.Trim();
            profile.interests = new List<string>(_selectedInterests);
            profile.Save();
            _game.Profile = profile;
            _game.ShowHub();
        }
    }
}
