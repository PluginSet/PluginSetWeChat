package com.sagi.barrettc2.wxapi;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.pluginset.wechatsdk.WeChatApi;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
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
    protected void onResume() {
        super.onResume();
        WeChatApi inst = WeChatApi.GetInstance();
        inst.onResume();
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
        if (resp.getType() == ConstantsAPI.COMMAND_SUBSCRIBE_MESSAGE) {

        }

        if (resp.getType() == ConstantsAPI.COMMAND_LAUNCH_WX_MINIPROGRAM) {

        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_BUSINESS_VIEW) {

        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_BUSINESS_WEBVIEW) {

        }

        WeChatApi inst = WeChatApi.GetInstance();
        if (resp.getType() == ConstantsAPI.COMMAND_PAY_BY_WX) {
            // 0:成功; -1:错误; -2:用户取消;
            PayResp payResp = (PayResp) resp;
            inst.OnPay(payResp);
        }
        finish();
    }
}
