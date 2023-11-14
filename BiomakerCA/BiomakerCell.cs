public enum CellType
{
    Void,
    Air,
    Earth,
    Immovable,
    Sun,
    OutOfBounds,
    AgentUnspecialized,
    AgentRoot,
    AgentLeaf,
    AgentFlower
    // ... other cell types
}

public interface INutrientConsumer
{
    void AbsorbNutrients();
}

public class CellBiomaker
{
    public CellType Type { get; private set; }
    public float StructuralIntegrity { get; private set; }
    public int Age { get; private set; }
    public float EarthNutrients { get; private set; }
    public float AirNutrients { get; private set; }
    public string AgentId { get; private set; }
    // ... other properties as needed

    public CellBiomaker(CellType type)
    {
        Type = type;
        // Initialize other properties as needed
    }

    // Methods for aging, structural integrity adjustment, etc.

    public void SetAgentId(string id)
    {
        // Validation logic before setting the AgentId
        AgentId = id;
    }

    // ... other methods as needed
}

public class AgentCell : CellBiomaker, INutrientConsumer
{
    public AgentCell(CellType type) : base(type)
    {
        // Additional initialization for agent cells
    }

    public void AbsorbNutrients()
    {
        // Implementation for absorbing nutrients
    }

    // ... other agent-specific behaviors
}

// ... additional subclasses for specific agent types if needed
