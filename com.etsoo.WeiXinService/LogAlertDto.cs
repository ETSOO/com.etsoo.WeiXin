using System.ComponentModel.DataAnnotations;

namespace com.etsoo.WeiXinService
{
    /// <summary>
    /// Log Alert Dto
    /// 日志警报数据
    /// </summary>
    public class LogAlertDto
    {
        [Required, MinLength(1), MaxLength(10)]
        public string[] Tokens { get; init; } = default!;

        [Required]
        [StringLength(30)]
        public string Service { get; init; } = default!;

        [Required]
        [StringLength(20)]
        public string Id { get; init; } = default!;

        [Required]
        [StringLength(20)]
        public string Level { get; init; } = default!;

        [Required]
        [StringLength(512)]
        public string Message { get; init; } = default!;

        [Required]
        public DateTime Datetime { get; set; } = default!;
    }
}