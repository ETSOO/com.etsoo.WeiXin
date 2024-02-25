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
        [StringLength(128)]
        public required string Host
        {
            get => host;
            init => host = value.ToMaxLength(128);
        }

        string[] tokens = default!;
        public required string[] Tokens
        {
            get => tokens;
            set
            {
                if (value.Length is >= 1 and <= 10)
                {
                    tokens = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(Tokens), "The length of tokens should be between 1 and 10.");
                }
            }
        }

        string description = default!;
        [StringLength(512)]
        public required string Description
        {
            get => description;
            init => description = value.ToMaxLength(512);
        }

        string id = default!;
        [StringLength(30)]
        public required string Id
        {
            get => id;
            init => id = value.ToMaxLength(30);
        }

        string status = default!;
        [StringLength(30)]
        public required string Status
        {
            get => status;
            init => status = value.ToMaxLength(30);
        }

        string remark = default!;
        [StringLength(256)]
        public required string Remark
        {
            get => remark;
            init => remark = value.ToMaxLength(256);
        }

        string? url;
        [StringLength(256)]
        public string? Url
        {
            get => url;
            init => url = value?.ToMaxLength(256);
        }

        public required DateTimeOffset Datetime { get; init; }
    }
}
