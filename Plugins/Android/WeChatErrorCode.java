package com.pluginset.wechatsdk;

public interface WeChatErrorCode {
    int ERR_AUTH_DENIED = -4; // 用户拒绝授权
    int ERR_USER_CANCEL = -2; // 用户取消

    int UnInit = 1; // 未实始化
    int NoInstallation = 2; // 微信未安装
    int InvalidImagePath = 3; // 读取图片失败
    int DoubleOperation = 4; // 重复操作
    int RequestIdNotMatch = 5; // 任务id不一致
    int WechatFailed = 6; // 微信客户端请求失败
    int SwitchedBack = 7; // 用户切回来了
    int RequestIdLost = 100; // 微信把我东西传丢了
    int JSONException = 101; // json异常
}
