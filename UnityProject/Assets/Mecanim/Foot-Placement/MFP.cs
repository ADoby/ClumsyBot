//	MFP (Mecanim Foot Placement) Extension for Animator class.
//	Publishing: 2013 OUSE Games by Unity Asset Store.
//	Author: Bruno Xavier Leite.
//	::
//	You are free to modify this extention by any means for whatever reason.
//	No refences or credits to original author required. Code is all yours.
//	Note that support may be limited to original unchanged code only.

// :: Update Notes ::

// V1.32:
// Monobehaviour version added. For Legacy users.

// V1.3:
// FinalIK method added.

// V1.21:
// Bug fixes for LayerMasked methods.

// V1.2:
// Method without FBX Curves is now using 'L_Weight' and 'R_Weight' to adjust IK weights. You can modify these curves by calling 'MyActor.FP_LeftCurve()' and 'MyActor.FP_RightCurve()'.
// You can create public AnimationCurve on your controller and pass that curve to the function, so you can control your curves from inspector. I.e, 'MyActor.FP_LeftCurve(MyCurve);'
// If no curve is given, the non FBX based method will auto-create a curve for you. You still have FBX Curves option if your run/walk animations are kind of not standard.
// Basic method don't require weight parameter anymore.

// V1.1:
// Given requests, 'Hip bone's transform is now called from Mecanim internals instead of asking you to give it as a parameter.
// Added method for detailed placement without FBX Curves, courtesy of Eli Curtz.

/// <summary>
/// If you wish to use MFP with FINALIK, simply uncomment '#define FINALIK' to compile. Note that FINALIK package is required for this!
/// </summary>
//#define FINALIK

#if (FINALIK)
	using RootMotion.FinalIK;
#endif
using UnityEngine;

#region FOOT-PLACEMENT
public static class MFP {
	#region UNITYIK
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Raw placement, only heel bones are considered. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='Offset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	public static void FootPlacement(this Animator animator, bool Do, float Offset) {
		if (!Do) {return;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f);
		//
		RaycastHit LYH, RYH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+Offset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
		}
		if (Physics.Raycast(R_Y, out RYH)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+Offset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
		}
	}
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Detailed foot-placement. IK weight affects both left and right legs. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='HeelOffset'>
	/// Use this to adjust mesh height of character's heels. 
	/// </param>
	/// <param name='SoleOffset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	/// <param name='Weight'>
	/// IK Weights are subtracted by this. 0 = Full IK Weight, 1 = Zero IK Weight. 
	/// </param>
	/// <param name='L_Toe'>
	/// Left foot's tip, joint's transform. 
	/// </param>
	/// <param name='R_Toe'>
	/// Right foot's tip, joint's transform. 
	/// </param>
	/// <param name='L_Tip'>
	/// Left Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	/// <param name='R_Tip'>
	/// Right Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset, float Weight, Transform L_Toe, Transform R_Toe, Transform L_Tip, Transform R_Tip) {
		if (!Do) {return;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f-Weight);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1f-Weight);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f-Weight);
		animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,1f-Weight);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_T = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z); Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH)) {
				L_Tip.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot); L_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				L_Tip.rotation = Quaternion.FromToRotation(L_Tip.up,LYH.normal); L_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.LeftFoot),L_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan); Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_T = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+HeelOffset*2f,R_T.z); Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH)) {
				R_Tip.position = animator.GetIKPosition(AvatarIKGoal.RightFoot); R_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				R_Tip.rotation = Quaternion.FromToRotation(R_Tip.up,RYH.normal); R_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightFoot),R_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan); Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
	}
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Detailed foot-placement. IK weight for left and right legs are separate values. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='HeelOffset'>
	/// Use this to adjust mesh height of character's heels. 
	/// </param>
	/// <param name='SoleOffset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	/// <param name='Weight'>
	/// IK Weight curves. X = Left Leg, Y = Right Leg. Use Mecanim curves to control this. 
	/// </param>
	/// <param name='L_Toe'>
	/// Left foot's tip, joint's transform. 
	/// </param>
	/// <param name='R_Toe'>
	/// Right foot's tip, joint's transform. 
	/// </param>
	/// <param name='L_Tip'>
	/// Left Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	/// <param name='R_Tip'>
	/// Right Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset, Vector2 Weight, Transform L_Toe, Transform R_Toe, Transform L_Tip, Transform R_Tip) {
		if (!Do) {return;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,Weight.x);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,Weight.x);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,Weight.y);
		animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,Weight.y);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_T = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z); Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH)) {
				L_Tip.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot); L_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				L_Tip.rotation = Quaternion.FromToRotation(L_Tip.up,LYH.normal); L_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.LeftFoot),L_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan); Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_T = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+HeelOffset*2f,R_T.z); Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH)) {
				R_Tip.position = animator.GetIKPosition(AvatarIKGoal.RightFoot); R_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				R_Tip.rotation = Quaternion.FromToRotation(R_Tip.up,RYH.normal); R_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightFoot),R_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan); Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
	}
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Detailed foot-placement. IK weight for left and right legs are separate values. Rotation and Position weights are separate values. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='HeelOffset'>
	/// Use this to adjust mesh height of character's heels. 
	/// </param>
	/// <param name='SoleOffset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	/// <param name='PositionWeight'>
	/// IK Weight curves. X = Left Leg, Y = Right Leg. Use Mecanim curves to control this. 
	/// </param>
	/// <param name='RotationWeight'>
	/// IK Weight curves. X = Left Leg, Y = Right Leg. Use Mecanim curves to control this. 
	/// </param>
	/// <param name='L_Toe'>
	/// Left foot's tip, joint's transform. 
	/// </param>
	/// <param name='R_Toe'>
	/// Right foot's tip, joint's transform. 
	/// </param>
	/// <param name='L_Tip'>
	/// Left Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	/// <param name='R_Tip'>
	/// Right Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset, Vector2 PositionWeight, Vector2 RotationWeight, Transform L_Toe, Transform R_Toe, Transform L_Tip, Transform R_Tip) {
		if (!Do) {return;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,PositionWeight.x);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,RotationWeight.x);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,PositionWeight.y);
		animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,RotationWeight.y);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			//
			Vector3 L_T = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z); Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH)) {
				L_Tip.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot); L_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				L_Tip.rotation = Quaternion.FromToRotation(L_Tip.up,LYH.normal); L_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.LeftFoot),L_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan); Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		if (Physics.Raycast(R_Y, out RYH)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_T = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+HeelOffset*2f,R_T.z); Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH)) {
				R_Tip.position = animator.GetIKPosition(AvatarIKGoal.RightFoot); R_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				R_Tip.rotation = Quaternion.FromToRotation(R_Tip.up,RYH.normal); R_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightFoot),R_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan); Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
	}
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Detailed foot-placement. IK weight for left and right legs are separate values. Rotation and Position weights are separate values. 
	/// Returns a Vector2: X = Left Foot IK distance to desired point, Y = Right Foot IK distance. You can use these values to adjust CharacterController's offset. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='HeelOffset'>
	/// Use this to adjust mesh height of character's heels. 
	/// </param>
	/// <param name='SoleOffset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	/// <param name='Weight'>
	/// IK Weights curves. X = Left Leg, Y = Right Leg. Use Mecanim curves to control this. 
	/// </param>
	/// <param name='Mask'>
	/// Tells which layers should be raycasted or ignored for placement. 
	/// </param>
	/// <param name='L_Toe'>
	/// Left foot's tip, joint's transform. 
	/// </param>
	/// <param name='R_Toe'>
	/// Right foot's tip, joint's transform. 
	/// </param>
	/// <param name='L_Tip'>
	/// Left Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	/// <param name='R_Tip'>
	/// Right Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	public static Vector2 FootPlacementV2(this Animator animator, bool Do, float HeelOffset, float SoleOffset, Vector2 PositionWeight, Vector2 RotationWeight, Transform L_Toe, Transform R_Toe, Transform L_Tip, Transform R_Tip) {
		if (!Do) {return Vector2.zero;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,PositionWeight.x);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,RotationWeight.x);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,PositionWeight.y);
		animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,RotationWeight.y);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_T = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z); Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH)) {
				L_Tip.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot); L_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				L_Tip.rotation = Quaternion.FromToRotation(L_Tip.up,LYH.normal); L_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.LeftFoot),L_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan); Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_T = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+HeelOffset*2f,R_T.z); Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH)) {
				R_Tip.position = animator.GetIKPosition(AvatarIKGoal.RightFoot); R_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				R_Tip.rotation = Quaternion.FromToRotation(R_Tip.up,RYH.normal); R_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightFoot),R_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan); Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
		//
		Transform LF_JT = animator.GetBoneTransform(HumanBodyBones.LeftFoot); Transform RF_JT = animator.GetBoneTransform(HumanBodyBones.RightFoot);
		return new Vector2( Vector3.Distance(LF_JT.position,LYH.point)-(HeelOffset/2) , Vector3.Distance(RF_JT.position,RYH.point)-(HeelOffset/2) );
	}
	//
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Raw placement, only heel bones are considered. Using a Layer-Mask for Raycasting. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='Offset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	public static void FootPlacement(this Animator animator, bool Do, float Offset, LayerMask Mask) {
		if (!Do) {return;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f);
		//
		RaycastHit LYH, RYH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+Offset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
		}
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+Offset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
		}
	}
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Detailed foot-placement. IK weight affects both left and right legs. Using a Layer-Mask for Raycasting. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='HeelOffset'>
	/// Use this to adjust mesh height of character's heels. 
	/// </param>
	/// <param name='SoleOffset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	/// <param name='Weight'>
	/// IK Weights are subtracted by this. 0 = Full IK Weight, 1 = Zero IK Weight. 
	/// </param>
	/// <param name='L_Toe'>
	/// Left foot's tip, joint's transform. 
	/// </param>
	/// <param name='R_Toe'>
	/// Right foot's tip, joint's transform. 
	/// </param>
	/// <param name='L_Tip'>
	/// Left Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	/// <param name='R_Tip'>
	/// Right Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset, float Weight, Transform L_Toe, Transform R_Toe, Transform L_Tip, Transform R_Tip, LayerMask Mask) {
		if (!Do) {return;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f-Weight);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1f-Weight);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f-Weight);
		animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,1f-Weight);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_T = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z); Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH, Mathf.Infinity, Mask.value)) {
				L_Tip.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot); L_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				L_Tip.rotation = Quaternion.FromToRotation(L_Tip.up,LYH.normal); L_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.LeftFoot),L_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan); Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_T = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+HeelOffset*2f,R_T.z); Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH, Mathf.Infinity, Mask.value)) {
				R_Tip.position = animator.GetIKPosition(AvatarIKGoal.RightFoot); R_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				R_Tip.rotation = Quaternion.FromToRotation(R_Tip.up,RYH.normal); R_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightFoot),R_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan); Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
	}
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Detailed foot-placement. IK weight for left and right legs are separate values. Using a Layer-Mask for Raycasting. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='HeelOffset'>
	/// Use this to adjust mesh height of character's heels. 
	/// </param>
	/// <param name='SoleOffset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	/// <param name='Weight'>
	/// IK Weight curves. X = Left Leg, Y = Right Leg. Use Mecanim curves to control this. 
	/// </param>
	/// <param name='L_Toe'>
	/// Left foot's tip, joint's transform. 
	/// </param>
	/// <param name='R_Toe'>
	/// Right foot's tip, joint's transform. 
	/// </param>
	/// <param name='L_Tip'>
	/// Left Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	/// <param name='R_Tip'>
	/// Right Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset, Vector2 Weight, Transform L_Toe, Transform R_Toe, Transform L_Tip, Transform R_Tip, LayerMask Mask) {
		if (!Do) {return;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,Weight.x);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,Weight.x);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,Weight.y);
		animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,Weight.y);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_T = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z); Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH, Mathf.Infinity, Mask.value)) {
				L_Tip.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot); L_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				L_Tip.rotation = Quaternion.FromToRotation(L_Tip.up,LYH.normal); L_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.LeftFoot),L_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan); Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_T = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+HeelOffset*2f,R_T.z); Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH, Mathf.Infinity, Mask.value)) {
				R_Tip.position = animator.GetIKPosition(AvatarIKGoal.RightFoot); R_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				R_Tip.rotation = Quaternion.FromToRotation(R_Tip.up,RYH.normal); R_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightFoot),R_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan); Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
	}	
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Detailed foot-placement. IK weight for left and right legs are separate values. Rotation and Position weights are separate values. 
	/// Using a Layer-Mask for Raycasting. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='HeelOffset'>
	/// Use this to adjust mesh height of character's heels. 
	/// </param>
	/// <param name='SoleOffset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	/// <param name='PositionWeight'>
	/// IK Weight curves. X = Left Leg, Y = Right Leg. Use Mecanim curves to control this. 
	/// </param>
	/// <param name='RotationWeight'>
	/// IK Weight curves. X = Left Leg, Y = Right Leg. Use Mecanim curves to control this. 
	/// </param>
	/// <param name='L_Toe'>
	/// Left foot's tip, joint's transform. 
	/// </param>
	/// <param name='R_Toe'>
	/// Right foot's tip, joint's transform. 
	/// </param>
	/// <param name='L_Tip'>
	/// Left Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	/// <param name='R_Tip'>
	/// Right Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset, Vector2 PositionWeight, Vector2 RotationWeight, Transform L_Toe, Transform R_Toe, Transform L_Tip, Transform R_Tip, LayerMask Mask) {
		if (!Do) {return;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,PositionWeight.x);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,RotationWeight.x);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,PositionWeight.y);
		animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,RotationWeight.y);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			//
			Vector3 L_T = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z); Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH, Mathf.Infinity, Mask.value)) {
				L_Tip.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot); L_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				L_Tip.rotation = Quaternion.FromToRotation(L_Tip.up,LYH.normal); L_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.LeftFoot),L_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan); Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_T = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+HeelOffset*2f,R_T.z); Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH, Mathf.Infinity, Mask.value)) {
				R_Tip.position = animator.GetIKPosition(AvatarIKGoal.RightFoot); R_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				R_Tip.rotation = Quaternion.FromToRotation(R_Tip.up,RYH.normal); R_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightFoot),R_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan); Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
	}	
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only).
	/// This method is courtesy from Eli Curtz. This gives you detailed foot-placement while removing the need to use FBX Curves everywhere. 
	/// Notice however that this method does not give you fine control over animation to IK transitions. 
	/// Using a Layer-Mask for Raycasting. 
	/// </summary>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset, Transform L_Toe, Transform R_Toe, LayerMask Mask) {
        if (!Do) {return;}
		//
		Vector3 T_Pos; Quaternion T_Rot;
		//
		animator.SetIKPositionWeight (AvatarIKGoal.LeftFoot, (animator.pivotWeight < 0.51f) ? 1f : 0f);
		animator.SetIKRotationWeight (AvatarIKGoal.LeftFoot, (animator.pivotWeight < 0.51f) ? 1f : 0f);
		animator.SetIKPositionWeight (AvatarIKGoal.RightFoot, (animator.pivotWeight > 0.49f) ? 1f : 0f);
		animator.SetIKRotationWeight (AvatarIKGoal.RightFoot, (animator.pivotWeight > 0.49f) ? 1f : 0f);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray (new Vector3 (animator.GetIKPosition (AvatarIKGoal.LeftFoot).x, Hips.position.y, animator.GetIKPosition (AvatarIKGoal.LeftFoot).z), new Vector3 (0, -1, 0));
		Ray R_Y = new Ray (new Vector3 (animator.GetIKPosition (AvatarIKGoal.RightFoot).x, Hips.position.y, animator.GetIKPosition (AvatarIKGoal.RightFoot).z), new Vector3 (0, -1, 0));
		if (Physics.Raycast (L_Y, out LYH, Mathf.Infinity, Mask.value)) {
            Vector3 Y = new Vector3 (LYH.point.x, LYH.point.y + HeelOffset, LYH.point.z);
            animator.SetIKPosition (AvatarIKGoal.LeftFoot, Y);
            if (animator.GetBoneTransform (HumanBodyBones.LeftFoot).position.y < Y.y) {
                animator.SetIKPositionWeight (AvatarIKGoal.LeftFoot, 1f);
            }
            Debug.DrawLine (Hips.position, L_Y.origin, Color.cyan);
            Debug.DrawLine (L_Y.origin, LYH.point, Color.green);
            Vector3 L_T = new Vector3 (L_Toe.position.x, animator.GetIKPosition (AvatarIKGoal.LeftFoot).y, L_Toe.position.z);
            Vector3 L_Sole = new Vector3 (L_T.x, L_T.y + HeelOffset * 2f, L_T.z);
            Ray L_Z = new Ray (L_Sole, new Vector3 (0, -1, 0));
			if (Physics.Raycast(L_Z, out LZH, Mathf.Infinity, Mask.value)) {
                T_Pos = animator.GetIKPosition (AvatarIKGoal.LeftFoot);
                T_Rot = animator.GetIKRotation (AvatarIKGoal.LeftFoot);
                Vector3 SoleTarget = new Vector3 (LZH.point.x, LZH.point.y + SoleOffset, LZH.point.z);
                T_Rot.SetLookRotation(SoleTarget - T_Pos, LYH.normal);
                animator.SetIKRotation (AvatarIKGoal.LeftFoot, Quaternion.Slerp (animator.GetIKRotation (AvatarIKGoal.LeftFoot), T_Rot, Time.time));
                Debug.DrawLine (animator.GetIKPosition (AvatarIKGoal.LeftFoot), L_Sole, Color.cyan);
                Debug.DrawLine (L_Sole, LZH.point, Color.green);
            }
        }
		//
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
            Vector3 Y = new Vector3 (RYH.point.x, RYH.point.y + HeelOffset, RYH.point.z);
            animator.SetIKPosition (AvatarIKGoal.RightFoot, Y);
            if (animator.GetBoneTransform (HumanBodyBones.RightFoot).position.y < Y.y) {
                animator.SetIKPositionWeight (AvatarIKGoal.RightFoot, 1f);
            }
            Debug.DrawLine (Hips.position, R_Y.origin, Color.cyan);
            Debug.DrawLine (R_Y.origin, RYH.point, Color.green);
            Vector3 R_T = new Vector3 (R_Toe.position.x, animator.GetIKPosition (AvatarIKGoal.RightFoot).y, R_Toe.position.z);
            Vector3 R_Sole = new Vector3 (R_T.x, R_T.y + HeelOffset * 2f, R_T.z);
            Ray R_Z = new Ray (R_Sole, new Vector3 (0, -1, 0));
			if (Physics.Raycast(R_Z, out RZH, Mathf.Infinity, Mask.value)) {
                T_Pos = animator.GetIKPosition (AvatarIKGoal.RightFoot);
                T_Rot = animator.GetIKRotation (AvatarIKGoal.RightFoot);
                Vector3 SoleTarget = new Vector3 (RZH.point.x, RZH.point.y + SoleOffset, RZH.point.z);
                T_Rot.SetLookRotation(SoleTarget - T_Pos, RYH.normal);
                animator.SetIKRotation (AvatarIKGoal.RightFoot, Quaternion.Slerp (animator.GetIKRotation (AvatarIKGoal.RightFoot), T_Rot, Time.time));
                Debug.DrawLine (animator.GetIKPosition (AvatarIKGoal.RightFoot), R_Sole, Color.cyan);
                Debug.DrawLine (R_Sole, RZH.point, Color.green);
            }
        }
    }	
	//
	//
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only). 
	/// Detailed foot-placement. IK weight for left and right legs are separate values. Rotation and Position weights are separate values. 
	/// Returns a Vector2: X = Left Foot IK distance to desired point, Y = Right Foot IK distance. You can use these values to adjust CharacterController's offset. 
	/// Using a Layer-Mask for Raycasting. 
	/// </summary>
	/// <param name='Do'>
	/// If 'Do' = true, do placement. 
	/// </param>
	/// <param name='HeelOffset'>
	/// Use this to adjust mesh height of character's heels. 
	/// </param>
	/// <param name='SoleOffset'>
	/// Use this to adjust mesh height of character's sole. 
	/// </param>
	/// <param name='Weight'>
	/// IK Weights curves. X = Left Leg, Y = Right Leg. Use Mecanim curves to control this. 
	/// </param>
	/// <param name='Mask'>
	/// Tells which layers should be raycasted or ignored for placement. 
	/// </param>
	/// <param name='L_Toe'>
	/// Left foot's tip, joint's transform. 
	/// </param>
	/// <param name='R_Toe'>
	/// Right foot's tip, joint's transform. 
	/// </param>
	/// <param name='L_Tip'>
	/// Left Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	/// <param name='R_Tip'>
	/// Right Foot Tip. Any transform in world space used as reference. Do not parent this to foot joints. 
	/// </param>
	public static Vector2 FootPlacementV2(this Animator animator, bool Do, float HeelOffset, float SoleOffset, Vector2 PositionWeight, Vector2 RotationWeight, Transform L_Toe, Transform R_Toe, Transform L_Tip, Transform R_Tip, LayerMask Mask) {
		if (!Do) {return Vector2.zero;}
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,PositionWeight.x);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,RotationWeight.x);
		animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,PositionWeight.y);
		animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,RotationWeight.y);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan); Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_T = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z); Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH, Mathf.Infinity, Mask.value)) {
				L_Tip.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot); L_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				L_Tip.rotation = Quaternion.FromToRotation(L_Tip.up,LYH.normal); L_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.LeftFoot),L_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan); Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan); Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_T = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_Sole = new Vector3(R_T.x,R_T.y+HeelOffset*2f,R_T.z); Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH, Mathf.Infinity, Mask.value)) {
				R_Tip.position = animator.GetIKPosition(AvatarIKGoal.RightFoot); R_Tip.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				R_Tip.rotation = Quaternion.FromToRotation(R_Tip.up,RYH.normal); R_Tip.LookAt(SoleTarget);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,Quaternion.Slerp(animator.GetIKRotation(AvatarIKGoal.RightFoot),R_Tip.rotation,Time.time));
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan); Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
		//
		Transform LF_JT = animator.GetBoneTransform(HumanBodyBones.LeftFoot); Transform RF_JT = animator.GetBoneTransform(HumanBodyBones.RightFoot);
		return new Vector2( Vector3.Distance(LF_JT.position,LYH.point)-(HeelOffset/2) , Vector3.Distance(RF_JT.position,RYH.point)-(HeelOffset/2) );
	}
	//
	//
	//
	public static AnimationCurve L_Weight {get; set;}
	public static AnimationCurve R_Weight {get; set;}
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only).
	/// This method is courtesy from Eli Curtz, with a bit of modifications from Bruno. This gives you detailed foot-placement while removing the need to build custom curves. 
	/// Only parameters needed here are Heel's and Sole's height offsets. If you don't use FP_LeftCurve() or FP_RightCurve(), default curves will be created.
	/// </summary>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset) {
        if (!Do) {return;}
		//
		animator.stabilizeFeet = false;
		if (L_Weight == null || R_Weight == null) {
			L_Weight = new AnimationCurve(new Keyframe(-0.1f,1.1f),new Keyframe(1.1f,0.1f));
			R_Weight = new AnimationCurve(new Keyframe(-0.1f,0.1f),new Keyframe(1.1f,1.1f));
		}
		//
		float LIK = L_Weight.Evaluate(animator.pivotWeight); float RIK = R_Weight.Evaluate(animator.pivotWeight);
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,LIK); animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,RIK);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1); animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,1);
		//
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Transform L_Toe = animator.GetBoneTransform(HumanBodyBones.LeftToes);
		Transform R_Toe = animator.GetBoneTransform(HumanBodyBones.RightToes);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		Vector3 T_Pos_L, T_Pos_R; Quaternion T_Rot_L, T_Rot_R;
		if (Physics.Raycast(L_Y, out LYH)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
            if (animator.GetBoneTransform(HumanBodyBones.LeftFoot).position.y < Y.y) {animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);}
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan);
			Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_TJ = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_T = new Vector3(L_TJ.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_TJ.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z);
			Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH)) {
				T_Pos_L = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
				T_Rot_L = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				T_Rot_L.SetLookRotation(SoleTarget-T_Pos_L,LYH.normal);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,T_Rot_L);
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan);
				Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			if (animator.GetBoneTransform(HumanBodyBones.RightFoot).position.y < Y.y) {animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f);}
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan);
			Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_TJ = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_T = new Vector3(R_TJ.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_TJ.z);
			Vector3 R_Sole = new Vector3(R_T.x, R_T.y+HeelOffset*2f,R_T.z);
			Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH)) {
				T_Pos_R = animator.GetIKPosition(AvatarIKGoal.RightFoot);
				T_Rot_R = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				T_Rot_R.SetLookRotation(SoleTarget - T_Pos_R,LYH.normal);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,T_Rot_R);
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan);
				Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
		//
		AnimationInfo[] Info = animator.GetCurrentAnimationClipState(0);
		if (!Info[0].clip.name.ToLower().Contains("idle")) {
			animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,0);
			animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,0);
		}
    }
    //
    //
    //
	/// <summary>
	/// Automatic foot placement for Mecanim (Pro Only).
	/// This method is courtesy from Eli Curtz, with a bit of modifications from Bruno. This gives you detailed foot-placement while removing the need to build custom curves. 
	/// Parameters needed here are Heel's and Sole's height offsets. If you don't use FP_LeftCurve() or FP_RightCurve(), default curves will be created.
	/// </summary>
	public static void FootPlacement(this Animator animator, bool Do, float HeelOffset, float SoleOffset, LayerMask Mask) {
        if (!Do) {return;}
		//
		animator.stabilizeFeet = false;
		if (L_Weight == null || R_Weight == null) {
			L_Weight = new AnimationCurve(new Keyframe(-0.1f,1.1f),new Keyframe(1.1f,0.1f));
			R_Weight = new AnimationCurve(new Keyframe(-0.1f,0.1f),new Keyframe(1.1f,1.1f));
		}
		//
		float LIK = L_Weight.Evaluate(animator.pivotWeight); float RIK = R_Weight.Evaluate(animator.pivotWeight);
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,LIK); animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,RIK);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1); animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,1);
		//
		Transform Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		Transform L_Toe = animator.GetBoneTransform(HumanBodyBones.LeftToes);
		Transform R_Toe = animator.GetBoneTransform(HumanBodyBones.RightToes);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		Vector3 T_Pos_L, T_Pos_R; Quaternion T_Rot_L, T_Rot_R;
		if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
            if (animator.GetBoneTransform(HumanBodyBones.LeftFoot).position.y < Y.y) {animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);}
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan);
			Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_TJ = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_T = new Vector3(L_TJ.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_TJ.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z);
			Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH, Mathf.Infinity, Mask.value)) {
				T_Pos_L = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
				T_Rot_L = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				T_Rot_L.SetLookRotation(SoleTarget-T_Pos_L,LYH.normal);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,T_Rot_L);
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan);
				Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			if (animator.GetBoneTransform(HumanBodyBones.RightFoot).position.y < Y.y) {animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f);}
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan);
			Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_TJ = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_T = new Vector3(R_TJ.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_TJ.z);
			Vector3 R_Sole = new Vector3(R_T.x, R_T.y+HeelOffset*2f,R_T.z);
			Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH, Mathf.Infinity, Mask.value)) {
				T_Pos_R = animator.GetIKPosition(AvatarIKGoal.RightFoot);
				T_Rot_R = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				T_Rot_R.SetLookRotation(SoleTarget - T_Pos_R,LYH.normal);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,T_Rot_R);
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan);
				Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
		//
		AnimationInfo[] Info = animator.GetCurrentAnimationClipState(0);
		if (!Info[0].clip.name.ToLower().Contains("idle")) {
			animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,0);
			animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,0);
		}
    }
	//
	//
	//
	public static void FP_LeftCurve(this Animator animator, AnimationCurve Value) {L_Weight = Value;}
	public static void FP_RightCurve(this Animator animator, AnimationCurve Value) {R_Weight = Value;}
	#endregion
	//
	#if (FINALIK)
	private static Animator FIKAnimator;
	/// <summary>
	/// Automatic foot placement for Mecanim (Final IK).
	/// This gives you detailed foot-placement while removing the need to build custom curves. 
	/// Parameters needed here are Heel's and Sole's height offsets. If you don't use FP_LeftCurve() or FP_RightCurve(), default curves will be created.
	/// </summary>
	public static void FootPlacement(this BipedIK animator, bool Do, float HeelOffset, float SoleOffset, LayerMask Mask) {
        if (!Do) {return;} if (FIKAnimator != animator.GetComponent<Animator>()) {FIKAnimator = animator.GetComponent<Animator>();}
		//
		FIKAnimator.stabilizeFeet = false;
		if (L_Weight == null || R_Weight == null) {
			L_Weight = new AnimationCurve(new Keyframe(-0.1f,1.1f),new Keyframe(1.1f,0.1f));
			R_Weight = new AnimationCurve(new Keyframe(-0.1f,0.1f),new Keyframe(1.1f,1.1f));
		}
		//
		float LIK = L_Weight.Evaluate(FIKAnimator.pivotWeight); float RIK = R_Weight.Evaluate(FIKAnimator.pivotWeight);
		animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,LIK); animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,RIK);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,LIK); animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,RIK);
		//
		Transform Hips = animator.references.pelvis;
		Transform L_Foot = animator.references.leftFoot;
		Transform R_Foot = animator.references.rightFoot;
		Transform L_Toe = FIKAnimator.GetBoneTransform(HumanBodyBones.LeftToes);		// FinalIK Has no references for this, we wait for a fix.
		Transform R_Toe = FIKAnimator.GetBoneTransform(HumanBodyBones.RightToes);		// FinalIK Has no references for this, we wait for a fix.
		//
		animator.SetIKPosition(AvatarIKGoal.LeftFoot,L_Foot.position);
		animator.SetIKPosition(AvatarIKGoal.RightFoot,R_Foot.position);
		//
		RaycastHit LYH, RYH, LZH, RZH;
		Ray L_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.LeftFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.LeftFoot).z),new Vector3(0,-1,0));
		Ray R_Y = new Ray(new Vector3(animator.GetIKPosition(AvatarIKGoal.RightFoot).x,Hips.position.y,animator.GetIKPosition(AvatarIKGoal.RightFoot).z),new Vector3(0,-1,0));
		Vector3 T_Pos_L, T_Pos_R; Quaternion T_Rot_L, T_Rot_R;
		if (Physics.Raycast(L_Y, out LYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(LYH.point.x,LYH.point.y+HeelOffset,LYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.LeftFoot,Y);
            if (L_Foot.position.y < Y.y) {animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);}
			Debug.DrawLine(Hips.position,L_Y.origin,Color.cyan);
			Debug.DrawLine(L_Y.origin,LYH.point,Color.green);
			Vector3 L_TJ = new Vector3(L_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_Toe.position.z);
			Vector3 L_T = new Vector3(L_TJ.x,animator.GetIKPosition(AvatarIKGoal.LeftFoot).y,L_TJ.z);
			Vector3 L_Sole = new Vector3(L_T.x,L_T.y+HeelOffset*2f,L_T.z);
			Ray L_Z = new Ray(L_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(L_Z, out LZH, Mathf.Infinity, Mask.value)) {
				T_Pos_L = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
				T_Rot_L = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
				Vector3 SoleTarget = new Vector3(LZH.point.x,LZH.point.y+SoleOffset,LZH.point.z);
				T_Rot_L.SetLookRotation(SoleTarget-T_Pos_L,LYH.normal);
				animator.SetIKRotation(AvatarIKGoal.LeftFoot,T_Rot_L);
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.LeftFoot),L_Sole,Color.cyan);
				Debug.DrawLine(L_Sole,LZH.point,Color.green);
			}
		}
		//
		if (Physics.Raycast(R_Y, out RYH, Mathf.Infinity, Mask.value)) {
			Vector3 Y = new Vector3(RYH.point.x,RYH.point.y+HeelOffset,RYH.point.z);
			animator.SetIKPosition(AvatarIKGoal.RightFoot,Y);
			if (R_Foot.position.y < Y.y) {animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f);}
			Debug.DrawLine(Hips.position,R_Y.origin,Color.cyan);
			Debug.DrawLine(R_Y.origin,RYH.point,Color.green);
			Vector3 R_TJ = new Vector3(R_Toe.position.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_Toe.position.z);
			Vector3 R_T = new Vector3(R_TJ.x,animator.GetIKPosition(AvatarIKGoal.RightFoot).y,R_TJ.z);
			Vector3 R_Sole = new Vector3(R_T.x, R_T.y+HeelOffset*2f,R_T.z);
			Ray R_Z = new Ray(R_Sole,new Vector3(0,-1,0));
			if (Physics.Raycast(R_Z, out RZH, Mathf.Infinity, Mask.value)) {
				T_Pos_R = animator.GetIKPosition(AvatarIKGoal.RightFoot);
				T_Rot_R = animator.GetIKRotation(AvatarIKGoal.RightFoot);
				Vector3 SoleTarget = new Vector3(RZH.point.x,RZH.point.y+SoleOffset,RZH.point.z);
				T_Rot_R.SetLookRotation(SoleTarget - T_Pos_R,LYH.normal);
				animator.SetIKRotation(AvatarIKGoal.RightFoot,T_Rot_R);
				Debug.DrawLine(animator.GetIKPosition(AvatarIKGoal.RightFoot),R_Sole,Color.cyan);
				Debug.DrawLine(R_Sole,RZH.point,Color.green);
			}
		}
		//
		AnimationInfo[] Info = FIKAnimator.GetCurrentAnimationClipState(0);
		if (!Info[0].clip.name.ToLower().Contains("idle")) {
			animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,0);
			animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,0);
		}
    }
	#endif
}
#endregion