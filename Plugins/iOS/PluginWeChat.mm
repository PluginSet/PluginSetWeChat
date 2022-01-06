#import "WXApi.h"

#import "PluginListener.h"
#import "PluginUtils.h"

#import "NSDictionary+SafeUseDictionary.h"

@interface PluginWeChat : NSObject<PluginListener, WXApiDelegate>
@property int _loginRequestId;
@end

REGISTER_PLUGIN_LISTENER(PluginWeChat)

NSString* _u3dGameStr;
NSString* _u3dCallStr;
const char* u3dGameObject;
const char* u3dCallback;

@implementation PluginWeChat

#pragma mark PluginListener
- (void) onOpenURL:(NSNotification *)notification
{
    NSDictionary* notifData = notification.userInfo;
    if (![notifData hasKey:@"url"]) return;
    NSURL * url = notifData[@"url"];
    [WXApi handleOpenURL:url delegate:self];
}

- (void) onContinueUserActivity:(NSNotification *)notification
{
    NSDictionary* notifData = notification.userInfo;
    if (![notifData hasKey:@"activity"]) return;
    NSUserActivity * activity = notifData[@"activity"];
    [WXApi handleOpenUniversalLink:activity delegate:self];
}

#pragma mark WXApiDelegate
- (void)onReq:(BaseReq *)req {
    if ([req isKindOfClass:[ShowMessageFromWXReq class]]) {
        ShowMessageFromWXReq *showMessageReq = (ShowMessageFromWXReq *)req;
        [self managerDidRecvShowMessageReq:showMessageReq];
    } else if ([req isKindOfClass:[LaunchFromWXReq class]]) {
        LaunchFromWXReq *launchReq = (LaunchFromWXReq *)req;
        [self managerDidRecvLaunchFromWXReq:launchReq];
    }
}

- (void)onResp:(BaseResp *)resp {
    if ([resp isKindOfClass:[SendMessageToWXResp class]]) {
        SendMessageToWXResp *messageResp = (SendMessageToWXResp *)resp;
        [self managerDidRecvMessageResponse:messageResp];
    } else if ([resp isKindOfClass:[SendAuthResp class]]) {
        SendAuthResp *authResp = (SendAuthResp *)resp;
        [self managerDidRecvAuthResponse:authResp];
    }else if ([resp isKindOfClass:[WXLaunchMiniProgramResp class]]){
        [self managerDidRecvLaunchMiniProgram:(WXLaunchMiniProgramResp *)resp];
    }
}

- (void)managerDidRecvShowMessageReq:(ShowMessageFromWXReq *)req {
//    WXMediaMessage *msg = req.message;
//
//    //显示微信传过来的内容
//    NSString *strTitle = [NSString stringWithFormat:@"微信请求App显示内容"];
//    NSString *strMsg = nil;
//
//    if ([msg.mediaObject isKindOfClass:[WXAppExtendObject class]]) {
//        WXAppExtendObject *obj = msg.mediaObject;
//        strMsg = [NSString stringWithFormat:@"openID: %@, 标题：%@ \n描述：%@ \n附带信息：%@ \n文件大小:%lu bytes\n附加消息:%@\n", req.openID, msg.title, msg.description, obj.extInfo, (unsigned long)obj.fileData.length, msg.messageExt];
//    }
//    else if ([msg.mediaObject isKindOfClass:[WXTextObject class]]) {
//        WXTextObject *obj = msg.mediaObject;
//        strMsg = [NSString stringWithFormat:@"openID: %@, 标题：%@ \n描述：%@ \n内容：%@\n", req.openID, msg.title, msg.description, obj.contentText];
//    }
//    else if ([msg.mediaObject isKindOfClass:[WXImageObject class]]) {
//        WXImageObject *obj = msg.mediaObject;
//        strMsg = [NSString stringWithFormat:@"openID: %@, 标题：%@ \n描述：%@ \n图片大小:%lu bytes\n", req.openID, msg.title, msg.description, (unsigned long)obj.imageData.length];
//    }
//    else if ([msg.mediaObject isKindOfClass:[WXLocationObject class]]) {
//        WXLocationObject *obj = msg.mediaObject;
//        strMsg = [NSString stringWithFormat:@"openID: %@, 标题：%@ \n描述：%@ \n经纬度：lng:%f_lat:%f\n", req.openID, msg.title, msg.description, obj.lng, obj.lat];
//    }
//    else if ([msg.mediaObject isKindOfClass:[WXFileObject class]]) {
//        WXFileObject *obj = msg.mediaObject;
//        strMsg = [NSString stringWithFormat:@"openID: %@, 标题：%@ \n描述：%@ \n文件类型：%@ 文件大小:%lu\n", req.openID, msg.title, msg.description, obj.fileExtension, (unsigned long)obj.fileData.length];
//    }
//    else if ([msg.mediaObject isKindOfClass:[WXWebpageObject class]]) {
//        WXWebpageObject *obj = msg.mediaObject;
//        strMsg = [NSString stringWithFormat:@"openID: %@, 标题：%@ \n描述：%@ \n网页地址：%@\n", req.openID, msg.title, msg.description, obj.webpageUrl];
//    }
//    [UIAlertView showWithTitle:strTitle message:strMsg sure:nil];
}

- (void)managerDidRecvLaunchFromWXReq:(LaunchFromWXReq *)req {
    WXMediaMessage *msg = req.message;
    
    //从微信启动App
    NSString *strTitle = [NSString stringWithFormat:@"从微信启动"];
    NSString *strMsg = [NSString stringWithFormat:@"openID: %@, messageExt:%@", req.openID, msg.messageExt];
    NSLog(@"%@: %@", strTitle, strMsg);
}

- (void)managerDidRecvAuthResponse:(SendAuthResp *)response {
    int errorCode = response.errCode;
    if (errorCode) {
        [self OnCompleteFailCall:[NSString stringWithFormat:@"wechat error: %d", errorCode]];
    } else {
        [self OnCompleteSuccessCall:response.code];
    }
}

- (void)managerDidRecvMessageResponse:(SendMessageToWXResp *)response {
    NSString *strTitle = [NSString stringWithFormat:@"发送媒体消息结果"];
    NSString *strMsg = [NSString stringWithFormat:@"errcode:%d", response.errCode];
    NSLog(@"%@: %@", strTitle, strMsg);
//    [UIAlertView showWithTitle:strTitle message:strMsg sure:nil];
}

- (void)managerDidRecvLaunchMiniProgram:(WXLaunchMiniProgramResp *)response
{
    NSString *strTitle = [NSString stringWithFormat:@"LaunchMiniProgram结果"];
    NSString *strMsg = [NSString stringWithFormat:@"errMsg:%@,errcode:%d", response.extMsg, response.errCode];
    NSLog(@"%@: %@", strTitle, strMsg);
}

- (void) loginRequest
{
    int requestId = ++self._loginRequestId;
    NSString * requestState = [NSString stringWithFormat:@"request_%d", requestId];
    
    SendAuthReq * request = [SendAuthReq new];
    request.scope = @"snsapi_userinfo";
    request.state = requestState;
    [WXApi sendReq:request completion:^(BOOL success) {
        if (success) {
            NSLog(@"wechat login callback gives success");
        } else {
            NSLog(@"wechat login callback gives failure");
            [self OnCompleteFailCall:@"wechat login callback gives failure"];
        }
    }];
}

-(void)OnCompleteSuccessCall:(NSString*)msg
{
    NSDictionary* receiveMap = @{@"isSuccess":@"true",@"code":msg};
    NSString* jsonStr = [PluginUtils dictToJson:receiveMap];
    const char *p1 = [jsonStr UTF8String];
    [self OnCallUnityC:p1];
}

-(void)OnCompleteFailCall:(NSString*)msg
{
    NSDictionary* receiveMap = @{@"isSuccess":@"false",@"code":msg};
    NSString* jsonStr = [PluginUtils dictToJson:receiveMap];
    const char *p1 = [jsonStr UTF8String];
    [self OnCallUnityC:p1];
}

-(void)OnCallUnityC:(const char*) msg
{
    UnitySendMessage(u3dGameObject,u3dCallback,msg);
}

#if defined(__cplusplus)
extern "C"
{
#endif
    //是否使用微信登陆
    bool IsSupportWechatLogin()
    {
        if (@available(iOS 12.0, *)) {
            return [WXApi isWXAppInstalled] && [WXApi isWXAppSupportApi];
        } else {
            return NO;
        }
    }
    
    void WeChatInit(const char* _appId,const char* universalLink,const char* u3dGameObjectName,const char* u3dCallName)
    {
        NSString* appId = [PluginUtils createNSString:_appId];
        NSString* wx_ul = [PluginUtils createNSString:universalLink];
        _u3dGameStr = [PluginUtils createNSString:u3dGameObjectName];
        _u3dCallStr = [PluginUtils createNSString:u3dCallName];
        u3dGameObject = [_u3dGameStr UTF8String];
        u3dCallback = [_u3dCallStr UTF8String];
        
        [WXApi registerApp:appId universalLink:wx_ul];

        NSLog(@"%@", [NSString stringWithFormat:@"wechat init success. AppID=%@", appId]);
    }
        
    //发起登陆
    void WeChatLogin()
    {
        NSLog(@"called wechat login");

        //判断用户是否已安装微信App
        if ([WXApi isWXAppInstalled]) {
            [[PluginWeChat GetInstance] loginRequest];
        } else {
            NSLog(@"wechat is not installed, cant login");
            [[PluginWeChat GetInstance] OnCompleteFailCall:@"wechat_not_installed"];
        }
    }
    
#if defined(__cplusplus)
}
#endif


@end
