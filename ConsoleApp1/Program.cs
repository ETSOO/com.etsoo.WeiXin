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

using var httpClient = new HttpClient();

var client = new WXClient(httpClient, parts[0].Trim(), parts[1].Trim());

try
{
    var result = await client.CreateJsApiSignature("https://www.etsoo.com/");
    Console.WriteLine($"Js Api Ticket: {result}");
}
catch (Exception ex)
{
    Console.WriteLine("错误：" + ex.Message);
}