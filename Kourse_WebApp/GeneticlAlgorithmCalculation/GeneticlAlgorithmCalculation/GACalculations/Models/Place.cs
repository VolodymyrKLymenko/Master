using System;

namespace GeneticlAlgorithmCalculation.GACalculations.Models
{
    public class Place
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Place()
        {
        }

        public Place(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double GetDistance(Place toCity)
        {
            var deltaX = Math.Pow(X - toCity.X, 2);
            var deltaY = Math.Pow(Y - toCity.Y, 2);

            return Math.Sqrt(Math.Abs(deltaX + deltaY));
        }
    }
}
