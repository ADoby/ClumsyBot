//#define FINALIK
// Foot-placement for FinalIK's Biped Ik solver.
//
using UnityEngine;
using System.Collections;
//
#if (FINALIK)
	using RootMotion.FinalIK;
	[RequireComponent(typeof(BipedIK))]
#endif
public class FootPlacement : MonoBehaviour {
#if (FINALIK)
	public LayerMask Mask = 1 << 0;
	public Vector2 HeelOffset = new Vector2(0.05f,0.05f);
	public Vector2 SoleOffset = new Vector2(0.05f,0.05f);
	//
	public AnimationCurve LeftFootCurve;
	public AnimationCurve LeftSoleCurve;
	public AnimationCurve RightFootCurve;
	public AnimationCurve RightSoleCurve;
	//
	public bool IgnoreCurvesIfIdle;
	//
	private BipedIK BIK;
	private Animator Actor;
	private Animation Act;
	//
	//
	private void Start() {
		BIK = GetComponent<BipedIK>();
		Actor = GetComponent<Animator>();
		Act = GetComponent<Animation>();
		//
		if (LeftFootCurve.keys.Length == 0)		{LeftFootCurve = new AnimationCurve(new Keyframe(0f,1f),new Keyframe(1f,0f));}
		if (LeftSoleCurve.keys.Length == 0)		{LeftSoleCurve = new AnimationCurve(new Keyframe(0f,1f),new Keyframe(1f,0f));}
		if (RightFootCurve.keys.Length == 0)	{RightFootCurve = new AnimationCurve(new Keyframe(0f,1f),new Keyframe(1f,0f));}
		if (RightSoleCurve.keys.Length == 0)	{RightSoleCurve = new AnimationCurve(new Keyframe(0f,1f),new Keyframe(1f,0f));}
	}
	//
	//
	private void LateUpdate() {
		Vector3 VL = new Vector3(BIK.references.leftFoot.position.x,BIK.references.pelvis.position.y,BIK.references.leftFoot.position.z);
		Vector3 VR = new Vector3(BIK.references.rightFoot.position.x,BIK.references.pelvis.position.y,BIK.references.rightFoot.position.z);
		Vector2 CenterOfMass = new Vector2(
			Vector3.Distance(BIK.references.pelvis.position,VL),
			Vector3.Distance(BIK.references.pelvis.position,VR)
		);
		//
		float LIK = LeftFootCurve.Evaluate(CenterOfMass.normalized.x); float RIK = RightFootCurve.Evaluate(CenterOfMass.normalized.y);
		float LIS = LeftSoleCurve.Evaluate(CenterOfMass.normalized.x); float RIS = RightSoleCurve.Evaluate(CenterOfMass.normalized.y);
		BIK.SetIKPositionWeight(AvatarIKGoal.LeftFoot,LIK); BIK.SetIKPositionWeight(AvatarIKGoal.RightFoot,RIK);
		BIK.SetIKRotationWeight(AvatarIKGoal.LeftFoot,LIS); BIK.SetIKRotationWeight(AvatarIKGoal.RightFoot,RIS);
		//
		//
		Transform Hips = BIK.references.pelvis;
		Transform L_Foot = BIK.references.leftFoot;
		Transform R_Foot = BIK.references.rightFoot;
		Transform L_Toe = BIK.references.leftFoot.GetChild(0);
		Transform R_Toe = BIK.references.rightFoot.GetChild(0);
		//
		BIK.SetIKPosition(AvatarIKGoal.LeftFoot,L_Foot.position); BIK.SetIKPosition(AvatarIKGoal.RightFoot,R_Foot.position);
		BIK.SetIKRotation(AvatarIKGoal.LeftFoot,L_Foot.rotation); BIK.SetIKRotation(AvatarIKGoal.RightFoot,R_Foot.rotation);
		//
		//
		RaycastHit LYH, LZH;
		Ray L_Y = new Ray(new Vector3(BIK.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,BIK.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Vector3 T_Pos_L; Quaternion T_Rot_L;
		if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset.x,LYH.point.z);
			BIK.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			if (L_Foot.position.y < Y.y) {BIK.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);}
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan);
			Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_TJ = new Vector3(L_Toe.position.x,BIK.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_T = new Vector3(L_TJ.x,BIK.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_TJ.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+SoleOffset.x*2f,L_T.z);
			Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH, Mathf.Infinity, Mask.value)) {
				T_Pos_L = BIK.GetIKPosition(AvatarIKGoal.LeftFoot);
				T_Rot_L = BIK.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset.x,LZH.point.z);
				T_Rot_L.SetLookRotation(SoleTarget-T_Pos_L,LYH.normal);
				BIK.SetIKRotation(AvatarIKGoal.LeftFoot,T_Rot_L);
				Debug.DrawLine(BIK.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan);
				Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		RaycastHit RYH, RZH;
		Ray R_Y = new Ray(new Vector3(BIK.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,BIK.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		Vector3 T_Pos_R; Quaternion T_Rot_R;
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset.y,RYH.point.z);
			BIK.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			if (R_Foot.position.y < Y.y) {BIK.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f);}
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan);
			Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_TJ = new Vector3(R_Toe.position.x,BIK.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_T = new Vector3(R_TJ.x,BIK.GetIKPosition(AvatarIKGoal.RightFoot).y,R_TJ.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+SoleOffset.y*2f,R_T.z);
			Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH, Mathf.Infinity, Mask.value)) {
				T_Pos_R = BIK.GetIKPosition(AvatarIKGoal.RightFoot);
				T_Rot_R = BIK.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset.y,RZH.point.z);
				T_Rot_R.SetLookRotation(SoleTarget-T_Pos_R,RYH.normal);
				BIK.SetIKRotation(AvatarIKGoal.RightFoot,T_Rot_R);
				Debug.DrawLine(BIK.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan);
				Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
		//
		//
		if (IgnoreCurvesIfIdle) { if (Actor) {
			AnimationInfo[] Info = Actor.GetCurrentAnimationClipState(0);
			if (Info[0].clip.name.ToLower().Contains("idle")) {
				BIK.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1); BIK.SetIKPositionWeight(AvatarIKGoal.RightFoot,1);
				BIK.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1); BIK.SetIKRotationWeight(AvatarIKGoal.RightFoot,1);
			}
		} else if (Act) { if (Act.clip.name.ToLower().Contains("idle")) {
			BIK.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1); BIK.SetIKPositionWeight(AvatarIKGoal.RightFoot,1);
			BIK.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1); BIK.SetIKRotationWeight(AvatarIKGoal.RightFoot,1);
		}}}
    }
#else
	private void Start() {
		Debug.LogWarning("This Component needs 'FINALIK' directive to run; If you already have FinalIK in your project, please uncomment '//#define FINALIK' on line 1.");
	}
#endif
}