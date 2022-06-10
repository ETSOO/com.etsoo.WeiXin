using com.etsoo.Utils;
using com.etsoo.WeiXin;
using com.etsoo.WeiXin.Dto;
using com.etsoo.WeiXin.Message;
using System.Text;
using System.Xml.Serialization;

namespace TestProject1
{
    [TestClass]
    public class WXMessageUnitTests
    {
        readonly WXClient client = new(new HttpClient(), "wx5823bf96d3bd56c7", "***", "QDG6eK", "jWmYm7qr5nMoAUwZRjGtBxmz3KA1tkAj3ykkR6q2B2C");
        readonly WXMessageCallbackInput rq = new("abc", "1409659813", "1372623149", "d2157f2f9079f4d6257b45edf665c43c62e60a0a")
        {
            EncryptType = "aes",
            MsgSignatureSnake = "477715d11cdb4164915debcba66cb864d751f3e6"
        };


        [TestMethod]
        public async Task DecryptTests()
        {
            // Arrange
            var input = SharedUtils.GetStream("<xml><ToUserName><![CDATA[wx5823bf96d3bd56c7]]></ToUserName><Encrypt><![CDATA[RypEvHKD8QQKFhvQ6QleEB4J58tiPdvo+rtK1I9qca6aM/wvqnLSV5zEPeusUiX5L5X/0lWfrf0QADHHhGd3QczcdCUpj911L3vg3W/sYYvuJTs3TUUkSUXxaccAS0qhxchrRYt66wiSpGLYL42aM6A8dTT+6k4aSknmPj48kzJs8qLjvd4Xgpue06DOdnLxAUHzM6+kDZ+HMZfJYuR+LtwGc2hgf5gsijff0ekUNXZiqATP7PF5mZxZ3Izoun1s4zG4LUMnvw2r+KqCKIw+3IQH03v+BCA9nMELNqbSf6tiWSrXJB3LAVGUcallcrw8V2t9EL4EhzJWrQUax5wLVMNS0+rUPA3k22Ncx4XXZS9o0MBH27Bo6BpNelZpS+/uh9KsNlY6bHCmJU9p8g7m3fVKn28H3KDYA5Pl/T8Z1ptDAVe0lXdQ2YoyyH2uyPIGHBZZIs2pDBS8R07+qN+E7Q==]]></Encrypt></xml>");

            // Act
            var (message, _) = await client.ParseMessageAsync<WXTextMessage>(input, rq);

            // Assert
            Assert.AreEqual(WXMessageType.text, message?.MsgType);
            Assert.AreEqual("hello", message?.Content);
            Assert.AreEqual(4561255354251345929, message?.MsgId);
        }

        [TestMethod]
        public async Task EncryptTests()
        {
            // Arrange
            var source = "<xml><ToUserName><![CDATA[mycreate]]></ToUserName><FromUserName><![CDATA[wx582测试一下中文的情况，消息长度是按字节来算的396d3bd56c7]]></FromUserName><CreateTime>1348831860</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[this is a test]]></Content><MsgId>1234567890123456</MsgId></xml>";
            var input = SharedUtils.GetStream(source);

            // Act
            var (message, _) = await client.ParseMessageAsync<WXTextMessage>(input, rq);

            // Output
            if (message is not null)
            {
                using var stream = new MemoryStream();
                await client.ReplyMessageAsync(stream, message, (writer) => WXTextMessage.ReplyWithAsync(writer, "reply"));
                stream.Position = 0;

                // Parse
                var dic = await XmlUtils.ParseXmlAsync(stream);
                stream.Position = 0;
                var (sourceMessage, _) = await client.ParseMessageAsync<WXTextMessage>(stream, new WXMessageCallbackInput("abc", dic["TimeStamp"], dic["Nonce"], await client.CreateSignatureAsync(dic["TimeStamp"], dic["Nonce"])) { EncryptType = "aes", MsgSignature = dic["MsgSignature"] });

                Assert.AreEqual(message.FromUserName, sourceMessage?.ToUserName);
                Assert.AreEqual("reply", sourceMessage?.Content);
            }
        }

        [TestMethod]
        public async Task ParseTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
                  <ToUserName><![CDATA[toUser]]></ToUserName>
                  <FromUserName><![CDATA[fromUser]]></FromUserName>
                  <CreateTime>1348831860</CreateTime>
                  <MsgType><![CDATA[text]]></MsgType>
                  <Content><![CDATA[this is a test]]></Content>
                  <MsgId>1234567890123456</MsgId>
                  <MsgDataId>xxxx</MsgDataId>
                </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXTextMessage));
            var message = s.Deserialize(input) as WXTextMessage;

            if (message !=null)
            {
                using var ms = new MemoryStream();
                await message.ReplyAsync(ms, (writer) =>
                {
                    return WXImageMessage.ReplyWithAsync(writer, "media_id");
                });
                var result = Encoding.UTF8.GetString(ms.ToArray());

                Assert.IsTrue(result.Contains("<FromUserName><![CDATA[toUser]]></FromUserName>"));
                Assert.IsTrue(result.Contains("<Image><MediaId><![CDATA[media_id]]></MediaId></Image>"));
            }

            // Assert
            Assert.AreEqual(WXMessageType.text, message?.MsgType);
            Assert.AreEqual("this is a test", message?.Content);
        }

        [TestMethod]
        public async Task ImageMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[fromUser]]></FromUserName>
              <CreateTime>1348831860</CreateTime>
              <MsgType><![CDATA[image]]></MsgType>
              <PicUrl><![CDATA[this is a url]]></PicUrl>
              <MediaId><![CDATA[media_id]]></MediaId>
              <MsgId>1234567890123456</MsgId>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXImageMessage));
            var m1 = s.Deserialize(input) as WXImageMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXImageMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.MediaId, m2?.MediaId);
            Assert.AreEqual("media_id", m1?.MediaId);
            Assert.AreEqual("this is a url", m2?.PicUrl);
        }

        [TestMethod]
        public async Task VoiceMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[fromUser]]></FromUserName>
              <CreateTime>1357290913</CreateTime>
              <MsgType><![CDATA[voice]]></MsgType>
              <MediaId><![CDATA[media_id]]></MediaId>
              <Format><![CDATA[Format]]></Format>
              <Recognition><![CDATA[腾讯微信团队]]></Recognition>
              <MsgId>1234567890123456</MsgId>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXVoiceMessage));
            var m1 = s.Deserialize(input) as WXVoiceMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXVoiceMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.MediaId, m2?.MediaId);
            Assert.AreEqual("media_id", m1?.MediaId);
            Assert.AreEqual("Format", m2?.Format);
            Assert.AreEqual("腾讯微信团队", m2?.Recognition);
        }

        [TestMethod]
        public async Task ShortVideoMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[fromUser]]></FromUserName>
              <CreateTime>1357290913</CreateTime>
              <MsgType><![CDATA[shortvideo]]></MsgType>
              <MediaId><![CDATA[media_id]]></MediaId>
              <ThumbMediaId><![CDATA[thumb_media_id]]></ThumbMediaId>
              <MsgId>1234567890123456</MsgId>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXShortVideoMessage));
            var m1 = s.Deserialize(input) as WXShortVideoMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXShortVideoMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.MediaId, m2?.MediaId);
            Assert.AreEqual("media_id", m1?.MediaId);
            Assert.AreEqual("thumb_media_id", m2?.ThumbMediaId);
        }

        [TestMethod]
        public async Task LocationMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
                <ToUserName><![CDATA[toUser]]></ToUserName>
                <FromUserName><![CDATA[fromUser]]></FromUserName>
                <CreateTime>1351776360</CreateTime>
                <MsgType><![CDATA[location]]></MsgType>
                <Location_X>23.134521</Location_X>
                <Location_Y>113.358803</Location_Y>
                <Scale>20</Scale>
                <Label><![CDATA[位置信息]]></Label>
                <MsgId>1234567890123456</MsgId>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXLocationMessage));
            var m1 = s.Deserialize(input) as WXLocationMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXLocationMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.LocationX, m2?.LocationX);
            Assert.AreEqual(23.134521M, m1?.LocationX);
            Assert.AreEqual(113.358803M, m2?.LocationY);
            Assert.AreEqual(20, m2?.Scale);
            Assert.AreEqual("位置信息", m2?.Label);
        }

        [TestMethod]
        public async Task LinkMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[fromUser]]></FromUserName>
              <CreateTime>1351776360</CreateTime>
              <MsgType><![CDATA[link]]></MsgType>
              <Title><![CDATA[公众平台官网链接]]></Title>
              <Description><![CDATA[公众平台官网链接]]></Description>
              <Url><![CDATA[url]]></Url>
              <MsgId>1234567890123456</MsgId>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXLinkMessage));
            var m1 = s.Deserialize(input) as WXLinkMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXLinkMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.Title, m2?.Title);
            Assert.AreEqual("公众平台官网链接", m1?.Title);
            Assert.AreEqual("公众平台官网链接", m2?.Description);
            Assert.AreEqual("url", m2?.Url);
        }

        [TestMethod]
        public async Task SubscribeEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[FromUser]]></FromUserName>
              <CreateTime>123456789</CreateTime>
              <MsgType><![CDATA[event]]></MsgType>
              <Event><![CDATA[subscribe]]></Event>
              <EventKey><![CDATA[qrscene_123123]]></EventKey>
              <Ticket><![CDATA[TICKET]]></Ticket>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXSubscribeEventMessage));
            var m1 = s.Deserialize(input) as WXSubscribeEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXSubscribeEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("qrscene_123123", m1?.EventKey);
            Assert.AreEqual("TICKET", m2?.Ticket);
            Assert.AreEqual("123123", m2?.SceneId);
        }

        [TestMethod]
        public async Task ScanEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[FromUser]]></FromUserName>
              <CreateTime>123456789</CreateTime>
              <MsgType><![CDATA[event]]></MsgType>
              <Event><![CDATA[SCAN]]></Event>
              <EventKey><![CDATA[SCENE_VALUE]]></EventKey>
              <Ticket><![CDATA[TICKET]]></Ticket>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXScanEventMessage));
            var m1 = s.Deserialize(input) as WXScanEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXScanEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("SCENE_VALUE", m1?.EventKey);
            Assert.AreEqual("TICKET", m2?.Ticket);
            Assert.AreEqual(123456789, m2?.CreateTime);
        }

        [TestMethod]
        public async Task LocationEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[fromUser]]></FromUserName>
              <CreateTime>123456789</CreateTime>
              <MsgType><![CDATA[event]]></MsgType>
              <Event><![CDATA[LOCATION]]></Event>
              <Latitude>23.137466</Latitude>
              <Longitude>113.352425</Longitude>
              <Precision>119.385040</Precision>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXLocationEventMessage));
            var m1 = s.Deserialize(input) as WXLocationEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXLocationEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.Latitude, m2?.Latitude);
            Assert.AreEqual(23.137466M, m1?.Latitude);
            Assert.AreEqual(113.352425M, m2?.Longitude);
            Assert.AreEqual(119.385040M, m2?.Precision);
        }

        [TestMethod]
        public async Task ClickEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[FromUser]]></FromUserName>
              <CreateTime>123456789</CreateTime>
              <MsgType><![CDATA[event]]></MsgType>
              <Event><![CDATA[CLICK]]></Event>
              <EventKey><![CDATA[EVENTKEY]]></EventKey>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXClickEventMessage));
            var m1 = s.Deserialize(input) as WXClickEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXClickEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("EVENTKEY", m1?.EventKey);
        }

        [TestMethod]
        public async Task ViewEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
              <ToUserName><![CDATA[toUser]]></ToUserName>
              <FromUserName><![CDATA[FromUser]]></FromUserName>
              <CreateTime>123456789</CreateTime>
              <MsgType><![CDATA[event]]></MsgType>
              <Event><![CDATA[VIEW]]></Event>
              <EventKey><![CDATA[www.qq.com]]></EventKey>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXViewEventMessage));
            var m1 = s.Deserialize(input) as WXViewEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXViewEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("www.qq.com", m1?.EventKey);
        }

        [TestMethod]
        public async Task ScanCodeEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
                <FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
                <CreateTime>1408090502</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[scancode_push]]></Event>
                <EventKey><![CDATA[6]]></EventKey>
                <ScanCodeInfo><ScanType><![CDATA[qrcode]]></ScanType>
                <ScanResult><![CDATA[1]]></ScanResult>
                </ScanCodeInfo>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXScanCodeEventMessage));
            var m1 = s.Deserialize(input) as WXScanCodeEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXScanCodeEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("6", m1?.EventKey);
            Assert.AreEqual("1", m1?.ScanCodeInfo.ScanResult);
            Assert.AreEqual("1", m2?.ScanCodeInfo.ScanResult);
            Assert.AreEqual("qrcode", m2?.ScanCodeInfo.ScanType);
        }

        [TestMethod]
        public async Task ScanCodeWaitEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
                <FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
                <CreateTime>1408090606</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[scancode_waitmsg]]></Event>
                <EventKey><![CDATA[6]]></EventKey>
                <ScanCodeInfo><ScanType><![CDATA[qrcode]]></ScanType>
                <ScanResult><![CDATA[2]]></ScanResult>
                </ScanCodeInfo>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXScanCodeWaitEventMessage));
            var m1 = s.Deserialize(input) as WXScanCodeWaitEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXScanCodeWaitEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("6", m1?.EventKey);
            Assert.AreEqual("2", m1?.ScanCodeInfo.ScanResult);
            Assert.AreEqual("2", m2?.ScanCodeInfo.ScanResult);
            Assert.AreEqual("qrcode", m2?.ScanCodeInfo.ScanType);
        }

        [TestMethod]
        public async Task SysPhotoEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
                <FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
                <CreateTime>1408090651</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[pic_sysphoto]]></Event>
                <EventKey><![CDATA[6]]></EventKey>
                <SendPicsInfo><Count>1</Count>
                <PicList><item><PicMd5Sum><![CDATA[1b5f7c23b5bf75682a53e7b6d163e185]]></PicMd5Sum></item></PicList>
                </SendPicsInfo>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXSysPhotoEventMessage));
            var m1 = s.Deserialize(input) as WXSysPhotoEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXSysPhotoEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("6", m1?.EventKey);
            Assert.AreEqual(1, m2?.SendPicsInfo.Count);
            Assert.AreEqual("1b5f7c23b5bf75682a53e7b6d163e185", m2?.SendPicsInfo.PicList.First().PicMd5Sum);
        }

        [TestMethod]
        public async Task AlbumPhotoEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
                <FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
                <CreateTime>1408090816</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[pic_photo_or_album]]></Event>
                <EventKey><![CDATA[6]]></EventKey>
                <SendPicsInfo><Count>1</Count>
                <PicList><item><PicMd5Sum><![CDATA[5a75aaca956d97be686719218f275c6b]]></PicMd5Sum>
                </item>
                </PicList>
                </SendPicsInfo>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXAlbumPhotoEventMessage));
            var m1 = s.Deserialize(input) as WXAlbumPhotoEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXAlbumPhotoEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("6", m1?.EventKey);
            Assert.AreEqual(1, m2?.SendPicsInfo.Count);
            Assert.AreEqual("5a75aaca956d97be686719218f275c6b", m2?.SendPicsInfo.PicList.First().PicMd5Sum);
        }

        [TestMethod]
        public async Task WeiXinPhotoEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
                <FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
                <CreateTime>1408090816</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[pic_weixin]]></Event>
                <EventKey><![CDATA[7]]></EventKey>
                <SendPicsInfo><Count>1</Count>
                <PicList><item><PicMd5Sum><![CDATA[5a75aaca956d97be686719218f275c6c]]></PicMd5Sum>
                </item>
                </PicList>
                </SendPicsInfo>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXWeiXinPhotoEventMessage));
            var m1 = s.Deserialize(input) as WXWeiXinPhotoEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXWeiXinPhotoEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("7", m1?.EventKey);
            Assert.AreEqual(1, m2?.SendPicsInfo.Count);
            Assert.AreEqual("5a75aaca956d97be686719218f275c6c", m2?.SendPicsInfo.PicList.First().PicMd5Sum);
        }

        [TestMethod]
        public async Task LocationSelectEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
                <FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
                <CreateTime>1408091189</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[location_select]]></Event>
                <EventKey><![CDATA[6]]></EventKey>
                <SendLocationInfo><Location_X><![CDATA[23]]></Location_X>
                <Location_Y><![CDATA[113]]></Location_Y>
                <Scale><![CDATA[15]]></Scale>
                <Label><![CDATA[广州市海珠区客村艺苑路 106号]]></Label>
                <Poiname><![CDATA[]]></Poiname>
                </SendLocationInfo>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXLocationSelectEventMessage));
            var m1 = s.Deserialize(input) as WXLocationSelectEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXLocationSelectEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("6", m1?.EventKey);
            Assert.AreEqual(23M, m2?.SendLocationInfo.LocationX);
            Assert.AreEqual(113M, m2?.SendLocationInfo.LocationY);
            Assert.AreEqual(15, m2?.SendLocationInfo.Scale);
            Assert.AreEqual("广州市海珠区客村艺苑路 106号", m2?.SendLocationInfo.Label);
            Assert.AreEqual("", m2?.SendLocationInfo.Poiname);
        }

        [TestMethod]
        public async Task ViewMiniprogramEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
                <ToUserName><![CDATA[toUser]]></ToUserName>
                <FromUserName><![CDATA[FromUser]]></FromUserName>
                <CreateTime>123456789</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[view_miniprogram]]></Event>
                <EventKey><![CDATA[pages/index/index]]></EventKey>
                <MenuId>MENUID</MenuId>
             </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXViewMiniprogramEventMessage));
            var m1 = s.Deserialize(input) as WXViewMiniprogramEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXViewMiniprogramEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.EventKey, m2?.EventKey);
            Assert.AreEqual("pages/index/index", m1?.EventKey);
            Assert.AreEqual("MENUID", m2?.MenuId);
        }

        [TestMethod]
        public async Task SubscribePopupEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
                <ToUserName><![CDATA[gh_123456789abc]]></ToUserName>
                <FromUserName><![CDATA[otFpruAK8D-E6EfStSYonYSBZ8_4]]></FromUserName>
                <CreateTime>1610969440</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[subscribe_msg_popup_event]]></Event>
                <SubscribeMsgPopupEvent>
                    <List>
                        <TemplateId><![CDATA[VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc]]></TemplateId>
                        <SubscribeStatusString><![CDATA[accept]]></SubscribeStatusString>
                        <PopupScene>2</PopupScene>
                    </List>
                    <List>
                        <TemplateId><![CDATA[9nLIlbOQZC5Y89AZteFEux3WCXRRRG5Wfzkpssu4bLI]]></TemplateId>
                        <SubscribeStatusString><![CDATA[reject]]></SubscribeStatusString>
                        <PopupScene>2</PopupScene>
                    </List>
                </SubscribeMsgPopupEvent>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXSubscribePopupEventMessage));
            var m1 = s.Deserialize(input) as WXSubscribePopupEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXSubscribePopupEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.FromUserName, m2?.FromUserName);
            Assert.AreEqual("otFpruAK8D-E6EfStSYonYSBZ8_4", m1?.FromUserName);
            Assert.AreEqual(2, m2?.SubscribeMsgPopupEvent.Length);
            Assert.AreEqual("VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc", m2?.SubscribeMsgPopupEvent[0].TemplateId);
            Assert.AreEqual("reject", m2?.SubscribeMsgPopupEvent[1].SubscribeStatusString);
            Assert.AreEqual(2, m2?.SubscribeMsgPopupEvent[1].PopupScene);
        }

        [TestMethod]
        public async Task SubscribeManageEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
                <ToUserName><![CDATA[gh_123456789abc]]></ToUserName>
                <FromUserName><![CDATA[otFpruAK8D-E6EfStSYonYSBZ8_4]]></FromUserName>
                <CreateTime>1610969440</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[subscribe_msg_change_event]]></Event>
                <SubscribeMsgChangeEvent>
                    <List>
                        <TemplateId><![CDATA[VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc]]></TemplateId>
                        <SubscribeStatusString><![CDATA[reject]]></SubscribeStatusString>
                    </List>
                </SubscribeMsgChangeEvent>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXSubscribeManageEventMessage));
            var m1 = s.Deserialize(input) as WXSubscribeManageEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXSubscribeManageEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.FromUserName, m2?.FromUserName);
            Assert.AreEqual("otFpruAK8D-E6EfStSYonYSBZ8_4", m1?.FromUserName);
            Assert.AreEqual(1, m2?.SubscribeMsgChangeEvent.Length);
            Assert.AreEqual("VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc", m2?.SubscribeMsgChangeEvent[0].TemplateId);
            Assert.AreEqual("reject", m2?.SubscribeMsgChangeEvent[0].SubscribeStatusString);
        }

        [TestMethod]
        public async Task SubscribeSendEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml>
                <ToUserName><![CDATA[gh_123456789abc]]></ToUserName>
                <FromUserName><![CDATA[otFpruAK8D-E6EfStSYonYSBZ8_4]]></FromUserName>
                <CreateTime>1610969468</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[subscribe_msg_sent_event]]></Event>
                <SubscribeMsgSentEvent>
                    <List>
                        <TemplateId><![CDATA[VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc]]></TemplateId>
                        <MsgID>1700827132819554304</MsgID>
                        <ErrorCode>1</ErrorCode>
                        <ErrorStatus><![CDATA[error]]></ErrorStatus>
                    </List>
                </SubscribeMsgSentEvent>
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXSubscribeSendEventMessage));
            var m1 = s.Deserialize(input) as WXSubscribeSendEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXSubscribeSendEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.ToUserName, m2?.ToUserName);
            Assert.AreEqual("gh_123456789abc", m1?.ToUserName);
            Assert.AreEqual(1, m2?.SubscribeMsgSentEvent.Length);
            Assert.AreEqual("VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc", m2?.SubscribeMsgSentEvent[0].TemplateId);
            Assert.AreEqual("1700827132819554304", m2?.SubscribeMsgSentEvent[0].MsgID);
            Assert.AreEqual(1, m2?.SubscribeMsgSentEvent[0].ErrorCode);
            Assert.AreEqual("error", m2?.SubscribeMsgSentEvent[0].ErrorStatus);
        }

        [TestMethod]
        public async Task TemplateSendEventMessageTests()
        {
            // Arrange
            var input = SharedUtils.GetStream(@"<xml> 
              <ToUserName><![CDATA[gh_7f083739789a]]></ToUserName>  
              <FromUserName><![CDATA[oia2TjuEGTNoeX76QEjQNrcURxG8]]></FromUserName>  
              <CreateTime>1395658984</CreateTime>  
              <MsgType><![CDATA[event]]></MsgType>  
              <Event><![CDATA[TEMPLATESENDJOBFINISH]]></Event>  
              <MsgID>200163840</MsgID>  
              <Status><![CDATA[failed:user block]]></Status> 
            </xml>");

            // Act
            var s = new XmlSerializer(typeof(WXTemplateSendEventMessage));
            var m1 = s.Deserialize(input) as WXTemplateSendEventMessage;

            input.Position = 0;
            var (m2, _) = await client.ParseMessageAsync<WXTemplateSendEventMessage>(input, rq);

            // Assert
            Assert.AreEqual(m1?.MsgID, m2?.MsgID);
            Assert.AreEqual(200163840, m1?.MsgID);
            Assert.AreEqual("failed:user block", m2?.Status);
            Assert.IsFalse(m2?.Success);
        }
    }
}