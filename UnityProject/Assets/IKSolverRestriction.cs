using UnityEngine;
using System.Collections;
using System.Collections.Generic;
///

///  Original source code :   http://www.darwin3d.com/gamedev/CCD3D.cpp
///  This class is ported on the original source code for Unity
/// 

public class IKSolverRestriction : MonoBehaviour
{

    [System.Serializable]
    public class BoneEntity
    {
        public Transform bone;
        public Rigidbody boneRigid;
        public bool restrictionEnabled;
        public Restriction restrictionRange;
        
        public Vector3 currentAngularVelocity = Vector3.zero;
    }

    [System.Serializable]
    public class Restriction
    {
        public float xMin = 0.0f;
        public float xMax = 360.0f;
        public float yMin = 0.0f;
        public float yMax = 360.0f;
        public float zMin = 0.0f;
        public float zMax = 360.0f;
    }

    public Transform target;
    public BoneEntity[] boneEntity;
    public bool damping = false;
    public float dampingMax = 0.5f;

    public float IK_POS_THRESH = 0.0025f;
    public int MAX_IK_TRIES = 20;

    public Transform StartBone;



    public float RotationSpring = 2.0f;
    public float RotationDamping = 0.2f;

    void Start()
    {
        if (target == null)
            target = transform;

        if (boneEntity.Length == 0)
        {
            List<BoneEntity> bones = new List<BoneEntity>();
            Transform currentBone = StartBone;
            Transform lastBone = null;
            while (true)
            {
                if (currentBone == lastBone)
                    break;
                if (!currentBone.rigidbody)
                    break;

                bones.Add(new BoneEntity() { bone = currentBone.transform, boneRigid = currentBone.rigidbody });
                lastBone = currentBone;

                foreach (Transform child in currentBone)
                {
                    if (child != currentBone && child.rigidbody)
                    {
                        currentBone = child;
                        break;
                    }
                }
            }
            boneEntity = bones.ToArray();
        }
    }

    void LateUpdate()
    {
        Solve();
    }

    public void Solve()
    {
        if(boneEntity.Length == 0)
            return;

        BoneEntity endEffector = boneEntity[boneEntity.Length - 1];
        Vector3 rootPos = Vector3.zero;
        Vector3 curEnd = Vector3.zero;

        Vector3 targetDirection = Vector3.zero;
        Vector3 currentDirection = Vector3.zero;
        Vector3 crossResult = Vector3.zero;

        float theDot = 0;
        float turnRadians = 0;
        float turnDeg = 0;

        int link = boneEntity.Length - 1;
        int tries = 0;

        // POSITION OF THE END EFFECTOR
        //curEnd = endEffector.position;

        // QUIT IF I AM CLOSE ENOUGH OR BEEN RUNNING LONG ENOUGH
        // SEE IF I AM ALREADY CLOSE ENOUGH
        while (tries < MAX_IK_TRIES && (curEnd - target.position).sqrMagnitude > IK_POS_THRESH)
        {

            if (link < 0)
            {
                link = boneEntity.Length - 1;
            }

            rootPos = boneEntity[link].bone.position;
            curEnd = endEffector.bone.position;

            // CREATE THE VECTOR TO THE CURRENT EFFECTOR POS
            currentDirection = curEnd - rootPos;
            // CREATE THE DESIRED EFFECTOR POSITION VECTOR
            targetDirection = target.position - rootPos;

            // NORMALIZE THE VECTORS (EXPENSIVE, REQUIRES A SQRT)
            currentDirection.Normalize();
            targetDirection.Normalize();

            // THE DOT PRODUCT GIVES ME THE COSINE OF THE DESIRED ANGLE
            theDot = Vector3.Dot(currentDirection, targetDirection);

            // IF THE DOT PRODUCT RETURNS 1.0, I DON'T NEED TO ROTATE AS IT IS 0 DEGREES
            if (theDot < 0.999999f)
            {

                
            }
            // USE THE CROSS PRODUCT TO CHECK WHICH WAY TO ROTATE

            crossResult = Vector3.Cross(currentDirection, targetDirection);
            currentDirection.Normalize();

            turnRadians = Mathf.Acos(theDot);

            if (damping && turnRadians > dampingMax)
                turnRadians = dampingMax;

            turnDeg = turnRadians * Mathf.Rad2Deg;

            if (float.IsNaN(turnDeg))
                turnDeg = 0f;

            //NonNAN(ref crossResult);

            boneEntity[link].currentAngularVelocity += crossResult * turnDeg * Time.deltaTime * RotationSpring;
            boneEntity[link].currentAngularVelocity -= boneEntity[link].currentAngularVelocity * RotationDamping * Time.deltaTime;

            //NonNAN(ref boneEntity[link].currentAngularVelocity);

            boneEntity[link].bone.Rotate(boneEntity[link].currentAngularVelocity * Time.deltaTime);

            //boneEntity[link].bone.rotation = Quaternion.AngleAxis(turnDeg, crossResult) * boneEntity[link].bone.rotation;

            if (boneEntity[link].restrictionEnabled)
                CheckRestrictions(boneEntity[link]);


            tries++;
            link--;
        }

    }

    void NonNAN(ref Vector3 vec)
    {
        if (float.IsNaN(vec.x))
                vec.x = 0f;
            if (float.IsNaN(vec.y))
                vec.y = 0f;
            if (float.IsNaN(vec.z))
                vec.z = 0f;
        
    }

    void CheckRestrictions(BoneEntity boneEntity)
    {

        // FIRST STEP IS TO CONVERT LINK QUATERNION BACK TO EULER ANGLES
        Vector3 euler = boneEntity.bone.rotation.eulerAngles;

        // CHECK THE DOF SETTINGS
        if (euler.x > boneEntity.restrictionRange.xMax)
            euler.x = boneEntity.restrictionRange.xMax;
        if (euler.x < boneEntity.restrictionRange.xMin)
            euler.x = boneEntity.restrictionRange.xMin;
        if (euler.y > boneEntity.restrictionRange.yMax)
            euler.y = boneEntity.restrictionRange.yMax;
        if (euler.y < boneEntity.restrictionRange.yMin)
            euler.y = boneEntity.restrictionRange.yMin;
        if (euler.z > boneEntity.restrictionRange.zMax)
            euler.z = boneEntity.restrictionRange.zMax;
        if (euler.z < boneEntity.restrictionRange.zMin)
            euler.z = boneEntity.restrictionRange.zMin;

        // BACK TO QUATERNION
        boneEntity.bone.localRotation = Quaternion.Euler(euler);
    }
}