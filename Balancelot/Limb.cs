using UnityEngine;

public class Limb : MonoBehaviour {

	public Transform connectionPoint;
	[HideInInspector]
	public Transform endJoint;

	private Vector2 direction;
	private Vector3 positionFromPoint;

	Quaternion rotationQ;

	private float objectLength;
	private float angle_;

	void Start()
	{
		endJoint = gameObject.transform.Find("EndJoint");
		objectLength = (endJoint.position - transform.position).magnitude;
	}

	public void ConnectLimb()
	{
		direction = (connectionPoint.position - transform.position).normalized;

		angle_ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		rotationQ = Quaternion.AngleAxis(angle_, transform.forward);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotationQ, 1);

		positionFromPoint = direction * objectLength * -1;
		transform.position = connectionPoint.position + positionFromPoint;

	}
}
