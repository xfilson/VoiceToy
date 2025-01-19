using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.config.dialogue
{
    //https://cloud.tencent.com/developer/article/2108845?cps_key=1d358d18a7a17b4a6df8d67a62fd3d3d
    
    /// <summary>
    /// 课程事件类型;
    /// </summary>
    [System.Serializable]
    public enum CourceEventType
    {
        [Header("老师")]
        Teacher,
        [Header("学生")]
        Student,
    }

    /// <summary>
    /// 课程讨论对话模板;
    /// </summary>
    [System.Serializable]
    public class CourceEventItem
    {
        [Header("注释")]
        public string comment;
        [Header("事件类型")]
        public CourceEventType eventType = CourceEventType.Teacher;
        // [ShowIf("eventType", CourceEventType.Student)]
        public SentenceTemplate teacher_opening_remark;
    }
    
    /// <summary>
    /// 课程内部小节模板;
    /// </summary>
    [System.Serializable]
    public class CourceSectionTemplateItem
    {
        [Header("注释")]
        public string comment;
        [Header("事件列表")]
        // [BoxGroup("Box")]
        [ListDrawerSettings(DraggableItems = true, Expanded = false, ShowIndexLabels = true, ShowItemCount = true, ListElementLabelName = "comment")]
        public List<CourceEventItem> itemList = new List<CourceEventItem>();
    }
    
    /// <summary>
    /// 语句模板;
    /// </summary>
    [CreateAssetMenu(fileName = "课程模板", menuName = "模板资产创建/课程模板", order = 1)]
    [System.Serializable]
    public class CourseTemplate :ScriptableObject
    {
        [Header("注释")]
        public string comment;

        [Header("老师-开场白")]
        public SentenceTemplate teacher_opening_remark;
        [Header("课程小节")]
        [ListDrawerSettings(DraggableItems = true, Expanded = false, ShowIndexLabels = true, ShowItemCount = true, ListElementLabelName = "comment")]
        public List<CourceSectionTemplateItem> sectionList;
        [Header("老师-结束语")]
        public SentenceTemplate teacher_concluding_remark;
    }
}