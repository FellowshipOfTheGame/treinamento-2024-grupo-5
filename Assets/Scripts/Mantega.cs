using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SocialPlatforms;
using static UnityEngine.LowLevel.PlayerLoopSystem;

namespace Mantega
{
    public static class Generics
    {
        public static bool ReallyTryGetComponent<T>(GameObject gameObject, out T component) where T : UnityEngine.Component
        {
            // Tenta obter o componente diretamente e nos filhos
            if (!(gameObject == null))
            {
                if (gameObject.TryGetComponent(out component))
                    return true;
                // Tenta obter nos pais
                if ((component = gameObject.GetComponentInParent<T>()) != null)
                    return true;
            }

            // Se n�o encontrar, tenta encontrar no cen�rio
            component = GameObject.FindObjectOfType<T>();

            // Se ainda n�o encontrar, retorna um erro
            if (component == null)
            {
                UnityEngine.Debug.LogError($"{typeof(T).Name} component not found in {gameObject.name} GameObject", gameObject);
                return false;
            }

            return true;
        }

        public static Vector2 ChangeVectorCordinates(Vector2 V, Vector2 X, Vector2 Y)
        {
            float alpha = (V.y * Y.x - V.x * Y.y) / (X.y * Y.x - X.x * Y.y);
            float beta = (V.x - alpha * X.x) / Y.x;

            return new Vector2(alpha, beta);
        }

        // Gets sign of a float value
        public static int Sign(float value) => value > 0 ? 1 : value < 0 ? -1 : 0;

        #region Update Event

        // Custom update event
        public static event Action<float> Update;

        // Call update event and pass deltaTime
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoadRuntimeMethod()
        {
            CreateCustomUpdate(typeof(Generics), CallUpdate);
            UnityEngine.Debug.Log($"Loaded {nameof(Generics)}");
        }

        public static void CreateCustomUpdate(Type type, UpdateFunction @delegate)
        {
            // Get all loops
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

            // Get Update loop (Monobehaviour void Update)
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type == typeof(UnityEngine.PlayerLoop.Update))
                {
                    // Creates new loop element
                    var customSystem = new PlayerLoopSystem
                    {
                        type = type,
                        updateDelegate = @delegate
                    };

                    // Creates a new Update loop with the new loop element
                    var subSystems = playerLoop.subSystemList[i].subSystemList;
                    var newSubSystems = new PlayerLoopSystem[subSystems.Length + 1];
                    subSystems.CopyTo(newSubSystems, 0);
                    newSubSystems[subSystems.Length] = customSystem;

                    playerLoop.subSystemList[i].subSystemList = newSubSystems;
                    break;
                }
            }

            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        private static void CallUpdate() => Update?.Invoke(Time.deltaTime);

        #endregion
    }

    // Other classes

    [Serializable]
    public class m_Time : IDisposable
    {
        public float time { get; protected set; }

        public m_Time() => Reset();

        public virtual void Reset() => time = 0;
        
        public static float CurrentTime() => UnityEngine.Time.time;

        // Destroy

        bool destroyed = false;

        public void Dispose()
        {
            OnDestroy(true);
            GC.SuppressFinalize(this);
        }

        ~m_Time()
        {
            OnDestroy(false);
        }

        protected virtual void OnDestroy(bool dispose) 
        {
            if(destroyed)
                return;

            UnityEngine.Debug.Log($"Destroy type: {dispose}");
            destroyed = true;
        }
    }

    public class TimerScope : IDisposable
    {
        private Timer _timer;

        public TimerScope(Timer timer)
        {
            _timer = timer;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    [Serializable]
    public class Timer : m_Time
    {
        public float duration;
        [SerializeField] public bool finished { get; protected set; } = false;

        public bool repeatAfterEnd = false;

        public bool paused = false;
        public Action TimerEnd;
        private static readonly ConditionalWeakTable<object, TimerScope> _scopes = new ConditionalWeakTable<object, TimerScope>();

        public Timer(float duration = 1, bool repeatAfterEnd = false, bool paused = false)
        {
            this.duration = duration;
            this.repeatAfterEnd = repeatAfterEnd;
            Generics.Update += Update;
            this.paused = paused;

            // referencia void f
            _scopes.Add(new StackTrace().GetFrame(1), new TimerScope(this));
        }

        protected virtual void Update(float deltaTime)
        {
            if (paused || finished) return;

            if (time >= duration)
            {
                TimerEnd?.Invoke();
                
                if(repeatAfterEnd)
                    Reset();
                else
                    finished = true;
            }
            else
                time += deltaTime;
        }

        public override void Reset()
        {
            base.Reset();
            finished = false;
        }

        public virtual void Initialize()
        {
            paused = false;
            Reset();
        }

        // Destroy
        protected override void OnDestroy(bool dispose)
        {
            base.OnDestroy(dispose);
            Generics.Update -= Update;
        }

        ~Timer()
        {
            OnDestroy(false);
        }
    }

    [Serializable]
    public class InterpolatedFloat
    {
        [SerializeField] protected float startValue;
        [SerializeField] private float target;
        private float _time;
        [SerializeField] private float duration;

        public InterpolatedFloat(float target, float duration, float startValue = 0)
        {
            this.startValue = startValue;
            SetTarget(target);
            this.duration = duration;
        }

        public void Update(float time) => _time += time;

        public void SetTarget(float target) => this.target = target;

        public float GetTarget() => target; 

        public void SetDuration(float duration) => this.duration = duration;

        public float GetDuration() => duration;

        public void Reset() => _time = 0;

        public float GetValue() => Mathf.Lerp(startValue, target, _time / duration);

        public float GetStartValue() => startValue;

        public float SetStartValue(float value) => startValue = value;

        public float GetTime() => _time;
    }
}
