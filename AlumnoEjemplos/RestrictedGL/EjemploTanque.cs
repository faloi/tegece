using System;
using System.Collections.Generic;
using System.Text;
using AlumnoEjemplos.RestrictedGL.GuiWrappers;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Example;

namespace AlumnoEjemplos.RestrictedGL
{
    public class EjemploTanque : TgcExample
    {
        private Terreno terreno;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "RestrictedGL.Tanque";
        }

        public override string getDescription()
        {
            return "Ejemplo del tanque sobre una superficie plana";
        }

        public override void init() {
            throw new NotImplementedException();
        }

        public override void close() {
            throw new NotImplementedException();
        }

        public override void render(float elapsedTime) {
            throw new NotImplementedException();
        }
    }
}
