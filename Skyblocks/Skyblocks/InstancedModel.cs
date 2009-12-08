using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Skyblocks
{
    public enum InstancingTechnique
    {
        HardwareInstancing,
        ShaderInstancing,
        NoInstancing,
        NoInstancingOrStateBatching
    }

    class InstancedModel
    {

        [ContentSerializer]
        List<InstancedModelPart> modelParts = null;

        GraphicsDevice device;

        // Constructor private. For use by XNB deserializer.
        private InstancedModel()
        {
        }

        public void Initialize(GraphicsDevice device)
        {
            this.device = device;

            foreach (InstancedModelPart modelPart in modelParts)
            {
                modelPart.Initialize(device);
            }

            InstancingTechnique technique = 0;

            while (!IsTechniqueSupported(technique))
                technique++;

            SetInstancingTechnique(technique);
        }

        private InstancingTechnique instancingTechnique;
        public InstancingTechnique InstancingTechnique
        {
            get { return instancingTechnique; }
        }

        public void SetInstancingTechnique(InstancingTechnique technique)
        {
            instancingTechnique = technique;

            foreach (InstancedModelPart modelPart in modelParts)
            {
                modelPart.SetInstancingTechnique(technique);
            }
        }

        public bool IsTechniqueSupported(InstancingTechnique technique)
        {
            if (technique == InstancingTechnique.HardwareInstancing)
            {
                return device.GraphicsDeviceCapabilities.PixelShaderVersion.Major >= 3;
            }

            return true;
        }

        public void DrawInstances(Matrix[] instanceTransforms, Matrix view, Matrix projection)
        {
            if (device == null)
            {
                throw new InvalidOperationException(
                        "InstanceModel.Initialize must be called before DrawInstances.");
            }

            if (instanceTransforms.Length == 0)
                return;

            foreach (InstancedModelPart modelPart in modelParts)
            {
                modelPart.Draw(instancingTechnique, instanceTransforms, view, projection);
            }
        }
    }
}
