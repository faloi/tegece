using System.Collections.Generic;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RestrictedGL {
    public class Terreno {
        //Datos del HeightMap:
        private List<TgcSimpleTerrain> heightMaps;
        private Vector3[] heightMapsUbication;
        private const int MAX_HEIGHT_MAPS = 9;
        private const float DISTANCE = 62.75f;
        private const float SCALE_XZ = 10f;
        private float scaleY = 0.25f;
        private readonly string pathHeightMap = Shared.MediaFolder + "\\Terreno\\Heightmap.jpg";
        private readonly string pathTextura = Shared.MediaFolder + "\\Terreno\\Mapa.jpg";

        //Datos del SkyBox:
        private TgcSkyBox skyBox;
        private const float SKYBOX_DEPTH = 9000f;
        private readonly string pathSkyTop = Shared.MediaFolder + "\\Terreno\\SkyTop.jpg";
        private readonly string pathSkyLeft = Shared.MediaFolder + "\\Terreno\\SkyLeft.jpg";
        private readonly string pathSkyRight = Shared.MediaFolder + "\\Terreno\\SkyRight.jpg";
        private readonly string pathSkyFront = Shared.MediaFolder + "\\Terreno\\SkyFront.jpg";
        private readonly string pathSkyBack = Shared.MediaFolder + "\\Terreno\\SkyBack.jpg";

        public Terreno() {
            heightMapInic();
            skyBoxInic();

            //Crea modifiers para el terreno:
            GuiController.Instance.Modifiers.addFloat("scaleY", 0f, 1f, scaleY);
        }

        private void heightMapInic() {
            //Instanciar HeightMaps:
            heightMaps = new List<TgcSimpleTerrain>();
            for (var i = 0; i < MAX_HEIGHT_MAPS; i++) {
                heightMaps.Add(new TgcSimpleTerrain());
            }

            //Determinar ubicación de cada uno:
            heightMapsUbication = new Vector3[MAX_HEIGHT_MAPS];
            var offsetX = -DISTANCE;
            for (var i = 0; i < MAX_HEIGHT_MAPS; i++) {
                heightMapsUbication[i].X = offsetX;
                if (offsetX == DISTANCE) offsetX = -DISTANCE;
                else offsetX += DISTANCE;

                if (i <= 2) heightMapsUbication[i].Z = DISTANCE;
                else if (i <= 5) heightMapsUbication[i].Z = 0;
                else heightMapsUbication[i].Z = -DISTANCE;
            }
            /*esto crearía una matriz así:
             * xxx
             * xxx
             * xxx
            */ //donde cada x es un heightmap, y la del medio es el (0,0,0)

            heightMapLoad();
        }

        private void heightMapLoad() {
            for (var i = 0; i < MAX_HEIGHT_MAPS; i++) {
                var heightMapActual = heightMaps[i];
                heightMapActual.loadHeightmap(pathHeightMap, SCALE_XZ, scaleY, heightMapsUbication[i]);
                heightMapActual.loadTexture(pathTextura);
            }
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

        private void comprobarCambios() {
            //Compruba cambios en modifiers y actualiza:
            var scaleYNew = Modifiers.get<float>("scaleY");

            if (scaleY == scaleYNew) return;

            scaleY = scaleYNew;
            heightMapLoad();
        }

        public void render() {
            comprobarCambios();

            for (var i = 0; i < MAX_HEIGHT_MAPS; i++) {
                //renderizar los N HeightMaps
                var heightMapActual = heightMaps[i];
                heightMapActual.render();
            }

            skyBox.render();
        }
    }
}