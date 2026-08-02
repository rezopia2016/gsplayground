using ShippingPlatform.Application.DTOs;

namespace ShippingPlatform.Application.Services;

public interface ILabelService
{
    Task<IReadOnlyList<LabelResponse>> GenerateLabelsAsync(GenerateLabelsRequest request, CancellationToken ct = default);
    Task<LabelResponse> ReprintAsync(Guid labelId, ReprintLabelRequest request, CancellationToken ct = default);
}
