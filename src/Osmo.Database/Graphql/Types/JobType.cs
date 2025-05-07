using Osmo.Common.Database.Models;

namespace Osmo.Database.Graphql.Types;

public class JobType : ObjectType<Job>
{
    protected override void Configure(IObjectTypeDescriptor<Job> descriptor)
    {
        descriptor.Field(j => j.Id).Type<NonNullType<IdType>>();
        descriptor.Field(j => j.GivenJobId);
        descriptor.Field(j => j.JobName);
        descriptor.Field(j => j.JobDescription);
    }
}