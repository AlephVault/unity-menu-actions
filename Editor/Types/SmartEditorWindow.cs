using UnityEditor;
using UnityEngine;

namespace AlephVault.Unity.MenuActions
{
    namespace Types
    {
        /// <summary>
        ///   This is a smart editor class. It auto-adjusts the size.
        /// </summary>
        public abstract class SmartEditorWindow : EditorWindow
        {
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
            
            /// <summary>
            ///   Applies whatever post-render logic is needed.
            /// </summary>
            protected virtual void OnAfterAdjustedGUI() {}
        }
    }
}
