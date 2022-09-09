namespace HardwareServiceOrderServices.Models;

public enum ServiceOrderAddonsEnum
{
    /// <summary>
    ///     A <see langword="null"/> equivalent enum-value. This indicates a missing value or a bad/default value-mapping.
    ///     This value should always be treated as an error, as it should not never actually be assigned.
    /// </summary>
    Null = 0,

    /// <summary>
    ///     <b>Packing Service</b> which is an addon supported service provider Conmodo. <para>
    ///     
    ///     <b>Service Provider:</b> Conmodo <br/>
    ///     <b>Addon:</b> Packaging </para>
    /// </summary>
    CONMODO_PACKAGING = 1,
}