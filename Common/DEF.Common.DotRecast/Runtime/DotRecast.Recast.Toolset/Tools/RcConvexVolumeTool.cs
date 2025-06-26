﻿using System;
using System.Collections.Generic;
using DotRecast.Core;
using DotRecast.Recast.Geom;

namespace DotRecast.Recast.Toolset.Tools
{
    public class RcConvexVolumeTool : IRcToolable
    {
        public string GetName()
        {
            return "Create Convex Volumes";
        }

        public RcConvexVolume RemoveByPos(IInputGeomProvider geom, RcVec3f pos)
        {
            // Delete
            int nearestIndex = -1;
            IList<RcConvexVolume> vols = geom.ConvexVolumes();
            for (int i = 0; i < vols.Count; ++i)
            {
                if (PolyUtils.PointInPoly(vols[i].verts, pos) && pos.y >= vols[i].hmin
                                                              && pos.y <= vols[i].hmax)
                {
                    nearestIndex = i;
                }
            }

            // If end point close enough, delete it.
            if (nearestIndex == -1)
                return null;

            var removal = geom.ConvexVolumes()[nearestIndex];
            geom.ConvexVolumes().RemoveAt(nearestIndex);
            return removal;
        }

        public void Add(IInputGeomProvider geom, RcConvexVolume volume)
        {
            geom.AddConvexVolume(volume);
        }

        public static RcConvexVolume CreateConvexVolume(List<RcVec3f> pts, List<int> hull, RcAreaModification areaType, float boxDescent, float boxHeight, float polyOffset)
        {
            // 
            if (hull.Count <= 2)
            {
                return null;
            }

            // Create shape.
            float[] verts = new float[hull.Count * 3];
            for (int i = 0; i < hull.Count; ++i)
            {
                verts[i * 3] = pts[hull[i]].x;
                verts[i * 3 + 1] = pts[hull[i]].y;
                verts[i * 3 + 2] = pts[hull[i]].z;
            }

            float minh = float.MaxValue, maxh = 0;
            for (int i = 0; i < hull.Count; ++i)
            {
                minh = Math.Min(minh, verts[i * 3 + 1]);
            }

            minh -= boxDescent;
            maxh = minh + boxHeight;

            if (polyOffset > 0.01f)
            {
                float[] offset = new float[verts.Length * 2];
                int noffset = PolyUtils.OffsetPoly(verts, hull.Count, polyOffset, offset, offset.Length);
                if (noffset > 0)
                {
                    verts = RcArrayUtils.CopyOf(offset, 0, noffset * 3);
                }
            }

            return new RcConvexVolume()
            {
                verts = verts,
                hmin = minh,
                hmax = maxh,
                areaMod = areaType,
            };
        }
    }
}