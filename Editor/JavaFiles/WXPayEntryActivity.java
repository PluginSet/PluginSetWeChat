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
import com.tencent.mm.opensdk.modelpay.PayResp;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;

public class WXPayEntryActivity extends Activity implements IWXAPIEventHandler {
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
        Log.i(TAG, "WXPayEntry onResp");
        WeChatApi inst = WeChatApi.GetInstance();
        switch (resp.getType()) {
            case ConstantsAPI.COMMAND_PAY_BY_WX:
                // 0:成功; -1:错误; -2:用户取消;
                PayResp payResp = (PayResp) resp;
                inst.OnPay(payResp);
                break;
            case ConstantsAPI.COMMAND_SUBSCRIBE_MESSAGE:
            case ConstantsAPI.COMMAND_LAUNCH_WX_MINIPROGRAM:
            case ConstantsAPI.COMMAND_OPEN_BUSINESS_VIEW:
            case ConstantsAPI.COMMAND_OPEN_BUSINESS_WEBVIEW:
            case ConstantsAPI.COMMAND_SENDAUTH:
            default:
                break;
        }

        finish();
    }
}
