using System.Reflection;

namespace PetCafe.Infrastructures;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
