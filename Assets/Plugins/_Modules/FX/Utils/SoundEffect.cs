using DCFApixels;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace Modules.FX
{
    [CreateAssetMenu]
    public class SoundEffect : ScriptableObject
    {
        public AudioSource AudioSourcePrefab;
        public ClipRecord[] Clips = new ClipRecord[0];
        public float VolumeMultiplier = 1;
        public float PitchMultiplier = 1;
        public float DurationMultiplier = 1;
        public float ScaleBlend = 0.5f;

        private QuasiRandom _pitchRandom;
        private void OnEnable()
        {
            _pitchRandom = new QuasiRandom(GetInstanceID());
        }

        [Button("Play")]
        protected async void PlayInEditor()
        {
            var src = Instantiate(AudioSourcePrefab);
            src.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            var clip = GetRandomClip();
            src.PlayOneShot(clip);
        
            await Awaitable.WaitForSecondsAsync(clip.Clip.length + 0.1f);
            src.Stop();
            DestroyImmediate(src);
        }
        
        public ResultClip GetRandomClip()
        {
            return GetRandomClip(UnityEngine.Random.value, UnityEngine.Random.value, _pitchRandom.NextFloat());
        }
        public ResultClip GetRandomClip(float clipValue)
        {
            return GetRandomClip(clipValue, UnityEngine.Random.value, _pitchRandom.NextFloat());
        }
        public ResultClip GetRandomClip(float clipValue, float volumeValue, float pitchValue)
        {
            ResultClip result = default;
            ref ClipRecord rec = ref Clips.GetRandom(clipValue);
            result.Clip = rec.Clip;
            result.Volume = rec.Volume.Lerp(volumeValue) * VolumeMultiplier;
            result.Pitch = rec.Pitch.Lerp(pitchValue) * PitchMultiplier;
            result.DurationMultiplier = DurationMultiplier * rec.DurationMultiplier;
            return result;
        }
        
        
        //public void Play(Vector3 position)
        //{
        //    Play(position, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        //}
        //public void Play(Vector3 position, float clipValue)
        //{
        //    Play(position, clipValue, UnityEngine.Random.value, UnityEngine.Random.value);
        //}
        //public void Play(Vector3 position, float clipValue, float volumeValue, float pitchValue)
        //{
        //    var clip = GetRandomClip(clipValue, volumeValue, pitchValue);
        //    AudioSourcePrefab.SpawnTemp(out var source, null, position).Duration(clip.GetDuration() + 0.1f);
        //    source.PlayOneShot(clip);
        //}
        [System.Serializable]
        public struct ClipRecord
        {
            public AudioClip Clip;
        
            public MinMaxRange Volume;
            public MinMaxRange Pitch;
            public float DurationMultiplier;
        }
        [System.Serializable]
        public struct ResultClip
        {
            public AudioClip Clip;
            public float Volume;
            public float Pitch;
            public float DurationMultiplier;
            public float GetDuration()
            {
                return Clip.length * DurationMultiplier;
            }
        }
        [System.Serializable]
        public struct MinMaxRange
        {
            public float Min;
            public float Max;
            public MinMaxRange(float min, float max)
            {
                Min = min;
                Max = max;
            }
            public float Lerp(float t)
            {
                return Mathf.LerpUnclamped(Min, Max, t);
            }
            public static MinMaxRange Blend(MinMaxRange a, MinMaxRange b)
            {
                return new MinMaxRange(a.Min * b.Min, a.Max * b.Max);
            }
            public static MinMaxRange operator *(MinMaxRange range, float scalar) { return new MinMaxRange(range.Min * scalar, range.Max * scalar); }
            public static MinMaxRange operator *(float scalar, MinMaxRange range) { return range * scalar; }
        }
    }

    public static class SoundEffectExt
    {
        public static void PlayOneShot(this AudioSource source, SoundEffect sfx)
        {
            PlayOneShot(source, sfx.GetRandomClip());
        }
        public static void PlayOneShot(this AudioSource source, SoundEffect.ResultClip sfx)
        {
            source.volume = sfx.Volume;
            source.pitch = sfx.Pitch;
            source.PlayOneShot(sfx.Clip);
        }
     
        internal static ref T GetRandom<T>(this T[] array, float t)
        {
            if (array == null)
            {
                throw new System.ArgumentNullException(nameof(array));

            }
            if (array.Length == 0)
            {
                throw new System.ArgumentException("Array cannot be empty", nameof(array));
            }

            // Ограничиваем t в пределах [0, 1]
            float clampedT = Mathf.Clamp(t, 0f, 1f);

            // Вычисляем индекс
            int index = (int)(clampedT * array.Length);
            // Если t == 1, индекс может оказаться равным Length, корректируем
            if (index == array.Length)
                index = array.Length - 1;

            return ref array[index];
        }
    }

#if UNITY_EDITOR
    namespace Editors
    {
        [CustomPropertyDrawer(typeof(SoundEffect.MinMaxRange))]
        internal class MiMaxRangeDrawer : PropertyDrawer
        {
            private GUIContent _label;
            private GUIContent Label(string text)
            {
                if (_label == null)
                {
                    _label = new GUIContent();
                }
                _label.text = text;
                return _label;
            }
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                float dw = EditorGUIUtility.labelWidth;
                // Находим свойства min и max внутри структуры MinMaxRange
                SerializedProperty minProp = property.FindPropertyRelative("Min");
                SerializedProperty maxProp = property.FindPropertyRelative("Max");

                if (minProp == null || maxProp == null)
                {
                    EditorGUI.LabelField(position, label.text, "MinMaxRange requires 'min' and 'max' fields");
                    return;
                }

                // Рисуем стандартный label и получаем прямоугольник для полей
                Rect fieldsRect = EditorGUI.PrefixLabel(position, label);

                // Делим прямоугольник на две равные части (можно добавить небольшой отступ между полями)
                float spacing = 2f;
                float fieldWidth = (fieldsRect.width - spacing) * 0.5f;
                Rect minRect = new Rect(fieldsRect.x, fieldsRect.y, fieldWidth, fieldsRect.height);
                Rect maxRect = new Rect(fieldsRect.x + fieldWidth + spacing, fieldsRect.y, fieldWidth, fieldsRect.height);

                EditorGUIUtility.labelWidth = 28f;
                // Рисуем поля без подписей
                EditorGUI.PropertyField(minRect, minProp, Label("min"));
                EditorGUI.PropertyField(maxRect, maxProp, Label("max"));
                EditorGUIUtility.labelWidth = dw;
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                // Высота одной строки
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
#endif
}

