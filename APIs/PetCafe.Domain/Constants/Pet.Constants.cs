namespace PetCafe.Domain.Constants;

public static class HealthStatusConstant
{
    public const string HEALTHY = "HEALTHY";
    public const string SICK = "SICK";
    public const string RECOVERING = "RECOVERING";
    public const string UNDER_OBSERVATION = "UNDER_OBSERVATION";
    public const string QUARANTINE = "QUARANTINE";

    public static readonly List<string> ALL_STATUSES = [
        HEALTHY,
        SICK,
        RECOVERING,
        UNDER_OBSERVATION,
        QUARANTINE
    ];
}

