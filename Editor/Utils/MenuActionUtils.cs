using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text.RegularExpressions;

namespace AlephVault.Unity.MenuActions
{
    namespace Utils
    {
        using AlephVault.Unity.Layout.Utils;

        public static class MenuActionUtils
        {
            public static GUIStyle GetWordWrappingStyle()
            {
                GUIStyle style = new GUIStyle();
                style.wordWrap = true;
                return style;
            }

            public static GUIStyle GetSingleLabelStyle()
            {
                GUIStyle style = GetWordWrappingStyle();
                style.wordWrap = true;
                style.margin.left = 6;
                style.margin.right = 6;
                style.margin.bottom = 3;
                style.margin.top = 3;
                return style;
            }

            public static GUIStyle GetCaptionLabelStyle()
            {
                GUIStyle style = GetSingleLabelStyle();
                style.fontStyle = FontStyle.Bold;
                return style;
            }

            public static GUIStyle GetIndentedStyle()
            {
                GUIStyle style = GetSingleLabelStyle();
                style.margin.left = 10;
                return style;
            }

            public static string SimplifySpaces(string source)
            {
                return Regex.Replace(source, @"\s{2,}", " ").Trim();
            }

            public static string EnsureNonEmpty(string source, string defaultValue)
            {
                string simplified = SimplifySpaces(source);
                return simplified != "" ? simplified : defaultValue;
            }

            /// <summary>
            ///   Returns the default colors to be used in any color transition.
            ///   These colors are just a suggestion and can be changed.
            /// </summary>
            /// <returns>A <see cref="ColorBlock"/> with default colors.</returns>
            public static ColorBlock DefaultColors()
            {
                ColorBlock colors = new ColorBlock();
                colors.normalColor = new Color32(255, 255, 255, 255);
                colors.highlightedColor = new Color32(245, 245, 245, 255);
                colors.pressedColor = new Color32(200, 200, 200, 255);
                colors.disabledColor = new Color32(200, 200, 200, 255);
                colors.fadeDuration = 0.1f;
                colors.colorMultiplier = 1f;
                return colors;
            }

            /// <summary>
            ///   Generates the UI to change a specific color set.
            /// </summary>
            /// <param name="source">The input colors</param>
            /// <returns>The new colors</returns>
            public static ColorBlock ColorsGUI(ColorBlock source)
            {
                ColorBlock colors = new ColorBlock();
                colors.normalColor = EditorGUILayout.ColorField("Normal color", source.normalColor);
                colors.highlightedColor = EditorGUILayout.ColorField("Highlighted color", source.highlightedColor);
                colors.pressedColor = EditorGUILayout.ColorField("Pressed color", source.pressedColor);
                colors.disabledColor = EditorGUILayout.ColorField("Disabled color", source.disabledColor);
                colors.fadeDuration = source.fadeDuration;
                colors.colorMultiplier = source.colorMultiplier;
                return colors;
            }

            /// <summary>
            ///   Creates a button, inside a parent, using specific coordinates (based on bottom-left) and size.
            /// </summary>
            /// <param name="parent">The parent under which the button will be located</param>
            /// <param name="position">The button's from-top-bottom position</param>
            /// <param name="size">The button's size</param>
            /// <param name="caption">The button's settings</param>
            /// <param name="name">The button's name</param>
            /// <param name="textColor">The button's text color</param>
            /// <param name="colors">The button's color settings</param>
            /// <param name="fontSize">Optional font size. Will default to half the height</param>
            /// <returns>The button being created, or <c>null</c> if arguments are negative or somehow inconsistent</returns>
            public static Button AddButton(RectTransform parent, Vector2 position, Vector2 size, string caption, string name, Color textColor, ColorBlock colors, int fontSize = 0)
            {
                if (size.x <= 0 || size.y <= 0 || position.x < 0 || position.y < 0)
                {
                    return null;
                }
                GameObject buttonObject = new GameObject(name);
                buttonObject.transform.parent = parent;
                RectTransform rectTransformComponent = Behaviours.AddComponent<RectTransform>(buttonObject);
                rectTransformComponent.pivot = Vector2.zero;
                rectTransformComponent.anchorMin = Vector2.zero;
                rectTransformComponent.anchorMax = Vector2.zero;
                rectTransformComponent.offsetMin = position;
                rectTransformComponent.offsetMax = position;
                rectTransformComponent.sizeDelta = size;
                rectTransformComponent.localScale = Vector3.one;
                Image buttonImageComponent = Behaviours.AddComponent<Image>(buttonObject);
                buttonImageComponent.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                buttonImageComponent.type = Image.Type.Sliced;
                buttonImageComponent.fillCenter = true;
                Button buttonComponent = Behaviours.AddComponent<Button>(buttonObject);
                buttonComponent.colors = colors;
                buttonComponent.targetGraphic = buttonImageComponent;
                GameObject textObject = new GameObject("Text");
                textObject.transform.parent = buttonObject.transform;
                Text textComponent = Behaviours.AddComponent<Text>(textObject);
                textComponent.text = caption;
                textComponent.fontSize = (fontSize >= 0) ? fontSize : (int)(size.y / 2);
                textComponent.alignment = TextAnchor.MiddleCenter;
                textComponent.color = textColor;
                RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
                textRectTransform.pivot = Vector2.one / 2f;
                textRectTransform.anchorMin = Vector2.zero;
                textRectTransform.anchorMax = Vector2.one;
                textRectTransform.offsetMin = Vector2.zero;
                textRectTransform.offsetMax = Vector2.zero;
                textRectTransform.localScale = Vector3.one;
                return buttonComponent;
            }

            /// <summary>
            ///   Adds a component to an object in this editor context: adding it to the UNDO stack.
            /// </summary>
            /// <typeparam name="T">The type of component to add</typeparam>
            /// <param name="gameObject">The game object to add a component to</param>
            /// <param name="data">The data to set for the component being added</param>
            /// <param name="ensureInactive">Be default true, it tells whether the game object must be inactive while this method is running</param>
            /// <returns>The newly created component</returns>
            public static T AddUndoableComponent<T>(GameObject gameObject, Dictionary<string, object> data = null, bool ensureInactive = true) where T : Component
            {
                Func<T> action = delegate () {
                    T component = Undo.AddComponent<T>(gameObject);
                    if (data != null)
                    {
                        Behaviours.SetObjectFieldValues(component, data);
                    }
                    return component;
                };

                if (ensureInactive)
                {
                    return Behaviours.EnsureInactive(gameObject, action);
                }
                else
                {
                    return action();
                }
            }
        }
    }
}
