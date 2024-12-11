namespace Shared.Core.Constants;

public static class IOTConstants
{
    private const string PLC_Topic = "PLC";
    public const string PLC_Status_Topic = $"{PLC_Topic}/MachineStatus";
    public const string PLC_Command_Topic = $"{PLC_Topic}/Command";
}