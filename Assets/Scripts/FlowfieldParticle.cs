using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowfieldParticle : MonoBehaviour {
    public float _movespeed;

	
	// Update is called once per frame
	private void Update ()
    {
        transform.position += transform.forward * _movespeed * Time.deltaTime;
	}

    public void ApplyRotation(Vector3 rotation, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
