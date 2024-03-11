using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AlephVault.Unity.MenuActions
{
    namespace Types
    {
        /// <summary>
        ///   This is a smart editor class. It auto-adjusts the size.
        ///   It also provides a way to safely queue actions to do
        ///   after adjusting (e.g. executing a button).
        /// </summary>
        public abstract class SmartEditorWindow : EditorWindow
        {
            /// <summary>
            ///   The close type for when an action is executed.
            /// </summary>
            public enum CloseType
            {
                NoClose, OnSuccess, Always
            }

            // The actions to execute.
            private List<Action> afterAdjustedGUI = new();

            /// <summary>
            ///   A width to be forced to the window. If positive,
            ///   it will be the width of the window. Otherwise,
            ///   it will be disabled and guessed from the rect.
            /// </summary>
            /// <returns>The forced width</returns>
            protected virtual float GetSmartWidth()
            {
                return 400;
            }
            
            /// <summary>
            ///   Runs all the inner GUI logic but wrapping it in a
            ///   Vertical group, and then calculates the size so it
            ///   can update the size later. It runs the post-render
            ///   logic (e.g. buttons execution logic) AFTER the new
            ///   size is applied.
            /// </summary>
            private void OnGUI()
            {
                Rect r = EditorGUILayout.BeginVertical();

                try
                {
                    OnAdjustedGUI();
                }
                finally
                {
                    // r WILL have a value by this time.
                    // It is important that we close here.
                    EditorGUILayout.EndVertical();

                    float buffer = 2f * EditorGUIUtility.standardVerticalSpacing;
                    if (r.size != Vector2.zero)
                    {
                        // The cached width stays forever. It is determined
                        // from scratch and MIGHT be changed later, but it
                        // is typically set only once and never again. On
                        // the other hand, the height may always be forced.
                        float width = GetSmartWidth();
                        if (width <= 0)
                        {
                            width = r.size.x + buffer;
                        }

                        maxSize = minSize = new(width, r.size.y + buffer);
                    }
                }

                OnAfterAdjustedGUI();
            }
            
            /// <summary>
            ///   Defines all the GUI that will be rendered.
            /// </summary>
            protected virtual void OnAdjustedGUI() {}
            
            // Executes all the queued actions.
            private void OnAfterAdjustedGUI()
            {
                try
                {
                    foreach (Action action in afterAdjustedGUI)
                    {
                        action();
                    }
                }
                finally
                {
                    afterAdjustedGUI = new();
                }
            }

            // ReSharper disable Unity.PerformanceAnalysis
            /// <summary>
            ///   Executes an action smartly. It actually waits
            ///   for the UI to be completely rendered and the
            ///   window size to be adjusted and then it runs
            ///   the specified action.
            /// </summary>
            /// <param name="action">The action to execute</param>
            /// <param name="closeType">Whether to close (and how) or not the window</param>
            protected void SmartExecute(Action action, CloseType closeType = CloseType.OnSuccess)
            {
                Action finalAction;
                switch (closeType)
                {
                    case CloseType.OnSuccess:
                        finalAction = () =>
                        {
                            action();
                            Close();
                        };
                        break;
                    case CloseType.Always:
                        finalAction = () =>
                        {
                            try
                            {
                                action();
                            }
                            finally
                            {
                                Close();
                            }
                        };
                        break;
                    default:
                        finalAction = action;
                        break;
                }
                afterAdjustedGUI.Add(finalAction);
            }

            /// <summary>
            ///   Renders a GUI button that, when clicked, executes
            ///   an action smartly (see <see cref="SmartExecute"/>
            ///   for more details).
            /// </summary>
            /// <param name="text">The button text</param>
            /// <param name="action">The action to execute</param>
            /// <param name="closeType">Whether to close (and how) or not the window</param>
            /// <returns>Whether the button was pressed or not</returns>
            protected bool SmartButton(string text, Action action, CloseType closeType = CloseType.OnSuccess)
            {
                if (GUILayout.Button(text))
                {
                    SmartExecute(action, closeType);
                    return true;
                }

                return false;
            }
        }
    }
}
