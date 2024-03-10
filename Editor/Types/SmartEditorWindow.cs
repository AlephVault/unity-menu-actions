using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AlephVault.Unity.MenuActions
{
    namespace Types
    {
        public abstract class SmartEditorWindow<T> : EditorWindow where T : SmartEditorWindow<T>
        {
            /// <summary>
            ///   Gets the document to use.
            /// </summary>
            /// <returns>The document to use</returns>
            protected abstract VisualTreeAsset GetDocument();

            /// <summary>
            ///   Gets the stylesheets to use.
            /// </summary>
            /// <returns>The stylesheets to use</returns>
            protected virtual StyleSheet[] GetStyleSheets()
            {
                return Array.Empty<StyleSheet>();
            }

            /// <summary>
            ///   Tells which element is the pivot to resolve the size.
            /// </summary>
            protected virtual VisualElement GetContainer()
            {
                return rootVisualElement.Q<VisualElement>("Root");
            }

            /// <summary>
            ///   Loads a document from file.
            /// </summary>
            /// <param name="path">The path to the UXML file</param>
            /// <returns>The document asset</returns>
            protected VisualTreeAsset LoadDocument(string path)
            {
                return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            }

            /// <summary>
            ///   Loads a stylesheet from file.
            /// </summary>
            /// <param name="path">The path to the USS file</param>
            /// <returns>The stylesheet asset</returns>
            protected StyleSheet LoadSheet(string path)
            {
                return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            }

            private void InitDocument()
            {
                rootVisualElement.Add(GetDocument().Instantiate());
                foreach (StyleSheet styleSheet in GetStyleSheets())
                {
                    rootVisualElement.styleSheets.Add(styleSheet);
                }
            }

            private void UpdateSize()
            {
                var element = GetContainer();
                var width = element.resolvedStyle.width;
                var height = element.resolvedStyle.height;
                Debug.Log($"Container: {element} Width: {width} Height: {height}");
                minSize = new Vector2(width, height);
                maxSize = minSize;
            }

            /// <summary>
            ///   Executes the inner action and then refreshes
            ///   the size of the window.
            /// </summary>
            /// <param name="action">The action to execute</param>
            protected void DoAndUpdateSize(Action action)
            {
                action();
                rootVisualElement.schedule.Execute(UpdateSize).StartingIn(100);
            }

            /// <summary>
            ///   Performs a setup over the rootVisualElement data.
            /// </summary>
            protected abstract void Setup();

            /// <summary>
            ///   Shows a window and sets its title.
            /// </summary>
            /// <param name="title">The title content</param>
            public static void ShowSmart(GUIContent title)
            {
                T window = CreateInstance<T>();
                window.titleContent = title;
                window.ShowUtility();
            }

            /// <summary>
            ///   Shows a window and sets its title.
            /// </summary>
            /// <param name="title">The title content</param>
            public static void ShowSmart(string title)
            {
                ShowSmart(new GUIContent(title));
            }

            private void OnEnable()
            {
                DoAndUpdateSize(InitDocument);
                Setup();
            }
        }
    }
}
