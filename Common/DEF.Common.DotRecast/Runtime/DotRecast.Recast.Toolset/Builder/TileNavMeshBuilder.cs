/*
recast4j copyright (c) 2015-2019 Piotr Piastucki piotr@jtilia.org

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
 claim that you wrote the original software. If you use this software
 in a product, an acknowledgment in the product documentation would be
 appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
 misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotRecast.Detour;
using DotRecast.Recast.Geom;
using DotRecast.Recast.Toolset.Geom;

namespace DotRecast.Recast.Toolset.Builder
{
    public class TileNavMeshBuilder
    {
        public TileNavMeshBuilder()
        {
        }

        public NavMeshBuildResult Build(IInputGeomProvider geom, RcNavMeshBuildSettings settings)
        {
            return Build(geom,
                settings.tileSize,
                RcPartitionType.OfValue(settings.partitioning),
                settings.cellSize, settings.cellHeight,
                settings.agentMaxSlope, settings.agentHeight, settings.agentRadius, settings.agentMaxClimb,
                settings.minRegionSize, settings.mergedRegionSize,
                settings.edgeMaxLen, settings.edgeMaxError,
                settings.vertsPerPoly, settings.detailSampleDist, settings.detailSampleMaxError,
                settings.filterLowHangingObstacles, settings.filterLedgeSpans, settings.filterWalkableLowHeightSpans);
        }

        public NavMeshBuildResult Build(IInputGeomProvider geom,
            int tileSize,
            RcPartition partitionType,
            float cellSize, float cellHeight,
            float agentMaxSlope, float agentHeight, float agentRadius, float agentMaxClimb,
            int regionMinSize, int regionMergeSize,
            float edgeMaxLen, float edgeMaxError,
            int vertsPerPoly,
            float detailSampleDist, float detailSampleMaxError,
            bool filterLowHangingObstacles, bool filterLedgeSpans, bool filterWalkableLowHeightSpans)
        {
            List<RecastBuilderResult> results = BuildRecastResult(
                geom,
                tileSize,
                partitionType,
                cellSize, cellHeight,
                agentMaxSlope, agentHeight, agentRadius, agentMaxClimb,
                regionMinSize, regionMergeSize,
                edgeMaxLen, edgeMaxError,
                vertsPerPoly,
                detailSampleDist, detailSampleMaxError,
                filterLowHangingObstacles, filterLedgeSpans, filterWalkableLowHeightSpans
            );

            var tileMeshData = BuildMeshData(geom, cellSize, cellHeight, agentHeight, agentRadius, agentMaxClimb, results);
            var tileNavMesh = BuildNavMesh(geom, tileMeshData, cellSize, tileSize, vertsPerPoly);
            return new NavMeshBuildResult(results, tileNavMesh);
        }

        public List<RecastBuilderResult> BuildRecastResult(IInputGeomProvider geom,
            int tileSize,
            RcPartition partitionType,
            float cellSize, float cellHeight,
            float agentMaxSlope, float agentHeight, float agentRadius, float agentMaxClimb,
            int regionMinSize, int regionMergeSize,
            float edgeMaxLen, float edgeMaxError,
            int vertsPerPoly,
            float detailSampleDist, float detailSampleMaxError,
            bool filterLowHangingObstacles, bool filterLedgeSpans, bool filterWalkableLowHeightSpans)

        {
            RcConfig cfg = new RcConfig(true, tileSize, tileSize,
                RcConfig.CalcBorder(agentRadius, cellSize),
                partitionType,
                cellSize, cellHeight,
                agentMaxSlope, agentHeight, agentRadius, agentMaxClimb,
                regionMinSize * regionMinSize * cellSize * cellSize, regionMergeSize * regionMergeSize * cellSize * cellSize,
                edgeMaxLen, edgeMaxError,
                vertsPerPoly,
                detailSampleDist, detailSampleMaxError,
                filterLowHangingObstacles, filterLedgeSpans, filterWalkableLowHeightSpans,
                SampleAreaModifications.SAMPLE_AREAMOD_WALKABLE, true);
            RecastBuilder rcBuilder = new RecastBuilder();
            return rcBuilder.BuildTiles(geom, cfg, Task.Factory);
        }

        public DtNavMesh BuildNavMesh(IInputGeomProvider geom, List<DtMeshData> meshData, float cellSize, int tileSize, int vertsPerPoly)
        {
            DtNavMeshParams navMeshParams = new DtNavMeshParams();
            navMeshParams.orig = geom.GetMeshBoundsMin();
            navMeshParams.tileWidth = tileSize * cellSize;
            navMeshParams.tileHeight = tileSize * cellSize;

            // Snprintf(text, 64, "Tiles %d x %d", tw, th);

            navMeshParams.maxTiles = GetMaxTiles(geom, cellSize, tileSize);
            navMeshParams.maxPolys = GetMaxPolysPerTile(geom, cellSize, tileSize);
            DtNavMesh navMesh = new DtNavMesh(navMeshParams, vertsPerPoly);
            meshData.ForEach(md => navMesh.AddTile(md, 0, 0));
            return navMesh;
        }

        public List<DtMeshData> BuildMeshData(IInputGeomProvider geom, float cellSize, float cellHeight, float agentHeight,
            float agentRadius, float agentMaxClimb, IList<RecastBuilderResult> results)
        {
            // Add tiles to nav mesh
            List<DtMeshData> meshData = new List<DtMeshData>();
            foreach (RecastBuilderResult result in results)
            {
                int x = result.tileX;
                int z = result.tileZ;
                DtNavMeshCreateParams option = DemoNavMeshBuilder
                    .GetNavMeshCreateParams(geom, cellSize, cellHeight, agentHeight, agentRadius, agentMaxClimb, result);

                option.tileX = x;
                option.tileZ = z;
                DtMeshData md = DtNavMeshBuilder.CreateNavMeshData(option);
                if (md != null)
                {
                    meshData.Add(DemoNavMeshBuilder.UpdateAreaAndFlags(md));
                }
            }

            return meshData;
        }


        public int GetMaxTiles(IInputGeomProvider geom, float cellSize, int tileSize)
        {
            int tileBits = GetTileBits(geom, cellSize, tileSize);
            return 1 << tileBits;
        }

        public int GetMaxPolysPerTile(IInputGeomProvider geom, float cellSize, int tileSize)
        {
            int polyBits = 22 - GetTileBits(geom, cellSize, tileSize);
            return 1 << polyBits;
        }

        private int GetTileBits(IInputGeomProvider geom, float cellSize, int tileSize)
        {
            RcUtils.CalcGridSize(geom.GetMeshBoundsMin(), geom.GetMeshBoundsMax(), cellSize, out var gw, out var gh);
            int tw = (gw + tileSize - 1) / tileSize;
            int th = (gh + tileSize - 1) / tileSize;
            int tileBits = Math.Min(DtUtils.Ilog2(DtUtils.NextPow2(tw * th)), 14);
            return tileBits;
        }

        public int[] GetTiles(DemoInputGeomProvider geom, float cellSize, int tileSize)
        {
            RcUtils.CalcGridSize(geom.GetMeshBoundsMin(), geom.GetMeshBoundsMax(), cellSize, out var gw, out var gh);
            int tw = (gw + tileSize - 1) / tileSize;
            int th = (gh + tileSize - 1) / tileSize;
            return new int[] { tw, th };
        }
    }
}