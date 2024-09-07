using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mantega
{
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

    public static class Generics
    {
        public static bool ReallyTryGetComponent<T>(GameObject gameObject, out T component) where T : Component
        {
            // Tenta obter o componente diretamente
            if (!(gameObject == null) && gameObject.TryGetComponent(out component))
                return true;

            // Se não encontrar, tenta encontrar no cenário
            component = GameObject.FindObjectOfType<T>();

            // Se ainda não encontrar, retorna um erro
            if (component == null)
            {
                Debug.LogError($"{typeof(T).Name} component not found in {gameObject.name} GameObject", gameObject);
                return false;
            }

            return true;
        }

        public static int Sign(float value) => value > 0 ? 1 : value < 0 ? -1 : 0;
    }
}
