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
using DotRecast.Core;
using DotRecast.Detour;
using DotRecast.Recast.Toolset.Geom;

namespace DotRecast.Recast.Toolset.Builder
{
    public class SoloNavMeshBuilder
    {
        public NavMeshBuildResult Build(DemoInputGeomProvider geom, RcNavMeshBuildSettings settings)
        {
            return Build(geom,
                RcPartitionType.OfValue(settings.partitioning),
                settings.cellSize, settings.cellHeight,
                settings.agentMaxSlope, settings.agentHeight, settings.agentRadius, settings.agentMaxClimb,
                settings.minRegionSize, settings.mergedRegionSize,
                settings.edgeMaxLen, settings.edgeMaxError,
                settings.vertsPerPoly,
                settings.detailSampleDist, settings.detailSampleMaxError,
                settings.filterLowHangingObstacles, settings.filterLedgeSpans, settings.filterWalkableLowHeightSpans);
        }

        public NavMeshBuildResult Build(DemoInputGeomProvider geom,
            RcPartition partitionType,
            float cellSize, float cellHeight,
            float agentMaxSlope, float agentHeight, float agentRadius, float agentMaxClimb,
            int regionMinSize, int regionMergeSize,
            float edgeMaxLen, float edgeMaxError,
            int vertsPerPoly,
            float detailSampleDist, float detailSampleMaxError,
            bool filterLowHangingObstacles, bool filterLedgeSpans, bool filterWalkableLowHeightSpans)
        {
            RecastBuilderResult rcResult = BuildRecastResult(
                geom,
                partitionType,
                cellSize, cellHeight,
                agentMaxSlope, agentHeight, agentRadius, agentMaxClimb,
                regionMinSize, regionMergeSize,
                edgeMaxLen, edgeMaxError,
                vertsPerPoly,
                detailSampleDist, detailSampleMaxError,
                filterLowHangingObstacles, filterLedgeSpans, filterWalkableLowHeightSpans);

            var meshData = BuildMeshData(geom, cellSize, cellHeight, agentHeight, agentRadius, agentMaxClimb, rcResult);
            if (null == meshData)
            {
                return new NavMeshBuildResult();
            }

            var navMesh = BuildNavMesh(meshData, vertsPerPoly);
            return new NavMeshBuildResult(RcImmutableArray.Create(rcResult), navMesh);
        }

        private DtNavMesh BuildNavMesh(DtMeshData meshData, int vertsPerPoly)
        {
            return new DtNavMesh(meshData, vertsPerPoly, 0);
        }

        private RecastBuilderResult BuildRecastResult(DemoInputGeomProvider geom,
            RcPartition partitionType,
            float cellSize, float cellHeight,
            float agentMaxSlope, float agentHeight, float agentRadius, float agentMaxClimb,
            int regionMinSize, int regionMergeSize,
            float edgeMaxLen, float edgeMaxError,
            int vertsPerPoly,
            float detailSampleDist, float detailSampleMaxError,
            bool filterLowHangingObstacles, bool filterLedgeSpans, bool filterWalkableLowHeightSpans)
        {
            RcConfig cfg = new RcConfig(
                partitionType,
                cellSize, cellHeight,
                agentMaxSlope, agentHeight, agentRadius, agentMaxClimb,
                regionMinSize, regionMergeSize,
                edgeMaxLen, edgeMaxError,
                vertsPerPoly,
                detailSampleDist, detailSampleMaxError,
                filterLowHangingObstacles, filterLedgeSpans, filterWalkableLowHeightSpans,
                SampleAreaModifications.SAMPLE_AREAMOD_WALKABLE, true);
            RecastBuilderConfig bcfg = new RecastBuilderConfig(cfg, geom.GetMeshBoundsMin(), geom.GetMeshBoundsMax());
            RecastBuilder rcBuilder = new RecastBuilder();
            return rcBuilder.Build(geom, bcfg);
        }

        public DtMeshData BuildMeshData(DemoInputGeomProvider geom,
            float cellSize, float cellHeight,
            float agentHeight, float agentRadius, float agentMaxClimb,
            RecastBuilderResult result)
        {
            DtNavMeshCreateParams option = DemoNavMeshBuilder
                .GetNavMeshCreateParams(geom, cellSize, cellHeight, agentHeight, agentRadius, agentMaxClimb, result);
            var meshData = DtNavMeshBuilder.CreateNavMeshData(option);
            if (null == meshData)
            {
                return null;
            }

            return DemoNavMeshBuilder.UpdateAreaAndFlags(meshData);
        }
    }
}