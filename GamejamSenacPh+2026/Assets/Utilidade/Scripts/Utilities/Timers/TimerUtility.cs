using UnityEngine;
using System;

namespace GameJam.Utilities
{
 
    public struct TimerUtility
    {
        private float remainingTime;
        private float duration;
        private bool isRunning;
        private Action onComplete;

        public TimerUtility(float duration, Action onComplete = null)
        {
            this.duration = duration;
            this.remainingTime = duration;
            this.onComplete = onComplete;
            this.isRunning = true;
        }

       
        public bool Tick(float deltaTime = 0)
        {
            if (!isRunning) return false;

            if (deltaTime <= 0)
                deltaTime = Time.deltaTime;

            remainingTime -= deltaTime;

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                isRunning = false;
                onComplete?.Invoke();
                return true;
            }

            return false;
        }

        
        public void Pause()
        {
            isRunning = false;
        }

        public void Resume()
        {
            if (remainingTime > 0)
                isRunning = true;
        }

       
        public void Stop()
        {
            isRunning = false;
            remainingTime = duration;
        }

        public void Restart()
        {
            remainingTime = duration;
            isRunning = true;
        }

       
        public void Restart(float newDuration)
        {
            duration = newDuration;
            remainingTime = newDuration;
            isRunning = true;
        }

       
        public float Progress => Mathf.Max(0, 1 - (remainingTime / duration));

        
        public float RemainingTime => remainingTime;

       
        public float ElapsedTime => duration - remainingTime;

        
        public bool IsRunning => isRunning;

        
        public bool IsComplete => !isRunning && remainingTime <= 0;

       
        public void SetOnComplete(Action callback)
        {
            onComplete = callback;
        }
    }

   
    public struct CooldownTimer
    {
        private float remainingTime;
        private float duration;

        public CooldownTimer(float duration)
        {
            this.duration = duration;
            this.remainingTime = 0;
        }

       
        public void Tick(float deltaTime = 0)
        {
            if (deltaTime <= 0)
                deltaTime = Time.deltaTime;

            if (remainingTime > 0)
                remainingTime -= deltaTime;
        }

      
        public bool CanUse => remainingTime <= 0;

      
        public void Use()
        {
            remainingTime = duration;
        }

       
        public float Progress => Mathf.Max(0, remainingTime / duration);

       
        public float RemainingTime => remainingTime;

       
        public void Reset()
        {
            remainingTime = 0;
        }
    }

    
    public struct Stopwatch
    {
        private float elapsedTime;
        private bool isRunning;

        public Stopwatch(bool startRunning = true)
        {
            elapsedTime = 0;
            isRunning = startRunning;
        }

       
        public void Tick(float deltaTime = 0)
        {
            if (!isRunning) return;

            if (deltaTime <= 0)
                deltaTime = Time.deltaTime;

            elapsedTime += deltaTime;
        }

        
        public void Start()
        {
            if (!isRunning)
                isRunning = true;
        }

      
        public void Stop()
        {
            isRunning = false;
        }

        
        public float StopAndGetTime()
        {
            isRunning = false;
            return elapsedTime;
        }

       
        public void Reset()
        {
            elapsedTime = 0;
            isRunning = false;
        }

     
        public void Restart()
        {
            elapsedTime = 0;
            isRunning = true;
        }

        
        public float ElapsedTime => elapsedTime;

       
        public bool IsRunning => isRunning;
    }
}
