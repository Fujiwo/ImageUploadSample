using ImageUploadSample.Models;
using ImageUploadSample.Utilities;
using ImageUploadSample.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageUploadSample.Controllers;

public class ImagesController : Controller
{
    private readonly ImagesContext context;

    public ImagesController(ImagesContext context) => this.context = context;

    public async Task<IActionResult> Index()
        => context.Images is null
           ? Problem("Entity set 'ImagesContext.Images'  is null.")
           : View(model: await context.Images.ToListAsync());

    public async Task<IActionResult> Originals(int? id)
    {
        if (id is null || context.Images is null)
            return NotFound();

        var image = await context.Images.FirstOrDefaultAsync(m => m.Id == id);
        return image is null ? NotFound() : View(image);
    }

    public async Task<IActionResult> Thumbnails(int? id)
    {
        if (id is null || context.Images is null)
            return NotFound();

        var image = await context.Images.FirstOrDefaultAsync(m => m.Id == id);
        return image is null
               ? NotFound()
               : File(image.ThumbImage, "image/jpeg");
    }

    public IActionResult Upload() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(ImageViewModel model)
    {
        const int maximumImageFileSize = 2 * 1024 * 1024; // 画像ファイルのサイズは2MBまで
        const int thumbnailSize = 70;

        if (!ModelState.IsValid || context.Images is null)
            return View(model);

        using var memoryStream = new MemoryStream();
        await model.PostedFile.CopyToAsync(memoryStream);

        if (memoryStream.Length > maximumImageFileSize) {
            ModelState.AddModelError("PostedFile", "サイズは 2MB 以下");
            return View(model);
        }
        var byteArray = memoryStream.ToArray();
        var image = new Image {
            FileName = model.Title,
            Description = model.Description ?? "",
            ThumbImage = ImageUtility.CreateThumbnail(byteArray, (thumbnailSize, thumbnailSize)),
            OriginalImage = byteArray
        };
        context.Images.Add(image);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null || context.Images is null)
            return NotFound();

        var image = await context.Images.FindAsync(id);
        return image is null ? NotFound() : View(image);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int? id)
    {
        if (id is null || context.Images is null)
            return NotFound();

        var imageToUpdate = await context.Images
                                         .FirstOrDefaultAsync(f => f.Id == id);
        if (imageToUpdate is not null &&
            await TryUpdateModelAsync<Image>(imageToUpdate, "", image => image.FileName, image => image.Description)) {
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(imageToUpdate);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null || context.Images is null)
            return NotFound();

        var file = await context.Images.AsNoTracking()
                                       .FirstOrDefaultAsync(image => image.Id == id);
        return file is null ? NotFound() : View(file);
    }

    // POST: Images/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (context.Images is not null) {
            var image = await context.Images.FindAsync(id);
            if (image is not null) {
                context.Images.Remove(image);
                await context.SaveChangesAsync();
            }
        }
        return RedirectToAction("Index");
    }
}
