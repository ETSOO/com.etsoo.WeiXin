using System.ComponentModel.DataAnnotations;

namespace com.etsoo.WeiXinService
{
    /// <summary>
    /// Log Alert Dto
    /// 日志警报数据
    /// </summary>
    public record LogAlertDto
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

        string service = default!;
        [StringLength(30)]
        public required string Service
        {
            get => service;
            init => service = value.ToMaxLength(30);
        }

        string id = default!;
        [StringLength(30)]
        public required string Id
        {
            get => id;
            init => id = value.ToMaxLength(30);
        }

        string level = default!;
        [StringLength(20)]
        public required string Level
        {
            get => level;
            init => level = value.ToMaxLength(20);
        }

        string message = default!;
        [StringLength(512)]
        public required string Message
        {
            get => message;
            init => message = value.ToMaxLength(512);
        }

        public required DateTimeOffset Datetime { get; init; }
    }
}