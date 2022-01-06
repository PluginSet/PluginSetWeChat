package com.pluginset.wechatsdk;

public interface IWechatCallback {
    void Call(boolean success, String pluginName, String json, int code, String msg);
    void Message(String whatMsg, String content);
}

