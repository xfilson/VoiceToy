using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipPlayer:MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            this._audioSource = this.gameObject.TryGetComponent<AudioSource>();
        }

        public void PlayAudioClip(AudioClip audioClip, Action onPlayComplete)
        {
            _audioSource.Stop();
            Debug.Log("Audio PlayAudioClip "+audioClip.name);
            _audioSource.clip = audioClip;
            _audioSource.Play();
            // 如果是异步播放完成回调，可以使用 coroutines 或监听事件
            StartCoroutine(WaitForAudioToFinish(onPlayComplete));
        }
        
        private IEnumerator WaitForAudioToFinish(Action onPlayComplete)
        {
            while (_audioSource.isPlaying)
            {
                yield return null;  // 等待音频播放完成
            }
            Debug.Log("Audio has finished playing.");
            // 播放完毕，调用回调
            onPlayComplete?.Invoke();
        }
    }
}