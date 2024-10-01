using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Adam.Core.Models;
using MediatR;

namespace Adam.Core.MediatR.Requests.Commands;

public class UpdateProductCommand : IRequest<Product?>
{
    [JsonPropertyName("id")] public required int Id { get; init; }

    [JsonPropertyName("name")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "The Name must be between 5 and 50 characters.")]
    public required string? Name { get; init; }

    [JsonPropertyName("price")]
    [Range(1, 10000, ErrorMessage = "The Price must be between 1 and 10000.")]
    public required decimal? Price { get; init; }
}