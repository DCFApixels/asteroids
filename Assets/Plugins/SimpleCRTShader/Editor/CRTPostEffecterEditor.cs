using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CRTPostEffecter))]
public class CRTPostEffecterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CRTPostEffecter effect = target as CRTPostEffecter;
        effect.material = (Material)EditorGUILayout.ObjectField("Effect Material", effect.material, typeof(Material), false);

        using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
        {
            effect.whiteNoiseFrequency = EditorGUILayout.IntField("White Noise Freaquency (x/1000)", effect.whiteNoiseFrequency);
            effect.whiteNoiseLength = EditorGUILayout.FloatField("White Noise Time Left (sec)", effect.whiteNoiseLength);
        }
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            effect.screenJumpFrequency = EditorGUILayout.IntField("Screen Jump Freaquency (x/1000)", effect.screenJumpFrequency);
            effect.screenJumpLength = EditorGUILayout.FloatField("Screen Jump Length", effect.screenJumpLength);
            using (new EditorGUILayout.HorizontalScope())
            {
                effect.screenJumpMinLevel = EditorGUILayout.FloatField("min", effect.screenJumpMinLevel);
                effect.screenJumpMaxLevel = EditorGUILayout.FloatField("max", effect.screenJumpMaxLevel);
            }
        }

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            effect.isSlippageNoise = EditorGUILayout.Toggle("Slippage Noise", effect.isSlippageNoise);
            effect.slippageStrength = EditorGUILayout.FloatField("Slippage Strength", effect.slippageStrength);
            effect.slippageInterval = EditorGUILayout.FloatField("Slippage Interval", effect.slippageInterval);
            effect.slippageScrollSpeed = EditorGUILayout.FloatField("Slippage Scroll Speed", effect.slippageScrollSpeed);
            effect.slippageSize = EditorGUILayout.FloatField("Slippage Size", effect.slippageSize);
        }

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            effect.isLowResolution = EditorGUILayout.Toggle("Low Resolution", effect.isLowResolution);
            //effect.resolutions = EditorGUILayout.Vector2IntField("Resolutions", effect.resolutions);
        }

        Material material = effect.material;
        if (material != null)
        {
            if (_materialEditor == null || _materialEditor.target != material)
            {
                _materialEditor = Editor.CreateEditor(material) as MaterialEditor;
            }
            _materialEditor.DrawHeader();
            _materialEditor.OnInspectorGUI();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    private MaterialEditor _materialEditor;


    private void OnDisable()
    {
        // Очищаем созданный редактор материала
        if (_materialEditor != null)
        {
            DestroyImmediate(_materialEditor);
        }
    }
}
