//#define FINALIK
//
// Foot-placement for FinalIK's Full Body Ik solver. Oh yeah, this one is amazing!! :D
//
using UnityEngine;
using System.Collections;
//
#if (FINALIK)
	using RootMotion.FinalIK;
	[RequireComponent(typeof(FullBodyBipedIK))]
#endif
public class FBFootPlacement : MonoBehaviour {
#if (FINALIK)
	public LayerMask Mask = 1 << 0;
	public Vector2 HeelOffset = new Vector2(0.05f,0.05f);
	//
	public bool AdaptiveLeftFoot = true;
	public bool AdaptiveRightFoot = true;
	//
	public AnimationCurve LeftFootCurve;
	public AnimationCurve LeftSoleCurve;
	public AnimationCurve RightFootCurve;
	public AnimationCurve RightSoleCurve;
	//
	public bool IgnoreCurvesIfIdle;
	//
	private FullBodyBipedIK FBIK;
	private Animator Actor;
	private Animation Act;
	//
	//
	private void Start() {
		FBIK = GetComponent<FullBodyBipedIK>();
		Actor = GetComponent<Animator>();
		Act = GetComponent<Animation>();
		//
		if (LeftFootCurve.keys.Length == 0)		{LeftFootCurve = new AnimationCurve(new Keyframe(0f,1f),new Keyframe(1f,0f));}
		if (LeftSoleCurve.keys.Length == 0)		{LeftSoleCurve = new AnimationCurve(new Keyframe(1f,1f),new Keyframe(1f,1f));}
		if (RightFootCurve.keys.Length == 0)	{RightFootCurve = new AnimationCurve(new Keyframe(0f,1f),new Keyframe(1f,0f));}
		if (RightSoleCurve.keys.Length == 0)	{RightSoleCurve = new AnimationCurve(new Keyframe(1f,1f),new Keyframe(1f,1f));}
	}
	//
	//
	private void LateUpdate() {
		Vector3 VL = new Vector3(FBIK.references.leftFoot.position.x,FBIK.references.pelvis.position.y,FBIK.references.leftFoot.position.z);
		Vector3 VR = new Vector3(FBIK.references.rightFoot.position.x,FBIK.references.pelvis.position.y,FBIK.references.rightFoot.position.z);
		Vector2 CenterOfMass = new Vector2(
			Vector3.Distance(FBIK.references.pelvis.position,VL),
			Vector3.Distance(FBIK.references.pelvis.position,VR)
		);
		//
		float LIK = LeftFootCurve.Evaluate(CenterOfMass.normalized.x); float RIK = RightFootCurve.Evaluate(CenterOfMass.normalized.y);
		float LIS = LeftSoleCurve.Evaluate(CenterOfMass.normalized.x); float RIS = RightSoleCurve.Evaluate(CenterOfMass.normalized.y);
		FBIK.solver.leftFootEffector.positionWeight = LIK; FBIK.solver.rightFootEffector.positionWeight = RIK;
		FBIK.solver.leftFootEffector.rotationWeight = LIS; FBIK.solver.rightFootEffector.rotationWeight = RIS;
		//
		//
		Transform Hips = FBIK.references.pelvis;
		Transform L_Foot = FBIK.references.leftFoot;
		Transform R_Foot = FBIK.references.rightFoot;
		//
		FBIK.solver.leftFootEffector.position = L_Foot.position; FBIK.solver.rightFootEffector.position = R_Foot.position;
		FBIK.solver.leftFootEffector.rotation = L_Foot.rotation; FBIK.solver.rightFootEffector.rotation = R_Foot.rotation;
		//
		//
		Ray L_Y = new Ray(new Vector3(FBIK.solver.leftFootEffector.position.x,Hips.position.y,FBIK.solver.leftFootEffector.position.z),new Vector3(0,-1,0));
		//
		RaycastHit LYH; if (AdaptiveLeftFoot) {
		if (Physics.Raycast(L_Y, out LYH, Vector3.Distance(Hips.position,L_Foot.position)+HeelOffset.x, Mask.value)) {		
			if (Vector3.Distance(LYH.point,FBIK.solver.leftFootEffector.position) < Vector3.Distance(LYH.point,L_Foot.position*HeelOffset.x)) {			
				Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset.x,LYH.point.z);
				FBIK.solver.leftFootEffector.position = Y; if (L_Foot.position.y < Y.y)
				{FBIK.solver.leftFootEffector.positionWeight = 1f;} else {FBIK.solver.leftFootEffector.position = L_Foot.position;}
				Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan);
				Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
				Quaternion T_Rot_L = Quaternion.FromToRotation(Vector3.up,LYH.normal);
				FBIK.solver.leftFootEffector.rotation = T_Rot_L * FBIK.solver.leftFootEffector.bone.rotation;
			}		
		}} else { if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {		
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset.x,LYH.point.z);
			FBIK.solver.leftFootEffector.position = Y;
			if (L_Foot.position.y < Y.y) {FBIK.solver.leftFootEffector.positionWeight = 1f;}
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan);
			Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Quaternion T_Rot_L = Quaternion.FromToRotation(Vector3.up,LYH.normal);
			FBIK.solver.leftFootEffector.rotation = T_Rot_L * FBIK.solver.leftFootEffector.bone.rotation;	
		}}
		//
		Ray R_Y = new Ray(new Vector3(FBIK.solver.rightFootEffector.position.x,Hips.position.y,FBIK.solver.rightFootEffector.position.z),new Vector3(0,-1,0));
		//
		RaycastHit RYH; if (AdaptiveRightFoot) {
		if (Physics.Raycast(R_Y, out RYH, Vector3.Distance(Hips.position,R_Foot.position)+HeelOffset.y, Mask.value)) {		
			if (Vector3.Distance(RYH.point,FBIK.solver.rightFootEffector.position) < Vector3.Distance(RYH.point,R_Foot.position*HeelOffset.y)) {			
				Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset.y,RYH.point.z);
				FBIK.solver.rightFootEffector.position = Y; if (R_Foot.position.y < Y.y)
				{FBIK.solver.rightFootEffector.positionWeight = 1f;} else {FBIK.solver.rightFootEffector.position = R_Foot.position;}
				Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan);
				Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
				Quaternion T_Rot_R = Quaternion.FromToRotation(Vector3.up,RYH.normal);
				FBIK.solver.rightFootEffector.rotation = T_Rot_R * FBIK.solver.rightFootEffector.bone.rotation;
			}		
		}} else { if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {		
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset.y,RYH.point.z);
			FBIK.solver.rightFootEffector.position = Y;
			if (R_Foot.position.y < Y.y) {FBIK.solver.rightFootEffector.positionWeight = 1f;}
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan);
			Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Quaternion T_Rot_R = Quaternion.FromToRotation(Vector3.up,RYH.normal);
			FBIK.solver.rightFootEffector.rotation = T_Rot_R * FBIK.solver.rightFootEffector.bone.rotation;	
		}}
		//
		//
		if (IgnoreCurvesIfIdle) { if (Actor) {
			AnimationInfo[] Info = Actor.GetCurrentAnimationClipState(0);
			if (Info[0].clip.name.ToLower().Contains("idle")) {
				FBIK.solver.leftFootEffector.positionWeight = 1f; FBIK.solver.rightFootEffector.positionWeight = 1f;
				FBIK.solver.leftFootEffector.rotationWeight = 1f; FBIK.solver.rightFootEffector.rotationWeight = 1f;
			}
		} else if (Act) { if (Act.clip.name.ToLower().Contains("idle")) {
			FBIK.solver.leftFootEffector.positionWeight = 1f; FBIK.solver.rightFootEffector.positionWeight = 1f;
			FBIK.solver.leftFootEffector.rotationWeight = 1f; FBIK.solver.rightFootEffector.rotationWeight = 1f;
		}}}
    }
#else
	private void Start() {
		Debug.LogWarning("This Component needs 'FINALIK' directive to run; If you already have FinalIK in your project, please uncomment '//#define FINALIK' on line 1.");
	}
#endif
}