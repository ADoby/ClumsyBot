/*Author: Tobias Zimmerlin
 * 30.01.2015
 * V1
 * 
 */


using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SimpleLibrary
{
    public class SimpleKeyboardShortcuts
    {

        [MenuItem("GameObject/Toggle Active State &#a")]
        static void ToggleGameObjectActiveState()
        {
            Selection.activeGameObject.SetActive(!Selection.activeGameObject.activeSelf);
        }

        [MenuItem("GameObject/Create Empty Child &#n")]
        static void CreateNewEmptyGameObjectChild()
        {
            GameObject go = new GameObject("Child");
            go.transform.parent = Selection.activeTransform;
            Selection.activeTransform = go.transform;
        }
        

        [MenuItem("GameObject/Wrap in Empty &#w")]
        static void WrapInObject()
        {
            if (Selection.gameObjects.Length == 0)
                return;
            GameObject go = new GameObject("Wrapper:NameMe");
            go.transform.parent = Selection.activeTransform.parent;
            go.transform.position = Vector3.zero;
            foreach (GameObject g in Selection.gameObjects)
            {
                g.transform.parent = go.transform;
            }

            Selection.activeTransform = go.transform;
        }
    }
}