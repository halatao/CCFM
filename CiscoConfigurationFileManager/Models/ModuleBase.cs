namespace CiscoConfigurationFileManager.Models;

/// <summary>
/// Base class for module objects, containing basic properties such as name, tag, and operation type.
/// </summary>
public class ModuleBase
{
    public string? Name { get; set; }
    public string? Tag { get; set; }
    public OperationEnum OperationEnum { get; set; } = OperationEnum.None;
}