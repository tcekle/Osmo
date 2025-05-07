namespace Osmo.ConneX.Models;

public record ProgrammingTimes(DateTime bucket, decimal avg_program_duration, decimal avg_verify_duration, decimal avg_blank_check_duration, decimal avg_erase_duration, decimal avg_overhead);