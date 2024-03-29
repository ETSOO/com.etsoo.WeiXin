﻿namespace com.etsoo.WeiXin.RQ
{
    /// <summary>
    /// Create WeiXin JS siganture request data
    /// 创建微信 Js 接口签名请求数据
    /// </summary>
    public class CreateJsApiSignatureRQ
    {
        /// <summary>
        /// Signed Uri
        /// 签名地址
        /// </summary>
        public required string Url { get; set; }
    }
}