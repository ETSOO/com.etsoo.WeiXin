using System.ComponentModel.DataAnnotations;

namespace com.etsoo.WeiXinService
{
    /// <summary>
    /// Event Alert Dto
    /// 事件警报数据
    /// </summary>
    public class EventAlertDto
    {
        [Required, MinLength(1), MaxLength(10)]
        public string[] Tokens { get; init; } = default!;

        [Required]
        [StringLength(256)]
        public string Description { get; init; } = default!;

        [Required]
        [StringLength(20)]
        public string Id { get; init; } = default!;

        [Required]
        [StringLength(30)]
        public string Status { get; init; } = default!;

        [Required]
        [StringLength(512)]
        public string Remark { get; init; } = default!;

        [Required]
        public DateTime Datetime { get; set; } = default!;
    }
}
