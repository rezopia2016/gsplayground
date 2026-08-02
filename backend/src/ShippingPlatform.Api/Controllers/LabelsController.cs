using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Services;

namespace ShippingPlatform.Api.Controllers;

[ApiController]
[Route("api/labels")]
[Authorize(Policy = "LabOperations")]
public class LabelsController : ControllerBase
{
    private readonly ILabelService _labelService;

    public LabelsController(ILabelService labelService)
    {
        _labelService = labelService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<IReadOnlyList<LabelResponse>>> Generate(GenerateLabelsRequest request, CancellationToken ct) =>
        Ok(await _labelService.GenerateLabelsAsync(request, ct));

    [HttpPost("{id:guid}/reprint")]
    public async Task<ActionResult<LabelResponse>> Reprint(Guid id, ReprintLabelRequest request, CancellationToken ct) =>
        Ok(await _labelService.ReprintAsync(id, request, ct));
}
