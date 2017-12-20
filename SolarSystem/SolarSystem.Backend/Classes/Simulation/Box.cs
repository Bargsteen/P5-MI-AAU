namespace SolarSystem.Backend.Classes.Simulation
{
    public abstract class Box
    {
        public int Id;

        private static int _nextId = 1;
        
        public Box()
        {
            Id = _nextId++;
        }
    }
}
