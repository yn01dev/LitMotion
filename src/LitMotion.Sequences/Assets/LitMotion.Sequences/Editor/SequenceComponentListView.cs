using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace LitMotion.Sequences.Editor
{
    public sealed class SequenceComponentListView : VisualElement
    {
        static readonly Dictionary<UnityObject, VisualElement> inspectorCache = new();
        static VisualElement GetOrCreateInspector(UnityObject key)
        {
            if (key == null) return null;
            if (!inspectorCache.TryGetValue(key, out var element))
            {
                element = UnityEditor.Editor.CreateEditor(key).CreateInspectorGUI();
                element.Bind(new SerializedObject(key));
            }

            return element;
        }

        static readonly string LightStyleSheetGUID = "e0272c41884fc453e86f6260dd9a0eae";
        static readonly string DarkStyleSheetGUID = "a298adbec4db64c3997962890f9f359e";

        public SequenceComponentListView(SerializedObject serializedObject)
        {
            var componentsProperty = serializedObject.FindProperty("components");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(
                EditorGUIUtility.isProSkin ? DarkStyleSheetGUID : LightStyleSheetGUID
            ));

            var listView = new ListView
            {
                showBorder = false,
                showFoldoutHeader = false,
                showBoundCollectionSize = false,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = () => new VisualElement(),
                bindItem = (element, index) =>
                {
                    var elementProperty = componentsProperty.GetArrayElementAtIndex(index);
                    var inspector = GetOrCreateInspector(elementProperty.objectReferenceValue);
                    if (inspector != null) element.Add(inspector);
                },
                unbindItem = (element, index) =>
                {
                    element.Clear();
                },
                style = {
                    marginLeft = -15f,
                    marginRight = -5f,
                },
                bindingPath = "components",
            };
            listView.Bind(serializedObject);
            listView.itemIndexChanged += (i, j) => OnOrdered?.Invoke();
            listView.styleSheets.Add(styleSheet);

            Add(listView);

            Button button = null;
            button = new Button(() =>
            {
                const int MaxTypePopupLineCount = 13;

                var baseType = typeof(SequenceComponent);
                var dropdown = new TypeDropdown(
                    TypeCache.GetTypesDerivedFrom(baseType).Where(t =>
                        (t.IsPublic || t.IsNestedPublic) &&
                        !t.IsAbstract &&
                        !t.IsGenericType
                    ),
                    MaxTypePopupLineCount,
                    new AdvancedDropdownState()
                );

                dropdown.OnItemSelected += item =>
                {
                    var target = serializedObject.targetObject;
                    var component = ScriptableObject.CreateInstance(item.type);
                    component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
                    component.name = item.type.Name;
                    var path = AssetDatabase.GetAssetPath(target);
                    if (EditorUtility.IsPersistent(target))
                    {
                        AssetDatabase.AddObjectToAsset(component, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.ImportAsset(path);
                    }

                    var so = new SerializedObject(target);
                    var componentsProperty = so.FindProperty("components");
                    var index = componentsProperty.arraySize;
                    componentsProperty.InsertArrayElementAtIndex(index);
                    componentsProperty.GetArrayElementAtIndex(index).objectReferenceValue = component;
                    so.ApplyModifiedPropertiesWithoutUndo();
                };

                var position = button.contentRect;
                dropdown.Show(position);
            })
            {
                text = "Add Motion",
                style = {
                    alignSelf = Align.Center,
                    width = 220f,
                    height = 22f,
                    marginTop = 5f, marginBottom = 5f
                }
            };

            Add(button);
        }

        public event Action OnOrdered;
    }
}
