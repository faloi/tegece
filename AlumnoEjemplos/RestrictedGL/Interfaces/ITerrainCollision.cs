namespace AlumnoEjemplos.RestrictedGL.Interfaces {
    public interface ITerrainCollision {

        int getYValueFor(float x, float z);

        void deform(float x, float z, float radius, int power);

    }
}