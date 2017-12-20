namespace SolarSystem.Backend.Classes.Simulation.Boxes
{
    public abstract class Box
    {
        protected int Id;

        private static int _nextId = 1;

        protected Box()
        {
            Id = _nextId++;
        }
    }
}
