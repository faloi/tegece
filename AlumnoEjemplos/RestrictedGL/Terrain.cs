using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.Terrain;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;

namespace AlumnoEjemplos.RestrictedGL
{
    public class Terrain
    {
        //Datos del HeightMap:
        private AdaptativeHeightmap heightMap;
        private const float SCALE_XZ = 10f;
        private float scaleY = 0.25f;
        private readonly string pathHeightMap = Shared.MediaFolder + "\\Terreno\\Heightmap.jpg";
        private readonly string pathTexture = Shared.MediaFolder + "\\Terreno\\Mapa.jpg";

        //Datos del SkyBox:
        private TgcSkyBox skyBox;
        private const float SKYBOX_DEPTH = 9000f;
        private readonly string pathSkyTop = Shared.MediaFolder + "\\Terreno\\SkyTop.jpg";
        private readonly string pathSkyLeft = Shared.MediaFolder + "\\Terreno\\SkyLeft.jpg";
        private readonly string pathSkyRight = Shared.MediaFolder + "\\Terreno\\SkyRight.jpg";
        private readonly string pathSkyFront = Shared.MediaFolder + "\\Terreno\\SkyFront.jpg";
        private readonly string pathSkyBack = Shared.MediaFolder + "\\Terreno\\SkyBack.jpg";

        public Terrain() {
            heightMapInic();
            skyBoxInic();

            //Crea modifiers para el terreno:
            GuiController.Instance.Modifiers.addFloat("scaleY", 0f, 1f, scaleY);
        }

        private void heightMapInic() {
            //Instanciar HeightMaps:
            heightMap = new AdaptativeHeightmap();
            heightMapLoad();
        }

        private void heightMapLoad() {
            heightMap.loadHeightmap(pathHeightMap, SCALE_XZ, scaleY, new Vector3(0,0,0));
            heightMap.loadTexture(pathTexture);
        }

        private void skyBoxInic() {
            //Crear SkyBox:
            skyBox = new TgcSkyBox
                         {
                             Center = new Vector3(0, 0, 0),
                             Size = new Vector3(SKYBOX_DEPTH, SKYBOX_DEPTH, SKYBOX_DEPTH)
                         };

            //Configurar las texturas para cada una de las 6 caras:
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, pathSkyTop);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, pathSkyTop);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, pathSkyLeft);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, pathSkyRight);

            //Los back&front se invierten por usar sistema left-handed...
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, pathSkyFront);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, pathSkyBack);

            skyBox.updateValues();
        }

        private void updateValues() {
            //Compruba cambios en modifiers y actualiza:
            var scaleYNew = Modifiers.get<float>("scaleY");
            if (scaleY == scaleYNew) return;

            scaleY = scaleYNew;
            heightMapLoad();
        }

        public void render() {
            updateValues();
            heightMap.render();
            skyBox.render();
        }
    }
}