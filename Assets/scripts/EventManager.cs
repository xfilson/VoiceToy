using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    // 事件委托，包含事件类型（string）、发送者（object）和事件参数（object）
    public delegate void EventDelegate(string eventType, object sender, object param);

    // 事件监听结构体
    private struct EventListener
    {
        public bool isNeedSenderExist;
        public object Sender;
        public EventDelegate Callback;

        public EventListener(object sender, EventDelegate callback)
        {
            Sender = sender;
            Callback = callback;
            if (Sender == null)
            {
                isNeedSenderExist = false;
            }
            else
            {
                isNeedSenderExist = true;
            }
        }
    }

    // 事件字典，key为事件类型，value为对应的事件监听列表
    private static Dictionary<string, List<EventListener>> eventDict = new Dictionary<string, List<EventListener>>();

    // 添加事件监听
    public static void AddListener(string eventType, object sender, EventDelegate callback)
    {
        // 如果事件字典中没有该事件类型，则添加
        if (!eventDict.ContainsKey(eventType))
        {
            eventDict[eventType] = new List<EventListener>();
        }

        // 创建事件监听结构体并添加到列表中
        eventDict[eventType].Add(new EventListener(sender, callback));
    }

    // 移除事件监听
    public static void RemoveListener(string eventType, object sender, EventDelegate callback = null)
    {
        // 如果事件字典中存在该事件类型
        if (eventDict.ContainsKey(eventType))
        {
            List<EventListener> toRemove = new List<EventListener>();

            // 遍历事件监听列表
            foreach (var listener in eventDict[eventType])
            {
                // 如果指定了callback，则检查是否是同一个委托
                if (callback != null && listener.Callback == callback && listener.Sender == sender)
                {
                    toRemove.Add(listener);
                }
                // 如果未指定callback，则检查监听的Sender是否是sender
                else if (callback == null && listener.Sender == sender)
                {
                    toRemove.Add(listener);
                }
            }

            // 移除标记的事件监听
            foreach (var listener in toRemove)
            {
                eventDict[eventType].Remove(listener);
            }

            // 如果事件监听列表为空，则从事件字典中移除该事件类型
            if (eventDict[eventType].Count == 0)
            {
                eventDict.Remove(eventType);
            }
        }
    }

    // 派发事件
    public static void DispatchEvent(string eventType, object param, object sender = null)
    {
        // 如果事件字典中存在该事件类型
        if (eventDict.ContainsKey(eventType))
        {
            List<EventListener> toRemove = new List<EventListener>();

            // 遍历事件监听列表，执行每个事件委托
            foreach (var listener in eventDict[eventType])
            {
                // 判断sender是否有效，即是否被销毁
                if (listener.Sender == null && listener.isNeedSenderExist)
                {
                    toRemove.Add(listener);
                    continue;
                }

                // 执行事件回调
                listener.Callback(eventType, sender, param);
            }

            // 移除标记的事件监听
            foreach (var listener in toRemove)
            {
                eventDict[eventType].Remove(listener);
            }

            // 如果事件监听列表为空，则从事件字典中移除该事件类型
            if (eventDict[eventType].Count == 0)
            {
                eventDict.Remove(eventType);
            }
        }
    }
}