/*Author: Tobias Zimmerlin
 * 30.01.2015
 * V1
 * 
 */


using UnityEditor;
using UnityEngine;

namespace SimpleLibrary
{
    public class SimpleEditor
    {

        public static float EditorLineHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

    }
}