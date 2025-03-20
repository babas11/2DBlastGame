using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Controllers.UI
{
    public class UIObjectiveItemController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private TextMeshProUGUI _objectiveRemainingText;
        [SerializeField] private Image _objectiveImage;

        #endregion
        
        #endregion
        
        public void SetItem(Sprite objectiveSprite,byte objectiveRemaining)
        {
            _objectiveRemainingText.text = objectiveRemaining.ToString();
            _objectiveImage.sprite = objectiveSprite;
        }
        
        public void UpdateItem(byte objectiveRemaining)
        {
            _objectiveRemainingText.text = objectiveRemaining.ToString();
        }
    }
}