using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CareerCompass
{
    public class TextButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _background;
        [SerializeField] private Text _label;

        public void Setup(string label, UnityAction onClick)
        {
            _label.text = label;
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(onClick);
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        public void SetHighlighted(bool isHighlighted)
        {
            _background.color = isHighlighted ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.86f, 0.86f, 0.86f);
            _label.color = isHighlighted ? Color.white : Color.black;
        }
    }
}
