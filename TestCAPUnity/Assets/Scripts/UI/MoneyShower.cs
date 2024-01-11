using DG.Tweening;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MoneyShower : MonoBehaviour
    {
        [SerializeField] private Text moneyCount;

        [SerializeField] private bool onlyChangedValue;

        private void OnEnable()
        {
            EventsManager.OnHeartsUpdated += SetNewCount;

            if (!onlyChangedValue)
                SetNewCount();
        }

        private void OnDisable() => EventsManager.OnHeartsUpdated -= SetNewCount;

        private void SetNewCount(int valueChanged)
        {
            if (onlyChangedValue)
            {
                string valueChangedString = valueChanged.ToString();

                if (moneyCount.text != valueChangedString)
                {
                    int value = int.Parse(moneyCount.text);

                    DOTween.To(() => value, x => value = x, int.Parse(valueChangedString), .7f)
                        .OnUpdate(() => { moneyCount.text = value.ToString(); });
                }
            }
            else
                SetNewCount();
        }

        private void SetNewCount()
        {
            string totalGoldCount = PlayerPrefs.GetInt("Hearts", 0).ToString();

            if (moneyCount.text != totalGoldCount)
            {
                int value = int.Parse(moneyCount.text);

                DOTween.To(() => value, x => value = x, int.Parse(totalGoldCount), .7f)
                    .OnUpdate(() => { moneyCount.text = value.ToString(); });
            }
        }
    }
}