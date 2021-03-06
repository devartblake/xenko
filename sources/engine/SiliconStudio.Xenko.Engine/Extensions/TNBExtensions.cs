﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Rendering;
using SiliconStudio.Xenko.Graphics.Data;

namespace SiliconStudio.Xenko.Extensions
{
    public static class TNBExtensions
    {
        /// <summary>
        /// Generates the tangents and binormals for this mesh data.
        /// Tangents and bitangents will be encoded as float4:
        /// float3 for tangent and an additional float for handedness (1 or -1),
        /// so that bitangent can be reconstructed.
        /// More info at http://www.terathon.com/code/tangent.html
        /// </summary>
        /// <param name="meshData">The mesh data.</param>
        public static unsafe void GenerateTangentBinormal(this MeshDraw meshData)
        {
            if (!meshData.IsSimple())
                throw new ArgumentException("meshData is not simple.");

            if (meshData.PrimitiveType != PrimitiveType.TriangleList
                && meshData.PrimitiveType != PrimitiveType.TriangleListWithAdjacency)
                throw new NotImplementedException();

            var oldVertexBufferBinding = meshData.VertexBuffers[0];
            var indexBufferBinding = meshData.IndexBuffer;
            var indexData = indexBufferBinding != null ? indexBufferBinding.Buffer.GetSerializationData().Content : null;

            var oldVertexStride = oldVertexBufferBinding.Declaration.VertexStride;
            var bufferData = oldVertexBufferBinding.Buffer.GetSerializationData().Content;

            fixed (byte* indexBufferStart = indexData)
            fixed (byte* oldBuffer = bufferData)
            {
                var result = VertexHelper.GenerateTangentBinormal(oldVertexBufferBinding.Declaration, (IntPtr)oldBuffer, oldVertexBufferBinding.Count, oldVertexBufferBinding.Offset, oldVertexBufferBinding.Stride, (IntPtr)indexBufferStart, indexBufferBinding != null && indexBufferBinding.Is32Bit, indexBufferBinding != null ? indexBufferBinding.Count : 0);

                // Replace new vertex buffer binding
                meshData.VertexBuffers[0] = new VertexBufferBinding(new BufferData(BufferFlags.VertexBuffer, result.VertexBuffer).ToSerializableVersion(), result.Layout, oldVertexBufferBinding.Count);
            }
        }
    }
}