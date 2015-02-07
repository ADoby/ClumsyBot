/* Author: Tobias Zimmerlin
 * 30.01.2015
 * V1
 * 
 * Completely different to "SetPivot" you can actually move the pivot around while this script is active
 * Center Pivot works well with non rotated objects, rotated objects work ok but move a little bit around
 */

using UnityEngine;
using UnityEditor;

namespace SimpleLibrary
{

    public class SimplePivotController : EditorWindow
    {
        bool activated = true;
        Vector3 pivot;

        GameObject obj; //Selected object in the Hierarchy
        Transform trans; //Selected object in the Hierarchy
        MeshFilter meshFilter; //Mesh Filter of the selected object
        Mesh mesh; //Mesh of the selected object
        Collider col; //Collider of the selected object

        bool pivotUnchanged; //Flag to decide when to instantiate a copy of the mesh

        Vector3 LastPosition;

        static SimplePivotController window;

        [MenuItem("GameObject/Pivot Controller &#p")] //Place the Set Pivot menu item in the GameObject menu
        static void Init()
        {
            if (window)
            {
                window.Close();
                return;
            }
            window = (SimplePivotController)EditorWindow.GetWindow(typeof(SimplePivotController));
            window.RecognizeSelectedObject(); //Initialize the variables by calling RecognizeSelectedObject on the class instance
            window.Show();
        }

        public void Update()
        {
            if (obj && mesh && trans)
            {
                Vector3 positionBefore = trans.position;
                Vector3 difference = LastPosition - positionBefore;
                difference = trans.InverseTransformDirection(difference);
                if (activated) UpdatePivot(difference);
                trans.position = positionBefore;
                LastPosition = trans.position;

                UpdatePivotVector();
            }

            Repaint();
        }

        void OnGUI()
        {
            if (obj)
            {
                if (mesh)
                {
                    activated = EditorGUILayout.Toggle(new GUIContent("Enabled"), activated);

                    bool care = trans.eulerAngles.magnitude != 0f;

                    EditorGUILayout.LabelField(new GUIContent(string.Format("Rotation Magnitude:{0}", trans.eulerAngles.magnitude)));
                    if (care)
                        EditorGUILayout.LabelField(new GUIContent("Centering the pivot for a rotated object may move the object"));
                    Vector3 difference = -mesh.bounds.center;

                    if (GUILayout.Button(care ? "Center Pivot (Caution)" : "Center Pivot"))
                    {
                        UpdatePivot(difference);
                        trans.position -= difference;
                        Vector3 newPosition = trans.position;
                        newPosition = newPosition.Round(3);

                        trans.position = newPosition;
                        LastPosition = trans.position;

                        if (col)
                        {
                            if (col is BoxCollider)
                            {
                                ((BoxCollider)col).center = ((BoxCollider)col).center.Round(3);
                            }
                            else if (col is CapsuleCollider)
                            {
                                ((CapsuleCollider)col).center = ((CapsuleCollider)col).center.Round(3);
                            }
                            else if (col is SphereCollider)
                            {
                                ((SphereCollider)col).center = ((SphereCollider)col).center.Round(3);
                            }
                        }
                    }

                    EditorGUILayout.LabelField(string.Format("Relative Pivot: x:{0} y:{1} z:{2}", pivot.x, pivot.y, pivot.z));

                    GUILayout.Label("Bounds " + mesh.bounds.ToString());
                }
                else
                {
                    GUILayout.Label("Selected object does not have a Mesh specified.");
                }
            }
            else
            {
                GUILayout.Label("No object selected in Hierarchy.");
            }
        }

        void UpdatePivot(Vector3 diff)
        {
            Vector3[] verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] += diff;
            }
            mesh.vertices = verts;
            mesh.RecalculateBounds();

            if (col)
            {
                if (col is BoxCollider)
                {
                    ((BoxCollider)col).center += diff;
                }
                else if (col is CapsuleCollider)
                {
                    ((CapsuleCollider)col).center += diff;
                }
                else if (col is SphereCollider)
                {
                    ((SphereCollider)col).center += diff;
                }
            }
        }

        void UpdatePivotVector()
        {
            Bounds b = mesh.bounds;
            Vector3 offset = -1 * b.center;
            pivot = new Vector3(offset.x / b.extents.x, offset.y / b.extents.y, offset.z / b.extents.z);
            pivot = pivot.Round(3);
        }

        void OnSelectionChange()
        {
            RecognizeSelectedObject();
        }

        void RecognizeSelectedObject()
        {
            trans = Selection.activeTransform;
            if (trans == null)
            {
                mesh = null;
                obj = null;
                return;
            }

            obj = trans.gameObject;
            LastPosition = trans.position;
            if (obj)
            {
                meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
                mesh = meshFilter ? meshFilter.sharedMesh : null;
                if (mesh)
                    UpdatePivotVector();
                col = obj.GetComponent(typeof(Collider)) as Collider;
                pivotUnchanged = true;
            }
            else
            {
                mesh = null;
            }
        }
    }
}
