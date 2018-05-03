using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour {

	public Limb[] limbs;
	public Transform basePoint;

	private void Update()
	{
		for(int i = limbs.Length-1; i >= 0; i--)
		{
			limbs[i].ConnectLimb();
		}

		limbs[0].transform.position = basePoint.position;

		for(int i = 1; i < limbs.Length; i++)
		{
			limbs[i].transform.position = limbs[i - 1].endJoint.position;
		}
	}
}
