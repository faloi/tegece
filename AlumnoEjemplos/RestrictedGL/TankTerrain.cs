using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.RestrictedGL{
    class TankTerrain{
        private TgcBox surface;
        private TgcMesh tree;
        public void init(string alumnoMediaFolder){
            //Cargo Superficie
            Microsoft.DirectX.Direct3D.Device d3DDevice = GuiController.Instance.D3dDevice;
            TgcTexture surfaceTexture = TgcTexture.createTexture(d3DDevice, alumnoMediaFolder + "RestrictedGL\\#TankExample\\Textures\\tierra.jpg");
            surface = TgcBox.fromSize(new Vector3(0, 0, 0),new Vector3(500, 0, 500), surfaceTexture);
            //Cargo Arbol
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            tree = scene.Meshes[1];

        }
        public void render() {
            surface.render();
            tree.render();
            bool showBoundingBox = (bool)GuiController.Instance.Modifiers["showBoundingBox"];
            if (showBoundingBox) {
                tree.BoundingBox.render();
                surface.BoundingBox.render();
            }
        }
        public void dispose() {
            surface.dispose();
        }
    }
}
