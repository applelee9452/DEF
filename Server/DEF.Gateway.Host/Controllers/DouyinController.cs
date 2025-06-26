using Microsoft.AspNetCore.Mvc;

namespace DEF.Gateway;

[ApiController]
public class DouyinController : ControllerBase
{
	ILogger<DouyinController> Logger { get; set; }
	ServiceClient ServiceClient { get; set; }

	public DouyinController(ILogger<DouyinController> log, ServiceClient serviceClient)
	{
		Logger = log;
		ServiceClient = serviceClient;
	}

	// 测试
	[HttpGet]
	[Route("douyin/get")]
	public IEnumerable<string> Get()
	{
		Logger.LogDebug($"DouyinController.Get()");

		return new List<string>() { "aaaaaaa" };
	}

	// 评论
	[HttpPost]
	[Route("douyin/comment")]
	public async Task WebhookComment([FromQuery] string r)
	{
		int len = (int)Request.ContentLength;
		byte[] buf = new byte[len];
        await Request.Body.ReadExactlyAsync(buf.AsMemory(0, len));
        byte[] method_data = buf;

		// 转换为SerializeObj对象实例，并使用ProboBuf序列化

		await ForwardContainerRpcNoResult(method_data);
	}

	// 点赞
	[HttpPost]
	[Route("douyin/like")]
	public async Task WebhookLike([FromQuery] string r)
	{
		int len = (int)Request.ContentLength;
		byte[] buf = new byte[len];
        await Request.Body.ReadExactlyAsync(buf.AsMemory(0, len));
        byte[] method_data = buf;

		// 转换为SerializeObj对象实例，并使用ProboBuf序列化

		await ForwardContainerRpcNoResult(method_data);
	}

	// 送礼物
	[HttpPost]
	[Route("douyin/gift")]
	public async Task WebhookGift([FromQuery] string r)
	{
		int len = (int)Request.ContentLength;
		byte[] buf = new byte[len];
        await Request.Body.ReadExactlyAsync(buf.AsMemory(0, len));
        byte[] method_data = buf;

		// 转换为SerializeObj对象实例，并使用ProboBuf序列化

		await ForwardContainerRpcNoResult(method_data);
	}

	Task ForwardContainerRpcNoResult(byte[] method_data)
	{
		string service_name = "danmu";
		int containerstate_type = 1;
		string container_type = "room";
		string container_id = string.Empty;
		string method_name = string.Empty;

		//Log.LogInformation($"DouyinController.ClientContainerRpc() Len={len}");

		return ServiceClient.ForwardContainerRpcNoResult(
			service_name, containerstate_type, container_type, container_id, method_name, method_data);
	}
}