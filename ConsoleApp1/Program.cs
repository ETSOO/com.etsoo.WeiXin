using com.etsoo.WeiXin;

string? appData;
do
{
    Console.WriteLine("请输入公众号的AppId和AppSecret，用分号隔开：");
    appData = Console.ReadLine();
} while (string.IsNullOrEmpty(appData));

var parts = appData.Split(';', StringSplitOptions.RemoveEmptyEntries);
if (parts.Length != 2)
{
    Console.Write("*** 无法读取两个参数 ***");
    return;
}

Console.WriteLine("正在处理...");
Console.WriteLine("");

//using var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));
using var httpClient = new HttpClient();

var client = new WXClient(httpClient, parts[0].Trim(), parts[1].Trim());

try
{
    // 发送订阅通知
    /*
    var message = new WXSendMessageInput
    {
        Touser = "oCkMJj86v6J_auePAut2p0AIQy5s",
        TemplateId = "yp_TBs7IVbU5jHmJLrA2pR3HX5GqdM1rvKlEv5IkpY0",
        Data = new Dictionary<string, WXSendMessageDataItem>
        {
            ["thing2"] = new WXSendMessageDataItem("Serilog日志"),
            ["thing3"] = new WXSendMessageDataItem("严重"),
            ["time5"] = new WXSendMessageDataItem("2020-10-10 12:11:30"),
            ["thing4"] = new WXSendMessageDataItem("软件运行故障"),
            ["character_string1"] = new WXSendMessageDataItem("E3548")
        }
    };
    var result = await client.SendSubscribeMessageAsync(message);
    */

    /*
    var message = new WXSendMessageInput
    {
        Touser = "oCkMJj86v6J_auePAut2p0AIQy5s",
        TemplateId = "4deepMnGbgcZfirs53a5rNCqwuL3vVBX3KWIdPbFKTk",
        Data = new Dictionary<string, WXSendMessageDataItem>
        {
            ["first"] = new WXSendMessageDataItem("Serilog日志，https://www.etsoo.com/api/User/Get，对象为空"),
            ["keyword1"] = new WXSendMessageDataItem("严重", "#ff0000"),
            ["keyword2"] = new WXSendMessageDataItem("2020-10-10 12:11:30"),
            ["remark"] = new WXSendMessageDataItem("请检查日志文件，尽快查明具体原因并修复"),
        }
    };
    var result = await client.SendTemplateMessageAsync(message);
    */

    /*
    var bytes = await SharedUtils.StreamToBytesAsync(File.Open("D:\\test.png", FileMode.Open));
    var uploadResult = await client.UploadMediaAsync("test.png", bytes.ToArray());
    var result = uploadResult.Error;
    if (uploadResult.Success)
    {
        await using var saveStream = File.Create("D:\\a.png");
        var filename = await client.DownloadMediaAsync(uploadResult.Data.MediaId, saveStream);
        Console.WriteLine(filename);
    }
    */

    /*
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = new WXClientJsonNamingPolicy(),
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    var json = JsonSerializer.Serialize(message, options);
    Console.WriteLine(json);

    Console.WriteLine(await client.GetTemplateListAsync());
    */

    /*
    await using var stream = SharedUtils.GetStream();
    await client.GetMenuAsync(stream);
    stream.Position = 0;
    Console.WriteLine(await SharedUtils.StreamToStringAsync(stream));
    */

    // var result = await client.DeleteMenuAsync();
    // var result = await client.CreateMenuAsync("""{"button":[{"name":"亿速","sub_button":[{"type":"view","name":"司友云平台","url":"https://cn.etsoo.com"},{"type":"view","name":"官方网站","url":"https://www.etsoo.com"}]},{"name":"服务","sub_button":[{"type":"click","name":"访问令牌","key":"etsoo_token"}]},{"name":"工具","sub_button":[{"type":"scancode_waitmsg","name":"扫条形码","key":"etsoo_scan_barcode"}]}]}""");

    //var tagResult = await client.CreateTagAsync("服务");
    //var result = tagResult.Error;

    /*
    await using var stream = SharedUtils.GetStream();
    await client.GetTagListAsync(stream);
    stream.Position = 0;
    Console.WriteLine(await SharedUtils.StreamToStringAsync(stream));
    */

    // var result = await client.GetTagUserListAsync(100, null);
    // var result = await client.GetUserTagAsync("oCkMJj86v6J_auePAut2p0AIQy5s");
    var result = await client.BatchUntagAsync(105, new[] { "oCkMJj86v6J_auePAut2p0AIQy5s" });

    // var result = await client.UpdateTagAsync(105, "接口服务");
    if (result is not null)
    {
        Console.WriteLine(result.Errmsg + $" - {result.Errcode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine("错误：" + ex.Message);
}