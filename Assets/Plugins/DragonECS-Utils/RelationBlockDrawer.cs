using DCFApixels.DragonECS;
using DCFApixels.DragonECS.Unity.Editors;
using UnityEditor;
using UnityEngine;

namespace Project
{
    public class RelationBlockDrawer : EntityEditorBlockDrawer
    {
        public override void Draw(entlong entity)
        {
            if (entity.TryUnpack(out int relE, out EcsWorld world) && world.IsGraphWorld())
            {
                var graph = world.GetGraph();
                if (graph.IsRelation(relE))
                {
                    var se = graph.GetRelationStartEnd(relE);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.IntField(se.start);
                    GUILayout.Label("->", GUILayout.ExpandWidth(false));
                    EditorGUILayout.IntField(se.end);
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
