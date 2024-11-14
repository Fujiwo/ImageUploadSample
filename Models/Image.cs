using System.ComponentModel.DataAnnotations;

namespace ImageUploadSample.Models
{
    public class Image
    {
        public int Id { get; set; }

        [Display(Name = "タイトル")]
        [Required(ErrorMessage = "{0} は必須")]
        [StringLength(25)]
        public string FileName { get; set; } = "";

        [Display(Name = "説明")]
        [Required(ErrorMessage = "{0} は必須")]
        [StringLength(250)]
        public string Description { get; set; } = "";

        [Display(Name = "サムネイル画像")]
        [Required]
        public byte[] ThumbImage { get; set; } = [];

        [Required]
        public byte[] OriginalImage { get; set; } = [];
    }
}
