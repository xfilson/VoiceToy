using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.config.dialogue;
using UnityEngine;

public class CourceManager : MonoBehaviour
{
    public enum CourceTemplateType
    {
        None,
        teacher_opening_remark,
        sectioning,
        teacher_concluding_remark,
        End,
    }

    public CourseTemplate curCourseData;
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

    #endregion
    
    private void Awake()
    {
        EventManager.AddListener(Event_Cource_Start, null, this.OnCourceStart);
        EventManager.AddListener(Event_Teacher_Say_End, null, this.OnTeacherSayEnd);
        EventManager.AddListener(Event_Student_Say_End, null, this.OnStudentSayEnd);
    }

    private void OnCourceStart(string eventtype, object sender, object param)
    {
        this.CheckNextSegment();
    }
    
    private void OnTeacherSayEnd(string eventtype, object sender, object param)
    {
        this.CheckNextSegment();
    }
    
    private void OnStudentSayEnd(string eventtype, object sender, object param)
    {
        this.CheckNextSegment();
    }

    private void CheckNextSegment()
    {
        switch (this._courceTemplateType)
        {
            case CourceTemplateType.None:
                this._courceTemplateType = CourceTemplateType.teacher_opening_remark;
                this.CheckNextSegment();
                break;
            case CourceTemplateType.teacher_opening_remark:
                this.Goto_teacher_opening_remark();
                break;
            case CourceTemplateType.sectioning:
                this.Goto_sectioning();
                break;
            case CourceTemplateType.teacher_concluding_remark:
                this.Goto_teacher_concluding_remark();
                break;
            case CourceTemplateType.End:
                break;
        }
    }

    private void Goto_sectioning()
    {
        if (this._sectionIndex < this.curCourseData.sectionList.Count)
        {
            do
            {
                var curSectionData = this.curCourseData.sectionList[this._sectionIndex];
                if (this._eventIndex < curSectionData.itemList.Count)
                {
                    var itemData = curSectionData.itemList[this._eventIndex];
                    var sentenceItem = itemData.teacher_opening_remark.GetRandomItem();
                    if (sentenceItem != null)
                    {
                        this._eventIndex++;
                        string eventEndString = Event_Teacher_Say_End;
                        CharacterBase curCharacter = character_teacher;
                        switch (itemData.eventType)
                        {
                            case CourceEventType.Teacher:
                                eventEndString = Event_Teacher_Say_End;
                                curCharacter = character_teacher;
                                break;
                            case CourceEventType.Student:
                                eventEndString = Event_Student_Say_End;
                                curCharacter = character_student;
                                break;
                        }
                        curCharacter.OnSayStart(sentenceItem.audioClip, () =>
                        {
                            StartCoroutine(this.OnCharacterSayEnd(curCharacter, eventEndString));
                        });      
                        break;
                    }
                }
                this._sectionIndex++;
                this._eventIndex = 0;
                this.CheckNextSegment();
            } while (false);
        }
        else
        {
            this._courceTemplateType = CourceTemplateType.teacher_concluding_remark;
            this.CheckNextSegment();
        }
    }

    private void Goto_teacher_concluding_remark()
    {
        this._courceTemplateType = CourceTemplateType.End;
        if (this.curCourseData.teacher_concluding_remark != null)
        {
            var itemData = this.curCourseData.teacher_concluding_remark.GetRandomItem();
            character_teacher.OnSayStart(itemData.audioClip, () =>
            {
                StartCoroutine(this.OnCharacterSayEnd(character_teacher, Event_Teacher_Say_End));
            });
        }
        else
        {
            this.CheckNextSegment();
        }
    }

    private void Goto_teacher_opening_remark()
    {
        this._courceTemplateType = CourceTemplateType.sectioning;
        if (this.curCourseData.teacher_opening_remark != null)
        {
            var itemData = this.curCourseData.teacher_opening_remark.GetRandomItem();
            character_teacher.OnSayStart(itemData.audioClip, () =>
            {
                StartCoroutine(this.OnCharacterSayEnd(character_teacher, Event_Teacher_Say_End));
            });
        }
        else
        {
            this.CheckNextSegment();
        }
    }

    private IEnumerator OnCharacterSayEnd(CharacterBase character, string eventSayEndSendType)
    {
        character.OnSayEnd();
        yield return null;
        EventManager.DispatchEvent(eventSayEndSendType, null);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(Event_Cource_Start, null);
        EventManager.RemoveListener(Event_Teacher_Say_End, null, this.OnTeacherSayEnd);
        EventManager.RemoveListener(Event_Student_Say_End, null, this.OnStudentSayEnd);
    }
}
