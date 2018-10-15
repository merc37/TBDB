using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyNamespace
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class PathfindingVehicle : MonoBehaviour
	{
		[SerializeField] private float maxSpeed = 10;

		[SerializeField] private float maxForce = 1;
		//[SerializeField] private Transform target;

		private Vector3 _target;
		private Vector3 _desiredVelocity;
		private Vector3 _steeringVelocity;

		private Rigidbody2D _rigidbody;

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			_desiredVelocity = Camera.main.ScreenToWorldPoint(Input.mousePosition) -
			                   (Vector3) _rigidbody.position;

			//_desiredVelocity = _desiredVelocity.normalized * maxSpeed;
			_desiredVelocity = Vector3.ClampMagnitude(_desiredVelocity, maxSpeed);
			
			_steeringVelocity = _desiredVelocity - (Vector3)_rigidbody.velocity;
			
			Vector3.ClampMagnitude(_steeringVelocity, maxForce);
			
			_rigidbody.AddForce(_steeringVelocity);
			
			transform.rotation = Quaternion.LookRotation(transform.forward, new Vector2(-_rigidbody.velocity.y, _rigidbody.velocity.x));
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position+_desiredVelocity);
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.position, transform.position+_steeringVelocity);
		}
	}
}