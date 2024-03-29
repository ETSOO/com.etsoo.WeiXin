﻿using com.etsoo.WeiXin;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

string? appData;
do
{
    Console.WriteLine("请输入公众号的AppId和AppSecret，用分号隔开：");
    appData = Console.ReadLine();
} while (string.IsNullOrEmpty(appData));

var parts = appData.Split(';', StringSplitOptions.RemoveEmptyEntries);
if (parts.Length < 2)
{
    Console.Write("*** 无法读取两个参数 ***");
    return;
}

Console.WriteLine("正在处理...");
Console.WriteLine("");

var serviceCollection = new ServiceCollection().AddHttpClient();
var serviceProvider = serviceCollection.BuildServiceProvider();
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>()!;

//using var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));
using var httpClient = httpClientFactory.CreateClient();

var options = new WXClientOptions
{
    AppId = parts[0].Trim(),
    AppSecret = parts[1].Trim(),
    Token = parts.Length > 2 ? parts[2] : null,
    EncodingAESKey = parts.Length > 3 ? parts[3] : null
};
var client = new WXClient(httpClient, options);

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
    //var tagRResult = await client.GetUserTagAsync("oCkMJj86v6J_auePAut2p0AIQy5s");
    //var result = tagRResult.Error;
    //var result = await client.BatchUntagAsync(105, new[] { "oCkMJj86v6J_auePAut2p0AIQy5s" });

    // var result = await client.UpdateTagAsync(105, "接口服务");
    /*
    if (result is not null)
    {
        Console.WriteLine(result.Errmsg + $" - {result.Errcode}");
    }
    */

    /*
    var input = SharedUtils.GetStream(@"<xml>
    <ToUserName><![CDATA[gh_d64e9c6d643e]]></ToUserName>
    <Encrypt><![CDATA[LNLVS1D1P9Nq0DsnDKfz+G7J90+azyxWzV5wXAB7kK2cS0qG2AcKcrYbjepz80Ghs+CpjIkmP4oezp7/c9xFDNlDMBB5hnS2qIf3xHkLPMH+EbjoeQoU/rkzp+6AN9Iwxw1oPa4yaaqRN55kCr0D67S6pG62SkHvCw2i20vcFP1mh6HuSj5PkykILygQ2uQQ45lBsUtjJbZxXZYPmkg1iiuaQijvCxt6+tMvtc8Jl9SSmIBW69tUKDfLROyQ+KjEuGwS5nXUtqwWOhNlrmOh7mmvdsJeLBMInJCKL2nSYaLEKQKyEeDyoaxngKJJbCRLQfzB4kPMvLAjzBQf9Fg1U6JI2A/X2O9Ku4M9DvoSM4Iopkau1GBOA0DUUqWi+Vve5oBcP5C5ZoeqXkbCTAMJ3fSPLGl4yXWlJgPvNcS7QOrjwVtHHuCkc3Yl2+s6eSmGXJtWqUfgOy4FoiCDyzv6PJoeXgynYtgbENKhLiM8/6ZwehPc9MS9f00jOkipbBDk]]></Encrypt>
</xml>");
    var (message, _) = await client.ParseMessageAsync(input, new WXMessageCallbackInput("oCkMJj86v6J_auePAut2p0AIQy5s", "1654919238", "440811030", "15cc73c62f326cd0697ef6adb3424e36519e92c5")
    {
        EncryptType = "aes",
        MsgSignature = "52e19e1860340e86627cfc42cd6fa9e942dbc627"
    });
    Console.WriteLine(message);
    */

    var d1 = JsonSerializer.Deserialize<Dictionary<string, DateTime>>("{\"a\": \"2022-06-11T11:28:13.6114593+08:00\"}");
    var d2 = JsonSerializer.Deserialize<Dictionary<string, DateTimeOffset>>("{\"a\": \"2022-06-11T11:28:13.6114593+08:00\"}");

    Console.WriteLine(JsonSerializer.Serialize(d1));
    Console.WriteLine(JsonSerializer.Serialize(d2));
}
catch (Exception ex)
{
    Console.WriteLine("错误：" + ex.Message);
}