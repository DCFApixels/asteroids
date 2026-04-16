#if UNITY_EDITOR
using DCFApixels.DragonECS.Core.Unchecked;
using DCFApixels.DragonECS.Unity.Internal;
using UnityEditor;
using UnityEngine;
using static DCFApixels.DragonECS.Unity.Editors.DragonGUI;

namespace DCFApixels.DragonECS.Unity.Editors
{
    internal class QuerySnapshotWindow : EditorWindow
    {
        private EcsWorld _world;
        private StructList<entlong> _list;

        private readonly Color _selectionColor = new Color(0.12f, 0.5f, 1f, 0.40f);
        private readonly Color _isAliveColor = new Color(0.2f, 0.6f, 1f);
        private readonly Color _hoverColor = new Color(1f, 1f, 1f, 0.12f);
        public static void ShowNew(EcsSpan entites)
        {
            var newWin = CreateWindow<QuerySnapshotWindow>("Query Snapshot");
            newWin.Setup(entites);
            newWin.ShowUtility();

        }
        private void Setup(EcsSpan entites)
        {
            _world = entites.World;
            _list = new StructList<entlong>(entites.Count);
            _list._count = entites.Longs.ToArray(ref _list._items);
        }
        private Vector2 _scrollState;
        private void OnGUI()
        {
            if (_world.IsDestroyed) { _world = null; }
            if (_world == null)
            {
                Close();
                return;
            }
            int selectedEntity = -1;
            var selectedGO = Selection.activeGameObject;
            if (selectedGO != null &&
                selectedGO.TryGetComponent<EntityMonitor>(out var selectedMonitor) && 
                selectedMonitor.Entity.TryUnpack(_world, out selectedEntity) == false)
            {
                selectedEntity = -1;
            }

            var line = EditorGUIUtility.singleLineHeight;
            var space = EditorGUIUtility.standardVerticalSpacing;
            var step = line + space;
            Event current = Event.current;

            int prevEntity = 0;
            int nextEntity = 0;

            int moveSign = 0;
            if (hasFocus && current.type == EventType.KeyUp && current.isKey)
            {
                if (current.keyCode == KeyCode.DownArrow)
                {
                    moveSign = 1;
                }
                if (current.keyCode == KeyCode.UpArrow)
                {
                    moveSign = -1;
                }
            }

            var rect = position;
            rect.x = 0;
            rect.y = 0;

            Rect hyperlinkButtonRect;
            Rect worldRect;
            (worldRect, rect) = rect.VerticalSliceTop(line + space);
            worldRect = worldRect.AddPadding(0, 0, 0, space);
            (worldRect, hyperlinkButtonRect) = worldRect.HorizontalSliceRight(18f);

            EditorGUI.IntField(worldRect, "World: ", _world.ID);
            using (DragonGUI.SetEnable(_world != null))
            {
                DragonGUI.WorldHyperlinkButton(hyperlinkButtonRect, _world);
            }

            var viewRect = rect;
            viewRect.x = 0;
            viewRect.y = 0;
            viewRect.height = (line + space) * _list.Count;
            viewRect.xMax -= GUI.skin.verticalScrollbar.fixedWidth;


            var lineRect = viewRect;
            lineRect.height = line;
            Rect statusR;
            (statusR, lineRect) = lineRect.HorizontalSliceLeft(3f);

            _scrollState = GUI.BeginScrollView(rect, _scrollState, viewRect, false, true);
            var scheckRect = rect;
            scheckRect.position = Vector2.zero;

            bool foundSelected = false;
            for (int i = 0; i < _list.Count; i++)
            {
                EntitySlotInfo entity = (EntitySlotInfo)_list[i];
                bool isAlive = _world.IsAlive(entity.id, entity.gen);
                bool selected = selectedEntity == entity.id && isAlive;
                foundSelected |= selected;
                bool isClick = false;


                
                bool visible = lineRect.Overlaps(scheckRect.AddOffset(_scrollState));
                if (visible)
                {
                    using (DragonGUI.SetAlpha(0)) { GUI.Label(lineRect, string.Empty, GUI.skin.button); }
                    if (DragonGUI.HitTest(lineRect))
                    {
                        EditorGUI.DrawRect(lineRect, _hoverColor);
                        if (current.type == EventType.MouseUp)
                        {
                            isClick = true;
                        }
                    }
                    if (selected)
                    {
                        DragonGUI.DrawRect(lineRect, _selectionColor);
                    }
                    if (isAlive)
                    {
                        DragonGUI.DrawRect(statusR, _isAliveColor);
                    }
                    var (labelR, infoR) = lineRect.HorizontalSliceLeft(45f);
                    infoR.width = Mathf.Min(infoR.width, 200f);
                    var (lR, rR) = infoR.HorizontalSliceLerp(0.5f);
                    GUI.Label(labelR, "Entity", GUI.skin.label);
                    EditorGUI.IntField(lR, entity.id, GUI.skin.label);
                    EditorGUI.IntField(rR, entity.gen, GUI.skin.label);
                }




                if (isClick && isAlive)
                {
                    SelectEntity(entity.id);
                }

                if (isAlive)
                {
                    if (foundSelected == false)
                    {
                        prevEntity = entity.id;
                    }
                    if (foundSelected && selected == false && nextEntity == 0)
                    {
                        nextEntity = entity.id;
                    }
                }

                lineRect.y += step;
                statusR.y += step;
            }
            GUI.EndScrollView();


            if (moveSign != 0)
            {
                if(moveSign < 0)
                {
                    SelectEntity(prevEntity);
                }
                else
                {
                    SelectEntity(nextEntity);
                }


                Repaint();
                return;
            }
        }

        private void SelectEntity(int entityID)
        {
            var monitor = _world.Get<EntityLinksComponent>().GetMonitorLink(entityID);
            EditorGUIUtility.PingObject(monitor);
            Selection.activeObject = monitor;
        }
        private void OnDestroy() { }
    }
}
#endif