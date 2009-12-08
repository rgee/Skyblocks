using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace InstancedModelPipeline
{
    [ContentSerializerRuntimeType("Skyblocks.InstancedModel, Skyblocks")]
    public class InstancedModelContent
    {
        [ContentSerializer]
        List<ModelPart> modelParts = new List<ModelPart>();

        [ContentSerializerRuntimeType("Skyblocks.InstancedModelPart, Skyblocks")]
        class ModelPart
        {
            public int IndexCount;
            public int VertexCount;
            public int VertexStride;
            public VertexElement[] VertexElements;
            public VertexBufferContent VertexBufferContent;
            public IndexCollection IndexCollection;

            [ContentSerializer(SharedResource = true)]
            public MaterialContent MaterialContent;
        }

        /// <summary>
        /// Add new modelpart to the model
        /// </summary>
        public void AddModelPart(int indexCount, int vertexCount, int vertexStride, VertexElement[] vertexElements,
                                 VertexBufferContent vertexBufferContent, IndexCollection indexCollection,
                                 MaterialContent materialContent)
        {
            ModelPart modelPart = new ModelPart();

            modelPart.IndexCount = indexCount;
            modelPart.VertexCount = vertexCount;
            modelPart.VertexStride = vertexStride;
            modelPart.VertexElements = vertexElements;
            modelPart.VertexBufferContent = vertexBufferContent;
            modelPart.IndexCollection = indexCollection;
            modelPart.MaterialContent = materialContent;

            modelParts.Add(modelPart);
        }
    }
}
