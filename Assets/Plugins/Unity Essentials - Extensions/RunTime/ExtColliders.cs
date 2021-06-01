using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEssentials.Extensions
{
    public static class ExtColliders
    {
        /// <summary>
        /// from a unit spriteRenderer, fit the collider2d to the sprite
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="collider"></param>
        public static void AutoSizeCollider2d(SpriteRenderer spriteRenderer, Collider2D collider)
        {
            System.Type typeCollider = collider.GetType();

            if (typeCollider == typeof(BoxCollider2D))
            {
                BoxCollider2D box = (BoxCollider2D)collider;
                FitBoxCollider2D(spriteRenderer, box);
            }
            else if (typeCollider == typeof(CapsuleCollider2D))
            {
                CapsuleCollider2D capsule = (CapsuleCollider2D)collider;
                FitCapsuleCollider2D(spriteRenderer, capsule);
            }
        }

        /// <summary>
        /// from a unit MeshFilter, and a collider, fit the collider to the bound of the mesh
        /// </summary>
        /// <param name="meshFilter"></param>
        /// <param name="collider"></param>
        public static void AutoSizeCollider3d(MeshFilter meshFilter, Collider collider)
        {
            AutoSizeCollider3d(meshFilter.sharedMesh.bounds, collider);
        }

        /// <summary>
        /// from a given bound, and a collider, fit the collider to the bound
        /// </summary>
        /// <param name="boundMeshRenderer"></param>
        /// <param name="collider"></param>
        public static void AutoSizeCollider3d(Bounds boundMeshRenderer, Collider collider)
        {
            System.Type typeCollider = collider.GetType();

            if (typeCollider == typeof(BoxCollider))
            {
                BoxCollider box = (BoxCollider)collider;
                FitBoxCollider(boundMeshRenderer, box);
            }
            else if (typeCollider == typeof(SphereCollider))
            {
                SphereCollider sphere = (SphereCollider)collider;
                FitSphereCollider(boundMeshRenderer, sphere);
            }
            else if (typeCollider == typeof(MeshCollider))
            {
                MeshCollider meshCollider = (MeshCollider)collider;
                MeshFilter meshFilter = meshCollider.gameObject.GetComponent<MeshFilter>();
                FitMeshCollider(meshFilter, meshCollider);
            }
            else if (typeCollider == typeof(CapsuleCollider))
            {
                CapsuleCollider capsule = (CapsuleCollider)collider;
                FitCapsuleCollider(boundMeshRenderer, capsule);
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// From a given GameObject and a Collider, Fit this collider to the
        /// gameObject mesh with all his children
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="collider"></param>
        public static void AutoSizeColliders3d(GameObject gameObject, Collider collider)
        {
            MeshFilter[] allChilds = gameObject.GetComponentsInChildren<MeshFilter>();
            if (allChilds.Length < 1)
            {
                return;
            }
            Bounds bigBounds = allChilds[0].sharedMesh.bounds;

            List<Vector3> positionChilds = new List<Vector3>();
            for (int i = 1; i < allChilds.Length; i++)
            {
                positionChilds.Add(allChilds[i].transform.position);
                bigBounds.Encapsulate(allChilds[i].sharedMesh.bounds);
            }
            AutoSizeCollider3d(bigBounds, collider);
        }


        /// <summary>
        /// from a spriteRenderer, fit a capsule collider on it
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="capsule"></param>
        public static void FitCapsuleCollider2D(SpriteRenderer spriteRenderer, CapsuleCollider2D capsule)
        {
            Vector2 sizeBounds = spriteRenderer.sprite.bounds.size;

            capsule.size = sizeBounds;

            Vector2 localScale = spriteRenderer.transform.localScale;
            if (Mathf.Abs(localScale.x) > Mathf.Abs(localScale.y))
            {
                capsule.direction = CapsuleDirection2D.Horizontal;
            }
            else
            {
                capsule.direction = CapsuleDirection2D.Vertical;
            }
        }

        /// <summary>
        /// from a spriteRenderer, fit a BoxCollider on it
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="box"></param>
        public static void FitBoxCollider2D(SpriteRenderer spriteRenderer, BoxCollider2D box)
        {
            if (spriteRenderer.drawMode == SpriteDrawMode.Simple)
            {
                Vector2 sizeBounds = spriteRenderer.sprite.bounds.size;
                box.size = sizeBounds;
                box.offset = Vector2.zero;
            }
            else
            {
                box.offset = Vector2.zero;
                box.size = spriteRenderer.size;
            }
        }

        /// <summary>
        /// from a meshRenderer, fit a BoxCollider on it
        /// </summary>
        /// <param name="meshRenderer"></param>
        /// <param name="box"></param>
        public static void FitBoxCollider(Bounds boundMeshRenderer, BoxCollider box)
        {
            box.size = boundMeshRenderer.size;
            box.center = boundMeshRenderer.center;
        }

        /// <summary>
        /// from a meshRenderer, fit a SphereCollider on it
        /// </summary>
        /// <param name="meshRenderer"></param>
        /// <param name="sphere"></param>
        public static void FitSphereCollider(Bounds boundMeshRenderer, SphereCollider sphere)
        {
            sphere.radius = Maximum(boundMeshRenderer.size) / 2f;
            sphere.center = boundMeshRenderer.center;
        }
        private static float Maximum(this Vector3 vector)
        {
            return Max(vector.x, vector.y, vector.z);
        }
        private static float Max(float value1, float value2, float value3)
        {
            float max = (value1 > value2) ? value1 : value2;
            return (max > value3) ? max : value3;
        }

        /// <summary>
        /// from a meshRenderer, fit a CapsuleCollider on it
        /// </summary>
        /// <param name="meshRenderer"></param>
        /// <param name="capsule"></param>
        public static void FitCapsuleCollider(Bounds boundMeshRenderer, CapsuleCollider capsule)
        {
            if (boundMeshRenderer.size.x > boundMeshRenderer.size.y && boundMeshRenderer.size.x > boundMeshRenderer.size.z)
            {
                capsule.direction = 0;
            }
            else if (boundMeshRenderer.size.z > boundMeshRenderer.size.x && boundMeshRenderer.size.z > boundMeshRenderer.size.y)
            {
                capsule.direction = 2;
            }
            else
            {
                capsule.direction = 1;
            }
            capsule.center = boundMeshRenderer.center;

            capsule.radius = boundMeshRenderer.size.x / 2;
            capsule.height = boundMeshRenderer.size.y;
        }


        /// <summary>
        /// from a meshRenderer, fit a MeshCollider on it
        /// </summary>
        /// <param name="meshRenderer"></param>
        /// <param name="sphere"></param>
        public static void FitMeshCollider(MeshFilter meshFilter, MeshCollider meshCollider)
        {
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
    }
}