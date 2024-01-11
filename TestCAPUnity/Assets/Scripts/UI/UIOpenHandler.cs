using Tools;
using UnityEngine;

namespace UI
{
    public class UIOpenHandler : MonoBehaviour
    {
        #region Fields

        [SerializeField] private CanvasGroup unlockCanvas;

        #endregion

        public void OpenUnlockCanvas() => CanvasHandler.ShowCanvas(unlockCanvas);
        
        public void CloseUnlockCanvas() => CanvasHandler.HideCanvas(unlockCanvas);
    }
}
