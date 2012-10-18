using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Utils;

namespace AlumnoEjemplos.RestrictedGL.Terrain {
    public class Water : IRenderObject {
        Effect effect;
        TgcScene scene;
        MeshShader mesh;
        float time;

        public Water() {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new MeshShaderFactory();

            scene = loader.loadSceneFromFile(Path.WaterScene);
            mesh = (MeshShader)scene.Meshes[0];
            mesh.Scale = new Vector3(350f, 0.1f, 2200f);
            mesh.Position = new Vector3(0f, -20f, 0f);

            string compilationErrors;
            effect = Effect.FromFile(d3dDevice, Path.WaterShader, null, null, ShaderFlags.None, null, out compilationErrors);
            mesh.effect = effect;
            if (effect == null) GuiController.Instance.Logger.log(compilationErrors);

            effect.SetValue("textureOffset", 0.1f);
        }

        public void render() {
            if (effect == null) return;
            Device device = GuiController.Instance.D3dDevice;

            time += Shared.ElapsedTime;
            effect.SetValue("time", time);
            if (time > float.MaxValue - 3) time = 0;
            
            mesh.render();
        }

        public void dispose() { }
        public bool AlphaBlendEnable { get; set; }
    }
}