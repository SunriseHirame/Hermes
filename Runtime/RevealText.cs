using TMPro;
using UnityEngine;

namespace Hirame.Hermes
{
    public class RevealText : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetText;

        [Min (0)]
        [SerializeField] private float characterDelay = 0.1f;

        [Range (0, 1)]
        [SerializeField] private float characterDelayVariance = 0;

        private float timeUpdated;

        private void OnEnable ()
        {
            timeUpdated = Time.time;
            targetText.maxVisibleCharacters = 0;
        }

        private void Update ()
        {
            var time = Time.time;
            var delay = GetCharacterDelay ();
            while (time > timeUpdated + delay)
            {
                targetText.maxVisibleCharacters++;
                timeUpdated += delay;
                delay = GetCharacterDelay ();
            }

            if (targetText.maxVisibleCharacters >= targetText.text.Length)
            {
                enabled = false;
            }
        }

        private float GetCharacterDelay ()
        {
            if (characterDelayVariance == 0)
                return characterDelay;

            var variance = characterDelayVariance * Random.Range (-characterDelayVariance, characterDelayVariance);
            return characterDelayVariance + variance;
        }
        
        private void Reset ()
        {
            if (targetText == false)
                targetText = GetComponent<TMP_Text> ();
        }
    }

}