package com.example.pluginsaars;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.pluginset.wechat.WeChatApi;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {
    private static final String TAG = "PluginSetWechat";

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        WeChatApi inst = WeChatApi.GetInstance();
        inst.OnApiHandle(getIntent(), this);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);

        setIntent(intent);

        WeChatApi inst = WeChatApi.GetInstance();
        inst.OnApiHandle(getIntent(), this);
    }

    @Override
    public void onReq(BaseReq req) {
        switch (req.getType()) {
            case ConstantsAPI.COMMAND_GETMESSAGE_FROM_WX:
                break;
            case ConstantsAPI.COMMAND_SHOWMESSAGE_FROM_WX:
                break;
            default:
                break;
        }
        finish();
    }

    @Override
    public void onResp(BaseResp resp) {
        Log.i(TAG, "WXEntry onResp");
        if (resp.getType() == ConstantsAPI.COMMAND_SUBSCRIBE_MESSAGE) {

        }

        if (resp.getType() == ConstantsAPI.COMMAND_LAUNCH_WX_MINIPROGRAM) {

        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_BUSINESS_VIEW) {

        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_BUSINESS_WEBVIEW) {

        }

        WeChatApi inst = WeChatApi.GetInstance();
        if (resp.getType() == ConstantsAPI.COMMAND_SENDAUTH) {
            SendAuth.Resp authResp = (SendAuth.Resp) resp;
            inst.OnLogin(authResp);
        }
        finish();
    }
}
