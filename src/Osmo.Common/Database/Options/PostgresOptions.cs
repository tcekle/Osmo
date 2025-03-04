namespace Osmo.Common.Database.Options;

/// <summary>
/// Options for Postgres database
/// </summary>
public class PostgresOptions
{
    /// <summary>
    /// Gets or sets the host
    /// </summary>
    public string Host { get; set; }
    
    /// <summary>
    /// Gets or sets the port
    /// </summary>
    public int Port { get; set; }
    
    /// <summary>
    /// Gets or sets the username
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password
    /// </summary>
    public string Password { get; set; }
}