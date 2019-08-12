using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkFPCScript : MonoBehaviour {

    float lastSyncTime = 0f;
    float syncDelay = 0f;
    float syncTime = 0f;
    Vector3 startPosition = Vector3.zero;
    Vector3 endPosition = Vector3.zero;
    Vector3 syncStartPosition = Vector3.zero;
    Vector3 syncEndPosition = Vector3.zero;

    Rigidbody rigidbody;

	// Use this for initialization
	void Start ()
    {
        NetworkView nView = GetComponent<NetworkView>();
        
        if (nView.isMine)
        {
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour m in components)
            {
                m.enabled = true;
            }
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);
            }
        }
        rigidbody = gameObject.GetComponent<Rigidbody>();
        
	}

    void Update()
    {
        if (!this.GetComponent<NetworkView>().isMine)
        {
            syncTime += Time.deltaTime;
            if (syncTime < syncDelay)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, syncTime / syncDelay);
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 v;
                VoxelChunk vcs;
                if (PickThisBlock(out v, out vcs, 4))
                {
                    NetworkView nv = vcs.GetComponent<NetworkView>();
                    if (nv != null)
                    {
                        nv.RPC("SetBlockOnline", RPCMode.All, v, 0);
                    }
                }
            }

            if (Input.GetButtonDown("Fire2"))
            {
                Vector3 v;
                VoxelChunk vcs;
                if (PickThisBlock(out v, out vcs, 4))
                {
                    NetworkView nv = vcs.GetComponent<NetworkView>();
                    if (nv != null)
                    {
                        nv.RPC("SetBlockOnline", RPCMode.All, v, 2);
                    }
                }
            }
        }
    }


    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;

        if (stream.isWriting)
        {
            //syncPosition = transform.position;
            //stream.Serialize(ref syncPosition);

            syncPosition = rigidbody.position;
            stream.Serialize(ref syncPosition);
            syncVelocity = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);
        }

        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            syncTime = 0f;
            syncDelay = Time.time - lastSyncTime;
            lastSyncTime = Time.time;

            // last predicted position         
            syncStartPosition = rigidbody.position;
            // predicted position         
            syncEndPosition = syncPosition + syncVelocity * syncDelay;
        }
    }

    bool PickThisBlock(out Vector3 v, out VoxelChunk voxelChunkScript, float dist)
    {
        v = new Vector3();
        voxelChunkScript = null;

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dist))
        {
            voxelChunkScript = hit.collider.gameObject.GetComponent<VoxelChunk>();
            if (voxelChunkScript != null)
            {
                v = hit.point - hit.normal / 2;
                v.x = Mathf.Floor(v.x);
                v.y = Mathf.Floor(v.y);
                v.z = Mathf.Floor(v.z);
                return true;
            }
        }
        return false;
    }
}
