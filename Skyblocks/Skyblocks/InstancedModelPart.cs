using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Skyblocks
{
    /// <summary>
    /// Internal worker class to support InstancedModel.
    /// </summary>
    class InstancedModelPart
    {

        // Must match constant at top of InstancedModel.fx
        const int MaxShaderMatrices = 60;
        const int SizeOfVector4 = sizeof(float) * 4;
        const int SizeOfMatrix = sizeof(float) * 16;

        // Model data.
        [ContentSerializer]
        int indexCount;

        [ContentSerializer]
        int vertexCount;

        [ContentSerializer]
        int vertexStride;

        [ContentSerializer]
        VertexDeclaration vertexDeclaration;

        [ContentSerializer]
        VertexBuffer vertexBuffer;

        [ContentSerializer]
        IndexBuffer indexBuffer;

        [ContentSerializer(SharedResource = true)]
        Effect effect;

        bool techniqueChanged;

        GraphicsDevice device;

        // Number of instances we can draw in a single batch
        int maxInstances;

        // Array of temporary matrices for the Shader Instancing technique
        Matrix[] tempMatrices = new Matrix[MaxShaderMatrices];

        // Secondary vertex data stream for HW instancing
        DynamicVertexBuffer instanceDataStream;

        // Store a copy of the original unmodified vertex decl to update as required.
        VertexElement[] originalVertexDeclaration;

        // Flag to epxand vertex and index buffers to include replicated
        // copies of the model data if using Shader Instancing.
        bool vertexDataReplicated;

        // Private constructor for the XNB deserializer
        private InstancedModelPart()
        {
        }

        internal void Initialize(GraphicsDevice device)
        {
            this.device = device;

            // How many shader instances can we fit in one batch?
            int indexOverflowLimit = ushort.MaxValue / vertexCount;

            maxInstances = Math.Min(indexOverflowLimit, MaxShaderMatrices);

            originalVertexDeclaration = vertexDeclaration.GetVertexElements();
        }

        internal void SetInstancingTechnique(InstancingTechnique technique)
        {
            switch (technique)
            {
                case InstancingTechnique.ShaderInstancing:
                    InitializeShaderInstancing();
                    break;

                case InstancingTechnique.HardwareInstancing:
                    InitializeHardwareInstancing();
                    break;
            }
            techniqueChanged = true;
        }


        /// <summary>
        /// Initialize geometry to use shader instancing
        /// </summary>
        void InitializeShaderInstancing()
        {
            if (!vertexDataReplicated)
            {
                ReplicateVertexData();
                ReplicateIndexData();

                vertexDataReplicated = true;
            }

            // Shader instancing specifies the index of the replicated vertex
            // using a float in tex coord channel 1. So add that to the vertex
            // declaration
            int instanceIndexOffset = vertexStride - sizeof(float);
            byte usageIndex = 1;
            short stream = 0;

            VertexElement[] extraElements =
            {
                new VertexElement(stream, (short)instanceIndexOffset,
                                    VertexElementFormat.Single,
                                    VertexElementMethod.Default,
                                    VertexElementUsage.TextureCoordinate, usageIndex)
            };

            ExtendVertexDeclaration(extraElements);

        }

        /// <summary>
        /// Initialize geometry to use hardware instancing
        /// </summary>
        void InitializeHardwareInstancing()
        {
            // Hardware instancing uses tex coord channels 1 to 4 to 
            // provide instance transform matrcies
            VertexElement[] extraElements = new VertexElement[4];

            short offset = 0;
            byte usageIndex = 1;
            short stream = 1;

            for (int i = 0; i < extraElements.Length; i++)
            {
                extraElements[i] = new VertexElement(stream, offset,
                                VertexElementFormat.Vector4,
                                VertexElementMethod.Default,
                                VertexElementUsage.TextureCoordinate,
                                usageIndex);

                offset += SizeOfVector4;
                usageIndex++;
            }

            ExtendVertexDeclaration(extraElements);
        }

        /// <summary>
        /// Modifies vertex declaration to include additional channels
        /// </summary>
        /// <param name="extraElements"></param>
        void ExtendVertexDeclaration(VertexElement[] extraElements)
        {
            vertexDeclaration.Dispose();

            int length = originalVertexDeclaration.Length + extraElements.Length;

            VertexElement[] elements = new VertexElement[length];

            originalVertexDeclaration.CopyTo(elements, 0);

            extraElements.CopyTo(elements, originalVertexDeclaration.Length);

            // Create 
            vertexDeclaration = new VertexDeclaration(device, elements);
        }


        /// <summary>
        /// Replicates vertex buffer several times, adding an additional index
        /// channel to indicate which instance each vertex belongs to. (Shader instancing)
        /// </summary>
        void ReplicateVertexData()
        {
            // Read existing vertex data
            byte[] oldVertexData = new byte[vertexCount * vertexStride];

            vertexBuffer.GetData(oldVertexData);
            vertexBuffer.Dispose();

            int oldVertexStride = vertexStride;

            vertexStride += sizeof(float);

            // Temporary array for replicated vertex data
            byte[] newVertexData = new byte[vertexCount * vertexStride * maxInstances];

            int outputPosition = 0;

            // Replicate
            for (int instanceIndex = 0; instanceIndex < maxInstances; instanceIndex++)
            {
                int sourcePosition = 0;

                // convert instance index from float to bits
                byte[] instanceIndexBits = BitConverter.GetBytes((float)instanceIndex);

                for (int i = 0; i < vertexCount; i++)
                {
                    // Copy existing data for this vertex
                    Array.Copy(oldVertexData, sourcePosition,
                                newVertexData, outputPosition, oldVertexStride);
                    outputPosition += oldVertexStride;
                    sourcePosition += oldVertexStride;

                    // set value of new index channel
                    instanceIndexBits.CopyTo(newVertexData, outputPosition);
                    outputPosition += instanceIndexBits.Length;
                }
            }

            // Create a vertex buffer and set the replicated data into it
            vertexBuffer = new VertexBuffer(device, newVertexData.Length, BufferUsage.None);
            vertexBuffer.SetData(newVertexData);
        }

        void ReplicateIndexData()
        {
            // Read old index data and destroy old index buffer
            ushort[] oldIndices = new ushort[indexCount];

            indexBuffer.GetData(oldIndices);
            indexBuffer.Dispose();

            ushort[] newIndices = new ushort[indexCount * maxInstances];

            int outputPosition = 0;

            // Replicate the index buffer once per instance
            for (int instanceIndex = 0; instanceIndex < maxInstances; instanceIndex++)
            {
                int instanceOffset = instanceIndex * vertexCount;

                for (int i = 0; i < indexCount; i++)
                {
                    newIndices[outputPosition] = (ushort)(oldIndices[i] + instanceOffset);

                    outputPosition++;
                }
            }

            indexBuffer = new IndexBuffer(device, sizeof(ushort) * newIndices.Length,
                                          BufferUsage.None, IndexElementSize.SixteenBits);
            indexBuffer.SetData(newIndices);
        }

        /// <summary>
        /// Draw a batch of instanced model geometry.
        /// </summary>
        public void Draw(InstancingTechnique technique,
                            Matrix[] instanceTransforms,
                            Matrix view, Matrix projection)
        {
            SetRenderStates(technique, view, projection);

            effect.Begin();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                switch (technique)
                {
                    case InstancingTechnique.HardwareInstancing:
                        DrawHardwareInstancing(instanceTransforms);
                        break;
                }
                pass.End();
            }
            effect.End();
        }

        void SetRenderStates(InstancingTechnique technique, Matrix view, Matrix projection)
        {
            device.VertexDeclaration = vertexDeclaration;
            device.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            device.Indices = indexBuffer;

            if (techniqueChanged)
            {
                string techniqueName = technique.ToString();
                effect.CurrentTechnique = effect.Techniques[techniqueName];
                techniqueChanged = false;
            }

            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
        }

        void DrawHardwareInstancing(Matrix[] instanceTransforms)
        {
            // Make sure our instance data vbuffer is big enough
            int instanceDataSize = SizeOfMatrix * instanceTransforms.Length;

            if ((instanceDataStream == null) ||
                (instanceDataStream.SizeInBytes < instanceDataSize))
            {
                if (instanceDataStream != null)
                    instanceDataStream.Dispose();

                instanceDataStream = new DynamicVertexBuffer(device, instanceDataSize, BufferUsage.WriteOnly);
            }

            // Upload transforms to the instance vertex data buffer
            instanceDataStream.SetData(instanceTransforms,0, instanceTransforms.Length, SetDataOptions.Discard);

            // Two streams: One for vertex data. One for per-instance transforms
            VertexStreamCollection vertices = device.Vertices;

            vertices[0].SetFrequencyOfIndexData(instanceTransforms.Length);
            vertices[1].SetSource(instanceDataStream, 0, SizeOfMatrix);
            vertices[1].SetFrequencyOfInstanceData(1);

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, indexCount / 3);

            // reset instancing streams
            vertices[0].SetSource(null, 0, 0);
            vertices[1].SetSource(null, 0, 0);

        }
    }
}
