using System.Collections;
using Configs;
using DG.Tweening;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EnoughShower : MonoBehaviour
    {
        #region Fields

        [Header("Skill Components")]
        [SerializeField] private Text price;
        [SerializeField] private Image goldCoin;
        [SerializeField] private Image blockedImage;
        [SerializeField] private string skinName;
        [SerializeField] private SkillsConfig allSkins;

        [SerializeField] private Button buyButton, equipButton;

        [SerializeField] private Sprite blockedSprite, equippedSprite;

        [SerializeField] private Text equippedText;

        [SerializeField] private string[] storeInteractiveTexts;

        [SerializeField] private ParticleSystem buyParticles;

        private bool _updating;

        private Skill _currSkin;

        #endregion
        
        private void OnEnable()
        {
            FindCurrentSkin();
            EventsManager.OnSkillUpdated += SetNewStatus;
            EventsManager.OnHeartsUpdated += SetNewStatus;
            SetNewStatus(0);
        }

        private void OnDisable()
        {
            EventsManager.OnHeartsUpdated -= SetNewStatus;
            EventsManager.OnSkillUpdated -= SetNewStatus;
        }

        public void SetNewStatus(int changedValue)
        {
            if (_updating) 
                return;
  
            _updating = true;
            FindCurrentSkin();
            
            string totalGoldCount = PlayerPrefs.GetInt("Hearts", 0).ToString();
            int priceValue = int.Parse(price.text);


            if (_currSkin.IsBought)
            {
                blockedImage.CrossFadeAlpha(0f, 1f, true);
                blockedImage.sprite = equippedSprite;
                price.enabled = false;
                goldCoin.enabled = false;
                buyButton.enabled = false;
                equipButton.enabled = true;
            }
            else
            {
                equipButton.enabled = false;
                if (priceValue < int.Parse(totalGoldCount))
                {
                    price.color = Color.green;
                    blockedImage.CrossFadeAlpha(0f, 1f, true);
                }
                else
                {
                    blockedImage.sprite = blockedSprite;
                }
            }

            if (_currSkin.IsEquipped)
            {
                blockedImage.CrossFadeAlpha(1f, 1f, true);
                blockedImage.sprite = equippedSprite;
                
                ColorBlock buttonColors = equipButton.colors;
                buttonColors.normalColor = new Color(buttonColors.normalColor.r, buttonColors.normalColor.g, buttonColors.normalColor.b, 30f / 255f);
                equipButton.colors = buttonColors;

                EventsManager.OnSkillUpdated?.Invoke(0);
            }
            else
            {
                blockedImage.CrossFadeAlpha(0f, 1f, true);
            }
            
            _updating = false;
        }


        public void BuyThis()
        {
            int totalGoldCount = PlayerPrefs.GetInt("Hearts", 0);

            int priceInt = int.Parse(price.text);

            if (priceInt <= totalGoldCount)
            {
                PlayerPrefs.SetInt("Hearts", totalGoldCount - priceInt);
                
                EventsManager.OnHeartsUpdated?.Invoke(priceInt);

                foreach (var skin in allSkins.AllSkills)
                {
                    if (skin.Name == skinName)
                        skin.SetBought();
                }
                buyButton.enabled = false;
                equipButton.enabled = true;
                
                buyParticles.gameObject.SetActive(true);
                buyParticles.Play();
            }
            else
            {
                FadeText(1);
            }


            SetNewStatus(0);
        }

        public void EquipThis()
        {
            if (_currSkin.IsBought)
            {
                _currSkin.SetEquipped(allSkins);
                SetNewStatus(0);
                FadeText(0);
                
                StartCoroutine(ReloadSceneAfterDelay(1f));
            }
        }

        private void FindCurrentSkin()
        {
            foreach (var skin in allSkins.AllSkills)
                if (skin.Name == skinName)
                    _currSkin = skin;
        }

        private void FadeText(int indexText)
        {
            equippedText.DOKill();

            equippedText.text = storeInteractiveTexts[indexText];
            equippedText.DOFade(1f, 0.5f)
                .OnComplete(() =>
                {
                    equippedText.DOFade(1f, 2f)
                        .OnComplete(() =>
                        {
                            equippedText.DOFade(0f, 0.5f);
                        });
                });
        }
        
        private IEnumerator ReloadSceneAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            DOTween.KillAll();
            DOTween.Clear();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}

