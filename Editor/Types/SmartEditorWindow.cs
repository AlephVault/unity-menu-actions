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
                    minSize = r.size + new Vector2(
                        0, GetVerticalSpacesCount() * EditorGUIUtility.standardVerticalSpacing
                    );
                    maxSize = minSize;
                }

                OnAfterAdjustedGUI();
            }

            /// <summary>
            ///   Returns the amount of vertical spacings to use when rendering.
            /// </summary>
            protected virtual uint GetVerticalSpacesCount()
            {
                return 3;
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
