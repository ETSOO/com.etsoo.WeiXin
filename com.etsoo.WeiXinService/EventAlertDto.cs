using System.ComponentModel.DataAnnotations;

namespace com.etsoo.WeiXinService
{
    /// <summary>
    /// Event Alert Dto
    /// 事件警报数据
    /// </summary>
    public record EventAlertDto
    {
        string host = default!;
        [Required]
        [StringLength(128)]
        public string Host
        {
            get => host;
            init => host = value.ToMaxLength(128);
        }

        [Required, MinLength(1), MaxLength(10)]
        public string[] Tokens { get; init; } = default!;

        string description = default!;
        [Required]
        [StringLength(512)]
        public string Description
        {
            get => description;
            init => description = value.ToMaxLength(512);
        }

        string id = default!;
        [Required]
        [StringLength(30)]
        public string Id
        {
            get => id;
            init => id = value.ToMaxLength(30);
        }

        string status = default!;
        [Required]
        [StringLength(30)]
        public string Status
        {
            get => status;
            init => status = value.ToMaxLength(30);
        }

        string remark = default!;
        [Required]
        [StringLength(256)]
        public string Remark
        {
            get => remark;
            init => remark = value.ToMaxLength(256);
        }

        string? url;
        [Required]
        [StringLength(256)]
        public string? Url
        {
            get => url;
            init => url = value?.ToMaxLength(256);
        }

        [Required]
        public DateTimeOffset Datetime { get; init; } = default!;
    }
}
