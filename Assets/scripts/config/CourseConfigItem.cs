using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace DefaultNamespace.config
{
    [CreateAssetMenu(fileName = "课程自定义模板", menuName = "模板资产创建/课程自定义模板", order = 1)]
    [System.Serializable]
    public class CourseConfigItem : ScriptableObject
    {
        [BoxGroup("CourceInfo")]
        [HideLabel, PreviewField(55)]
        [HorizontalGroup("CourceInfo/Icon", 55, LabelWidth = 86)]
        public Texture Icon;
        [VerticalGroup("CourceInfo/Icon/Meta")]
        [LabelWidth(100)]
        public string Name;
        [VerticalGroup("CourceInfo/Icon/Meta")]
        [LabelWidth(100)]
        public string comment;

        [FormerlySerializedAs("courseTimelineTemplate")]
        [FormerlySerializedAs("courseTimeline")]
        [AssetsOnly]
        [VerticalGroup("CourceInfo/Icon/Meta")]
        [LabelWidth(100)]
        public CourseLogicTemplate courseLogicTemplate;

        // [VerticalGroup("Description")]
        [BoxGroup("Description")]
        [HideLabel, TextArea(8, 20)]
        public string Description;

    }
}