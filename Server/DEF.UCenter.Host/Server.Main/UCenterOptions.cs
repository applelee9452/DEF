namespace DEF.UCenter;

public class UCenterOptions
{
    public const string Key = "DEF.UCenter";

    public string LogFileName { get; set; } = "UCenter";
    public bool EnableTest { get; set; } = false;
    public bool EnableGuestAccess { get; set; } = false;
    public string ImageContainerName { get; set; } = "{0}/UCenter/Images/";
    public string ImageDefaultProfileImageForMaleBlobName { get; set; } = "default_profile_image_male.jpg";
    public string ImageDefaultProfileThumbnailForMaleBlobName { get; set; } = "default_profile_thumbnail_male.jpg";
    public string ImageDefaultProfileImageForFemaleBlobName { get; set; } = "default_profile_image_female.jpg";
    public string ImageDefaultProfileThumbnailForFemaleBlobName { get; set; } = "default_profile_thumbnail_female.jpg";
    public string ImageProfileImageForBlobNameTemplate { get; set; } = "{0}_image.jpg";
    public string ImageProfileThumbnailForBlobNameTemplate { get; set; } = "{0}_thumbnail.jpg";
    public int ImageMaxThumbnailWidth { get; set; } = 128;
    public int ImageMaxThumbnailHeight { get; set; } = 128;
    public string StorageType { get; set; } = "Storage.Ali";
    public string StorageAliOssAccessKeyId { get; set; } = "";
    public string StorageAliOssAccessKeySecret { get; set; } = "";
    public string StorageAliOssEndpoint { get; set; } = "http://oss-cn-shanghai.aliyuncs.com";
    public string StoragePrimaryStorageConnectionString { get; set; } = "";
    public string StorageSecondaryStorageConnectionString { get; set; } = "";
    public string QCloudSmsAppId { get; set; } = "a";
    public string QCloudSmsAppKey { get; set; } = "b";
    public bool IdCardCheckEnable { get; set; } = true;
    public string IdCardCheckAppCode { get; set; } = "";
    public string CorsEnable { get; set; } = "";
    public string FacebookAppId { get; set; } = "";
    public string FacebookSecret { get; set; } = "";
    public string FacebookAccessToken { get; set; } = "";
    public string FacebookGetAccessTokenUrl { get; set; } = "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials";// {0} 是FacebookAppId; {1}是FacebookSecret
    public string FacebookAuthUrl { get; set; } = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}";// {0}是用户登录获取到的token; {1}是FacebookAppId|FacebookSecret 或者 FacebookAccessToken
    public string FacebookGetUserInfoUrl { get; set; } = "https://graph.facebook.com/{0}?fields=id,name,gender,friends&access_token={1}|{2}";// {0}是user_id；{1}是FacebookAppId|FacebookSecret 或者 FacebookAccessToken
    public string FacebookGetSmallHeadImageUrl { get; set; } = "https://graph.facebook.com/{0}/picture?type=normal";// {0}是user_id
    public string FacebookGetBigHeadImageUrl { get; set; } = "https://graph.facebook.com/{0}/picture?type=large";// {0}是user_id
    public string GooglePlayLoginVerifyUrl { get; set; } = "https://oauth2.googleapis.com/tokeninfo?id_token={0}";// Google登录验证
    public string GooglePlayLoginClientId { get; set; } = "";
    public string GooglePlayLoginClientSecret { get; set; } = "";
    public string GooglePayGetAccessTokenUrl { get; set; } = "https://oauth2.googleapis.com/token";
    public bool AppStoreSandbox { get; set; } = false;// 苹果内购是否开启沙盒模式
    public bool GooglePayTimingGetAccessToken { get; set; } = true;// 谷歌内购是否定时获取AccessToken
    public string GooglePayClientId { get; set; } = "";
    public string GooglePayClientSecret { get; set; } = "";
    public string GooglePayRefreshToken { get; set; } = "";// 这个是固定不变的
    public string GooglePayVerifyUrl { get; set; } = "https://www.googleapis.com/androidpublisher/v3/applications/{0}/purchases/products/{1}/tokens/{2}?access_token={3}";
    public string MeidaPayNotifyUrl { get; set; }
}