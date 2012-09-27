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
        private const float SCALE_XZ = 20f;
        private float scaleY = 0.8f;
        private float threshold = 0.075f;
        private readonly string pathHeightMap = Shared.MediaFolder + "\\Terreno\\Heightmap.jpg";
        private readonly string pathTexture = Shared.MediaFolder + "\\Terreno\\Mapa.jpg";
        public int[,] HeightMapData {
            get { return heightMap.HeightmapData; }
        }
        public float ScaleXZ {
            get { return heightMap.ScaleXZ; }
        }
        public float ScaleY {
            get { return heightMap.ScaleY; }
        }

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
            GuiController.Instance.Modifiers.addFloat("Scale Y", 0f, 1f, scaleY);
            GuiController.Instance.Modifiers.addFloat("ROAM Threshold", 0f, 1f, threshold);
        }

        private void heightMapInic() {
            //Instanciar HeightMaps:
            heightMap = new AdaptativeHeightmap();
            heightMapLoad();
        }

        private void heightMapLoad() {
            heightMap.loadHeightmap(pathHeightMap, SCALE_XZ, scaleY, new Vector3(0,0,0));
            heightMap.loadTexture(pathTexture);
            heightMap.Threshold = threshold;
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
            var scaleYNew = Modifiers.get<float>("Scale Y");
            var thresholdNew = Modifiers.get<float>("ROAM Threshold");
            if (scaleY != scaleYNew) {
                scaleY = scaleYNew;
                heightMapLoad();
            }
            if (threshold != thresholdNew) {
                threshold = thresholdNew;
                heightMap.Threshold = threshold;
            }
        }

        public void render() {
            updateValues();
            heightMap.render();
            skyBox.render();
        }
    }
}