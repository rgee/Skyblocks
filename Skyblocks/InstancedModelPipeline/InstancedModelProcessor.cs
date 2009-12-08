using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace InstancedModelPipeline
{
    [ContentProcessor(DisplayName = "Instanced Model")]
    public class InstancedModelProcessor : ContentProcessor<NodeContent, InstancedModelContent>
    {
        NodeContent rootNode;
        ContentProcessorContext context;
        InstancedModelContent outputModel;

        // Dictionary to make sure we only process each material once as a single material
        // will be reused on many objects.
        Dictionary<MaterialContent, MaterialContent> processedMaterials = new Dictionary<MaterialContent, MaterialContent>();


        public override InstancedModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            this.rootNode = input;
            this.context = context;
            outputModel = new InstancedModelContent();
            ProcessNode(input);
            return outputModel;
        }

        void ProcessNode(NodeContent node)
        {

            MeshHelper.TransformScene(node, node.Transform);
            node.Transform = Matrix.Identity;

            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Re-order vertex and index data for efficiency
                MeshHelper.OptimizeForCache(mesh);

                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    ProcessGeometry(geometry);
                }
            }

            foreach (NodeContent child in node.Children)
            {
                ProcessNode(child);
            }
        }

        void ProcessGeometry(GeometryContent geometry)
        {
            int indexCount = geometry.Indices.Count;
            int vertexCount = geometry.Vertices.VertexCount;

            if (vertexCount > ushort.MaxValue)
            {
                throw new InvalidContentException(
                     string.Format("Geometry contains {0} vertices: " +
                        "this is too many to be instanced.", vertexCount));
            }

            if (vertexCount > ushort.MaxValue / 8)
            {
                context.Logger.LogWarning(null, rootNode.Identity,
                                         "Geometry contains {0} vertices: " +
                                         "this will only allow it to be instanced " +
                                         "{1} times per batch. A model with fewer " +
                                         "vertices would be more efficient.",
                                         vertexCount, ushort.MaxValue / vertexCount);
            }

            VertexChannelCollection vertexChannels = geometry.Vertices.Channels;

            for (int i = 1; i <= 4; i++)
            {
                if (vertexChannels.Contains(VertexChannelNames.TextureCoordinate(i)))
                {
                    throw new InvalidContentException(
    string.Format("Model already contains data for texture " +
                  "coordinate channel {0}, but instancing " +
                  "requires this channel for its own use.", i));
                }
            }

            // Flatten vertex channel data into buffer array
            VertexBufferContent vertexBufferContent;
            VertexElement[] vertexElements;

            geometry.Vertices.CreateVertexBuffer(out vertexBufferContent, out vertexElements, context.TargetPlatform);
            int vertexStride = VertexDeclaration.GetVertexStrideSize(vertexElements, 0);

            MaterialContent material = ProcessMaterial(geometry.Material);

            outputModel.AddModelPart(indexCount, vertexCount, vertexStride, vertexElements, vertexBufferContent,
                                        geometry.Indices, material);


        }

        MaterialContent ProcessMaterial(MaterialContent material)
        {
            if(!processedMaterials.ContainsKey(material))
            {
                EffectMaterialContent instancedMaterial = new EffectMaterialContent();

                // Use the custom instancing effect
                instancedMaterial.Effect = new ExternalReference<EffectContent>("Instancing.fx", rootNode.Identity);

                if (!material.Textures.ContainsKey("Texture"))
                {
                    throw new InvalidContentException(
                        "Material has no texture, but the InstancedModel " +
                        "effect does not support untextured materials.");
                }
                instancedMaterial.Textures.Add("Texture", material.Textures["Texture"]);

                processedMaterials[material] = context.Convert<MaterialContent, MaterialContent>(instancedMaterial, "MaterialProcessor");
            }
            return processedMaterials[material];
        }
    }
}
