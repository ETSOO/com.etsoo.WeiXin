using System.ComponentModel.DataAnnotations;

namespace com.etsoo.WeiXinService
{
    /// <summary>
    /// Log Alert Dto
    /// 日志警报数据
    /// </summary>
    public record LogAlertDto
    {
        [Required, MinLength(1), MaxLength(10)]
        public string[] Tokens { get; init; } = default!;

        string service = default!;
        [Required]
        [StringLength(30)]
        public string Service
        {
            get => service;
            init => service = value.ToMaxLength(30);
        }

        string id = default!;
        [Required]
        [StringLength(30)]
        public string Id
        {
            get => id;
            init => id = value.ToMaxLength(30);
        }

        string level = default!;
        [Required]
        [StringLength(20)]
        public string Level
        {
            get => level;
            init => level = value.ToMaxLength(20);
        }

        string message = default!;
        [Required]
        [StringLength(512)]
        public string Message
        {
            get => message;
            init => message = value.ToMaxLength(512);
        }

        [Required]
        public DateTimeOffset Datetime { get; init; } = default!;
    }
}