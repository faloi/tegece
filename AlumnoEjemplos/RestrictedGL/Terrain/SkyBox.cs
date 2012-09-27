using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RestrictedGL.Terrain {
    
    public class SkyBox : TgcSkyBox {
        private readonly string pathSkyTop = Shared.MediaFolder + "\\Terreno\\SkyTop.jpg";
        private readonly string pathSkyLeft = Shared.MediaFolder + "\\Terreno\\SkyLeft.jpg";
        private readonly string pathSkyRight = Shared.MediaFolder + "\\Terreno\\SkyRight.jpg";
        private readonly string pathSkyFront = Shared.MediaFolder + "\\Terreno\\SkyFront.jpg";
        private readonly string pathSkyBack = Shared.MediaFolder + "\\Terreno\\SkyBack.jpg";

        public SkyBox(Vector3 center, Vector3 size) {
            this.Center = center;
            this.Size = size;

            this.setFaceTexture(SkyFaces.Up, pathSkyTop);
            this.setFaceTexture(SkyFaces.Down, pathSkyTop);
            this.setFaceTexture(SkyFaces.Left, pathSkyLeft);
            this.setFaceTexture(SkyFaces.Right, pathSkyRight);

            this.setFaceTexture(SkyFaces.Front, pathSkyFront);
            this.setFaceTexture(SkyFaces.Back, pathSkyBack);

            this.updateValues();
        }
    }

}
