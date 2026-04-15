#if UNITY_EDITOR
using DCFApixels.DragonECS.Core.Unchecked;
using DCFApixels.DragonECS.Unity.Internal;
using UnityEditor;
using UnityEngine;

namespace DCFApixels.DragonECS.Unity.Editors
{
    internal class QuerySnapshotEditor : EditorWindow
    {
        private StructList<entlong> _list;
        public static void ShowNew(EcsSpan entites)
        {
            var newWin = EditorWindow.CreateWindow<QuerySnapshotEditor>();
            newWin.Setup(entites);
            newWin.Show();
        }
        private void Setup(EcsSpan entites)
        {
            _list = new StructList<entlong>(entites.Count);
            _list._count = entites.Longs.ToArray(ref _list._items);
        }
        private Vector2 scrollState;
        private void OnGUI()
        {
            var rect = position;
            rect.x = 0;
            rect.y = 0;

            var line = EditorGUIUtility.singleLineHeight;
            var space = EditorGUIUtility.standardVerticalSpacing;

            var viewRect = rect;
            viewRect.x = 0;
            viewRect.y = 0;
            viewRect.height = (line + space) * _list.Count;

            var lineRect = viewRect;
            lineRect.height = line;
            Rect hyperlinkButtonRect = default;
            (lineRect, hyperlinkButtonRect) = lineRect.HorizontalSliceRight(18f);

            using (DragonGUI.BeginScrollView(rect, ref scrollState, viewRect))
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    EntitySlotInfo e = (EntitySlotInfo)_list[i];
                    DragonGUI.EntityField(lineRect, e);
                    EcsWorld.TryGetWorld(e.worldID, out EcsWorld world);

                    using (DragonGUI.SetEnable(world != null))
                    {
                        DragonGUI.EntityHyperlinkButton(hyperlinkButtonRect, world, e.id);
                    }

                    lineRect.y += line + space;
                    hyperlinkButtonRect.y += line + space;
                }
            }
        }
        private void OnDestroy() { }
    }
}
#endif