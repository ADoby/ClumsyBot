//#define FINALIK
////
// March 10, 2014 : 'Predictive Foot-Placement' for Full-body Biped Inverse Kinematics implementation based on Final IK.
// Due to the nature of procedural animation on this situation expect this component to be at least somewhat different
// from previous iterations of this bipedal and results may differ as well on an already developed level around the non
// predictive system; Also this component is a little bit more expensive than non-predictive computation so try to not
// abuse it, i.e: hundreds of characters on screen on desktop or tens on mobile.
// The focus here is to achieve at least as close as possible the results of the system implemented on Assassins Creed
// by Ubisoft's team as presented on GDC 2013 on foot-placement techniques based on HumanIK.
// Closest Full-body IK system we have to HumanIK on Unity is FinalIK (thanks Partel) thus the reason why we need it.
// Note: This is still not in perfect state.
////
using UnityEngine;
using System.Collections;
//
#if (FINALIK)
	using RootMotion.FinalIK;
	[RequireComponent(typeof(FullBodyBipedIK))]
#endif
//
public class FBBIKPFP : MonoBehaviour {
	#if (FINALIK)
	public LayerMask BodyMask = 1 << 0;								// Select which rendering layers body can raycast against.
	public LayerMask FeetMask = 1 << 0;								// Select which rendering layers feet can raycast against.
	public int BodySpineID = 1;									// The Spine Element of character's body effector.
	public float HeelOffset = 0.1f;									// Y Distance of Foot IK from Foot-planting base mesh.
	public float RayReach = 1.5f;									// How far to ray cast beyond character's leg length.
	public bool DoBodyRaycast;									// This is useful to prevent CharacterControllers from falling through ground.
	public bool DisableController;									// Forces a CharacterControllers component to be disabled.
	public Transform LeftToeTip;									// Character's left foot bone's toe tip. Can be None.
	public Transform RightToeTip;									// Character's right foot bone's toe tip. Can be None.
	//
	private FullBodyBipedIK FBIK;									// Full-Body IK System.
	private IKEffector BIK;										// The Body IK.
	private IKEffector LFIK;									// The Left Foot IK.
	private IKEffector RFIK;									// The Right Foot IK.
	//
	private Transform Body;										// Character's pelvis bone's transform.
	private Transform LeftFoot;									// Character's left foot bone's transform.
	private Transform RightFoot;									// Character's right foot bone's transform.
	private RaycastHit BodyFloorContact;								// Body to floor contact information.
	private RaycastHit LeftFootContact;								// Left Foot to floor contact information.
	private RaycastHit RightFootContact;								// Right Foot to floor contact information.
	private RaycastHit LeftToeContact;								// Left Foot's toe to floor contact information.
	private RaycastHit RightToeContact;								// Right Foot's toe to floor contact information.
	//
	private float GForce;										// Y acceleration based on project's gravity settings.
	private float LegLength;									// Estimated character's leg length on T pose.
	//
	private Vector3 LeftFootSpeed;									// Records left foot's movement speed calculation without a rigidBody.
	private Vector3 RightFootSpeed;									// Records right foot's movement speed calculation without a rigidBody.
	private Vector3 LeftFootLastPosition;								// Last frame position of left foot's transform.
	private Vector3 RightFootLastPosition;								// Last frame position of right foot's transform.
	//
	//
	private void OnEnable() {
		StartCoroutine(CalculateEntitySpeed());
		BIK.position = Body.position;
		LFIK.position = LeftFoot.position;
		RFIK.position = RightFoot.position;
		BIK.positionWeight =  0f; BIK.rotationWeight =  0f;
		LFIK.positionWeight = 1f; RFIK.positionWeight = 1f;
		LFIK.rotationWeight = 1f; RFIK.rotationWeight = 1f;
	}
	//
	private void OnDisable() {
		StopCoroutine("CalculateEntitySpeed");
		BIK.position = Body.position;
		LFIK.position = LeftFoot.position;
		RFIK.position = RightFoot.position;
		BIK.positionWeight =  0f; BIK.rotationWeight =  0f;
		LFIK.positionWeight = 0f; RFIK.positionWeight = 0f;
		LFIK.rotationWeight = 0f; RFIK.rotationWeight = 0f;
	}
	//
	private void Awake() {
		FBIK = GetComponent<FullBodyBipedIK>();
		//
		Body = FBIK.references.spine[BodySpineID];
		BIK = FBIK.solver.bodyEffector;
		LFIK = FBIK.solver.leftFootEffector;
		RFIK = FBIK.solver.rightFootEffector;
		//
		LeftFoot = FBIK.references.leftFoot;
		RightFoot = FBIK.references.rightFoot;
		//
		LeftFootContact = new RaycastHit(); RightFootContact = new RaycastHit();
		LeftToeContact = new RaycastHit(); RightToeContact = new RaycastHit();
		LegLength = Vector3.Distance(LeftFoot.position,Body.position);
	}
	//
	private void Update() {
		GForce = Time.deltaTime*Mathf.Abs(Physics.gravity.y)*2;
		if (RayReach < 1.5f) {RayReach = 1.5f;}
	}
	//
	private void LateUpdate() {
		GetBodyContact();
		GetFootContact(LFIK,LeftFoot,LeftToeTip,LeftFootSpeed,LeftFootContact,LeftToeContact);
		GetFootContact(RFIK,RightFoot,RightToeTip,RightFootSpeed,RightFootContact,RightToeContact);
	}
	//
	//
	private void GetBodyContact() {
		Ray RAY = new Ray(BIK.position,new Vector3(0,-1,0));
		if (Physics.Raycast(RAY,out BodyFloorContact,(LegLength+HeelOffset),BodyMask.value) && DoBodyRaycast) {
			BIK.position = Vector3.Lerp(BIK.position,new Vector3(transform.position.x,BodyFloorContact.point.y+LegLength,transform.position.z),GForce);
			transform.position = Vector3.Lerp(transform.position,new Vector3(transform.position.x,BodyFloorContact.point.y,transform.position.z),GForce);
			Debug.DrawLine(BIK.position,BodyFloorContact.point,Color.white);
		} else if (DoBodyRaycast) {
			transform.position = Vector3.Lerp(transform.position,new Vector3(transform.position.x,transform.position.y-GForce,transform.position.z),GForce);
			BIK.position = Vector3.Lerp(BIK.position,Body.position,GForce);
			Debug.DrawLine(BIK.position,new Vector3(BIK.position.x,BIK.position.y-LegLength,BIK.position.z),Color.grey);
		} else {BIK.position = Body.position;}
	}
	//
	private void GetFootContact(IKEffector IK, Transform Bone, Transform ToeTip, Vector3 BoneSpeed, RaycastHit HIT, RaycastHit THIT) {
		Ray RAY1 = new Ray(new Vector3(IK.position.x,BIK.position.y,IK.position.z),new Vector3(0,-1,0));
		Ray RAY2; if (ToeTip) {RAY2 = new Ray(new Vector3(ToeTip.position.x,BIK.position.y,ToeTip.position.z),new Vector3(0,-1,0));} else {RAY2 = RAY1;}
		//
		if (Physics.Raycast(RAY1,out HIT,(LegLength*RayReach),FeetMask.value)) {
			IK.positionWeight = 1f; IK.rotationWeight = 1f;
			IK.position = Vector3.Lerp(IK.position,new Vector3(Bone.position.x,HIT.point.y+HeelOffset,Bone.position.z),Time.time);
			IK.rotation = Quaternion.FromToRotation(Vector3.up,HIT.normal)*IK.bone.rotation;
			Debug.DrawLine(RAY1.origin,BIK.position,Color.cyan); Debug.DrawLine(RAY1.origin,IK.position,Color.green); Debug.DrawLine(IK.position,HIT.point,Color.red);
		}
		//
		if (ToeTip && Physics.Raycast(RAY2,out THIT,(LegLength*RayReach),FeetMask.value) && THIT.point.y > HIT.point.y) {
			IK.positionWeight = 1f; IK.rotationWeight = 1f;
			IK.position = Vector3.Lerp(IK.position,new Vector3(Bone.position.x,THIT.point.y+HeelOffset,Bone.position.z),Time.time);
			IK.rotation = Quaternion.FromToRotation(Vector3.up,THIT.normal)*IK.bone.rotation;
			Debug.DrawLine(RAY2.origin,BIK.position,Color.cyan); Debug.DrawLine(RAY2.origin,ToeTip.position,Color.green); Debug.DrawLine(ToeTip.position,THIT.point,Color.red);
		}
		//
		if (IK.position.y < THIT.point.y) {IK.position = new Vector3(Bone.position.x,THIT.point.y+HeelOffset,Bone.position.z);}
	}
	//
	private IEnumerator CalculateEntitySpeed() {
		while (this.enabled) {
			LeftFootLastPosition = LeftFoot.position;
			RightFootLastPosition = RightFoot.position;
			yield return new WaitForEndOfFrame();
			LeftFootSpeed = (LeftFootLastPosition-LeftFoot.position)/Time.deltaTime;
			RightFootSpeed = (RightFootLastPosition-RightFoot.position)/Time.deltaTime;
		}
	}
	//
	//
	#region DEBUG
	private void OnDrawGizmos() {
		try {Gizmos.DrawIcon(LFIK.position,"FBBIKPFP.png",true); Gizmos.DrawIcon(RFIK.position,"FBBIKPFP.png",true);}catch{}
	}
	#endregion
	//
	//
	#else
	private void Start() {
		Debug.LogWarning("This Component needs 'FINALIK' directive to run; If you already have FinalIK in your project, please uncomment '//#define FINALIK' on line 1.");
	}
	#endif
}