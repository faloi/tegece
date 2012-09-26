using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using AlumnoEjemplos.RestrictedGL.Interfaces;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.RestrictedGL.Tank {
    
    class TankTerrain : IAlumnoRenderObject {
        
        private TgcBox surface;
        private TgcMesh tree;
        
        public void init(string alumnoMediaFolder){
            var d3DDevice = GuiController.Instance.D3dDevice;
            var surfaceTexture = TgcTexture.createTexture(d3DDevice, alumnoMediaFolder + "RestrictedGL\\#TankExample\\Textures\\tierra.jpg");
            this.surface = TgcBox.fromSize(new Vector3(0, 0, 0),new Vector3(500, 0, 500), surfaceTexture);

            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(alumnoMediaFolder + "RestrictedGL\\#TankExample\\Scenes\\TanqueFuturistaOrugas-TgcScene.xml");
            this.tree = scene.Meshes[1];
        }

        public void render(float elapsedTime) {
            surface.render();
            tree.render();
            
            var showBoundingBox = Modifiers.get<bool>("showBoundingBox");
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
