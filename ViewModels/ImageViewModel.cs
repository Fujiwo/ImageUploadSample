using System.ComponentModel.DataAnnotations;

namespace ImageUploadSample.ViewModels
{
    public class ImageViewModel
    {
        [Display(Name = "タイトル")]
        [Required(ErrorMessage = "{0} は必須")]
        [StringLength(25, ErrorMessage = "{0} は {1} 文字以内")]
        public string Title { get; set; } = "";

        [Display(Name = "説明")]
        [Required(ErrorMessage = "{0} は必須")]
        [StringLength(250, ErrorMessage = "{0} は {1} 文字以内")]
        public string Description { get; set; } = "";

        [Display(Name = "ファイル")]
        [Required(ErrorMessage = "{0} は必須")]
        public IFormFile PostedFile { get; set; } = null!;
    }
}
