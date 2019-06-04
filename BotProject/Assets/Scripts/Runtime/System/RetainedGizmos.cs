namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameUtils;
    using GameUtils.Pool;
    using GameAI.Pathfinding.Core;

    public class RetainedGizmos
    {
        #region Built-in class
        public struct Hasher
        {
            ulong hash;
            bool includePathSearchInfo;
            bool includeAreaInfo;
            IPathHandler debugHandler;

            public Hasher(NavSystem active)
            {
                hash = 0;
                debugHandler = active.debugHandler;
                //Todo: Debug Mode
                includePathSearchInfo = debugHandler != null;
                includeAreaInfo = false;
                //AddHash((int)active.debugMode);
                //AddHash(active.debugFloor.GetHashCode());
                //AddHash(active.debugRoof.GetHashCode());
                AddHash(AStarColor.ColorHash());
            }

            public void AddHash(int hash)
            {
                this.hash = (1572869UL * this.hash) ^ (ulong)hash;
            }

            public void HashNode(NavNode node)
            {
                AddHash(node.GetGizmosHashCode());

                if (includePathSearchInfo)
                {
                    var pathNode = debugHandler.GetPathnode(node);
                    AddHash(pathNode.PathID);
                    AddHash(pathNode.PathID == debugHandler.PathID ? 1 : 0);
                    AddHash(pathNode.F);
                }
            }

            public ulong Hash
            {
                get { return hash; }
            }
        }
        struct MeshWithHash
        {
            public ulong hash;
            public Mesh mesh;
            public bool lines;
        }
        public class Builder : IPoolable
        {
            List<Vector3> lines = new List<Vector3>();
            List<Color32> lineColors = new List<Color32>();
            List<Mesh> meshes = new List<Mesh>();

            public void DrawMesh(RetainedGizmos gizmos, Vector3[] vertices, List<int> triangles, Color[] colors)
            {
                var mesh = gizmos.GetMesh();
                
                mesh.vertices = vertices;
                mesh.SetTriangles(triangles, 0);
                mesh.colors = colors;
                
                mesh.UploadMeshData(true);
                meshes.Add(mesh);
            }
            public void DrawWireCube(GraphTransform tr, Bounds bounds, Color color)
            {
                var min = bounds.min;
                var max = bounds.max;

                DrawLine(tr.Transform(new Vector3(min.x, min.y, min.z)), tr.Transform(new Vector3(max.x, min.y, min.z)), color);
                DrawLine(tr.Transform(new Vector3(max.x, min.y, min.z)), tr.Transform(new Vector3(max.x, min.y, max.z)), color);
                DrawLine(tr.Transform(new Vector3(max.x, min.y, max.z)), tr.Transform(new Vector3(min.x, min.y, max.z)), color);
                DrawLine(tr.Transform(new Vector3(min.x, min.y, max.z)), tr.Transform(new Vector3(min.x, min.y, min.z)), color);

                DrawLine(tr.Transform(new Vector3(min.x, max.y, min.z)), tr.Transform(new Vector3(max.x, max.y, min.z)), color);
                DrawLine(tr.Transform(new Vector3(max.x, max.y, min.z)), tr.Transform(new Vector3(max.x, max.y, max.z)), color);
                DrawLine(tr.Transform(new Vector3(max.x, max.y, max.z)), tr.Transform(new Vector3(min.x, max.y, max.z)), color);
                DrawLine(tr.Transform(new Vector3(min.x, max.y, max.z)), tr.Transform(new Vector3(min.x, max.y, min.z)), color);

                DrawLine(tr.Transform(new Vector3(min.x, min.y, min.z)), tr.Transform(new Vector3(min.x, max.y, min.z)), color);
                DrawLine(tr.Transform(new Vector3(max.x, min.y, min.z)), tr.Transform(new Vector3(max.x, max.y, min.z)), color);
                DrawLine(tr.Transform(new Vector3(max.x, min.y, max.z)), tr.Transform(new Vector3(max.x, max.y, max.z)), color);
                DrawLine(tr.Transform(new Vector3(min.x, min.y, max.z)), tr.Transform(new Vector3(min.x, max.y, max.z)), color);
            }
            public void DrawLine(Vector3 start, Vector3 end, Color color)
            {
                lines.Add(start);
                lines.Add(end);
                var col32 = (Color32)color;
                lineColors.Add(col32);
                lineColors.Add(col32);
            }

            public void Submit(RetainedGizmos gizmos, Hasher hasher)
            {
                SubmitLines(gizmos, hasher.Hash);
                SubmitMeshes(gizmos, hasher.Hash);
            }
            private void SubmitMeshes(RetainedGizmos gizmos, ulong hash)
            {
                for (int i = 0; i < meshes.Count; i++)
                {
                    gizmos.meshes.Add(new MeshWithHash { hash = hash, mesh = meshes[i], lines = false });
                    gizmos.existingHashes.Add(hash);
                }
            }
            private void SubmitLines(RetainedGizmos gizmos, ulong hash)
            {
                const int MaxLineEndPointsPerBatch = 65532 / 2;
                int batches = (lines.Count + MaxLineEndPointsPerBatch - 1) / MaxLineEndPointsPerBatch;

                for (int batch = 0; batch < batches; batch++)
                {
                    int startIndex = MaxLineEndPointsPerBatch * batch;
                    int endIndex = Mathf.Min(startIndex + MaxLineEndPointsPerBatch, lines.Count);
                    int lineEndPointCount = endIndex - startIndex;
                    UnityEngine.Assertions.Assert.IsTrue(lineEndPointCount % 2 == 0);
                    
                    var vertices = ListPool<Vector3>.Claim(lineEndPointCount * 2);
                    var colors = ListPool<Color32>.Claim(lineEndPointCount * 2);
                    var normals = ListPool<Vector3>.Claim(lineEndPointCount * 2);
                    var uv = ListPool<Vector2>.Claim(lineEndPointCount * 2);
                    var tris = ListPool<int>.Claim(lineEndPointCount * 3);

                    for (int j = startIndex; j < endIndex; j++)
                    {
                        var vertex = (Vector3)lines[j];
                        vertices.Add(vertex);
                        vertices.Add(vertex);

                        var color = (Color32)lineColors[j];
                        colors.Add(color);
                        colors.Add(color);
                        uv.Add(new Vector2(0, 0));
                        uv.Add(new Vector2(1, 0));
                    }
                    
                    for (int j = startIndex; j < endIndex; j += 2)
                    {
                        var lineDir = (Vector3)(lines[j + 1] - lines[j]);

                        normals.Add(lineDir);
                        normals.Add(lineDir);
                        normals.Add(lineDir);
                        normals.Add(lineDir);
                    }
                    
                    for (int j = 0, v = 0; j < lineEndPointCount * 3; j += 6, v += 4)
                    {
                        // First triangle
                        tris.Add(v + 0);
                        tris.Add(v + 1);
                        tris.Add(v + 2);

                        // Second triangle
                        tris.Add(v + 1);
                        tris.Add(v + 3);
                        tris.Add(v + 2);
                    }

                    var mesh = gizmos.GetMesh();
                    
                    mesh.SetVertices(vertices);
                    mesh.SetTriangles(tris, 0);
                    mesh.SetColors(colors);
                    mesh.SetNormals(normals);
                    mesh.SetUVs(0, uv);
                    
                    mesh.UploadMeshData(true);
                    
                    ListPool<Vector3>.Release(ref vertices);
                    ListPool<Color32>.Release(ref colors);
                    ListPool<Vector3>.Release(ref normals);
                    ListPool<Vector2>.Release(ref uv);
                    ListPool<int>.Release(ref tris);

                    gizmos.meshes.Add(new MeshWithHash { hash = hash, mesh = mesh, lines = true });
                    gizmos.existingHashes.Add(hash);
                }
            }

            public void Recycle()
            {
                lines.Clear();
                lineColors.Clear();
                meshes.Clear();
            }
        }
        #endregion

        List<MeshWithHash> meshes = new List<MeshWithHash>();
        HashSet<ulong> usedHashes = new HashSet<ulong>();
        HashSet<ulong> existingHashes = new HashSet<ulong>();
        Stack<Mesh> cachedMeshes = new Stack<Mesh>();

        public GraphGizmoHelper GetSingleFrameGizmoHelper(NavSystem active)
        {
            var uniqHash = new RetainedGizmos.Hasher();

            uniqHash.AddHash(Time.realtimeSinceStartup.GetHashCode());
            Draw(uniqHash);
            return GetGizmoHelper(active, uniqHash);
        }

        public GraphGizmoHelper GetGizmoHelper(NavSystem active, Hasher hasher)
        {
            var helper = Pool<GraphGizmoHelper>.Allocate();

            helper.Init(active, hasher, this);
            return helper;
        }

        private void PoolMesh(Mesh mesh)
        {
            mesh.Clear();
            cachedMeshes.Push(mesh);
        }
        private Mesh GetMesh()
        {
            if (cachedMeshes.Count > 0)
            {
                return cachedMeshes.Pop();
            }
            else
            {
                return new Mesh
                {
                    hideFlags = HideFlags.DontSave
                };
            }
        }

        public Material surfaceMaterial;
        public Material lineMaterial;

        public bool HasCachedMesh(Hasher hasher)
        {
            return existingHashes.Contains(hasher.Hash);
        }
        public bool Draw(Hasher hasher)
        {
            usedHashes.Add(hasher.Hash);
            return HasCachedMesh(hasher);
        }

        public void DrawExisting()
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                usedHashes.Add(meshes[i].hash);
            }
        }
        public void FinalizeDraw()
        {
            RemoveUnusedMeshes(meshes);

            var cam = Camera.current;
            var planes = GeometryUtility.CalculateFrustumPlanes(cam);

            // Silently do nothing if the materials are not set
            if (surfaceMaterial == null || lineMaterial == null) return;
            
            // First surfaces, then lines
            for (int matIndex = 0; matIndex <= 1; matIndex++)
            {
                var mat = matIndex == 0 ? surfaceMaterial : lineMaterial;
                for (int pass = 0; pass < mat.passCount; pass++)
                {
                    mat.SetPass(pass);
                    for (int i = 0; i < meshes.Count; i++)
                    {
                        if (meshes[i].lines == (mat == lineMaterial) && GeometryUtility.TestPlanesAABB(planes, meshes[i].mesh.bounds))
                        {
                            Graphics.DrawMeshNow(meshes[i].mesh, Matrix4x4.identity);
                        }
                    }
                }
            }

            usedHashes.Clear();
        }

        public void ClearCache()
        {
            usedHashes.Clear();
            RemoveUnusedMeshes(meshes);

            while (cachedMeshes.Count > 0)
            {
                Mesh.DestroyImmediate(cachedMeshes.Pop());
            }

            UnityEngine.Assertions.Assert.IsTrue(meshes.Count == 0);
        }
        private void RemoveUnusedMeshes(List<MeshWithHash> meshList)
        {
            for (int i = 0, j = 0; i < meshList.Count;)
            {
                if (j == meshList.Count)
                {
                    j--;
                    meshList.RemoveAt(j);
                }
                else if (usedHashes.Contains(meshList[j].hash))
                {
                    meshList[i] = meshList[j];
                    i++;
                    j++;
                }
                else
                {
                    PoolMesh(meshList[j].mesh);
                    existingHashes.Remove(meshList[j].hash);
                    j++;
                }
            }
        }
    }
}