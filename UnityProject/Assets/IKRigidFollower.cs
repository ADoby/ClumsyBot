using UnityEngine;
using System.Collections;

public class IKRigidFollower : MonoBehaviour {

    public IKSolverRestriction otherThing;

    public float Spring = 2.0f;
    public float Damping = 0.2f;

    public float RotationSpring = 2.0f;
    public float RotationDamping = 0.2f;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!otherThing || otherThing.boneEntity == null)
            return;

        IKSolverRestriction.BoneEntity bone;
        for (int i = 0; i < otherThing.boneEntity.Length; i++)
        {
            bone = otherThing.boneEntity[i];

            Vector3 targetDirection = Vector3.zero;
            Vector3 currentDirection = Vector3.zero;
            Vector3 crossResult = Vector3.zero;

            float theDot = 0;
            float turnRadians = 0;
            float turnDeg = 0;

            currentDirection = bone.boneRigid.transform.forward;
            // CREATE THE DESIRED EFFECTOR POSITION VECTOR
            targetDirection = bone.bone.forward;

            // NORMALIZE THE VECTORS (EXPENSIVE, REQUIRES A SQRT)
            currentDirection.Normalize();
            targetDirection.Normalize();

            // THE DOT PRODUCT GIVES ME THE COSINE OF THE DESIRED ANGLE
            theDot = Vector3.Dot(currentDirection, targetDirection);

            crossResult = Vector3.Cross(currentDirection, targetDirection);
            currentDirection.Normalize();

            turnRadians = Mathf.Acos(theDot);

            turnDeg = turnRadians * Mathf.Rad2Deg;


            bone.currentAngularVelocity += crossResult * turnDeg * Time.fixedDeltaTime * RotationSpring;
            bone.currentAngularVelocity -= bone.currentAngularVelocity * RotationDamping * Time.fixedDeltaTime;

            bone.boneRigid.transform.Rotate(bone.currentAngularVelocity * Time.fixedDeltaTime);

            if(i == otherThing.boneEntity.Length - 1)
            {
                //last
               // bone.boneRigid.velocity += (otherThing.target.position - bone.boneRigid.position) * Spring * Time.fixedDeltaTime;
                //bone.boneRigid.velocity -= bone.boneRigid.velocity * Damping * Time.fixedDeltaTime;
            }
        }
	}
}
