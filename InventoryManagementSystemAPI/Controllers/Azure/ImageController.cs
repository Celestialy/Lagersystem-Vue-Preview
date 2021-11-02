using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using InventoryManagementSystemAPI.Database;
using InventoryManagementSystemAPI.DTOs;
using InventoryManagementSystemAPI.Helpers;
using InventoryManagementSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InventoryManagementSystemAPI.Controllers.Azure
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, InventoryManager")]
    public class ImageController : Controller
    {

        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;
        private BlobServiceClient _client;
        public ImageController(DatabaseContext context, UserManager<UserModel> userManager, BlobServiceClient client)
        {
            this._context = context;
            this._userManager = userManager;
            _client = client;
        }

        // GET: /api/image
        [HttpGet]
        [Route("get_all_images")]
        public async Task<IActionResult> GetAllImages([FromQuery]ImageModeDTO getImageMode)
        {
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;

            if (!_context.Departments.Any(x => x.Id == department.Id))
                return NotFound("Department not found");

            switch (getImageMode)
            {
                case ImageModeDTO.standard:
                    {
                        var images = await _context.Images.Where(x => x.DepartmentId == department.Id).Select(x => new ImageResponseDTO
                        {
                            ImageId = x.Id,
                            ImageName = x.ImageName,
                            ImageUri = x.ImageUri,
                        }).ToListAsync();

                        if (images.Count < 1)
                            return NotFound("No images found");

                        return Ok(images);
                    }
                case ImageModeDTO.WitchCheck:
                    {
                        var images = await _context.Images.Where(x => x.DepartmentId == department.Id).Select(x => new ImageWithIsUsedResponseDTO
                        {
                            ImageId = x.Id,
                            ImageName = x.ImageName,
                            ImageUri = x.ImageUri,
                            IsUsed = _context.Items.Any(z => z.Image == x)
                        }).ToListAsync();

                        if (images.Count < 1)
                            return NotFound("No images found");

                        return Ok(images);
                    }
                default:
                    return BadRequest("invalidMode");
            }
        }

        [HttpGet]
        [Route("get_all_images_with_check")]
        public async Task<IActionResult> GetAllWithCheckImages()
        {
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;

            if (!_context.Departments.Any(x => x.Id == department.Id))
                return NotFound("Department not found");

            var images = await _context.Images.Where(x => x.DepartmentId == department.Id).Select(x => new ImageWithIsUsedResponseDTO
            {
                ImageId = x.Id,
                ImageName = x.ImageName,
                ImageUri = x.ImageUri,
                IsUsed = _context.Items.Any(z => z.Image == x)
            }).ToListAsync();

            if (images.Count < 1)
                return NotFound("No images found");

            return Ok(images);
        }

        // GET: /api/image
        [HttpGet]
        [Route("get_image")]
        public async Task<IActionResult> GetImage([FromQuery] GetImageDTO getImageDTO)
        {
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;

            if (!_context.Departments.Any(x => x.Id == department.Id))
                return NotFound("Department not found");

            if (!_context.Images.Any(x => x.Id == getImageDTO.ImageId))
                return NotFound("Image not found");

            var image = await _context.Images.Where(x => x.Id == getImageDTO.ImageId && x.DepartmentId == department.Id).Select(x => new ImageResponseDTO
            {
                ImageId = x.Id,
                ImageName = x.ImageName,
                ImageUri = x.ImageUri,
            }).FirstOrDefaultAsync();

            return Ok(image);
        }

        // POST: /api/image
        [HttpPost]
        [Route("upload_images")]
        public async Task<IActionResult> UploadImages([FromForm] IFormFile file, [FromForm] AddImageDTO addImageDTO)
        {
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;

            if (!_context.Departments.Any(x => x.Id == department.Id))
                return NotFound("Department not found");

            if (addImageDTO.ImageName == "" || addImageDTO.ImageName == null)
            {
                return BadRequest("No image name");
            }

            bool isUploaded = false;

            try
            {
                if (file == null)
                    return BadRequest("No file received from the upload");

                if (_client.AccountName == string.Empty)
                    return BadRequest("azure not connected");


                if (department.Name.ToLower() == string.Empty)
                    return BadRequest("Please provide a name for your image container in the azure blob storage");


                StorageHelper storageHelper = new StorageHelper(_client);
                Uri imageUri = new Uri("http://example.com");

                if (storageHelper.IsImage(file))
                {
                    if (file.Length > 0)
                    {
                        imageUri = await storageHelper.UploadFileToStorage(file.FileName, file.ContentType, department.Name.ToLower());

                        if (imageUri != null)
                            isUploaded = true;
                    }
                }
                else
                    return new UnsupportedMediaTypeResult();

                if (isUploaded)
                {
                    if (imageUri.Equals(new Uri("http://example.com")) == false)
                    {
                        bool nameExists = _context.Images.Any(x => x.ImageName == addImageDTO.ImageName && x.DepartmentId == department.Id);
                        bool imageExists = _context.Images.Any(x => x.ImageUri == imageUri && x.DepartmentId == department.Id);

                        if (nameExists && imageExists)
                            return BadRequest("Billedet findes allerede");
                        else if (nameExists)
                            return BadRequest("Name already exists");
                        else if (imageExists)
                            return BadRequest($"Billedet findes allerede, navnet er '{_context.Images.FirstOrDefault(x => x.ImageUri == imageUri && x.DepartmentId == department.Id).ImageName}'");
                        else
                        {
                            using Stream stream = file.OpenReadStream();

                            await storageHelper.UploadImage(stream);
                        }

                        var image = await UploadImageToDB(addImageDTO.ImageName, imageUri, department);

                        return Ok(image);
                    }
                    else
                        return BadRequest("Couldn't upload image to the server");
                }
                else
                    return BadRequest("Look like the image couldnt upload to the storage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<(bool, string)> testMethod()
        {
            await Task.Delay(1);

            return (true, "TestString");
        }

        private async Task<ImageModel> UploadImageToDB(string imageName, Uri imageUri, DepartmentModel department)
        {
            try
            {
                if (imageName == null)
                    return null;

                ImageModel image = new ImageModel()
                {
                    ImageName = imageName,
                    ImageUri = imageUri,
                    DepartmentId = department.Id,
                    CreatedAt = DateTime.Now
                };

                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // DELETE: /api/image
        [HttpDelete]
        [Route("delete_images")]
        public async Task<IActionResult> DeleteImages([FromQuery] GetImagesDTO getImagesDTO)
        {
            var user = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User));

            if (!_context.Departments.Any(x => x.Id == user.Department.Id))
                return NotFound("Department not found");

            if (!_context.Images.Any(x => getImagesDTO.ImageIdList.Contains(x.Id)))
                return NotFound("Image not found");

            var images = await _context.Images.Where(x => getImagesDTO.ImageIdList.Contains(x.Id)).ToListAsync();

            bool isDeleted = false;

            try
            {
                if (_client.AccountName == string.Empty)
                    return BadRequest("azure not connected");


                if (user.Department.Name.ToLower() == string.Empty)
                    return BadRequest("Please provide a name for your image container in the azure blob storage");

                StorageHelper storageHelper = new StorageHelper(_client);

                foreach (var image in images)
                {
                    if (_context.LoanItems.Include(i => i.Image).Any(x => x.Image.Id == image.Id) ||
                        _context.ConsumptionItems.Include(i => i.Image).Any(x => x.Image.Id == image.Id) ||
                        _context.UserLoans.Any(x => x.LoanItem.Item.Image.Id == image.Id) ||
                        _context.UserConsumptions.Any(x => x.ConsumptionItem.Item.Image.Id == image.Id))
                        return Conflict("Image is in use");

                    isDeleted = await storageHelper.DeleteBlob(image.ImageUri);
                    _context.Remove(image);
                }

                if (isDeleted)
                {
                    await _context.SaveChangesAsync();
                    return Ok("Images Deleted succesfully");
                }
                else
                    return BadRequest("Look like the image couldnt upload to the storage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            /*
            var images = await _context.Images.Where(x => getImageDTO.ImageIdList.Contains(x.Id)).ToListAsync();
            for (int i = 0; i < getImageDTO.ImageIdList.Count; i++)
            {
                var image = await _context.Images.FirstOrDefaultAsync(x => getImageDTO.ImageIdList.Contains(x.Id));

                _context.Remove(image);
            }
            */
        }
    }
}