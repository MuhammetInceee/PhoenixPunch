#if (UNITY_EDITOR)
using UnityEditor;
using UnityEngine;

namespace Mami.LabelBlocks
{
    [InitializeOnLoad]
    public class LabelBlocks : MonoBehaviour
    {
        static LabelBlocks() => EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj == null) return;
            var objName = obj.name;

            if (objName[0] != '-' || objName[1] != '-') return;
            var fontColor = Color.white;
            var backgroundColor = objName[2] switch
            {
                'r' => Color.red,
                'g' => Color.green,
                'b' => Color.blue,
                '#' when ColorUtility.TryParseHtmlString(objName[2..9], out Color col) => col,
                _ => Color.gray
            };

            Rect offsetRect = new(selectionRect.position, selectionRect.size);
            EditorGUI.DrawRect(selectionRect, backgroundColor);

            var newObjName = objName[(objName[2] != '#' ? 3 : 9)..];
            EditorGUI.LabelField(offsetRect, newObjName, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = fontColor },
                richText = true,
                alignment = TextAnchor.UpperCenter
            });
        }
    }
}
#endif