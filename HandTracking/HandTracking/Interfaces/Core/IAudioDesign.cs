namespace HandTracking.Interfaces.Core
{
    public enum AudioDesignType
    {
        Individual,
        Coalescent
    };

    public interface IAudioDesign
    {
        AudioDesignType Type { get; set; }
    }
}
