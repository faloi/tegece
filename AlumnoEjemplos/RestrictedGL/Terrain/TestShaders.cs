using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Utils;

namespace AlumnoEjemplos.RestrictedGL.Terrain {
    public class TestShaders : IRenderObject {
        Effect effect;
        TgcScene scene;
        MeshShader mesh;
        float time;

        public TestShaders() {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new MeshShaderFactory();

            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\TanqueFuturistaRuedas\\TanqueFuturistaRuedas-TgcScene.xml");
            mesh = (MeshShader)scene.Meshes[0];
            mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            mesh.Position = new Vector3(0f, 0f, 0f);

            string compilationErrors;
            effect = Effect.FromFile(d3dDevice, GuiController.Instance.ExamplesDir + "Shaders\\WorkshopShaders\\Shaders\\BasicShader.fx", null, null, ShaderFlags.None, null, out compilationErrors);
            if (effect == null) GuiController.Instance.Logger.log(compilationErrors);
            mesh.effect = effect;
        }

        public void render() {
            if (effect == null) return;
            Device device = GuiController.Instance.D3dDevice;

            time += Shared.ElapsedTime;
            //effect.Technique = "Test";
            effect.SetValue("time", time);

            mesh.render();
        }

        public void dispose() { }
        public bool AlphaBlendEnable { get; set; }
    }
}