using GreenDonut.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Osmo.Common.Database.Models;
using Osmo.Database.Extensions;
using Osmo.Database.Graphql.Types;

namespace Osmo.Database.Graphql.Query;

[ExtendObjectType(OsmoDatabaseExtensions.ROOT_QUERY_NAME)]
public class JobQuery
{
    [UseOffsetPaging(IncludeTotalCount = true)]
    // [UseConnection(EnableRelativeCursors = true)]
    [UseFiltering]
    [UseSorting]
    // public async Task<PageConnection<Job>> GetJobs(
    public IQueryable<Job> GetJobs(
        [Service] OsmoContext db)
    {
        return db.Jobs;
        //
        // var queryable = db.Jobs
        //     .With(query, s => s.AddAscending(t => t.JobName));
        //
        // var page = await queryable.ToPageAsync(pagingArguments);

        // var page = await GetJobsAsync(db, pagingArguments);
        // return new PageConnection<Job>(page);
        // return await db.Jobs.ApplyCursorPaginationAsync(context);
        // var page = await db.Jobs.ToPageAsync(pagingArguments);
        // return new PageConnection<Job>(page);
        // return new 
        // return await db.Jobs.ApplyCursorPaginationAsync(context);
    }
    
    public async Task<Job?> GetJobById(
        [Service] OsmoContext db,
        Guid id)
    {
        return await db.Jobs.FirstOrDefaultAsync(j => j.Id == id);
    }
    
    private async Task<Page<Job>> GetJobsAsync(OsmoContext db, PagingArguments pagingArguments)
    {
        return await db.Jobs.OrderBy(j => j.JobName).ToPageAsync(pagingArguments);
    }
}

// public class Query
// {
//     [UsePaging(IncludeTotalCount = true)]
//     [UseFiltering]
//     [UseSorting]
//     public IQueryable<Job> GetJobs([Service] OsmoContext db) => db.Jobs;
// }
//
// public class QueryType : ObjectType<Query>
// {
//     
// }