#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DefaultNamespace.config;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TextCore.Text;
using UnityEngine.Timeline;

public class CustomCourseEditor : OdinMenuEditorWindow
{
    public const string CourseLogicTemplatePath = "Assets/Res/prefabs/course_logic_base.prefab"; 
    public const string CourseTimelineTemplatePath = "Assets/Res/prefabs/course_timelime_template.playable";
    [MenuItem("Tools/课程编辑器")]
    private static void Open()
    {
        var window = GetWindow<CustomCourseEditor>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;

        // Adds the character overview table.
        // CharacterOverview.Instance.UpdateCharacterOverview();
        // tree.Add("课程配置", new CharacterTable(CharacterOverview.Instance.AllCharacters));
        
        // Adds all characters.
        tree.AddAllAssetsAtPath("课程配置", "Assets/Res/courses", typeof(CourseConfigItem), true, false).ForEach(this.AddDragHandles);
        
        // // Add all scriptable object items.
        // tree.AddAllAssetsAtPath("", "Assets/Plugins/Sirenix/Demos/SAMPLE - RPG Editor/Items", typeof(Item), true)
        //     .ForEach(this.AddDragHandles);
        //
        // // Add drag handles to items, so they can be easily dragged into the inventory if characters etc...
        // tree.EnumerateTree().Where(x => x.Value as Item).ForEach(AddDragHandles);
        
        // Add icons to characters and items.
        tree.EnumerateTree().AddIcons<CourseConfigItem>(x => x.Icon);
        // tree.EnumerateTree().AddIcons<Item>(x => x.Icon);

        return tree;
    }

    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }

    protected override void OnBeginDrawEditors()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("创建课程")))
            {
                ScriptableObjectCreator.ShowDialog<CourseConfigItem>("Assets/Res/courses",
                    obj =>
                    {
                        obj.Name = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                        //创建对应的资源;
                        var courseLogicTemplatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CourseLogicTemplatePath);
                        var courseLogicTemplateAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(CourseTimelineTemplatePath);
                        if (courseLogicTemplatePrefab != null && courseLogicTemplateAsset != null)
                        {
                            string objPath = AssetDatabase.GetAssetPath(obj);
                            // 实例化课程模板Prefab
                            GameObject instantiatedPrefab = PrefabUtility.InstantiatePrefab(courseLogicTemplatePrefab) as GameObject;
                            string newVariantPath = Path.Combine(Path.GetDirectoryName(objPath), Path.GetFileNameWithoutExtension(objPath) + "_root.prefab");
                            // 保存为新的 Prefab 变体
                            PrefabUtility.SaveAsPrefabAsset(instantiatedPrefab, newVariantPath);
                            AssetDatabase.Refresh();
                            obj.courseLogicTemplate = AssetDatabase.LoadAssetAtPath<GameObject>(newVariantPath).GetComponent<CourseLogicTemplate>();
                            //创建新的timeline，并重设binding;
                            string newTimelinePath = Path.Combine(Path.GetDirectoryName(objPath), Path.GetFileNameWithoutExtension(objPath) + "_timeline.asset");
                            AssetDatabase.CopyAsset(CourseTimelineTemplatePath, newTimelinePath);
                            AssetDatabase.Refresh();
                            GameObject newVariantGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(newVariantPath);
                            var oldPlayableDirector = instantiatedPrefab.GetComponent<PlayableDirector>();
                            var instantiatedPrefabPlayableDirector = newVariantGameObject.GetComponent<PlayableDirector>();
                            instantiatedPrefabPlayableDirector.playableAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(newTimelinePath);
                            //playable binding修复;
                            TimelineAssetCopier.CopyPlayableDirectorTimeline(oldPlayableDirector, instantiatedPrefabPlayableDirector);
                            DestroyImmediate(instantiatedPrefab);
                            PrefabUtility.SaveAsPrefabAsset(newVariantGameObject, newVariantPath);
                            AssetDatabase.Refresh();
                            //设置timelineAsset;
                            obj.courseLogicTemplate = AssetDatabase.LoadAssetAtPath<CourseLogicTemplate>(newVariantPath);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        else
                        {
                            Debug.LogError("找不到课程时间轴配置模板");
                        }
                    });
            }

            // if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Character")))
            // {
            //     // ScriptableObjectCreator.ShowDialog<Character>(
            //     //     "Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Character", obj =>
            //     //     {
            //     //         obj.Name = obj.name;
            //     //         base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
            //     //     });
            // }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}
#endif