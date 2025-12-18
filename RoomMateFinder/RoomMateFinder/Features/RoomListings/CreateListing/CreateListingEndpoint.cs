using MediatR;
using RoomMateFinder.Features.RoomListings.CreateListing;
using System.Security.Claims;

public static class CreateRoomListingEndpoint
{
    public static void MapCreateRoomListingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/listings", async (
                HttpContext http,
                IMediator mediator,
                IWebHostEnvironment env,
                CancellationToken ct) =>
            {
                var ownerId = Guid.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var form = await http.Request.ReadFormAsync(ct);
                
                // Parse form data
                var request = new CreateListingRequest
                {
                    Title = form["Title"].ToString(),
                    Description = form["Description"].ToString(),
                    Address = form["Address"].ToString(),
                    Price = decimal.Parse(form["Price"].ToString()),
                    RoommatesCount = int.Parse(form["RoommatesCount"].ToString()),
                    GenderPreference = form["GenderPreference"].ToString()
                };

                var id = await mediator.Send(new CreateRoomListingCommand(ownerId, request), ct);

                // Handle image upload if provided
                var imageFile = form.Files.GetFile("Image");
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                    
                    if (allowedExtensions.Contains(extension) && imageFile.Length <= 5 * 1024 * 1024)
                    {
                        // Create uploads directory
                        var uploadsFolder = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "uploads", "listings");
                        Directory.CreateDirectory(uploadsFolder);

                        // Generate unique filename
                        var fileName = $"{id}_{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream, ct);
                        }

                        // Update listing with image URL
                        var context = http.RequestServices.GetRequiredService<RoomMateFinder.Infrastructure.Persistence.AppDbContext>();
                        var listing = await context.RoomListings.FindAsync(new object[] { id }, ct);
                        if (listing != null)
                        {
                            listing.ImageUrl = $"/uploads/listings/{fileName}";
                            await context.SaveChangesAsync(ct);
                        }
                    }
                }

                return Results.Created($"/listings/{id}", id);
            })
            .RequireAuthorization()
            .DisableAntiforgery();
    }
}