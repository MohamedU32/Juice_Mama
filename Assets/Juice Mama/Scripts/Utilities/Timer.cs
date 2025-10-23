using System;
using System.Collections;
using UnityEngine;

namespace MamaJuice.Utilities
{
    public class Timer
    {
        class Runner : MonoBehaviour {}

        static Runner runner;

        static Runner GetRunner()
        {
            if (runner == null)
            {
                var go = new GameObject("TimerRunner");
                go.hideFlags = HideFlags.HideAndDontSave;
                UnityEngine.Object.DontDestroyOnLoad(go);
                runner = go.AddComponent<Runner>();
            }
            return runner;
        }

        readonly float duration;
        readonly bool unscaled;
        readonly Action onComplete;

        Coroutine routine;
        bool running;
        bool paused;
        float elapsed;

        public Timer(float seconds, Action onComplete = null, bool unscaledTime = false)
        {
            duration = Mathf.Max(0f, seconds);
            this.onComplete = onComplete;
            unscaled = unscaledTime;
            Start();
        }

        public static Timer StartNew(float seconds, Action onComplete = null, bool unscaledTime = false)
        {
            return new Timer(seconds, onComplete, unscaledTime);
        }

        void Start()
        {
            running = true;
            paused = false;
            elapsed = 0f;
            routine = GetRunner().StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            while (running && elapsed < duration)
            {
                if (!paused)
                {
                    elapsed += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                }
                yield return null;
            }
            if (running)
            {
                running = false;
                onComplete?.Invoke();
            }
        }

        public float Elapsed => Mathf.Min(elapsed, duration);
        public float Remaining => Mathf.Max(0f, duration - Elapsed);
        public bool IsRunning => running && !paused;
        public bool IsPaused => paused;

        public void Pause()
        {
            if (!running || paused) return;
            paused = true;
        }

        public void Resume()
        {
            if (!running || !paused) return;
            paused = false;
        }

        public void Stop()
        {
            if (!running) return;
            running = false;
            if (routine != null)
            {
                GetRunner().StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
