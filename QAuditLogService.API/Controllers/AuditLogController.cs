using MediatR;
using Microsoft.AspNetCore.Mvc;
using QAuditLogService.Application.Request;
using QAuditLogService.Application.Responses;
using QAuditLogService.Application.UseCases.AuditLogs.Queries.GetAllAuditLogs;

namespace QAuditLogService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogController: ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet("get-audit-logs")]
    public async Task<ActionResult<PagedResponse<AuditResponse>>> GetAuditLogsAsync([FromQuery] AuditRequest request)
    {
        var query = new GetAllAuditLogsQuery(request.UserId, request.Action, request.EntityName, request.ServiceName,
            request.From, request.To, request.PageNumber, request.PageSize);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}