using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;

namespace PetCafe.Application.Models.TeamModels;

public class TeamCreateModel
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid? LeaderId { get; set; }
    public List<Guid>? WorkTypeIds { get; set; } = [];

}

public class TeamUpdateModel : TeamCreateModel
{
    public bool IsActive { get; set; } = true;
}



public class MemberCreateModel
{
    public Guid EmployeeId { get; set; }
}

public class MemberUpdateModel : MemberCreateModel
{
    public bool IsActive { get; set; } = true;
}

public class TeamFilterQuery : FilterQuery
{
    [FromQuery(Name = "is_active")]
    public bool IsActive { get; set; } = true;

    [FromQuery(Name = "working_day")]
    public string? WorkingDay { get; set; }

    [FromQuery(Name = "start_working_time")]
    public TimeSpan? StartWorkingTime { get; set; }

    [FromQuery(Name = "end_working_time")]
    public TimeSpan? EndWorkingTime { get; set; }

    [FromQuery(Name = "work_type_id")]
    public Guid? WorkTypeId { get; set; }
}
