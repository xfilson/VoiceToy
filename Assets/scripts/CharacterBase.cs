using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[Serializable]
public class ZuixingConfigItem
{
    //振幅范围;
    public Vector2 amplitudeRange = new Vector2(0, 0);
    //图片;
    public Sprite sprite;

    public bool IsInRange(float amplitude)
    {
        return amplitude >= amplitudeRange.x && amplitude < amplitudeRange.y;
    }
}

public class CharacterBase : MonoBehaviour
{
    public Animator animator_character;
    public Animator animator_zuiba;
    public Animator animator_eyes;
    private LoopAnimatorState _loopAnimatorState_character;
    private LoopAnimatorState _loopAnimatorState_zuiba;
    private LoopAnimatorState _loopAnimatorState_eyes;
    public SpriteRenderer zuiba_spriteRenderer;
    [Header("嘴型数据")]
    public List<ZuixingConfigItem> zuixingConfigItems = new List<ZuixingConfigItem>();
    [HideInInspector]
    public ZuixingConfigItem curZuixingConfigItem = null;

    public ZuixingConfigItem GetZuixingConfigItem(float amplitude)
    {
        if (curZuixingConfigItem != null)
        {
            if (curZuixingConfigItem.IsInRange(amplitude))
            {
                return curZuixingConfigItem;
            }
        }
        for (int i = 0; i < zuixingConfigItems.Count; i++)
        {
            var curItem = zuixingConfigItems[i];
            if (curItem.IsInRange(amplitude))
            {
                curZuixingConfigItem = curItem;
                return curItem;
            }
        }
        return null;
    }
    
    public void RandomZuixingConfigItem()
    {
        int index = UnityEngine.Random.Range(0, zuixingConfigItems.Count);
        var zuixingConfigItem =  zuixingConfigItems[index];
        zuiba_spriteRenderer.sprite = zuixingConfigItem.sprite;
    }

    private void Awake()
    {
        this._loopAnimatorState_character = this.animator_character.gameObject.TryGetComponent<LoopAnimatorState>();
        this._loopAnimatorState_zuiba = this.animator_zuiba.gameObject.TryGetComponent<LoopAnimatorState>();
        this._loopAnimatorState_eyes = this.animator_eyes.gameObject.TryGetComponent<LoopAnimatorState>();
    }

    public void ReqScenePlayAudioClip(AudioClip audioClip)
    {
        AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
        // audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    
    public void DispatchEventUseSender(string eventType)
    {
        EventManager.DispatchEvent(eventType, this.gameObject);
    }
    
    public void DispatchEventNoSender(string eventType)
    {
        EventManager.DispatchEvent(eventType, null);
    }

    public void UseLipAmplitude(float amplitude)
    {
        ZuixingConfigItem zuixingConfigItem = GetZuixingConfigItem(amplitude);
        if (zuixingConfigItem != null)
        {
            zuiba_spriteRenderer.sprite = zuixingConfigItem.sprite;
        }
    }

    public void PlayAnimator_zuiba(string param)
    {
        string[] arr = param.Split(",");
        string stateName = arr[0];
        int loopCount = 0;
        if (arr.Length > 1)
        {
            loopCount = int.Parse(arr[1]);
        }
        this.animator_zuiba.Play(stateName);
        this._loopAnimatorState_zuiba.SetLoopCheck(true, this.animator_zuiba, stateName, loopCount);
    }
    
    public void PlayAnimator_eyes(string param)
    {
        string[] arr = param.Split(",");
        string stateName = arr[0];
        int loopCount = 0;
        if (arr.Length > 1)
        {
            loopCount = int.Parse(arr[1]);
        }
        this.animator_eyes.Play(stateName);
        this._loopAnimatorState_eyes.SetLoopCheck(true, this.animator_eyes, stateName, loopCount);
    }
    
    public void PlayAnimator_character(string param)
    {
        string[] arr = param.Split(",");
        string stateName = arr[0];
        int loopCount = 0;
        if (arr.Length > 1)
        {
            loopCount = int.Parse(arr[1]);
        }
        this.animator_character.Play(stateName);
        this._loopAnimatorState_character.SetLoopCheck(true, this.animator_character, stateName, loopCount);
    }
    
    public void OnSayStart(AudioClip audioClip, Action onPlayComplete)
    {
        print("OnSayStart "+this.gameObject.name+"   "+Time.frameCount);
        StartCoroutine(_OnSayStart(audioClip, onPlayComplete));
    }

    private IEnumerator _OnSayStart(AudioClip audioClip, Action onPlayComplete)
    {
        this.PlayAnimator_zuiba("random_say");
        this.PlayAnimator_eyes("think1,1");
        yield return null;
    }

    public void OnSayEnd()
    {
        print("OnSayEnd "+this.gameObject.name+"   "+Time.frameCount);
        this._loopAnimatorState_zuiba.SetLoopCheck(false);
        this._loopAnimatorState_eyes.SetLoopCheck(false);
        // this.animator_eyes.StopPlayback();
        // this.animator_zuiba.StopPlayback();
        this.PlayAnimator_zuiba("idle");
        this.PlayAnimator_eyes("idle");
    }


    public void SayStart()
    {
        this.PlayAnimator_zuiba("random_say");
        this.PlayAnimator_eyes("think1,1");
    }

    public void SayEnd()
    {
        this.PlayAnimator_zuiba("idle");
        this.PlayAnimator_eyes("idle");
    }
}
