using System;

namespace AlumnoEjemplos.RestrictedGL.Utils
{
    public class Randomizer {

        private readonly Random randomizer;
        private readonly int minValue;
        private readonly int maxValue;

        public Randomizer(int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.randomizer = new Random();
        }

        public int getNext()
        {
            var absolute = this.randomizer.Next(minValue, maxValue);
            var sign = this.randomizer.Next(2) == 0 ? -1 : 1;

            return absolute * sign;
        }
    }
}
