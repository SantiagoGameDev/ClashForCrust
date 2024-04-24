using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class PlayerTrail : MonoBehaviour
{
    [SerializeField] private BasicAttackManager BasicAttackManager;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Material mat;
    private bool trailActive;
    private bool dashing;

    [SerializeField] private float activeTime = 0.5f;
    [SerializeField] private float destroyDelay = 0.1f;
    [SerializeField] private float spawnRate = 0.1f;
    private SkinnedMeshRenderer trailRenderer;
    

    void Update()
    {
        dashing = BasicAttackManager.isDashing;
            if (dashing)
            {
                trailActive = true;
                StartCoroutine(doTrail(activeTime));
            }
    }

    IEnumerator doTrail(float time)
    {
        while(time > 0) 
        {
            time -= spawnRate;

            if (trailRenderer == null)
                trailRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

            GameObject gObj = new GameObject();
            gObj.transform.SetPositionAndRotation(spawnPos.position, spawnPos.rotation);

            MeshFilter mf = gObj.AddComponent<MeshFilter>();
            MeshRenderer mr = gObj.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();

            trailRenderer.BakeMesh(mesh);

            mf.mesh = mesh;
            mr.material = mat;

            yield return new WaitForSeconds(spawnRate);

            Destroy(gObj, destroyDelay);

        }
        trailActive = false;  
    }
}
