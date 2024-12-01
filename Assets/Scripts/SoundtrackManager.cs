using DG.Tweening;
using MarkusSecundus.Utils.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ParallelWorldType
{
    Depressive, Snow, Rainbow
}

public class SoundtrackManager : MonoBehaviour
{

    [SerializeField] float _transitionDuration_seconds = 1.0f;
    [SerializeField] ParallelWorldType _initialWorld = ParallelWorldType.Depressive;
    [SerializeField] SerializableDictionary<ParallelWorldType, AudioSource> _audios;

    private void Start()
    {
        SwitchToWorld(_initialWorld);
    }

    List<Tween> _runningTweens = new();
    public void SwitchToWorld(ParallelWorldType world, float? transition_seconds=null)
    {
        float transition = transition_seconds ?? _transitionDuration_seconds;
        foreach (var tw in _runningTweens) tw.Kill();
        _runningTweens.Clear();

        foreach(var (w, p) in _audios.Values)
        {
            float desiredLoudness = (w == world) ? 1.0f: 0.0f;
            var tw = p.DOFade(desiredLoudness, transition);
            _runningTweens.Add(tw);
        }
    }
    public void SwitchToWorld(int world) => SwitchToWorld((ParallelWorldType)world, null);
}
