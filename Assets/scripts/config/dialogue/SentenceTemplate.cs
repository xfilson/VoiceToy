using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.config.dialogue
{
    [System.Serializable]
    public class SentenceTemplateItem
    {
        [Header("描述")]
        public string desc;
        [Header("音频")]
        public AudioClip audioClip;
    }
    
    /// <summary>
    /// 语句模板;
    /// </summary>
    [CreateAssetMenu(fileName = "语句模板", menuName = "模板资产创建/语句模板", order = 1)]
    [System.Serializable]
    public class SentenceTemplate :ScriptableObject
    {
        [Header("注释")]
        public string comment;

        public List<SentenceTemplateItem> itemList = new List<SentenceTemplateItem>();

        public SentenceTemplateItem GetRandomItem()
        {
            if (itemList.Count == 0)
            {
                return null;
            }

            // 使用 UnityEngine.Random 类的静态方法生成随机索引
            int index = UnityEngine.Random.Range(0, itemList.Count);
            return itemList[index];
        }
    }
}