using TMPro;
using UnityEngine;

namespace Hirame.Hermes
{
    public class ShowDialog : MonoBehaviour
    {
        [SerializeField] private DialogTree dialogTree;
        [SerializeField] private TMP_Text targetTextField;
        
        private int index = -1;
        
        private void OnEnable ()
        {
            if (dialogTree.TryGetNextEntry (ref index, out var dialog))
                targetTextField.SetText (dialog);
        }
    }

}