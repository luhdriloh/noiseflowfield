using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowfield : MonoBehaviour
{
    private FastNoise _fastnoise;
    public Vector3Int _gridsize;
    public float _increment;
    public Vector3 _offset, _offsetSpeed;

    // gridsize simply tells the size of our grid


	// Use this for initialization
	private void Start ()
    {
		
	}
	
	// Update is called once per frame
	private void Update ()
    {
		
	}

    private void OnDrawGizmos()
    {
        _fastnoise = new FastNoise();

        float xOff = 0f;
        for (int x = 0; x < _gridsize.x; x++)
        {
            float yOff = 0f;
            for (int y = 0; y < _gridsize.y; y++)
            {
                float zOff = 0f;
                for (int z = 0; z < _gridsize.z; z++)
                {
                    float noise = _fastnoise.GetSimplex(xOff + _offset.x, yOff + _offset.y, zOff + _offset.z) + 1;
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));

                    Gizmos.color = new Color(noiseDirection.normalized.x, noiseDirection.normalized.y, noiseDirection.normalized.z, 1f);
                    Vector3 pos = new Vector3(x, y, z) + transform.position;
                    Vector3 endPosition = pos + Vector3.Normalize(noiseDirection);

                    Gizmos.DrawLine(pos, endPosition);
                    // Gizmos.DrawSphere(pos, .05f);
                    // Gizmos.DrawSphere(endPosition, .05f);
                    zOff += _increment;
                }

                yOff += _increment;
            }

            xOff += _increment;
        }
    }
}
