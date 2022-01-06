package com.pluginset.wechatsdk;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.util.Log;

import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.modelpay.PayReq;
import com.tencent.mm.opensdk.modelpay.PayResp;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;

import org.json.JSONObject;

import java.util.HashMap;

public class WeChatApi {
    private static final String TAG = "PluginSetWechat";
    private static final String Name = "WeChat";

    private static WeChatApi _instance = null;

    private static final int THUMB_SIZE = 150;

    public static WeChatApi GetInstance() {
        if (_instance == null)
            _instance = new WeChatApi();

        return _instance;
    }

    private final HashMap<String, IWechatCallback> callbackMap = new HashMap<>();
    private String appId;
    private IWXAPI api;
    private long requestId = 0;
    private IWechatCallback loginCall;
    private IWechatCallback payCall;

    private long nextRequestId() {
        return ++requestId;
    }

    @SuppressWarnings("unused")
    public void InitApi(Activity activity, String _appId) {
        appId = _appId;
        regToWx(activity);
    }

    private void regToWx(Activity activity) {
        // 通过WXAPIFactory工厂，获取IWXAPI的实例
        api = WXAPIFactory.createWXAPI(activity, appId, true);

        // 将应用的appId注册到微信
        api.registerApp(appId);

        //建议动态监听微信启动广播进行注册到微信
        activity.registerReceiver(new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                // 将该app注册到微信
                api.registerApp(appId);
            }
        }, new IntentFilter(ConstantsAPI.ACTION_REFRESH_WXAPP));
    }

    @SuppressWarnings("unused")
    public boolean IsWeChatInstalled() {
        if (api == null)
            return false;

        return api.isWXAppInstalled();
    }

    private void execLoginCall(boolean success, int code, String error, JSONObject data) {
        if (loginCall == null) {
            return;
        }
        loginCall.Call(success, Name, data == null ? "{}" : data.toString(), code, error);
        loginCall = null;
    }

    @SuppressWarnings("unused")
    public void Login(Activity activity, final IWechatCallback callback) {
        execLoginCall(false, WeChatErrorCode.DoubleOperation, "another login process is running", null);
        loginCall = callback;
        if (activity == null || api == null) {
            execLoginCall(false, WeChatErrorCode.UnInit, "api not initialized", null);
            return;
        }

        activity.runOnUiThread(() -> {
            if (!api.isWXAppInstalled()) {
                execLoginCall(false, WeChatErrorCode.NoInstallation, "wechat not installed", null);
                return;
            }

            String state = "Login" + nextRequestId();
            final SendAuth.Req req = new SendAuth.Req();
            req.scope = "snsapi_userinfo";
            req.state = state;
            SendRequest(req, state, callback);
        });
    }

    private void SendRequest(BaseReq request, String requestId, IWechatCallback callback) {
        request.transaction = requestId;
        callbackMap.put(requestId, callback);
        api.sendReq(request);
    }

    public void OnApiHandle(Intent intent, IWXAPIEventHandler handler) {
        if (api == null)
            return;

        try {
            api.handleIntent(intent, handler);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void OnLogin(final SendAuth.Resp resp) {
        IWechatCallback callback = GetCallback(resp);
        if (callback != loginCall) {
            execLoginCall(false, WeChatErrorCode.RequestIdLost, "request id lost", null);
            return;
        }

        // check state
        if (resp.state == null || !resp.state.equals(resp.transaction)) {
            execLoginCall(false, WeChatErrorCode.RequestIdNotMatch, "request id lost", null);
            return;
        }

        if (resp.errCode != BaseResp.ErrCode.ERR_OK) {
            execLoginCall(false, WeChatErrorCode.WechatFailed, Integer.toString(resp.errCode), null);
        } else {
            try {
                JSONObject obj = new JSONObject();
                obj.put("code", resp.code);
                obj.put("lang", resp.lang);
                obj.put("country", resp.country);
                execLoginCall(true, 0, "", obj);
            } catch (Exception e) {
                e.printStackTrace();
                execLoginCall(false, WeChatErrorCode.JSONException, "login success but assemble json failed", null);
            }
        }
    }

    private IWechatCallback GetCallback(BaseResp resp) {
        String key = resp.transaction;
        if (callbackMap.containsKey(key))
            return callbackMap.remove(key);

        return null;
    }

    private void execPayCall(boolean success, int code, String error, JSONObject data) {
        if (payCall == null) {
            return;
        }
        payCall.Call(success, Name, data == null ? "{}" : data.toString(), code, error);
        payCall = null;
    }

    @SuppressWarnings("unused")
    public void Pay(Activity activity, final String json, final IWechatCallback callback) {
        Log.i(TAG, "PurchaseManager Native WechatApi.Pay " + json);
        execPayCall(false, WeChatErrorCode.DoubleOperation, "another payment process is running", null);
        payCall = callback;
        if (activity == null || api == null) {
            execPayCall(false, WeChatErrorCode.UnInit, "api not initialized", null);
            return;
        }

        activity.runOnUiThread(() -> {
            if (!api.isWXAppInstalled()) {
                execPayCall(false, WeChatErrorCode.NoInstallation, "wechat not installed", null);
                return;
            }

            try {
                JSONObject obj = new JSONObject(json);
                PayReq request = new PayReq();

                request.appId = appId;
                request.partnerId = obj.getString("partnerId");
                String prepayId = obj.getString("prepayId");
                request.prepayId = prepayId;
                request.packageValue = obj.getString("package");
                request.nonceStr = obj.getString("nonceStr");
                request.timeStamp = obj.getString("timeStamp");
                request.sign = obj.getString("sign");
                String taskId = prepayId + nextRequestId();
                request.extData = taskId;
                SendRequest(request, taskId, callback);
            } catch (Exception e) {
                e.printStackTrace();
            }
        });
    }

    public void OnPay(PayResp resp) {
        IWechatCallback callback = GetCallback(resp);
        if (callback == payCall) {
            execPayCall(false, WeChatErrorCode.RequestIdLost, "pay request id lost", null);
            return;
        }

        if (resp.errCode != BaseResp.ErrCode.ERR_OK) {
            execPayCall(false, WeChatErrorCode.WechatFailed, Integer.toString(resp.errCode), null);
            return;
        }

        try {
            JSONObject obj = new JSONObject();

            obj.put("prepayId", resp.prepayId);
            obj.put("returnKey", resp.returnKey);
            obj.put("extData", resp.extData);

            execPayCall(true, 0, "", obj);
        } catch (Exception e) {
            e.printStackTrace();
            execPayCall(false, WeChatErrorCode.JSONException, "pay success but assemble json failed", null);
        }
    }

    public void onResume() {
        Log.i(TAG, "onResume");
        execLoginCall(false, WeChatErrorCode.SwitchedBack, "switched back", null);
        execPayCall(false, WeChatErrorCode.SwitchedBack, "switched back", null);
    }

    // ------------------- not reviewed ---------------------------
//
//    public void ShareText(String text, boolean isTimeline, IWeChatApiCallback callback) {
//        WXTextObject textObject = new WXTextObject();
//        textObject.text = text;
//
//        WXMediaMessage message = new WXMediaMessage();
//        message.mediaObject = textObject;
//        message.description = text;
//
//        Share(message, isTimeline, callback);
//    }
//
//    public void ShareImage(String path, boolean isTimeline, IWeChatApiCallback callback) {
//        Bitmap bmp = BitmapFactory.decodeFile(path);
//        if (bmp == null) {
//            callback.onFail(WeChatErrorCode.InvalidImagePath);
//            return;
//        }
//
//        WXImageObject imageObject = new WXImageObject(bmp);
//        imageObject.imagePath = path;
//
//        Bitmap thumbData = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
//        bmp.recycle();
//
//        WXMediaMessage message = new WXMediaMessage();
//        message.mediaObject = imageObject;
//        message.thumbData = Util.bmpToByteArray(thumbData, true);
//
//        Share(message, isTimeline, callback);
//    }
//
//    public void ShareMusic(String musicUrl, boolean isTimeline, IWeChatApiCallback callback, String title, String desc, String image) {
//        WXMusicObject musicObject = new WXMusicObject();
//        musicObject.musicUrl = musicUrl;
//
//        WXMediaMessage message = new WXMediaMessage();
//        message.mediaObject = musicObject;
//        if (title != null && title.length() > 0)
//            message.title = title;
//        if (desc != null && desc.length() > 0)
//            message.description = desc;
//
//        byte[] thumb = Util.getThumbData(image, THUMB_SIZE);
//        if (thumb != null)
//            message.thumbData = thumb;
//
//        Share(message, isTimeline, callback);
//    }
//
//    public void ShareVideo(String videoUrl, boolean isTimeline, IWeChatApiCallback callback, String title, String desc, String image, String extra) {
//        WXVideoObject videoObject = new WXVideoObject();
//        videoObject.videoUrl = videoUrl;
//
//        WXMediaMessage message = new WXMediaMessage();
//        message.mediaObject = videoObject;
//        if (title != null && title.length() > 0)
//            message.title = title;
//        if (desc != null && desc.length() > 0)
//            message.description = desc;
//
//        if (extra != null && extra.length() > 0)
//            message.messageAction = "MESSAGE_ACTION_SNS_VIDEO#" + extra;
//
//        byte[] thumb = Util.getThumbData(image, THUMB_SIZE);
//        if (thumb != null)
//            message.thumbData = thumb;
//
//        Share(message, isTimeline, callback);
//    }
//
//    public void ShareVideoFile(String filePath, boolean isTimeline, IWeChatApiCallback callback, String title, String desc, String image, String extra) {
//        WXVideoFileObject videoObject = new WXVideoFileObject();
//        videoObject.filePath = filePath;
//
//        WXMediaMessage message = new WXMediaMessage();
//        message.mediaObject = videoObject;
//        if (title != null && title.length() > 0)
//            message.title = title;
//        if (desc != null && desc.length() > 0)
//            message.description = desc;
//
//        if (extra != null && extra.length() > 0)
//            message.messageAction = "MESSAGE_ACTION_SNS_VIDEO#" + extra;
//
//        byte[] thumb = Util.getThumbData(image, THUMB_SIZE);
//        if (thumb != null)
//            message.thumbData = thumb;
//
//        Share(message, isTimeline, callback);
//    }
//
//    public void ShareWebPage(String url, boolean isTimeline, IWeChatApiCallback callback, String title, String desc, String image) {
//        WXWebpageObject webPageObject = new WXWebpageObject();
//        webPageObject.webpageUrl = url;
//
//        WXMediaMessage message = new WXMediaMessage();
//        message.mediaObject = webPageObject;
//
//        if (title != null && title.length() > 0)
//            message.title = title;
//        if (desc != null && desc.length() > 0)
//            message.description = desc;
//
//        byte[] thumb = Util.getThumbData(image, THUMB_SIZE);
//        if (thumb != null)
//            message.thumbData = thumb;
//
//        Share(message, isTimeline, callback);
//    }
//
//    public void ShareMiniProgram(String url, String path, String appId, IWeChatApiCallback callback, String title, String desc, String image) {
//        WXMiniProgramObject miniProgramObject = new WXMiniProgramObject();
//        miniProgramObject.webpageUrl = url;
//        miniProgramObject.userName = appId;
//        miniProgramObject.path = path;
//
//        WXMediaMessage message = new WXMediaMessage();
//        message.mediaObject = miniProgramObject;
//
//        if (title != null && title.length() > 0)
//            message.title = title;
//        if (desc != null && desc.length() > 0)
//            message.description = desc;
//
//        byte[] thumb = Util.getThumbData(image, THUMB_SIZE);
//        if (thumb != null)
//            message.thumbData = thumb;
//
//        Share(message, false, callback);
//    }
//
//    private void Share(final WXMediaMessage message, final boolean isTimeline, final IWeChatApiCallback callback) {
//        if (activity == null || api == null) {
//            callback.onFail(WeChatErrorCode.UnInit);
//            return;
//        }
//
//        activity.runOnUiThread(new Runnable() {
//            @Override
//            public void run() {
//                if (!api.isWXAppInstalled()) {
//                    callback.onFail(WeChatErrorCode.NoInstallation);
//                    return;
//                }
//
//                SendMessageToWX.Req req = new SendMessageToWX.Req();
//                req.message = message;
//                req.scene = isTimeline ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession;
//                SendRequest(req, "Share" + nextRequestId(), callback);
//            }
//        });
//    }
//
//    public void OnShare(SendMessageToWX.Resp resp) {
//        IWeChatApiCallback callback = GetCallback(resp);
//        if (callback == null) return;
//
//        if (resp.errCode != BaseResp.ErrCode.ERR_OK) {
//            callback.onFail(resp.errCode);
//        } else {
//            callback.onSuccess(null);
//        }
//    }
}
