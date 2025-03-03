namespace Osmo.Common.Database.Models;

/// <summary>
/// A unit reresenting a job that can be executed by a programmer.
/// </summary>
public class Job : Unit
{
    /// <summary>
    /// Gets or sets the related algorithm identifier.
    /// </summary>
    public Guid RelatedAlgorithmId { get; set; }
    
    /// <summary>
    /// Gets or sets the given job identifier from the job package.
    /// </summary>
    public Guid GivenJobId { get; set; }

    /// <summary>
    /// Gets or sets the job name.
    /// </summary>
    public string JobName
    {
        get => Name; 
        set => Name = value;
    }
    
    /// <summary>
    /// Gets or sets the optional job description.
    /// </summary>
    public string JobDescription { get; set; }
    
    /// <summary>
    /// Gets or sets the job checksum.
    /// </summary>
    public string JobChecksum { get; set; }
    
    /// <summary>
    /// Gets or sets the job's settings checksum.
    /// </summary>
    public string SettingChecksum { get; set; }
}