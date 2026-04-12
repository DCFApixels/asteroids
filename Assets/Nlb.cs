using System;
using UnityEngine;


namespace Utils
{
    /// <summary>
    /// Аналог Nullable<T> с поддержкой сериализации в Unity и кастомным PropertyDrawer.
    /// </summary>
    /// <typeparam name="T">Тип значения (структура).</typeparam>
    [System.Serializable]
    public struct Nlb<T> where T : struct
    {
        [SerializeField] private T _value;
        [SerializeField] private bool _isNull;

        /// <summary>Признак, что значение равно null (аналог !HasValue).</summary>
        public bool IsNull => _isNull;

        /// <summary>Признак, что значение не равно null (аналог HasValue).</summary>
        public bool HasValue => !_isNull;

        /// <summary>Значение, если не null. Иначе выбрасывает исключение.</summary>
        public T Value
        {
            get
            {
                if (_isNull)
                    throw new InvalidOperationException("Nlb<T> does not have a value.");
                return _value;
            }
        }

        /// <summary>Создаёт экземпляр с заданным значением (не null).</summary>
        public Nlb(T value)
        {
            _value = value;
            _isNull = false;
        }
        private Nlb(T value, bool isNull)
        {
            _value = value;
            _isNull = isNull;
        }
        public static Nlb<T> Manual(T value, bool isNull)
        {
            return new Nlb<T>(value, isNull);
        }

        /// <summary>Статическое свойство, возвращающее null-экземпляр.</summary>
        public static Nlb<T> Null => new Nlb<T> { _isNull = true, _value = default };

        /// <summary>Возвращает значение или значение по умолчанию, если null.</summary>
        public T GetValueOrDefault() => _isNull ? default : _value;

        /// <summary>Возвращает значение или указанное значение по умолчанию, если null.</summary>
        public T GetValueOrDefault(T defaultValue) => _isNull ? defaultValue : _value;

        // Неявное преобразование из T в Nlb<T> (значение не null)
        public static implicit operator Nlb<T>(T value) => new Nlb<T>(value);

        // Переопределения для удобства
        public override string ToString() => _isNull ? "null" : _value.ToString();
        public override bool Equals(object obj) => obj is Nlb<T> other && _isNull == other._isNull && (_isNull || _value.Equals(other._value));
        public override int GetHashCode() => _isNull ? 0 : _value.GetHashCode();



        // Сравнение с null
        //public static bool operator ==(Nlb<T> left, object right)
        //{
        //    if (right is null)
        //        return left._isNull;
        //    return false;
        //}
        //
        //public static bool operator !=(Nlb<T> left, object right)
        //{
        //    return !(left == right);
        //}

        // Сравнение двух Nlb<T>
        public static bool operator ==(Nlb<T> left, Nlb<T> right)
        {
            return left._isNull == right._isNull && (left._isNull || left._value.Equals(right._value));
        }

        public static bool operator !=(Nlb<T> left, Nlb<T> right)
        {
            return !(left == right);
        }

        // Сравнение с обычным значением T (неявно преобразуемым в Nlb<T>)
        public static bool operator ==(Nlb<T> left, T right)
        {
            return !left._isNull && left._value.Equals(right);
        }

        public static bool operator !=(Nlb<T> left, T right)
        {
            return !(left == right);
        }

        public static bool operator ==(T left, Nlb<T> right)
        {
            return right == left;
        }

        public static bool operator !=(T left, Nlb<T> right)
        {
            return right != left;
        }



        // Преобразование в Nullable<T>
        public static implicit operator T?(Nlb<T> nlb) => nlb._isNull ? null : nlb._value;

        // Преобразование из Nullable<T>
        public static implicit operator Nlb<T>(T? nullable) => nullable.HasValue ? new Nlb<T>(nullable.Value) : Null;
    }


#if UNITY_EDITOR
    namespace Editors
    {
        using UnityEditor;
        using UnityEngine;

        /// <summary>
        /// PropertyDrawer для Nlb<T>. Отображает поле значения и чекбокс null.
        /// При активном чекбоксе поле значения становится неактивным.
        /// </summary>
        [CustomPropertyDrawer(typeof(Nlb<>))]
        public class NlbDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // Находим поля структуры
                SerializedProperty valueProp = property.FindPropertyRelative("_value");
                SerializedProperty isNullProp = property.FindPropertyRelative("_isNull");

                // Рисуем стандартный label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                // Вычисляем прямоугольники: поле значения и чекбокс
                Rect valueRect = new Rect(position.x, position.y, position.width - 20, position.height);
                Rect toggleRect = new Rect(position.x + position.width - 18, position.y, 18, position.height);

                // Сохраняем состояние GUI.enabled
                bool previousEnabled = GUI.enabled;
                // Если чекбокс активен (null), поле значения отключаем
                GUI.enabled = !isNullProp.boolValue;

                // Рисуем поле значения без дополнительного label
                EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);

                // Восстанавливаем состояние
                GUI.enabled = previousEnabled;

                // Рисуем чекбокс для isNull (при включении значение становится null)
                EditorGUI.PropertyField(toggleRect, isNullProp, GUIContent.none);
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                SerializedProperty valueProp = property.FindPropertyRelative("_value");
                // Высота определяется полем значения (может быть многострочным)
                return EditorGUI.GetPropertyHeight(valueProp, GUIContent.none);
            }
        }
    }
#endif
}