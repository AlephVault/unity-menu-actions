# Unity Menu Actions

This package contains some utilities to create editor/menu actions, and some utilities to help with editor-time adding canvas components.

# Install

This package is not available in any UPM server. You must install it in your project like this:

1. In Unity, with your project open, open the Package Manager.
2. Either refer this GitHub project: https://github.com/AlephVault/unity-menu-actions.git or clone it locally and refer it from disk.
3. Also, the following packages are dependencies you need to install accordingly (in the same way and also ensuring all the recursive dependencies are satisfied):

     - https://github.com/AlephVault/unity-layout.git

# Usage

This is an **editor-only package**. It offers no features on runtime. Its purpose is to define editor-related stuff to create menus and windows to implement the menu actions (e.g. one menu action requires more data from the user, thus opening a new window to ask for the data - later, the window could have some way to finally commit / confirm the action).

## Utils

There are some utils defined in the `AlephVault.Unity.MenuActions.Utils` namespace. The classes will be detailed here:

### `MenuActionUtils`

This one is a static class with the following methods:

- `public static GUIStyle GetWordWrappingStyle()`: Generates a dumb `wordWrap: true` GUI style. Intended for styling custom inspectors and editor windows.
- `public static GUIStyle GetSingleLabelStyle()`: Generates a style for the label, including some margins and also `wordWrap: true`. Intended for styling custom inspectors and editor windows.
- `public static GUIStyle GetCaptionLabelStyle()`: Generates a caption style, only consisting of `fontStyle: bold`. Intended for styling custom inspectors and editor windows.
- `public static GUIStyle GetIndentedStyle()`: Creates an indented style (a big left margin).
- `public static string SimplifySpaces(string source)`: Trims and replaces spaces in strings to use single spaces only.
- `public static string EnsureNonEmpty(string source, string defaultValue)`: Uses a string or, if empty, a default value.
- `public static ColorBlock DefaultColors()`: Creates a `ColorBlock` with some default colors (tones of gray). Intended to be used when giving initial colors to an old-style UI component, like a button in a canvas.
- `public static ColorBlock ColorsGUI(ColorBlock source)`: Asks for those color properties and produces a `ColorBlock` out of them. Intended to be used as part of an editor window to generate a proper `ColorBlock` out of developer input.
- `public static Button AddButton(RectTransform parent, Vector2 position, Vector2 size, string caption, string name, Color textColor, ColorBlock colors, int fontSize = 0)`: Adds a new button component to a `RectTransform`. Intended to be used when developing an old-style canvas with some components. The parameters are configurations to the button itself (e.g. size, position, color and background text, ...).
- `public static T AddUndoableComponent<T>(GameObject gameObject, Dictionary<string, object> data = null, bool ensureInactive = true)`: Intended to be called for **any** component being created with a custom menu action or assistant window, so after being added it can be removed with Undo / Ctrl+Z.

## Types

There are also some types defined in the `AlephVault.Unity.MenuActions.Types` namespace. The classes will be detailed here:

### `SmartEditorWindow`

It's just an `EditorWindow` that must be overridden in order to be used. It has some helper methods. They are:

- `protected virtual float GetSmartWidth()`: It tells the width to force for the window. In contrast, the height will be calculated for this chosen width. By default, with no overriding, this method returns 400.
- `protected virtual void OnAdjustedGUI()`: This is the method that must be overridden when creating a child class. Add any custom logic you want here as if regularly defining `OnGUI`, but never override `OnGUI`.
- `protected void SmartExecute(Action action, CloseType closeType = CloseType.OnSuccess)`: Meant to be used inside `OnAdjustedGUI`, it executes an arbitrary action and chooses a `CloseType` for the window once the action is executed. This method is allowed but not always that practical. Perhaps good for a conditional action or more fine-grained control of what to do.
- `protected bool SmartButton(string text, Action action, CloseType closeType = CloseType.OnSuccess)`: Meant to be used inside `OnAdjustedGUI`, it renders a button that, when clicked, executes an arbitrary action and chooses a `CloseType` for the window once the action is executed. The return value is `true` if the button was clicked and `false` otherwise.

### `SmartEditorWindow.CloseType`

Tells the conditions for when a window will close:
- `NoClose`: An action will be executed but no implicit close will be executed.
- `OnSuccess`: An action will be executed and, on normal termination, the window will close (on error it will NOT close but remain open).
- `Always`: An action will be executed and, on any termination, the window will close.
