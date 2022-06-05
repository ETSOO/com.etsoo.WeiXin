namespace com.etsoo.WeiXin.Message
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum WXEventType
    {
        /// <summary>
        /// 关注事件
        /// </summary>
        subscribe,

        /// <summary>
        /// 取消关注
        /// </summary>
        unsubscribe,

        /// <summary>
        /// 用户已关注时的扫码事件
        /// </summary>
        SCAN,

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        LOCATION,

        /// <summary>
        /// 点击菜单项目
        /// </summary>
        CLICK,

        /// <summary>
        /// 浏览链接
        /// </summary>
        VIEW,

        /// <summary>
        /// 扫码推事件的事件推送
        /// </summary>
        scancode_push,

        /// <summary>
        /// 扫码推事件且弹出“消息接收中”提示框的事件推送
        /// </summary>
        scancode_waitmsg,

        /// <summary>
        /// 弹出系统拍照发图的事件推送
        /// </summary>
        pic_sysphoto,

        /// <summary>
        /// 弹出拍照或者相册发图的事件推送
        /// </summary>
        pic_photo_or_album,

        /// <summary>
        /// 弹出微信相册发图器的事件推送
        /// </summary>
        pic_weixin,

        /// <summary>
        /// 弹出地理位置选择器的事件推送
        /// </summary>
        location_select,

        /// <summary>
        /// 点击菜单跳转小程序的事件推送
        /// </summary>
        view_miniprogram,

        /// <summary>
        /// 用户操作订阅通知弹窗
        /// </summary>
        subscribe_msg_popup_event,

        /// <summary>
        /// 用户管理订阅通知
        /// </summary>
        subscribe_msg_change_event,

        /// <summary>
        /// 发送订阅通知
        /// </summary>
        subscribe_msg_sent_event
    }

    /// <summary>
    /// 事件消息
    /// </summary>
    public abstract class WXEventMessage : WXMessage
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public override WXMessageType MsgType => WXMessageType.@event;

        /// <summary>
        /// 事件类型
        /// </summary>
        public abstract WXEventType Event { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dic">字典数据</param>
        public WXEventMessage(Dictionary<string, string>? dic = null) : base(dic)
        {
        }
    }
}
