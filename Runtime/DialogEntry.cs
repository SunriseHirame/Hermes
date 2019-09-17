using System;
using UnityEngine;

namespace Hirame.Hermes
{
    [System.Serializable]
    public class DialogEntry : ScriptableObject
    {
        public Rect Position;

        public string Title;
        public string Content;
        
        public DialogEntry[] Branches;
        
        public Vector2 InPosition => new Vector2(Position.xMin, Position.center.y);
        public Vector2 OutPosition => new Vector2(Position.xMax, Position.center.y);

        public void AddBranch (DialogEntry entry)
        {
            var length = Branches.Length;
            Array.Resize (ref Branches, length + 1);
            Branches[length] = entry;
        }
    }
}
