using com.etsoo.WeiXin;
using com.etsoo.WeiXin.Dto;

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

using var httpClient = new HttpClient();

var client = new WXClient(httpClient, parts[0].Trim(), parts[1].Trim());

try
{
    // var result = await client.CreateJsCardApiSignatureAsync("p1Pj9jr90_SQRaVqYI239Ka1erk");
    // Console.WriteLine($"Js Api Ticket: {result}");

    // 发送订阅通知
    var message = new WXSendMessageInput
    {
        Touser = "oCkMJj86v6J_auePAut2p0AIQy5s;oCkMJj9UFdp47JeoL7w2SU5-KnAA",
        TemplateId = "yp_TBs7IVbU5jHmJLrA2pR3HX5GqdM1rvKlEv5IkpY0",
        Page = "https://erp.etsoo.com",
        Data = new Dictionary<string, string>
        {
            ["thing2"] = "Serilog日志",
            ["thing3"] = "严重",
            ["thing4"] = "2020-10-10 12:10:30",
            ["thing5"] = "软件运行故障",
            ["character_string1"] = "E3547"
        }
    };

    var result = await client.SendMessageAsync(message);
    if (result is not null)
    {
        Console.WriteLine(result.Errmsg + $" - {result.Errcode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine("错误：" + ex.Message);
}