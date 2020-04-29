using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Audio;

public class AudioMaster : MonoBehaviour
{
    public PlayableDirector titleTimeline;
    public PlayableDirector ingameTimeline;
    private TimelineAsset timelineAsset;

    public AudioSource playingOutput;
    public AudioSource mutedOutput;
    
    public int[] titleAtmosTrackIndices;
    public int[] titleLeadTrackIndices;
    public int[] titleRhythmTrackIndices;

    public int[] ingameAtmosTrackIndices;
    public int[] ingameLeadTrackIndices;
    public int[] ingameRhythmTrackIndices;

    //mixed for transition fades
    public AudioMixer mixer;
    public AudioMixerSnapshot fullVolumeSnap, inaudibleSnap;

    //sfx source and clips
    public AudioSource audioFX;
    public AudioClip[] fxSoundArray;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        titleTimeline.Stop();
        titleTimeline.time = 0.0f;
        ingameTimeline.Stop();
        ingameTimeline.time = 0.0f;
    }

    public void PlayTitles()
    {
        AudioTrack audioTrack;
        //Timeline playable graph asset needs to be rebuilt to change contributing tracks
        timelineAsset = (TimelineAsset)titleTimeline.playableAsset;

        Reset();

        //pick title atmos track index:
        int atmosIndex = pickRandomItem(titleAtmosTrackIndices);
        //pick title lead track index:
        int leadIndex = pickRandomItem(titleLeadTrackIndices);
        //pick title rhythm track index:
        int rhythmIndex = pickRandomItem(titleRhythmTrackIndices);

        //iterate over tracks and set output to playing or muted
        int count = 0;
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            audioTrack = track as AudioTrack;

            if (audioTrack == null)
            {
                //Debug.Log("Null track at count:" + count.ToString());
                continue;
            }
            else
            {
                //Debug.Log("Audio track "+count.ToString()+" clip:" + audioTrack.name);
            }

            if (count==atmosIndex||count==leadIndex||count==rhythmIndex)
                titleTimeline.SetGenericBinding(audioTrack, playingOutput);
            else
                titleTimeline.SetGenericBinding(audioTrack, mutedOutput);

            count++;
        }
        titleTimeline.time = 0.0f;
        titleTimeline.RebuildGraph();
        titleTimeline.Play();
    }

    public void PlayIngame()
    {
        AudioTrack audioTrack;
        //Timeline playable graph asset needs to be rebuilt to change contributing tracks
        timelineAsset = (TimelineAsset)ingameTimeline.playableAsset;

        Reset();

        //pick ingame atmos track index:
        int atmosIndex = pickRandomItem(ingameAtmosTrackIndices);
        //pick ingame lead track index:
        int leadIndex = pickRandomItem(ingameLeadTrackIndices);
        //pick ingame rhythm track index:
        int rhythmIndex = pickRandomItem(ingameRhythmTrackIndices);

        //iterate over tracks and set output to playing or muted
        int count = 0;
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            audioTrack = track as AudioTrack;

            if (audioTrack == null)
            {
                //Debug.Log("Null track at count:" + count.ToString());
                continue;
            }
            else
            {
                //Debug.Log("Audio track " + count.ToString() + " clip:" + audioTrack.name);
            }

            if (count == atmosIndex || count == leadIndex || count == rhythmIndex)
                ingameTimeline.SetGenericBinding(audioTrack, playingOutput);
            else
                ingameTimeline.SetGenericBinding(audioTrack, mutedOutput);

            count++;
        }
        ingameTimeline.time = 0.0f;
        ingameTimeline.RebuildGraph();
        ingameTimeline.Play();
    }

    public void PlayFX(int clipNum)
    {
        audioFX.Stop();
        if ((clipNum>-1)&&(clipNum<fxSoundArray.Length))
        {
            audioFX.clip = fxSoundArray[clipNum];
            audioFX.Play();
        }

    }

    public int pickRandomItem(int[] inputArray)
    {
        int arrayLength = inputArray.Length;
        int random = Random.Range(0, arrayLength);
        return inputArray[random];
    }

    public void FadeOut(float duration)
    {
        inaudibleSnap.TransitionTo(duration);
    }

    public void FadeIn(float duration)
    {
        fullVolumeSnap.TransitionTo(duration);
    }

}
