using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowfield : MonoBehaviour
{
    private FastNoise _fastnoise;

    public Vector3Int _gridsize;
    public float _cellSize;
    public float _increment;
    public Vector3 _offset, _offsetSpeed; 
    public Vector3[,,] _flowfieldDirection;

    // gridsize tells the size of our grid

    // particles
    public GameObject _particleProtoType;
    public int _numberOfParticles;
    public float _spawnRadius;
    public float _particleScale, _particleMovespeed, _particleRotation;

    [HideInInspector]
    public List<FlowfieldParticle> _particles;

    // Use this for initialization
    private void Start ()
    {
        _flowfieldDirection = new Vector3[_gridsize.x, _gridsize.y, _gridsize.z];
        _fastnoise = new FastNoise();
        _particles = new List<FlowfieldParticle>();

        for (int i = 0; i < _numberOfParticles; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(transform.position.x, transform.position.x + _gridsize.x * _cellSize),
                Random.Range(transform.position.y, transform.position.y + _gridsize.y * _cellSize),
                Random.Range(transform.position.z, transform.position.z + _gridsize.z * _cellSize));

            if (ParticleSpawnValidation(randomPosition))
            {
                GameObject particleInstance = Instantiate(_particleProtoType);
                particleInstance.transform.position = randomPosition;
                particleInstance.transform.parent = transform;
                particleInstance.transform.localScale = new Vector3(_particleScale, _particleScale, _particleScale);
                _particles.Add(particleInstance.GetComponent<FlowfieldParticle>());
            }
        }

        Debug.Log("Created " + _particles.Count + " particles.");
	}
	
	// Update is called once per frame
	private void Update ()
    {
        CalculateFlowfieldDirections();
        ParticleBehavior();
	}

    private void CalculateFlowfieldDirections()
    {
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
                    _flowfieldDirection[x, y, z] = Vector3.Normalize(noiseDirection);


                    zOff += _increment;
                }

                yOff += _increment;
            }

            xOff += _increment;
        }
    }

    private void ParticleBehavior()
    {
        foreach (FlowfieldParticle particle in _particles)
        {
            // X component out of bounds
            if (particle.transform.position.x >= transform.position.x + (_gridsize.x * _cellSize))
            {
                particle.transform.position = new Vector3(transform.position.x, particle.transform.position.y, particle.transform.position.z);
            }

            if (particle.transform.position.x < transform.position.x)
            {
                particle.transform.position = new Vector3(transform.position.x + (_gridsize.x * _cellSize), particle.transform.position.y, particle.transform.position.z);
            }

            // Y component out of bounds
            if (particle.transform.position.y >= transform.position.y + (_gridsize.y * _cellSize))
            {
                particle.transform.position = new Vector3(particle.transform.position.x, transform.position.y, particle.transform.position.z);
            }

            if (particle.transform.position.y < transform.position.y)
            {
                particle.transform.position = new Vector3(particle.transform.position.x, transform.position.y + (_gridsize.y * _cellSize), particle.transform.position.z);
            }

            // Z component out of bounds
            if (particle.transform.position.z >= transform.position.z + (_gridsize.z * _cellSize))
            {
                particle.transform.position = new Vector3(particle.transform.position.x, particle.transform.position.y, transform.position.z);
            }

            if (particle.transform.position.z < transform.position.z)
            {
                particle.transform.position = new Vector3(particle.transform.position.x, particle.transform.position.y, transform.position.z + (_gridsize.z * _cellSize));
            }

            // get the area that the particle occupies within the flowfield
            Vector3Int particlePosition = new Vector3Int(
                Mathf.FloorToInt(Mathf.Clamp((particle.transform.position.x - transform.position.x) / _cellSize, 0, _gridsize.x - 1)),
                Mathf.FloorToInt(Mathf.Clamp((particle.transform.position.y - transform.position.y) / _cellSize, 0, _gridsize.y - 1)),
                Mathf.FloorToInt(Mathf.Clamp((particle.transform.position.z - transform.position.z) / _cellSize, 0, _gridsize.z - 1))
            );

            particle.ApplyRotation(_flowfieldDirection[particlePosition.x, particlePosition.y, particlePosition.z], _particleRotation);
            particle._movespeed = _particleMovespeed;
            particle.transform.localScale = new Vector3(_particleScale, _particleScale, _particleScale);
        }
    }

    private bool ParticleSpawnValidation(Vector3 position)
    {
        foreach (FlowfieldParticle particle in _particles)
        {
            if (Vector3.Distance(position, particle.transform.position) <= _spawnRadius)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position + new Vector3((_gridsize.x * _cellSize) * .5f, (_gridsize.y * _cellSize) * .5f, (_gridsize.z * _cellSize) * .5f),
                            new Vector3(_gridsize.x * _cellSize, _gridsize.y * _cellSize, _gridsize.z * _cellSize));
    }
}
