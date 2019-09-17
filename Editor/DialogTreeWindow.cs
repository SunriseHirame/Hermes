using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hirame.Hermes.Editor
{
    public class DialogTreeWindow : EditorWindow
    {
        private enum InteractionMode { Default, CreateConnection }

        private InteractionMode interactionInteractionMode = InteractionMode.Default;
        
        public DialogTree Context;
        private SerializedObject SerializationData;
        
        private Rect dialogEditRect;
        private int currentlySelectedNode = -1;

        [MenuItem ("Hirame Tools/Dialog Tree Editor")]
        public static void OpenWindow ()
        {
            InitializeWindow ();
        }

        public static void OpenWindowWithContext (DialogTree treeData)
        {
            var window = InitializeWindow ();
            window.InitializeWindow (treeData);
        }

        private static DialogTreeWindow InitializeWindow ()
        {
            var window = GetWindow<DialogTreeWindow> ();
            window.titleContent = new GUIContent ("Dialog Tree Editor");

            window.dialogEditRect = window.position;
            window.dialogEditRect.x = 0;
            window.dialogEditRect.y = window.dialogEditRect.height - 200;
            window.dialogEditRect.height = 200;

            window.Show ();
            return window;
        }

        private void InitializeWindow (DialogTree treeData)
        {
            Context = treeData;
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        }

        private void OnGUI ()
        {
            if (Context == null)
            {
                GUI.Label (position, "No Dialog Tree Selected");
                return;
            }


            if (Context.DialogEntries == null)
            {
                Context.DialogEntries = new List<DialogEntry> ();
            }

            switch (interactionInteractionMode)
            {
                case InteractionMode.Default:
                    ResolveInput ();
                    DrawConnections ();
                    DrawWindows ();
                    break;
                case InteractionMode.CreateConnection:
                    //ResolveInput ();
                    ResolveConnection ();

                    DrawConnections ();
                    DrawWindows ();
                    break;
                default:
                    throw new ArgumentOutOfRangeException ();
            }
        }

        private void ResolveConnection ()
        {
            var eventCurrent = Event.current;
            
            if (IsMouseDownEvent (eventCurrent))
            {
                if (eventCurrent.button == 0 && IsMouseOverNode (out var nodeId))
                {
                    if (currentlySelectedNode != nodeId)
                    {
                        var currentNode = Context.DialogEntries[currentlySelectedNode];
                        var targetNode = Context.DialogEntries[nodeId];
                        
                        currentNode.AddBranch (targetNode);
                    }
                }
                
                interactionInteractionMode = InteractionMode.Default;
                eventCurrent.Use ();
            }
            else if (eventCurrent.type == EventType.Repaint)
            {
                var currentNode = Context.DialogEntries[currentlySelectedNode];
                var inPos = eventCurrent.mousePosition;
                var tangentMult = (inPos.x - currentNode.OutPosition.x) * 0.7f;

                Handles.DrawBezier (
                    currentNode.OutPosition,
                    inPos,
                    currentNode.OutPosition + Vector2.right * tangentMult,
                    inPos + Vector2.left * tangentMult,
                    Color.green,
                    Texture2D.whiteTexture,
                    2);
            }
        }

        private void ResolveInput ()
        {
            var eventCurrent = Event.current;

            if (IsMouseDownEvent (eventCurrent))
            {
                if (eventCurrent.button == 0)
                {
                    if (IsMouseOverNode (out var nodeId))
                    {
                        currentlySelectedNode = nodeId;
                    }
                }
                else if (eventCurrent.button == 1)
                {
                    ShowRightClickMenu (eventCurrent);
                }
            }
        }

        private bool IsMouseDownEvent (Event current)
        {
            return current.isMouse && current.type == EventType.MouseDown;
        }
        
        private void DrawConnections ()
        {
            foreach (var node in Context.DialogEntries)
            {
                foreach (var branch in node.Branches)
                {
                    var outPos = node.OutPosition;
                    var inPos = branch.InPosition;

                    var tangentMult = (inPos.x - outPos.x) * 0.7f;
                
                    Handles.DrawBezier (
                        outPos,
                        inPos,
                        outPos + Vector2.right * tangentMult,
                        inPos + Vector2.left * tangentMult,
                        Color.green,
                        Texture2D.whiteTexture,
                        2);
                }
            }
        }

        private void DrawWindows ()
        {
            BeginWindows ();

            var entries = Context.DialogEntries;
            for (var i = 0; i < entries.Count; i++)
            {
                entries[i].Position = GUI.Window (i, entries[i].Position, DrawNodeWindow, entries[i].Title);
            }

            DrawDialogEditorWindow ();

            EndWindows ();
        }

        private void DrawNodeWindow (int id)
        {
            GUI.DragWindow ();
        }
        private bool IsMouseOverNode (out int id)
        {
            var mousePosition = Event.current.mousePosition;
            var nodes = Context.DialogEntries;

            for (var i = 0; i < nodes.Count; i++)
            {
                if (!nodes[i].Position.Contains (mousePosition))
                    continue;

                id = i;
                return true;
            }

            id = -1;
            return false;
        }
        
        private void ShowRightClickMenu (Event currentEvent)
        {
            var mousePosition = currentEvent.mousePosition;
            var menu = new GenericMenu ();

            if (IsMouseOverNode (out var nodeId))
            {
                menu.AddItem (
                    new GUIContent ("Add Branch"), 
                    false, 
                    data => { CreateNewNode (mousePosition, data as DialogEntry); }, 
                    Context.DialogEntries[nodeId]);
                menu.AddItem (
                    new GUIContent ("Create Connection"), 
                    false,
                    data =>
                    {
                        currentlySelectedNode = nodeId;
                        interactionInteractionMode = InteractionMode.CreateConnection;
                    }, 
                    Context.DialogEntries[nodeId]);
            }
            else
            {
                menu.AddItem (new GUIContent ("New Node"), 
                    false, 
                    data => { CreateNewNode (mousePosition, data as DialogEntry); }, 
                    null);
            }
            
            menu.ShowAsContext ();
            currentEvent.Use ();
        }

        private void DrawDialogEditorWindow ()
        {
            var title = currentlySelectedNode != -1 ? Context.DialogEntries[currentlySelectedNode].Title : "DIALOG";
            
            dialogEditRect = GUI.Window (-100, dialogEditRect, DialogEditor, title);
            dialogEditRect.x = 0;
            dialogEditRect.width = position.width;
            dialogEditRect.height = position.height - dialogEditRect.y;

            if (dialogEditRect.height < 40)
            {
                dialogEditRect.y = position.height - 40;
                dialogEditRect.height = 40;
            }
        }

        private void DialogEditor (int id)
        {
            var textRect = dialogEditRect;
            textRect.x = 0;
            textRect.y = 15;
            textRect.height = dialogEditRect.height - 15;

            var dialog = currentlySelectedNode != -1 ? Context.DialogEntries[currentlySelectedNode] : null;
            if (dialog != null)
                dialog.Content = GUI.TextArea (textRect, dialog.Content);
            
            GUI.DragWindow ();
        }

        private DialogEntry CreateNode (Vector2 position, string title)
        {
            var node = CreateInstance<DialogEntry> ();
            node.Title = title;
            node.Position = new Rect(position, new Vector2(120, 35));
            node.Branches = new DialogEntry[0];
            
            if (Context.DialogEntries == null)
                Context.DialogEntries = new List<DialogEntry> ();
            
            Context.DialogEntries.Add (node);
            AssetDatabase.AddObjectToAsset (node, Context);
            AssetDatabase.Refresh ();

            
            return node;
        }

        private void CreateNewNode (Vector2 position, DialogEntry node)
        {
            var createdNode = CreateNode (position, $"Title {Context.DialogEntries.Count.ToString()}");

            if (node != null)
            {
                AddBranchToNode (node, createdNode);
            }
        }

        private static void AddBranchToNode (DialogEntry parent, DialogEntry child)
        {
            var parentBranches = parent.Branches.Length;
            Array.Resize (ref parent.Branches, parentBranches + 1);
            parent.Branches[parentBranches] = child;
        }
    }
}