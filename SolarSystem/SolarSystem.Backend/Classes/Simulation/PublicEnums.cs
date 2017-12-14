namespace SolarSystem.Backend.Classes.Simulation
{
    public enum StationResult
    {
        FullError,
        Success
    }
    
    public enum BoxResult
    {
        NotInOrder,
        AlreadyPicked,
        SuccesfullyAdded
    }

    public enum AreaCode
    {
        Area21,
        Area25,
        Area27,
        Area28,
        Area29,
        Pq
    }
    
    public enum OrderGenerationConfiguration
    {
        Random,
        FromFile
    }

    public enum SchedulerType
    {
        Fifo,
        Mi1,
        Mi6,
        Lst,
        Real
    }

    public enum RandomSeedType
    {
        Fixed,
        Random
    }

    public enum SimulationState
    {
        Real,
        Experimental
    }
}