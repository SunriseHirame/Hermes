using System.Collections.Generic;
using UnityEngine;

namespace Hirame.Hermes
{
    [CreateAssetMenu (menuName = "Hirame/Hermes/Dialog Tree")]
    public class DialogTree : ScriptableObject
    {
        public List<DialogEntry> DialogEntries;

        public bool TryGetNextEntry (ref int index, out string dialog)
        {
            dialog = null;
            if (index + 1 >= DialogEntries.Count)
                return false;

            index++;
            dialog = DialogEntries[index].Content;
            return true;
        }
        
        
    }

}
