namespace monitoring.Limit;

public class Limits
{
    public IDictionary<string, float[]> valueLimits;
    public Limits()
    {
        valueLimits = new Dictionary<string, float[]>();
        valueLimits.Add("ph", new float[] { 6.5F, 8.5F });
        valueLimits.Add("Organic_carbon", new float[] { 2.0F, 4.0F });
        valueLimits.Add("Turbidity", new float[] { 0.1F, 5.0F });
    }
}
