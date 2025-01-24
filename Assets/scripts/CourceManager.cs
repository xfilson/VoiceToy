using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.config;
using DefaultNamespace.config.dialogue;
using UnityEngine;

public class CourceManager : SingletonMono<CourceManager>
{
    public enum CourceTemplateType
    {
        None,
        teacher_opening_remark,
        sectioning,
        teacher_concluding_remark,
        End,
    }

    //当前的课程数据;
    public CourseConfigItem CourseConfigItem;
    public CharacterBase character_teacher;
    public CharacterBase character_student;
    
    private CourceTemplateType _courceTemplateType = CourceTemplateType.None;
    private int _sectionIndex = 0;
    private int _eventIndex = 0;

    #region 事件

    //课程开始
    public const string Event_Cource_Start = "cource_start";
    //老师说话结束
    public const string Event_Teacher_Say_End = "teacher_say_end";
    //学生说话结束
    public const string Event_Student_Say_End = "student_say_end";
    //老师说话开始
    public const string Event_Teacher_Say_Start = "teacher_say_start";
    //学生说话开始
    public const string Event_Student_Say_Start = "student_say_start";

    #endregion
    
    private void Awake()
    {
        EventManager.AddListener(Event_Cource_Start, null, this.OnCourceStart);
        EventManager.AddListener(Event_Teacher_Say_End, null, this.OnTeacherSayEnd);
        EventManager.AddListener(Event_Student_Say_End, null, this.OnStudentSayEnd);
        EventManager.AddListener(Event_Teacher_Say_Start, null, this.OnTeacherSayStart);
        EventManager.AddListener(Event_Student_Say_Start, null, this.OnStudentSayStart);
    }

    private void OnStudentSayStart(string eventtype, object sender, object param)
    {
        this.character_student.SayStart();
    }

    private void OnTeacherSayStart(string eventtype, object sender, object param)
    {
        this.character_teacher.SayStart();
    }

    private void OnCourceStart(string eventtype, object sender, object param)
    {
        if (CourseConfigItem != null)
        {
            var newLogicGo = GameObject.Instantiate(CourseConfigItem.courseLogicTemplate)
                .GetComponent<CourseLogicTemplate>();
            newLogicGo.PlayCourse();
        }
    }
    
    private void OnTeacherSayEnd(string eventtype, object sender, object param)
    {
        // this.CheckNextSegment();
        this.character_teacher.SayEnd();
    }
    
    private void OnStudentSayEnd(string eventtype, object sender, object param)
    {
        // this.CheckNextSegment();
        this.character_student.SayEnd();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(Event_Cource_Start, null);
        EventManager.RemoveListener(Event_Teacher_Say_End, null, this.OnTeacherSayEnd);
        EventManager.RemoveListener(Event_Student_Say_End, null, this.OnStudentSayEnd);
    }
}
