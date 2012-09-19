using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;

using TgcViewer.Utils.Terrain;
using System.Collections;

namespace AlumnoEjemplos.RestrictedGL
{
    class Terreno
    {
        ArrayList heightMaps;
        Vector3[] heightMapsUbic;
        string map; //path al heightmap
        string textura; //path a la textura
        float scaleXZ;
        float scaleY;
        TgcSkyBox skyBox;

        public Terreno() {
            heightMapInic();
            skyBoxInic();

            //Crea modifiers para el terreno:
            GuiController.Instance.Modifiers.addFloat("scaleY", 0f, 1f, scaleY);
        }

        public void heightMapInic() {
            //Cargar vars:
            map = Shared.mediaFolder + "\\Terreno\\Heightmap.jpg";
            textura = Shared.mediaFolder + "\\Terreno\\Mapa.jpg";
            scaleXZ = 10f;
            scaleY = 0.25f;

            //Instanciar HeightMaps:
            heightMaps = new ArrayList();
            for (int i = 0; i < 9; i++) {
                heightMaps.Add(new TgcSimpleTerrain());
            }

            //Determinar ubicación de cada uno:
            heightMapsUbic = new Vector3[9];
            float size = 62.75f;
            float offsetX = -size;
            for (int i = 0; i < 9; i++) {
                heightMapsUbic[i].X = offsetX;
                if (offsetX == size) {
                    offsetX = -size;
                } else {
                    offsetX += size;
                }

                if (i <= 2) {
                    heightMapsUbic[i].Z = size;
                } else if (i <= 5) {
                    heightMapsUbic[i].Z = 0;
                } else {
                    heightMapsUbic[i].Z = -size;
                }
            }
            /*esto crearía una matriz así:
             * xxx
             * xxx
             * xxx
            *///donde cada x es un heightmap, y la del medio es el (0,0,0)
       
            heightMapLoad();
        }

        public void heightMapLoad() {
            for (int i = 0; i < 9; i++) {
                TgcSimpleTerrain heightMapActual = (TgcSimpleTerrain) heightMaps[i];
                heightMapActual.loadHeightmap(map, scaleXZ, scaleY, heightMapsUbic[i]);
                heightMapActual.loadTexture(textura);
            }
        }

        public void skyBoxInic() {
            //Crear SkyBox:
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(9000, 9000, 9000);

            //Configurar las texturas para cada una de las 6 caras:
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, Shared.mediaFolder + "\\Terreno\\SkyTop.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, Shared.mediaFolder + "\\Terreno\\SkyTop.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, Shared.mediaFolder + "\\Terreno\\SkyLeft.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, Shared.mediaFolder + "\\Terreno\\SkyRight.jpg");
            //Los back&front se invierten por usar sistema left-handed...
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, Shared.mediaFolder + "\\Terreno\\SkyFront.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, Shared.mediaFolder + "\\Terreno\\SkyBack.jpg");

            skyBox.updateValues();
        }

        public void comprobarCambios() {
            //Compruba cambios en modifiers y actualiza:
            float scaleYNew = (float) GuiController.Instance.Modifiers["scaleY"];
            if (scaleY != scaleYNew) {
                scaleY = scaleYNew;
                heightMapLoad();
            }
        }

        public void render() {
            comprobarCambios();
            
            for (int i = 0; i < 9; i++) { //renderizar los 9 HeightMaps
                TgcSimpleTerrain heightMapActual = (TgcSimpleTerrain)heightMaps[i];
                heightMapActual.render();
            }

            skyBox.render();
        }
    }
}