using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RestrictedGL.Terrain
{
    public class Terrain : IRenderObject {
        private const float SKYBOX_DEPTH = 9000f;
        private readonly List<IRenderObject> objectsToRender; 

        public Terrain() {
            this.objectsToRender = new List<IRenderObject> {
               new AdaptativeHeightmap(),
               new SkyBox(new Vector3(0, 0, 0), new Vector3(SKYBOX_DEPTH, SKYBOX_DEPTH, SKYBOX_DEPTH))
            };
        }

        public void render() {
            this.objectsToRender.ForEach(o => o.render());
        }

        public void dispose() {
            this.objectsToRender.ForEach(o => o.dispose());
        }

        public bool AlphaBlendEnable { get; set; }
    }
}